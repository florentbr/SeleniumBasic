using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Selenium.Pdf {

    class PdfFont {

        private static int id = 0;

        public readonly string CodeName;
        public readonly string Name;
        public readonly short[] WidthsTs;
        public readonly short HeightTs;

        /// <summary>
        /// Font object
        /// </summary>
        /// <param name="graphics">GDI+ drawing surface</param>
        /// <param name="fontFamilly">Font familly</param>
        /// <param name="style">Font style</param>
        /// <param name="name">Font name</param>
        public PdfFont(Graphics graphics, FontFamily fontFamilly, FontStyle style, string name) {
            this.CodeName = "/F" + id++;
            this.Name = name;
            this.HeightTs = (short)((fontFamilly.GetLineSpacing(style) * 1000) / fontFamilly.GetEmHeight(style));
            this.WidthsTs = GetCharsWidth(graphics, fontFamilly, style);
        }

        /// <summary>
        /// Gets the width in points for the input text.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="size">Font size in points</param>
        /// <returns></returns>
        public float MesureWidth(string text, int size) {
            short[] widths = this.WidthsTs;
            int width = 0;
            for (int i = 0; i < text.Length; i++) {
                char c = text[i];
                if (c > 255) {
                    //unsupported character
                    if (c >= 0xD800 && c < 0xE000) //if surrogate char
                        i++;
                    c = '?';  //out of range char
                }
                width += widths[c];
            }
            return (width * size) / 999f;
        }

        /// <summary>
        /// Breaks a text to a maximum line width.
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="size">Font size in points</param>
        /// <param name="breakWidthPt">Maximum width in points</param>
        /// <param name="linesWidthsPt">Output - Widths in point for each line</param>
        /// <param name="lineHeightPt">Output - Height in point of a line</param>
        /// <returns>List of strings</returns>
        public List<string> BreakText(string text, int size, int breakWidthPt
            , out float[] linesWidthsPt, out float lineHeightPt) {

            List<string> lines = new List<string>();
            List<float> widths = new List<float>();
            int maxWidth = (int)(breakWidthPt * (1001f / size));
            float ratioPtTs = 999f / size;
            int charsCount = text.Length;
            short[] charsWidth = this.WidthsTs;
            char[] buffer = new char[charsCount];
            int bufferLen = 0;
            int chunkWidthTry = 0;
            int chunkWidth = 0;
            int chunkStart = 0;
            int chunkEnd = 0;

            for (int i = 0; i < charsCount; i++) {
                char c = text[i];
                if (c > 255) {
                    //unsupported character
                    if (c >= 0xD800 && c < 0xE000) //if surrogate character
                        i++;
                    c = '?';  //character out of range
                } else if (c == '\r' || c == '\n') {
                    //line breaking, add the buffered string
                    lines.Add(new string(buffer, chunkStart, bufferLen - chunkStart));
                    widths.Add((float)(chunkWidthTry / ratioPtTs));
                    chunkStart = chunkEnd = bufferLen;
                    chunkWidth = chunkWidthTry = 0;
                    //skips the next character if it's a CRLF sequence
                    if (c == '\r' && i + 1 < charsCount && text[i + 1] == '\n')
                        i++;
                    continue;
                }
                int charWidth = charsWidth[c];
                if (chunkWidthTry + charWidth > maxWidth) {
                    //max width reached
                    if (chunkEnd == chunkStart) {
                        //break the word as it doesn't fit in the maximum width
                        lines.Add(new string(buffer, chunkStart, bufferLen - chunkStart));
                        widths.Add((float)(chunkWidthTry / ratioPtTs));
                        chunkStart = chunkEnd = bufferLen;
                        chunkWidthTry = 0;
                    } else {
                        //add the buffered string
                        lines.Add(new string(buffer, chunkStart, chunkEnd - chunkStart));
                        widths.Add((float)(chunkWidth / ratioPtTs));
                        chunkStart = ++chunkEnd;
                        chunkWidthTry -= chunkWidth;
                    }
                    chunkWidth = 0;
                }
                if (c == ' ') {
                    //save the position and width
                    chunkEnd = bufferLen;
                    chunkWidth = chunkWidthTry;
                }
                chunkWidthTry += charWidth;
                buffer[bufferLen++] = c;
            }

            //add the remaining characters if any
            if (chunkStart < bufferLen) {
                lines.Add(new string(buffer, chunkStart, bufferLen - chunkStart));
                widths.Add((float)(chunkWidthTry / ratioPtTs));
            }

            //returns the results
            lineHeightPt = this.HeightTs / ratioPtTs;
            linesWidthsPt = widths.ToArray();
            return lines;
        }


        /// <summary>
        /// Returns the widths of each character of the font in logical units x1000.
        /// </summary>
        /// <param name="graphics">GDI+ surface drawing</param>
        /// <param name="fontFamilly">Font familly</param>
        /// <param name="fontStyle">Font style</param>
        private static unsafe short[] GetCharsWidth(Graphics graphics, FontFamily fontFamilly, FontStyle fontStyle) {
            const int charsCount = 256;
            using (Font font = new Font(fontFamilly, 1000, fontStyle, GraphicsUnit.Pixel, 0)) {
                IntPtr hDC = graphics.GetHdc();
                IntPtr hFont = font.ToHfont();
                IntPtr hFontPreviouse = NativeMethods.SelectObject(hDC, hFont);

                // Buffer :
                // int abcA     //entry 1
                // int abcB
                // int abcC
                // int abcA     //entry 2
                // ...

                IntPtr buffer = Marshal.AllocHGlobal(3 * charsCount * sizeof(int)); // n int * 4 bytes
                try {
                    if (!NativeMethods.GetCharABCWidths(hDC, (uint)0, (uint)charsCount - 1, buffer))
                        throw new Exception("Failed to retrieve the widths for the font " + font.Name);

                    int* widthsABC = (int*)buffer;
                    //Sum the widths: A + B + C
                    short[] widthsTs = new short[charsCount];
                    for (uint i = 0, p = 0; i < charsCount; i++, p += 3) {
                        widthsTs[i] = (short)(widthsABC[p] + widthsABC[p + 1] + widthsABC[p + 2]);
                    }
                    return widthsTs;
                } finally {
                    Marshal.FreeHGlobal(buffer);
                    NativeMethods.SelectObject(hDC, hFontPreviouse);
                    NativeMethods.DeleteObject(hFont);
                    graphics.ReleaseHdc(hDC);
                }
            }
        }


        static class NativeMethods {

            const string GDI32 = "gdi32.dll";

            [DllImport(GDI32, SetLastError = true)]
            public static extern bool GetCharABCWidths(IntPtr hdc, uint uFirstChar, uint uLastChar, IntPtr lpabc);

            [DllImport(GDI32)]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

            [DllImport(GDI32)]
            public static extern bool DeleteObject(IntPtr objectHandle);

        }

    }

}
