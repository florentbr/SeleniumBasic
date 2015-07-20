using System;
using System.Drawing;
using System.IO;

namespace Selenium.Pdf {

    static class PdfFonts {

        internal static readonly PdfFont FONT_REGULAR;
        internal static readonly PdfFont FONT_BOLD;
        internal static readonly PdfFont FONT_OBLIQUE;
        internal static readonly PdfFont FONT_BOLD_OBLIQUE;

        static PdfFonts() {
            using (var graphic = Graphics.FromHwnd(IntPtr.Zero))
            using (var fontFamilly = new FontFamily("Arial")) {
                FONT_REGULAR = new PdfFont(graphic, fontFamilly, FontStyle.Regular, "Helvetica");
                FONT_BOLD = new PdfFont(graphic, fontFamilly, FontStyle.Bold, "Helvetica-Bold");
                FONT_OBLIQUE = new PdfFont(graphic, fontFamilly, FontStyle.Italic, "Helvetica−Oblique");
                FONT_BOLD_OBLIQUE = new PdfFont(graphic, fontFamilly, FontStyle.Bold | FontStyle.Italic, "Helvetica−BoldOblique");
            }
        }

        /// <summary>
        /// Select a font from the predined ones.
        /// </summary>
        /// <param name="bold">True for a bold font</param>
        /// <param name="oblic">True for an italic font</param>
        /// <returns></returns>
        public static PdfFont Select(bool bold, bool oblic) {
            if (bold && oblic)
                return FONT_BOLD_OBLIQUE;
            if (bold)
                return FONT_BOLD;
            if (oblic)
                return FONT_OBLIQUE;
            return FONT_REGULAR;
        }

        public static void Write(StreamWriter writer, PdfXRefs xrefs) {
            PdfFont[] fonts = new[]{
                PdfFonts.FONT_REGULAR, 
                PdfFonts.FONT_BOLD,
                PdfFonts.FONT_OBLIQUE, 
                PdfFonts.FONT_BOLD_OBLIQUE
            };
            int[] fontIds = new int[fonts.Length];

            for (int i = 0; i < fonts.Length; i++) {
                fontIds[i] = xrefs.CreateAndRegisterObject();

                writer.WriteLine(fontIds[i] + " 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("/Type /Font");
                writer.WriteLine("/Subtype /Type1");
                writer.WriteLine("/BaseFont /" + fonts[i].Name);
                writer.WriteLine("/Encoding /WinAnsiEncoding");
                /*
                _pdfWriter.Write("/FirstChar 0");
                _pdfWriter.Write("/LastChar 255");
                _pdfWriter.Write("/Widths[");
                foreach (short w in font.WidthsTs) {
                    _pdfWriter.Write(w);
                    _pdfWriter.Write(' ');
                }
                _pdfWriter.Write(']');
                */
                writer.WriteLine(">>");
                writer.WriteLine("endobj");
            }

            xrefs.RegisterObject(PdfXRefs.ID_FONTS);
            writer.WriteLine(PdfXRefs.ID_FONTS + " 0 obj");
            writer.Write("<< ");
            for (int i = 0; i < fonts.Length; i++) {
                writer.Write(fonts[i].CodeName + " " + fontIds[i] + " 0 R ");
            }
            writer.WriteLine(">>");
            writer.WriteLine("endobj");
        }

    }

}
