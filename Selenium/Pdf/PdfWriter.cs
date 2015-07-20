using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Selenium.Pdf {

    class PdfWriter {

        const string FILE_HEADER = "%PDF-1.4\n%\xE2\xE3\xCF\xD3";

        MemoryStream _pdfStream;
        StreamWriter _pdfWriter;
        PdfXRefs _xRefs;
        PdfOutlines _outlines;
        PdfPages _pages;
        PdfPage _currentPage;
        bool _closed;

        public PdfWriter() {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Encoding encoding = Encoding.GetEncoding(1252);
            _pdfStream = new MemoryStream(1024 * 10);
            _pdfWriter = new StreamWriter(_pdfStream, encoding);
            _xRefs = new PdfXRefs(_pdfWriter);
            _outlines = new PdfOutlines();
            _pages = new PdfPages();
            _pdfWriter.NewLine = "\n";
            _pdfWriter.WriteLine(FILE_HEADER);
        }

        internal void Dispose() {
            if (!_closed)
                this.Close();
            if (_pdfStream == null)
                return;
            _pdfStream.Dispose();
            _pdfStream = null;
        }

        /// <summary>
        /// Returns an array of unsigned bytes
        /// </summary>
        public byte[] Buffer {
            get {
                return _pdfStream.GetBuffer();
            }
        }

        /// <summary>
        /// Length of the streamin bytes
        /// </summary>
        public int Length {
            get {
                return (int)_pdfStream.Length;
            }
        }

        /// <summary>
        /// Sets the size of the page in points.
        /// </summary>
        /// <param name="pageWidthPt">Page width in points</param>
        /// <param name="pageHeightPt">Page height in points</param>
        public void SetPageSize(int pageWidthPt, int pageHeightPt) {
            _currentPage.Width = pageWidthPt;
            _currentPage.Height = pageHeightPt;
        }

        /// <summary>
        /// Adds a new page.
        /// </summary>
        /// <param name="widthPt">Width in point</param>
        /// <param name="heightPt">Height in point</param>
        public void AddPage(int widthPt, int heightPt) {
            int id = _xRefs.CreateObject();
            _currentPage = _pages.Add(id, widthPt, heightPt);
        }

        /// <summary>
        /// Adds an image.
        /// Encodes in JPEG if the image is already in JPEG and in PNG otherwise.
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="xPt">X position from the bottom in points</param>
        /// <param name="yPt">Y position from the bottom in points</param>
        /// <param name="widthPt">Width in points</param>
        /// <param name="heightPt">Height in points</param>
        public void WriteBitmap(Bitmap bitmap, int xPt, int yPt, int widthPt, int heightPt) {
            int idImg = _xRefs.CreateObject();
            int idImgLength = _xRefs.CreateObject();
            string imgName = "/I" + idImg;
            bool isJpeg = bitmap.RawFormat.Guid == ImageFormat.Jpeg.Guid;
            int dataLength = 0;

            _xRefs.RegisterObject(idImg);
            _pdfWriter.WriteLine(idImg + " 0 obj");
            _pdfWriter.WriteLine("<<");
            _pdfWriter.WriteLine("/Type /XObject");
            _pdfWriter.WriteLine("/Subtype /Image");
            _pdfWriter.WriteLine("/ColorSpace /DeviceRGB");
            _pdfWriter.WriteLine("/BitsPerComponent 8");
            if (isJpeg) {
                _pdfWriter.WriteLine("/Filter /DCTDecode");
            } else {
                _pdfWriter.WriteLine("/Filter /FlateDecode");
                _pdfWriter.WriteLine("/DecodeParms <<");
                _pdfWriter.WriteLine(" /Columns " + bitmap.Width);
                _pdfWriter.WriteLine(" /Colors 3");
                _pdfWriter.WriteLine(" /Predictor 15");
                _pdfWriter.WriteLine(">>");
            }
            _pdfWriter.WriteLine("/Name " + imgName);
            _pdfWriter.WriteLine("/Height " + bitmap.Height);
            _pdfWriter.WriteLine("/Width " + bitmap.Width);
            _pdfWriter.WriteLine("/Length " + idImgLength + " 0 R");
            _pdfWriter.WriteLine(">>");
            _pdfWriter.WriteLine("stream");
            _pdfWriter.Flush();
            if (isJpeg) {
                dataLength = PdfImage.WriteJPEG(bitmap, _pdfStream);
            } else {
                dataLength = PdfImage.WritePNG(bitmap, _pdfStream);
            }
            _pdfWriter.WriteLine();
            _pdfWriter.WriteLine("endstream");
            _pdfWriter.WriteLine("endobj");

            //Writes the length object
            _xRefs.RegisterObject(idImgLength);
            _pdfWriter.WriteLine(idImgLength + " 0 obj");
            _pdfWriter.WriteLine(dataLength);
            _pdfWriter.WriteLine("endobj");

            //create image content and borders
            StringBuilder sb = new StringBuilder();
            sb.Append("q ");
            sb.Append(widthPt + " 0 0 " + heightPt + " " + xPt + " " + yPt + " cm ");
            sb.Append(imgName);
            sb.Append(" Do Q\n");
            sb.Append("0.5 w 0.2 G ");
            sb.Append(xPt + " " + yPt + " " + widthPt + " " + heightPt + " re s");
            int idContent = WriteStreamObject(sb);

            //add the created objects to the page
            _currentPage.AddContent(idContent);
            _currentPage.AddXObject(idImg, imgName);
        }

        /// <summary>
        /// Writes a multiple strings at the given position.
        /// </summary>
        /// <param name="lines">Text lines</param>
        /// <param name="offset">Offset of the text line</param>
        /// <param name="count">Count of lines to write</param>
        /// <param name="xPt">X position from the bottom in points</param>
        /// <param name="yPt">Y position from the bottom in points</param>
        /// <param name="font">Font</param>
        /// <param name="fontSize">Font size in em</param>
        /// <param name="rgb">RGB color. Blue is the lower byte</param>
        /// <param name="lineHeightPt">Line height in points</param>
        public void WriteLines(List<string> lines, int offset, int count, int xPt
            , int yPt, PdfFont font, int fontSize, int rgb, float lineHeightPt) {

            yPt += (int)(lineHeightPt * 0.3);
            StringBuilder sb = new StringBuilder();
            sb.Append("BT ");
            sb.Append(FormatRGB(rgb) + " rg "); //font color
            sb.Append(font.CodeName + " " + fontSize + " Tf ");  //font name, size
            sb.Append(lineHeightPt + " TL ");  //line height
            sb.Append(xPt + " " + yPt + " Td\n"); //x , y
            count += offset;
            for (int i = offset; i < count; i++) {
                sb.Append('(');
                sb.Append(EscapeText(lines[i]));
                sb.Append(")'\n");
            }
            sb.Append("ET");

            int id = WriteStreamObject(sb);
            _currentPage.AddContent(id);
        }

        /// <summary>
        /// Writes a multiple strings centered at the given position.
        /// </summary>
        /// <param name="lines">Text lines</param>
        /// <param name="offset">Offset of the text line</param>
        /// <param name="count">Count of lines to write</param>
        /// <param name="xPt">X position from the bottom in points</param>
        /// <param name="yPt">Y position from the bottom in points</param>
        /// <param name="font">Font</param>
        /// <param name="fontSize">Font size in em</param>
        /// <param name="rgb">RGB color. Blue is the lower byte</param>
        /// <param name="linesWidthsPt">Widths of the lines</param>
        /// <param name="lineHeightPt">Height of a single line</param>
        public void WriteLinesCenter(List<string> lines, int offset, int count
            , int xPt, int yPt, PdfFont font, int fontSize, int rgb
            , float[] linesWidthsPt, float lineHeightPt) {

            yPt += (int)(lineHeightPt * 0.3);
            StringBuilder sb = new StringBuilder();
            sb.Append("BT ");
            sb.Append(FormatRGB(rgb) + " rg "); //font color
            sb.Append(font.CodeName + " " + fontSize + " Tf\n");  //font name, size
            float y = yPt - lineHeightPt;
            count += offset;
            for (int i = offset; i < count; i++) {
                float x = xPt - linesWidthsPt[i] / 2;
                sb.Append("1 0 0 1 " + x + " " + y + " Tm (");
                sb.Append(EscapeText(lines[i]));
                sb.Append(")Tj\n");
                y -= lineHeightPt;
            }
            sb.Append("ET");

            int id = WriteStreamObject(sb);
            _currentPage.AddContent(id);
        }

        /// <summary>
        /// Writes an outline refering to the given poisiton.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="yPt">Y position from the bottom in points</param>
        public void WriteOutline(string text, int yPt = -1) {
            int id = _xRefs.CreateObject();
            _outlines.Add(id, text, _currentPage.Id, yPt);
        }

        /// <summary>
        /// Writes the pages numbers
        /// </summary>
        /// <param name="font">Font</param>
        /// <param name="fontSize">Fonr size</param>
        /// <param name="xPt">X position from the bottom in points</param>
        /// <param name="yPt">Y position from the bottom in points</param>
        /// <param name="rgb">RGB color. Blue is the lower byte</param>
        public void WritePagesNumbers(PdfFont font, int fontSize, int xPt, int yPt, int rgb) {
            string color = FormatRGB(rgb);
            int pageCount = _pages.Count;
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = _pages[i];
                string txt = (i + 1) + "/" + pageCount;
                float txtWidth = font.MesureWidth(txt, fontSize);
                int xPtAbs = xPt < 0 ? (int)(xPt + page.Width - txtWidth) : xPt;

                StringBuilder sb = new StringBuilder();
                sb.Append("BT ");
                sb.Append(color + " rg ");
                sb.Append(font.CodeName + " " + fontSize + " Tf ");
                sb.Append(xPtAbs + " " + yPt + " Td\n");
                sb.Append("(" + txt + ")Tj\n");
                sb.Append("ET");

                int id = WriteStreamObject(sb);
                page.AddContent(id);
            }
        }

        /// <summary>
        /// Writes an hyperlink area
        /// </summary>
        /// <param name="link">Hyperlink</param>
        /// <param name="x1">Rectangle left</param>
        /// <param name="y1">Rectangle top</param>
        /// <param name="x2">Rectangle right</param>
        /// <param name="y2">Rectangle bottom</param>
        public void WriteLink(string link, int x1, int y1, int x2, int y2) {
            int idLink = _xRefs.CreateAndRegisterObject();

            _pdfWriter.WriteLine(idLink + " 0 obj");
            _pdfWriter.WriteLine("<<");
            _pdfWriter.WriteLine("/Type /Annot");
            _pdfWriter.WriteLine("/Subtype /Link");
            _pdfWriter.WriteLine("/Rect [" + x1 + " " + y1 + " " + x2 + " " + y2 + "]");
            _pdfWriter.WriteLine("/Border [0 0 0]");
            _pdfWriter.WriteLine("/A <<");
            _pdfWriter.WriteLine(" /Type /Action");
            _pdfWriter.WriteLine(" /S /URI");
            _pdfWriter.WriteLine(" /URI (" + link + ")");
            _pdfWriter.WriteLine(">>");
            _pdfWriter.WriteLine(">>");
            _pdfWriter.WriteLine("endobj");

            _currentPage.AddAnnot(idLink);
        }

        /// <summary>
        /// Writes the end of the PDF file
        /// </summary>
        public void Close() {
            if (_closed)
                throw new Exception("Writer already closed.");

            //write fonts
            PdfFonts.Write(_pdfWriter, _xRefs);

            //write outlines
            _outlines.Write(_pdfWriter, _xRefs);

            //write pages
            _pages.Write(_pdfWriter, _xRefs);

            //write info
            string date = DateTime.UtcNow.ToUniversalTime().ToString("yyyyMMddHHmmss");
            _xRefs.RegisterObject(PdfXRefs.ID_INFO);
            _pdfWriter.WriteLine(PdfXRefs.ID_INFO + " 0 obj");
            _pdfWriter.WriteLine("<<");
            _pdfWriter.WriteLine("/Title ()");
            _pdfWriter.WriteLine("/Author ()");
            _pdfWriter.WriteLine("/Creator ()");
            _pdfWriter.WriteLine("/Producer ()");
            _pdfWriter.WriteLine("/CreationDate (D:" + date + "Z)");
            _pdfWriter.WriteLine("/ModDate (D:" + date + "Z)");
            _pdfWriter.WriteLine(">>");
            _pdfWriter.WriteLine("endobj");

            //write catalogue
            _xRefs.RegisterObject(PdfXRefs.ID_CATALOGUE);
            _pdfWriter.WriteLine(PdfXRefs.ID_CATALOGUE + " 0 obj");
            _pdfWriter.WriteLine("<<");
            _pdfWriter.WriteLine("/Type /Catalog");
            if (_outlines.Count > 0) {
                _pdfWriter.WriteLine("/PageMode /UseOutlines");
                _pdfWriter.WriteLine("/Outlines " + PdfXRefs.ID_OUTLINES + " 0 R");
            }
            _pdfWriter.WriteLine("/Pages " + PdfXRefs.ID_PAGES + " 0 R");
            _pdfWriter.WriteLine(">>");
            _pdfWriter.WriteLine("endobj");

            //Write reference table
            long xref_offset = _xRefs.Write();

            //Write Trailer
            string id = Generate32CharsId();
            _pdfWriter.WriteLine("trailer");
            _pdfWriter.WriteLine("<<");
            _pdfWriter.WriteLine("/Root " + PdfXRefs.ID_CATALOGUE + " 0 R");
            _pdfWriter.WriteLine("/Info " + PdfXRefs.ID_INFO + " 0 R");
            _pdfWriter.WriteLine("/Size " + _xRefs.Count);
            _pdfWriter.WriteLine("/ID [<" + id + "><" + id + ">]");
            _pdfWriter.WriteLine(">>");

            //Write xref address
            _pdfWriter.WriteLine("startxref");
            _pdfWriter.WriteLine(xref_offset.ToString()); // last one

            //Write end of file
            _pdfWriter.WriteLine("%%EOF");
            _pdfWriter.Flush();

            _closed = true;
        }

        /// <summary>
        /// Writes the temporary stream in the pdf stream
        /// </summary>
        /// <returns>Returns the created id</returns>
        private int WriteStreamObject(StringBuilder streamContent) {
            int idStream = _xRefs.CreateAndRegisterObject();

            _pdfWriter.WriteLine(idStream + " 0 obj");
            _pdfWriter.WriteLine("<<");
            _pdfWriter.WriteLine("/Length " + streamContent.Length);
            _pdfWriter.WriteLine(">>");
            _pdfWriter.WriteLine("stream");
            _pdfWriter.WriteLine(streamContent);
            _pdfWriter.WriteLine("endstream");
            _pdfWriter.WriteLine("endobj");

            return idStream;
        }

        /// <summary>
        /// Escapes these characters: ()\
        /// </summary>
        /// <param name="text">Input string</param>
        /// <returns>String</returns>
        internal static string EscapeText(string text) {
            return Regex.Replace(text, @"[\(\)\\]", @"\$&");
        }

        /// <summary>
        /// Converts an rgb value to a string. 0xFF0000 => "1 0 0"
        /// </summary>
        /// <param name="rgb">RGB integer with b as the lower byte</param>
        /// <returns>String</returns>
        private static string FormatRGB(int rgb) {
            double r = Math.Round(((rgb >> 16) & 0xFF) / 255f, 3);
            double g = Math.Round(((rgb >> 8) & 0xFF) / 255f, 3);
            double b = Math.Round((rgb & 0xFF) / 255f, 3);
            return r + " " + g + " " + b;
        }

        private static string Generate32CharsId() {
            Random rnd = new Random();
            char[] buffer = new char[32];
            for (int i = 0; i < 32; i++) {
                int c = rnd.Next(0, 16);
                buffer[i] = (char)((c < 10 ? 48 : 87) + c);
            }
            return new string(buffer, 0, 32);
        }

    }

}
