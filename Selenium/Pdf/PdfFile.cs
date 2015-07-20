using Selenium.Internal;
using Selenium.Pdf;
using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Bitmap = System.Drawing.Bitmap;

namespace Selenium {

    /// <summary>
    /// PDF object
    /// </summary>
    [ProgId("Selenium.PdfFile")]
    [Guid("0277FC34-FD1B-4616-BB19-CDCD9EB97FD6")]
    [Description("Creates a new empty PDF file")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class PdfFile : ComInterfaces._PdfFile, IDisposable {

        const int IMG_SPLIT_HEIGHT_MIN_PT = 50;

        private int _pageWidthPt = (int)(8.5 * 72);
        private int _pageHeightPt = (int)(11 * 72);
        private int _marginLeftPt = (int)(0.5 * 72);
        private int _marginRightPt = (int)(0.5 * 72);
        private int _marginTopPt = (int)(0.5 * 72);
        private int _marginBottomPt = (int)(0.5 * 72);
        private int _pageNumberRightPt = (int)(0.4 * 72);
        private int _pageNumberBottomPt = (int)(0.4 * 72);

        private PdfWriter _writer;
        private int _contentHeightPt = 0;
        private int _title1Count;

        /// <summary>
        /// Creates a new empty PDF
        /// </summary>
        public PdfFile() {
            _writer = new PdfWriter();
            _writer.AddPage(_pageWidthPt, _pageHeightPt);
        }

        /// <summary>
        /// Release the resources.
        /// </summary>
        public void Dispose() {
            if (_writer == null)
                return;
            _writer.Dispose();
            _writer = null;
        }

        private void EnsureNotDisposed() {
            if (_writer == null)
                throw new Errors.PdfError("The PDF has been disposed.");
        }

        /// <summary>
        /// Set the size of the page in millimeter(mm), inch(in) or point(pt).
        /// </summary>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <param name="metric">Optional metric: mm, in or pt</param>
        public void SetPageSize(int width, int heigth, string metric = "in") {
            EnsureNotDisposed();
            float ratio = GetRatio(metric);
            _pageWidthPt = (int)(width * ratio);
            _pageHeightPt = (int)(heigth * ratio);

            if (_pageWidthPt < 1 || _pageHeightPt < 1)
                throw new Errors.PdfError("Out of range: width and heigth.");
            _writer.SetPageSize(_pageWidthPt, _pageHeightPt);
        }

        /// <summary>
        /// Set the margins of the page in millimeter(mm), inch(in) or point(pt).
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="metric">Optional metric: mm, in or pt</param>
        public void SetMargins(int left, int right, int top, int bottom, string metric = "in") {
            EnsureNotDisposed();
            float ratio = GetRatio(metric);
            _marginLeftPt = (int)(left * ratio);
            _marginRightPt = (int)(right * ratio);
            _marginTopPt = (int)(top * ratio);
            _marginBottomPt = (int)(bottom * ratio);
            if (left < 0 || right < 0 || ((_marginLeftPt + _marginRightPt) * 100) / _pageWidthPt > 80)
                throw new Errors.PdfError("Out of range: left, right.");
            if (top < 0 || bottom < 0 || ((_marginTopPt + _marginBottomPt) * 100) / _pageHeightPt > 80)
                throw new Errors.PdfError("Out of range: top, bottom.");
        }

        /// <summary>
        /// Adds a new page.
        /// </summary>
        public void AddPage() {
            EnsureNotDisposed();
            _writer.AddPage(_pageWidthPt, _pageHeightPt);
            _contentHeightPt = 0;
        }

        /// <summary>
        /// Saves as a file.
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveAs(string filePath) {
            EnsureNotDisposed();
            PdfFont font = PdfFonts.Select(false, false);
            int x = -_pageNumberRightPt;
            int y = _pageNumberBottomPt;
            _writer.WritePagesNumbers(font, 9, x, y, 0);
            _writer.Close();
            try {
                if (filePath.IndexOf('{') != -1)
                    filePath = string.Format(filePath, DateTime.UtcNow);
                filePath = IOExt.ExpandPath(filePath);
                filePath = Path.GetFullPath(filePath);
                string folder = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                if (File.Exists(filePath))
                    File.Delete(filePath);

                using (var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write)) {
                    fs.Write(_writer.Buffer, 0, _writer.Length);
                }
            } catch (Exception ex) {
                throw new SeleniumException(ex);
            } finally {
                this.Dispose();
            }
        }

        /// <summary>
        /// Adds a bookmark.
        /// </summary>
        /// <param name="name"></param>
        public void AddBookmark(string name) {
            EnsureNotDisposed();
            int x = CurrentY();
            _writer.WriteOutline(name, x);
        }

        /// <summary>
        /// Adds a vacant height in millimeter(default), inch or point.
        /// </summary>
        /// <param name="size"></param>
        public void AddSpace(int size) {
            EnsureNotDisposed();
            _contentHeightPt += size;
        }

        /// <summary>
        /// Adds the text at the current position.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="size">Font size</param>
        /// <param name="bold">Font wigth</param>
        /// <param name="italic">Font italic</param>
        /// <param name="color">Name or hexa value. Ex: "red", "FF0000"</param>
        public void AddText(string text, short size = 10
            , bool bold = false, bool italic = false, string color = null) {
            EnsureNotDisposed();
            const int indent = 0;
            int space = (int)(size * 0.2f);
            const int spaceBefore = 0;
            const int spaceAfter = 4;
            WriteText(text, size, bold, italic, color, false, indent, space, spaceBefore, spaceAfter);
        }

        /// <summary>
        /// Adds text centered horizontally
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="size">Font size</param>
        /// <param name="bold">Font wigth</param>
        /// <param name="italic">Font italic</param>
        /// <param name="color">Name or hexa value. Ex: "red", "FF0000"</param>
        public void AddTextCenter(string text, short size = 10
            , bool bold = false, bool italic = false, string color = null) {
            EnsureNotDisposed();
            const int indent = 0;
            const int space = 0;
            const int spaceBefore = 0;
            const int spaceAfter = 4;
            WriteText(text, size, bold, italic, color, true, indent, space, spaceBefore, spaceAfter);
        }

        /// <summary>
        /// Adds the text at the current position.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="size">Font size</param>
        /// <param name="bold">Font weight</param>
        /// <param name="italic">Font is italic</param>
        public void AddTitle(string text, short size = 12, bool bold = true, bool italic = false) {
            EnsureNotDisposed();
            _title1Count++;
            string title = _title1Count + ". " + text;
            _writer.WriteOutline(title, CurrentY());
            int indent = size;
            int space = (int)(size * 0.2f);
            int spaceBefore = (int)(size * 0.8f);
            int spaceAfter = (int)(size * 0.4f);
            WriteText(title, size, bold, italic, null, false, indent, space, spaceBefore, spaceAfter);
        }

        /// <summary>
        /// Adds a link
        /// </summary>
        /// <param name="link">Link to add</param>
        /// <param name="text">Optional - Text for the link</param>
        /// <param name="size">Optional - Font size</param>
        public void AddLink(string link, string text = null, int size = 10) {
            if (string.IsNullOrEmpty(text))
                text = link;
            PdfFont font = PdfFonts.Select(false, false);
            int availableWidthPt = AvailableWidth();
            int availableHeightPt = AvailableHeight();
            float lineHeightPt;
            float[] linesWidthPt;
            var lines = font.BreakText(text, size, availableWidthPt, out linesWidthPt, out lineHeightPt);
            if (availableHeightPt < lineHeightPt * lines.Count) {
                this.AddPage();
            }
            int linesCount = lines.Count;
            int x1 = CurrentX();
            int x2 = x1 + (int)linesWidthPt[0];
            int y1 = CurrentY();
            int y2 = (int)(y1 - linesCount * lineHeightPt);
            _writer.WriteLines(lines, 0, linesCount, x1, y1, font, size, 0x0000F0, lineHeightPt);
            _writer.WriteLink(link, x1, y2, x2, y1);
            _contentHeightPt += (int)(linesCount * lineHeightPt) + 4;
        }

        /// <summary>
        /// Writes one or more lines of text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="size">Font size</param>
        /// <param name="bold">Font bold</param>
        /// <param name="italic">Font italic</param>
        /// <param name="color">Font color</param>
        /// <param name="center">Center the text or not</param>
        /// <param name="indent">Indentation in em</param>
        /// <param name="spacing">Spacing in em</param>
        /// <param name="spacebefore">Space before in em</param>
        /// <param name="spaceAfter">Space after in em</param>
        private void WriteText(string text, short size, bool bold, bool italic
            , string color, bool center, int indent, int spacing, int spacebefore, int spaceAfter) {
            EnsureNotDisposed();
            PdfFont font = PdfFonts.Select(bold, italic);
            int rgb = ParseColorToRGB(color);
            int availableWidthPt = AvailableWidth();
            float lineHeightPt;
            float[] linesWidthPt;
            var lines = font.BreakText(text, size, availableWidthPt, out linesWidthPt, out lineHeightPt);

            lineHeightPt += spacing;
            if (_contentHeightPt != 0)
                _contentHeightPt += spacebefore;

            int x = (int)(CurrentX() + indent);
            for (int iLine = 0, lineCount = lines.Count; iLine < lineCount; ) {
                int maxLineCount = (int)(AvailableHeight() / lineHeightPt);
                int chunkLineCount = Math.Min(lineCount - iLine, maxLineCount);
                if (chunkLineCount < 1) {
                    this.AddPage();
                    continue;
                }
                int y = CurrentY();
                if (center) {
                    int xx = x + (availableWidthPt / 2);
                    _writer.WriteLinesCenter(lines, iLine, chunkLineCount
                        , xx, y, font, size, rgb, linesWidthPt, lineHeightPt);
                } else {
                    _writer.WriteLines(lines, iLine, chunkLineCount
                        , x, y, font, size, rgb, lineHeightPt);
                }
                iLine += chunkLineCount;
                _contentHeightPt += (int)(chunkLineCount * lineHeightPt);
            }
            _contentHeightPt += spaceAfter;
        }

        /// <summary>
        /// Adds an image at the current position
        /// </summary>
        /// <param name="image"></param>
        /// <param name="autoDispose">Release the resources of the image if true</param>
        public void AddImage(global::Selenium.Image image, bool autoDispose = true) {
            EnsureNotDisposed();
            Bitmap bitmap = image.GetBitmap();
            int imgWidthPt = (int)(72.0 * bitmap.Width / bitmap.HorizontalResolution);
            int imgHeightPt = (int)(72.0 * bitmap.Height / bitmap.VerticalResolution);
            int availableWidthPt = AvailableWidth();
            if (imgWidthPt > availableWidthPt) {
                imgHeightPt = (imgHeightPt * availableWidthPt) / imgWidthPt;
                imgWidthPt = availableWidthPt;
            }
            int imgOffsetPt = 0;
            int x = CurrentX();
            while (true) {
                int availableHeightPt = AvailableHeight();
                if (availableHeightPt < IMG_SPLIT_HEIGHT_MIN_PT
                    && imgHeightPt > IMG_SPLIT_HEIGHT_MIN_PT) {
                    this.AddPage();
                    continue;
                }
                int chunkHeightPt = Math.Min(availableHeightPt, imgHeightPt - imgOffsetPt);

                int y = CurrentY() - chunkHeightPt;
                if (imgHeightPt != chunkHeightPt) {
                    int chunkHeightPx = (bitmap.Height * chunkHeightPt) / imgHeightPt;
                    int chunkOffsetPx = (bitmap.Height * imgOffsetPt) / imgHeightPt;
                    var rect = new System.Drawing.Rectangle(0, chunkOffsetPx, bitmap.Width, chunkHeightPx);
                    using (var imgChunk = bitmap.Clone(rect, PixelFormat.Format24bppRgb)) {
                        _writer.WriteBitmap(imgChunk, x, y, imgWidthPt, chunkHeightPt);
                    }
                    imgOffsetPt += chunkHeightPt;
                    if (imgOffsetPt >= imgHeightPt) {
                        _contentHeightPt += chunkHeightPt;
                        break;
                    }
                    imgOffsetPt -= 5;
                    this.AddPage();
                } else {
                    _writer.WriteBitmap(bitmap, x, y, imgWidthPt, imgHeightPt);
                    _contentHeightPt += imgHeightPt;
                    break;
                }
            }
            _contentHeightPt += 8;
            if (autoDispose)
                image.Dispose();
        }

        /// Content available width in points
        private int AvailableHeight() {
            return _pageHeightPt - _marginTopPt - _contentHeightPt - _marginBottomPt;
        }

        /// <summary>
        /// Content available height in points
        /// </summary>
        /// <returns></returns>
        private int AvailableWidth() {
            return _pageWidthPt - _marginLeftPt - _marginRightPt;
        }

        /// <summary>
        /// Current y position in points from the bottom
        /// </summary>
        /// <returns></returns>
        private int CurrentY() {
            return _pageHeightPt - _marginTopPt - _contentHeightPt;
        }

        /// <summary>
        /// Current x position in points from the left
        /// </summary>
        /// <returns></returns>
        private int CurrentX() {
            return _marginLeftPt;
        }

        /// <summary>
        /// Get or set the metric: millimeter=mm, inch=in or point=pt.
        /// </summary>
        private static float GetRatio(string metric) {
            switch (metric) {
                case "pt": return 1f;
                case "in": return 72f;
                case "mm": return 72f / 25.4f;
                default: throw new Errors.PdfError("Invalid metric: " + metric);
            }
        }

        /// <summary>
        /// Parse a color to a value. Can be a name or an RGB hexa value.
        /// Ex : "red", "FF0000" 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static int ParseColorToRGB(string color) {
            if (color == null || color.Length == 0)
                return 0;
            int rgb;
            if (int.TryParse(color, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out rgb))
                return rgb;
            try {
                return System.Drawing.Color.FromName(color).ToArgb();
            } catch {
                throw new Errors.PdfError("Invalid color: " + color);
            }
        }

    }

}
