using System;
using System.IO;

namespace Selenium.Pdf {

    class PdfXRefs {

        public const int ID_EMPTY = 0;
        public const int ID_CATALOGUE = 1;
        public const int ID_INFO = 2;
        public const int ID_PAGES = 3;
        public const int ID_FONTS = 4;
        public const int ID_OUTLINES = 5;
        public const int ID_START_GENERATE = 6;

        private StreamWriter _pdfWriter;
        private long[] _buffer = new long[100];
        private int _count = ID_START_GENERATE;

        public PdfXRefs(StreamWriter writer) {
            _pdfWriter = writer;
        }

        public int Count {
            get {
                return _count;
            }
        }

        public int CreateObject() {
            return _count++;
        }

        public void RegisterObject(int id) {
            _pdfWriter.Flush();
            RegisterObject(id, _pdfWriter.BaseStream.Position);
        }

        public int CreateAndRegisterObject() {
            int id = _count++;
            RegisterObject(id);
            return id;
        }

        /// <summary>
        /// Adds a new position in the reference table.
        /// </summary>
        public void RegisterObject(int id, long position) {
            if (id >= _buffer.Length)
                IncreaseCapacity();
            _buffer[id] = position;
        }

        /// <summary>
        /// Writes the xref table to the pdf stream.
        /// </summary>
        /// <returns></returns>
        public long Write() {
            _pdfWriter.Flush();
            long position = _pdfWriter.BaseStream.Position;
            int count = _count;

            _pdfWriter.WriteLine("xref");
            _pdfWriter.WriteLine("0 " + count);  //First and last object number
            _pdfWriter.Write("0000000000 65535 f\r\n");
            for (int i = 1; i < count; i++) {
                _pdfWriter.Write(_buffer[i].ToString().PadLeft(10, '0'));
                _pdfWriter.Write(" 00000 n\r\n");
            }

            return position;
        }

        /// <summary>
        /// Increase the capacity of the buffer holding the positions.
        /// </summary>
        private void IncreaseCapacity() {
            long[] newBuffer = new long[_buffer.Length * 2];
            Array.Copy(_buffer, newBuffer, _count);
            _buffer = newBuffer;
        }

    }

}
