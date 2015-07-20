using System;
using System.IO;
using System.Text;

namespace Selenium.Internal {

    /// <summary>
    /// Class to save and restor a directory recursively to and from a backup file.
    /// The created backup is a binary file not compressed.
    /// </summary>
    static class FolderCache {

        const ushort BUFFER_SIZE = 1024 * 7;

        /// <summary>
        /// Saves a directory recursively to a backup file
        /// </summary>
        /// <param name="folder">Full path of the directory source</param>
        /// <param name="path">Full path of the backup target file</param>
        /// <param name="excludes">Paths to exclude. ex: *.tmp</param>
        public static void Save(string folder, string path, params string[] excludes) {
            byte[] buffer = new byte[BUFFER_SIZE];
            Encoding encoder = Encoding.Unicode;

            //Add files to the binary file
            var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            var subPathIndex = folder.Length + 1;

            using (var outStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, BUFFER_SIZE * 2)) {
                foreach (var filepath in files) {
                    var relativePath = filepath.Substring(subPathIndex);
                    if (WildcardMatchAny(relativePath.ToLowerInvariant(), excludes))
                        continue;

                    using (var inStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, BUFFER_SIZE)) {
                        //set the length of the file name and the filename in the header
                        int namelen = encoder.GetBytes(relativePath, 0, relativePath.Length, buffer, 6);
                        buffer[0] = (byte)(namelen);
                        buffer[1] = (byte)(namelen >> 8);

                        //set the length of the data in the header
                        int datalen = (int)inStream.Length;
                        buffer[2] = (byte)(datalen);
                        buffer[3] = (byte)(datalen >> 8);
                        buffer[4] = (byte)(datalen >> 16);
                        buffer[5] = (byte)(datalen >> 24);

                        //write the header to the stream
                        outStream.Write(buffer, 0, 6 + namelen);
                        //write the data to the stream
                        CopyStream(inStream, outStream, buffer);
                    }
                }
            }
        }

        /// <summary>
        /// Restores a directory from a backup file
        /// </summary>
        /// <param name="bakpath">Full path of the backup source file</param>
        /// <param name="targetdir">Full path of the target directory</param>
        public unsafe static void Restore(string bakpath, string targetdir) {
            byte[] buffer = new byte[BUFFER_SIZE];
            string prevdir = null;
            Encoding encoder = Encoding.Unicode;

            //loads the binary file in the memory
            using (var inStream = new FileStream(bakpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, BUFFER_SIZE)) {
                //iterates all files
                while (inStream.Read(buffer, 0, 6) == 6) {
                    //read the length of the file path / 2 bytes
                    int namelen = buffer[0] | buffer[1] << 8;
                    //read the data length / 4 bytes
                    int datalen = buffer[2] | buffer[3] << 8 | buffer[4] << 16 | buffer[5] << 24;
                    //read name
                    inStream.Read(buffer, 0, namelen);
                    string relativePath = encoder.GetString(buffer, 0, namelen);

                    //create the folder if not present
                    var filepath = Path.Combine(targetdir, relativePath);
                    var dirname = Path.GetDirectoryName(filepath);
                    if (prevdir != dirname)
                        Directory.CreateDirectory(prevdir = dirname);

                    //Copy the data
                    using (var outStream = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.None, BUFFER_SIZE * 2)) {
                        CopyStream(inStream, outStream, datalen, buffer);
                    }
                }
            }
        }

        private static void CopyStream(Stream source, Stream target, byte[] buffer) {
            int size = buffer.Length;
            while ((size = source.Read(buffer, 0, size)) > 0)
                target.Write(buffer, 0, size);
        }

        private static void CopyStream(Stream source, Stream target, int count, byte[] buffer) {
            int size = Math.Min(buffer.Length, count);
            while ((size = source.Read(buffer, 0, size)) > 0) {
                target.Write(buffer, 0, size);
                if ((count -= size) < size)
                    size = count;
            }
        }

        private unsafe static bool WildcardMatchAny(string text, string[] patterns) {
            fixed (char* txt = text){
                foreach (string pattern in patterns) {
                    fixed (char* pat = pattern) {
                        if (WildcardMatch(txt, text.Length - 1, pat, pattern.Length - 1))
                            return true;
                    }
                }
            }
            return false;
        }

        private static unsafe bool WildcardMatch(char* txt, int itxt, char* pat, int ipat) {
            while (true) {
                if (ipat < 0)
                    return itxt < 0 || pat[0] == '*';
                if (itxt < 0)
                    return ipat == 0 && pat[ipat] == '*';
                if (pat[ipat] == '*'){
                    return WildcardMatch(txt, itxt, pat, ipat - 1)
                        || WildcardMatch(txt, itxt - 1, pat, ipat);
                }
                if (pat[ipat] != '?' && txt[itxt] != pat[ipat])
                    return false;
                itxt--;
                ipat--;
            }
        }

    }

}
