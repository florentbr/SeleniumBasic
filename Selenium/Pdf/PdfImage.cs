using Selenium.Internal;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Selenium.Pdf {

    static class PdfImage {

        private static long ENCODER_JPEG_QUALITY = 80L;
        private static readonly ImageCodecInfo ENCODER_PNG;
        private static readonly ImageCodecInfo ENCODER_JPEG;
        private static readonly EncoderParameters ENCODER_JPEG_PARAMS;

        static PdfImage() {

            //Cache the JPEG and PNG GDI+ encoders
            Guid guidPng = ImageFormat.Png.Guid;
            Guid guidJpeg = ImageFormat.Jpeg.Guid;
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders()) {
                if (codec.FormatID == guidJpeg) {
                    ENCODER_JPEG = codec;
                } else if (codec.FormatID == guidPng) {
                    ENCODER_PNG = codec;
                }
            }

            //Set the JPEG encoder Quality
            ENCODER_JPEG_PARAMS = new EncoderParameters(1);
            ENCODER_JPEG_PARAMS.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality, ENCODER_JPEG_QUALITY);
        }

        /// <summary>
        /// Encodes a bitmap to JPEG to a stream and returns the writen length.
        /// </summary>
        /// <param name="bitmap">Input bitmap</param>
        /// <param name="destStream">Stream holding the data</param>
        /// <returns>Lengths of the encoded data</returns>
        public static int WriteJPEG(Bitmap bitmap, Stream destStream) {
            //encode the bitmap in JPEG to the buffer
            int startPosition = (int)destStream.Position;
            bitmap.Save(destStream, ENCODER_JPEG, ENCODER_JPEG_PARAMS);
            int endPosition = (int)destStream.Position;

            //return the length
            return endPosition - startPosition;
        }

        /// <summary>
        /// Encodes a bitmap to PNG and writes it to a stream.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="destStream"></param>
        /// <returns>Lengths of the encoded data</returns>
        public static int WritePNG(Bitmap bitmap, Stream destStream) {
            //encode the bitmap to PNG
            MemoryStream buffer = new MemoryStream();
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb) {
                //Converts to a 3 channels RGB bitmmap as ARGB is not unsupported
                using (Bitmap tmpBitmap = ImgExt.RemoveAlphaChannel(bitmap)) {
                    tmpBitmap.Save(buffer, ENCODER_PNG, null);
                }
            } else {
                bitmap.Save(buffer, ENCODER_PNG, null);
            }
            var bytes = buffer.GetBuffer();

            //Since a PDF file doesn't support the PNG format, the encoded chunks need
            //to be extracted from it.
            //PNG chunk: [length 4 bytes BE][type 4 bytes][data of length][CRC 4bytes]
            int imageLength = 0;
            int offset = 8; //starts reading just after the 8 bytes signature
            int offsetBreak = (int)buffer.Position - 8;  //max length - min read length in a loop
            while (offset < offsetBreak) {
                //read the length on 4 bytes BE
                int chunkLength = ReadInt32BE(bytes, offset);
                //read the type on 4 bytes
                int chunkType = ReadInt32BE(bytes, offset + 4);
                if (chunkType == 0x49444154) { //if chunkType equals "IDAT"
                    //Writes the chunk to the stream
                    destStream.Write(bytes, offset + 8, chunkLength);
                    imageLength += chunkLength;
                }
                //move to the next chunk
                offset += chunkLength + 12;
            }
            return imageLength;
        }

        private static int ReadInt32BE(byte[] bytes, int offset) {
            return bytes[offset] << 24
                | bytes[offset + 1] << 16
                | bytes[offset + 2] << 8
                | bytes[offset + 3];
        }

    }

}
