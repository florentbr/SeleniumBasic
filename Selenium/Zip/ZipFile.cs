using Selenium.Internal;
using Selenium.Serializer;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Selenium.Zip {

    /// <summary>
    /// Class to extract and create a Zip archive.
    /// Implements the PKWARE specs: https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT
    /// Supports only the 32 bits deflate compression
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// using(var zip = new ZipFile()){
    ///     zip.AddFile(@"c:\archive\file1.txt");
    ///     zip.AddFile(@"c:\archive\file2.txt");
    ///     zip.SaveAs(@"c:\archive.zip");
    /// }
    /// ZipFile.ExtractAll(@"c:\archive.zip", @"c:\archive");
    /// </code>
    /// </example>
    public class ZipFile : IDisposable, IJsonBinary {

        /* "https://en.wikipedia.org/wiki/Zip_(file_format)"
        
            [Local file header]
         
            Offset Bytes Description
            0      4     Local file header signature = 0x04034b50 (read as a little-endian number)
            4      2     Version needed to extract (minimum)
            6      2     General purpose bit flag
            8      2     Compression method
            10     2     File last modification time
            12     2     File last modification date
            14     4     CRC-32
            18     4     Compressed size
            22     4     Uncompressed size
            26     2     File name length (n)
            28     2     Extra field length (m)
            30     n     File name
            30+n   m     Extra field
                
            [Central directory file header]
         
            Offset Bytes Description
            0      4     Central directory file header signature = 0x02014b50
            4      2     Version made by
            6      2     Version needed to extract (minimum)
            8      2     General purpose bit flag
            10     2     Compression method
            12     2     File last modification time
            14     2     File last modification date
            16     4     CRC-32
            20     4     Compressed size
            24     4     Uncompressed size
            28     2     File name length (n)
            30     2     Extra field length (m)
            32     2     File comment length (k)
            34     2     Disk number where file starts
            36     2     Internal file attributes
            38     4     External file attributes
            42     4     Relative offset of local file header. This is the number of bytes between the start of the first disk on which the file occurs, and the start of the local file header. This allows software reading the central directory to locate the position of the file inside the .ZIP file.
            46     n     File name
            46+n   m     Extra field
            46+n+m k     File comment

            [End of central directory record]

            Offset Bytes Description
            0      4     End of central directory signature = 0x06054b50
            4      2     Number of this disk
            6      2     Disk where central directory starts
            8      2     Number of central directory records on this disk
            10     2     Total number of central directory records
            12     4     Size of central directory (bytes)
            16     4     Offset of start of central directory, relative to start of archive
            20     2     Comment length (n)
            22     n     Comment

        */


        const int BUFFER_BYTES_SIZE = 1024 * 64;
        const int BUFFER_CHARS_SIZE = 1024;
        const int FLAG_COMPRESSION_STORE = 0x00;
        const int FLAG_COMPRESSION_DEFLATE = 0x08;
        const int FLAG_UTF8_FILENAME = 1 << 11;

        /// <summary>
        /// Extracts all the files from a Zip archive to a directory
        /// </summary>
        /// <param name="zip_path">Archive path</param>
        /// <param name="directory">Target directory</param>
        public static void ExtractAll(string zip_path, string directory) {
            using (var fstream = new FileStream(zip_path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                ExtractAll(fstream, directory);
            }
        }

        public static void ExtractAll(Stream stream, string directory) {
            Decoder decoder_utf8 = Encoding.UTF8.GetDecoder();
            Decoder decoder_437 = Encoding.GetEncoding(437).GetDecoder();
            byte[] bufBytes = new byte[BUFFER_BYTES_SIZE];
            char[] bufChars = new char[BUFFER_CHARS_SIZE];

            //search for the signature of the end of the central directory (0x06054b50)
            try {
                long offset_eocd = stream.Length - 22; //eocd offset when there is no comment
                stream.Position = offset_eocd;
                while (stream.ReadByte() != 0x50
                    || stream.ReadByte() != 0x4B
                    || stream.ReadByte() != 0x05
                    || stream.ReadByte() != 0x06) {
                    stream.Position = --offset_eocd;
                }
            } catch (IOException) {
                throw new ZipException("Failed to locate the end of central directory.");
            }

            //read the end of the central directory
            stream.Read(bufBytes, 4, 18);   // offset of 4 bytes just after the signature
            int h_entries_count = ReadUInt16(bufBytes, 8);
            long h_centraledir_offset = ReadUInt32(bufBytes, 16);
            if (h_entries_count == 0x0FFFF || h_centraledir_offset == 0xFFFFFFFFL)
                throw new ZipException("Zip 64 bits not supported.");

            //read entries
            string prevdir = null;
            long offset_next_entry = h_centraledir_offset;
            while (h_entries_count-- > 0) {
                //read the central directory headers
                stream.Position = offset_next_entry;
                stream.Read(bufBytes, 0, 46);
                uint h_header_signature = ReadUInt32(bufBytes, 0);
                if (h_header_signature != 0x02014b50)
                    throw new ZipException("Wrong central directory header signature.");
                int h_bit_flags = ReadUInt16(bufBytes, 8);
                int h_compression_method = ReadUInt16(bufBytes, 10);
                if (h_compression_method != FLAG_COMPRESSION_DEFLATE
                    && h_compression_method != FLAG_COMPRESSION_STORE)
                    throw new ZipException("Type of compression not supported.");
                uint h_lastwrite_dostime = ReadUInt32(bufBytes, 12);
                long h_uncompressed_size = ReadUInt32(bufBytes, 24);
                int h_filename_length = ReadUInt16(bufBytes, 28);
                int h_extrafield_length = ReadUInt16(bufBytes, 30);
                int h_comment_length = ReadUInt16(bufBytes, 32);
                long h_header_offset = ReadUInt32(bufBytes, 42);

                //read the file name and build the file path
                stream.Read(bufBytes, 0, h_filename_length);
                Decoder decoder = (h_bit_flags & FLAG_UTF8_FILENAME) == 1 ? decoder_utf8 : decoder_437;
                int filename_len = decoder.GetChars(bufBytes, 0, h_filename_length, bufChars, 0);
                string filename = new string(bufChars, 0, filename_len)
                    .Replace('/', Path.DirectorySeparatorChar);
                if (string.IsNullOrEmpty(filename))
                    throw new ZipException(string.Format("Invalid file name: {0}", filename));
                string filepath = Path.Combine(directory, filename);
                string dir = Path.GetDirectoryName(filepath);

                //convert the last write time
                DateTime lastwrite_datetime = ConvertDosTimeToDateTime(h_lastwrite_dostime);

                //create directories if needed
                if (dir != prevdir) {
                    Directory.CreateDirectory(dir);
                    Directory.SetLastWriteTime(dir, lastwrite_datetime);
                    prevdir = dir;
                }

                //save offset of the next entry
                offset_next_entry = stream.Position + h_extrafield_length + h_comment_length;

                //read the local entry and create the file or directory
                if (h_uncompressed_size > 0) {
                    stream.Position = h_header_offset;

                    //read the local file header
                    stream.Read(bufBytes, 0, 30);
                    uint hl_header_signature = ReadUInt32(bufBytes, 0);
                    if (hl_header_signature != 0x04034B50)
                        throw new ZipException("Wrong local file header signature.");
                    uint hl_filename_length = ReadUInt16(bufBytes, 26);
                    uint hl_extrafield_length = ReadUInt16(bufBytes, 28);

                    //seek begining of data
                    stream.Seek(hl_filename_length + hl_extrafield_length, SeekOrigin.Current);

                    //read the data and create the file
                    using (var outStream = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write)) {
                        if (h_compression_method == FLAG_COMPRESSION_DEFLATE) {
                            var defStream = new DeflateStream(stream, CompressionMode.Decompress, true);
                            CopyStream(defStream, outStream, (int)h_uncompressed_size, bufBytes);
                        } else {
                            CopyStream(stream, outStream, (int)h_uncompressed_size, bufBytes);
                        }
                    }

                    //update the time of the file
                    File.SetLastWriteTime(filepath, lastwrite_datetime);
                } else {
                    char lastChar = filename[filename.Length - 1];
                    if (lastChar != Path.DirectorySeparatorChar) {
                        //creates an empty empty file
                        File.Create(filepath).Dispose();
                        File.SetLastWriteTime(filepath, lastwrite_datetime);
                    }
                }
            }

        }



        private readonly MemoryStream _stream_locdir;
        private readonly MemoryStream _stream_centdir;
        private readonly MemoryStream _stream_eocd;
        private int _entries_count;
        private byte[] _buffer;
        private bool _compress;

        public ZipFile(bool compress = true) {
            _compress = compress;
            _buffer = new byte[BUFFER_BYTES_SIZE];
            _stream_locdir = new MemoryStream(1024 * 12);
            _stream_centdir = new MemoryStream(1024);
            _stream_eocd = new MemoryStream(22);
            _stream_eocd.SetLength(22);
        }

        public void Dispose() {
            _stream_locdir.Dispose();
            _stream_centdir.Dispose();
            _stream_eocd.Dispose();
        }

        /// <summary>
        /// Add a file to the archive
        /// </summary>
        /// <param name="file">File path</param>
        public void AddFile(string file) {
            _entries_count += 1;
            FileInfo fileinfo = new FileInfo(file);
            using (FileStream filestream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                WriteFile(fileinfo, filestream);
            }
        }

        /// <summary>
        /// Gets the lenght in bytes
        /// </summary>
        public long Length {
            get {
                return _stream_locdir.Length + _stream_centdir.Length + _stream_eocd.Length;
            }
        }

        /// <summary>
        /// Saves the composed Zip to the given path
        /// </summary>
        /// <param name="path">File path</param>
        public void SaveAs(string path) {
            using (var fstream = new FileStream(path, FileMode.Create, FileAccess.Write)) {
                this.Save(fstream);
            }
        }

        /// <summary>
        /// Saves the composed Zip to the stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream) {
            UpdateEndOfCentralDirectory();
            stream.SetLength(stream.Position + this.Length);
            stream.Write(_stream_locdir.GetBuffer(), 0, (int)_stream_locdir.Length);
            stream.Write(_stream_centdir.GetBuffer(), 0, (int)_stream_centdir.Length);
            stream.Write(_stream_eocd.GetBuffer(), 0, (int)_stream_eocd.Length);
        }

        private void WriteFile(FileInfo fileinfo, FileStream filestream) {
            byte[] file_name_bytes = Encoding.UTF8.GetBytes(fileinfo.Name);
            int file_name_length = file_name_bytes.Length;
            DateTime file_lastwrite_datetime = fileinfo.LastWriteTimeUtc.ToLocalTime();
            uint file_lastwrite_dostime = ConvertDateTimeToDosTime(file_lastwrite_datetime);

            //Save the header positon
            long offset_local_header = _stream_locdir.Position;

            //Write the file data at offset 30 + [length of the file name]
            long offset_data_start = offset_local_header + 30 + file_name_length;
            long file_data_length = filestream.Length;
            uint file_crc32 = 0;
            if (file_data_length != 0) {
                //read file data and compute the CRC32
                _stream_locdir.Position = offset_data_start;
                if (_compress) {
                    using (var deflatestream = new DeflateStream(_stream_locdir, CompressionMode.Compress, true))
                        CopyStream(filestream, deflatestream, (int)file_data_length, _buffer, out file_crc32);
                } else {
                    CopyStream(filestream, _stream_locdir, (int)file_data_length, _buffer, out file_crc32);
                }
            }
            long offset_data_end = _stream_locdir.Position;
            long file_compressed_size = offset_data_end - offset_data_start;

            //Write the local file header
            byte[] bufL = new byte[30];
            WriteUInt32(bufL, 0, 0x04034B50);           //Local file header signature
            WriteUInt16(bufL, 4, 0x0014);               //Version needed to extract
            WriteUInt16(bufL, 6, FLAG_UTF8_FILENAME);   //General purpose bit flag
            if (file_data_length != 0 && _compress)
                WriteUInt16(bufL, 8, FLAG_COMPRESSION_DEFLATE); //Compression method
            WriteUInt32(bufL, 10, file_lastwrite_dostime);      //File last modification date/time
            WriteUInt32(bufL, 14, file_crc32);           //CRC-32
            WriteUInt32(bufL, 18, file_compressed_size); //Compressed size
            WriteUInt32(bufL, 22, file_data_length);     //Uncompressed size
            WriteUInt16(bufL, 26, file_name_length);     //File name length

            _stream_locdir.Position = offset_local_header;
            _stream_locdir.Write(bufL, 0, bufL.Length); //Write local file header to stream
            _stream_locdir.Write(file_name_bytes, 0, file_name_length); //Write local file name to stream

            //restore the end of data position for the next entry
            _stream_locdir.Position = offset_data_end;

            //Write the central file header
            byte[] bufC = new byte[46];
            WriteUInt32(bufC, 0, 0x02014B50);               //Central directory header signature
            WriteUInt16(bufC, 4, 0x3F);                     //Version made by
            WriteUInt16(bufC, 6, 0x14);                     //Version needed to extract (minimum)
            WriteUInt16(bufC, 8, FLAG_UTF8_FILENAME);       //General purpose bit flag
            if (file_data_length != 0 && _compress)
                WriteUInt16(bufC, 10, FLAG_COMPRESSION_DEFLATE); //Compression method
            WriteUInt32(bufC, 12, file_lastwrite_dostime);       //File last modification date/time
            WriteUInt32(bufC, 16, file_crc32);                   //CRC-32
            WriteUInt32(bufC, 20, file_compressed_size);         //Compressed size
            WriteUInt32(bufC, 24, file_data_length);        //Uncompressed size
            WriteUInt16(bufC, 28, file_name_length);        //File name length
            WriteUInt32(bufC, 42, offset_local_header);     //Relative offset of local file header
            _stream_centdir.Write(bufC, 0, bufC.Length);    //Write central file header to stream
            _stream_centdir.Write(file_name_bytes, 0, file_name_length);  //Write file name to stream
        }

        private void UpdateEndOfCentralDirectory() {
            byte[] buf = _stream_eocd.GetBuffer();
            WriteUInt32(buf, 0, 0x06054B50);                //End of central directory signature
            WriteUInt16(buf, 8, _entries_count);            //Number of central directory records on this disk
            WriteUInt16(buf, 10, _entries_count);           //Total number of central directory records
            WriteUInt32(buf, 12, _stream_centdir.Length);   //Size of central directory (bytes)
            WriteUInt32(buf, 16, _stream_locdir.Length);    //Offset of start of central directory, relative to start of archive
        }



        #region Support methods

        private static void CopyStream(Stream source, Stream target, int count, byte[] buffer) {
            int size = Math.Min(buffer.Length, count);
            while ((size = source.Read(buffer, 0, size)) > 0) {
                target.Write(buffer, 0, size);
                if ((count -= size) < size)
                    size = count;
            }
        }

        private static void CopyStream(Stream source, Stream target, long count, byte[] buffer, out uint crc32) {
            uint crc = uint.MaxValue;
            uint[] table = Crc32.CRCTABLE;
            int size = buffer.Length;
            do {
                if (count < size)
                    size = (int)count;
                size = source.Read(buffer, 0, size);
                target.Write(buffer, 0, size);
                for (int i = 0; i < size; i++) {
                    crc = table[(crc ^ buffer[i]) & 0xFF] ^ (crc >> 8);
                }
            } while ((count -= size) > 0);
            crc32 = crc ^ uint.MaxValue;
        }

        private static ushort ReadUInt16(byte[] buffer, int offset) {
            return (ushort)(buffer[offset] | buffer[offset + 1] << 8);
        }

        private static uint ReadUInt32(byte[] buffer, int offset) {
            return (uint)(buffer[offset]
                | buffer[offset + 1] << 8
                | buffer[offset + 2] << 16
                | buffer[offset + 3] << 24);
        }

        private static void WriteUInt16(byte[] buffer, int offset, int value) {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
        }

        private static void WriteUInt32(byte[] buffer, int offset, long value) {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }

        private static uint ConvertDateTimeToDosTime(DateTime datetime) {
            int dostime = (datetime.Year - 1980) << 25
                | datetime.Month << 21
                | datetime.Day << 16
                | datetime.Hour << 11
                | datetime.Minute << 5
                | datetime.Second >> 1;
            return unchecked((uint)dostime);
        }

        private static DateTime ConvertDosTimeToDateTime(uint dostime) {
            DateTime datetime = new DateTime(
                (int)(dostime >> 25) + 1980,
                (int)(dostime >> 21) & 15,
                (int)(dostime >> 16) & 31,
                (int)(dostime >> 11) & 31,
                (int)(dostime >> 5) & 63,
                (int)(dostime & 31) * 2);
            return datetime;
        }

        #endregion

    }

}
