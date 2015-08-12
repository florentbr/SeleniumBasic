using Interop.Excel;
using Selenium.Internal;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Selenium {

    /// <summary>
    /// Image object
    /// </summary>
    /// <example>
    /// Take a screenshot and save it in a file on the desktop :
    /// <code lang="vbs">	
    /// driver.TakeScreenShoot().SaveAs "%USERPROFILE%\Desktop\capture_{yyyyMMdd-HHmmss}.png"
    /// </code>
    /// </example>
    [ProgId("Selenium.Image")]
    [Guid("0277FC34-FD1B-4616-BB19-2853F52DAD14")]
    [Description("Image object")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Image : ComInterfaces._Image, IDisposable {

        private System.Drawing.Bitmap _bitmap = null;
        private uint _crc32 = 0;
        private int _diffCount = 0;

        /// <summary>
        /// Creates an image object from bytes.
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        public Image(byte[] bytes) {
            if (bytes == null || bytes.Length == 0)
                throw new Errors.NullImageError();
            using (var stream = new MemoryStream(bytes)) {
                _bitmap = (Bitmap)Bitmap.FromStream(stream, false, false);
            }
            COMDisposable.Subscribe(this, typeof(ComInterfaces._Image));
        }

        /// <summary>
        /// Creates an image object from a bitmap
        /// </summary>
        /// <param name="image"></param>
        public Image(Bitmap image) {
            _bitmap = image;
            COMDisposable.Subscribe(this, typeof(ComInterfaces._Image));
        }

        /// <summary>
        /// Returns the underlying Bitmap
        /// </summary>
        public Bitmap GetBitmap() {
            EnsureNotDisposed();
            return _bitmap;
        }

        /// <summary>
        /// Empty the image ans release all the ressources.
        /// </summary>
        public void Dispose() {
            if (_bitmap != null)
                _bitmap.Dispose();
            _bitmap = null;
            _crc32 = 0;
        }

        private void EnsureNotDisposed() {
            if (_bitmap == null)
                throw new Errors.NullImageError();
        }

        /// <summary>
        /// Explicit casting to a Bitmap
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static explicit operator System.Drawing.Bitmap(Image image) {
            return image._bitmap;
        }

        /// <summary>
        /// Image width.
        /// </summary>
        public short Width {
            get {
                return (short)(_bitmap == null ? 0 : _bitmap.Width);
            }
        }

        /// <summary>
        /// Image height.
        /// </summary>
        public short Height {
            get {
                return (short)(_bitmap == null ? 0 : _bitmap.Height);
            }
        }

        /// <summary>
        /// Number of non matching pixels resulted from a comparison.
        /// </summary>
        public int DiffCount {
            get {
                return _diffCount;
            }
        }

        /// <summary>
        /// Load an image file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Self</returns>
        public Image Load(string filePath) {
            this.Dispose();
            filePath = IOExt.ExpandPath(filePath);
            if (!File.Exists(filePath))
                throw new Errors.ArgumentError("File not found " + filePath);
            _bitmap = (Bitmap)Bitmap.FromFile(filePath);
            return this;
        }

        /// <summary>
        /// Save the image to a file. Supported format: png, bmp, gif and jpg.
        /// </summary>
        /// <param name="filePath">File path. Ex: "C:\capture_{yyyyMMdd-HHmmss}.png"</param>
        /// <param name="autoDispose">Release the image resources once done</param>
        /// <returns>Full file path</returns>
        public string SaveAs(string filePath, bool autoDispose = true) {
            EnsureNotDisposed();
            try {
                if (filePath.IndexOf('{') != -1){
                    filePath = Regex.Replace(filePath, @"\{([^}]+)\}", 
                        (m)=> DateTime.UtcNow.ToString(m.Groups[1].Value));
                }

                filePath = IOExt.ExpandPath(filePath);
                filePath = Path.GetFullPath(filePath);
                string folder = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                File.Create(filePath).Dispose();

                ImageFormat format;
                string ext = Path.GetExtension(filePath).ToLower();
                switch(ext){
                    case ".bmp": format = ImageFormat.Bmp; break;
                    case ".png": format = ImageFormat.Png; break;
                    case ".jpg": format = ImageFormat.Jpeg; break;
                    case ".jpeg": format = ImageFormat.Jpeg; break;
                    case ".gif": format = ImageFormat.Gif; break;
                    default:
                        throw new Errors.ImageError("Format not supported. Supported: bmp, png, jpg, gif.");
                }
                _bitmap.Save(filePath, format);
                
                if (autoDispose)
                    Dispose();
            } catch (Exception ex) {
                throw new Errors.ImageError(ex.Message);
            }
            return filePath;
        }

        /// <summary>
        /// Copy the image to the Clipboard.
        /// </summary>
        /// <param name="autoDispose">Release the image resources once done</param>
        public void Copy(bool autoDispose = true) {
            EnsureNotDisposed();
            ClipboardExt.SetImage(_bitmap);
            if (autoDispose)
                Dispose();
        }

        /// <summary>
        /// Resizes the current image
        /// </summary>
        /// <param name="width">Pixel width or ratio</param>
        /// <param name="height">Pixel height or ratio</param>
        /// <returns>Return self</returns>
        public Image Resize(float width = 0, float height = 0) {
            EnsureNotDisposed();
            var img = _bitmap;
            int w = 0, h = 0;
            if (width > 10) {
                w = (int)width;
            } else if (width > 0) {
                w = (int)(img.Width * width);
            }
            if (height > 10) {
                h = (int)height;
            } else if (height > 0) {
                h = (int)(img.Height * height);
            }
            if (w == 0 && h != 0)
                w = (int)(img.Width * h / img.Height);
            if (h == 0 && w != 0)
                h = (int)(img.Height * w / img.Width);
            var newImg = new Bitmap(img, w, h);
            Dispose();
            _bitmap = newImg;
            return this;
        }

        /// <summary>
        /// Compare to the provided image
        /// </summary>
        /// <param name="image">Image path or Image object</param>
        /// <param name="center">Center the image B horizontally on image A</param>
        /// <param name="offsetX">Translates image B horizontally by the specified amount.</param>
        /// <param name="offsetY">Translates image B vertically by the specified amount.</param>
        /// <param name="autoDispose">Release the image resources once done</param>
        /// <returns>Output - Image representing all the differences</returns>
        public Image CompareTo(object image, bool center = false, int offsetX = 0, int offsetY = 0, bool autoDispose = true) {
            EnsureNotDisposed();
            Bitmap imgB = null;
            string imagePath = image as string;
            if (imagePath != null) {
                if (!File.Exists(imagePath))
                    throw new Errors.ArgumentError("File not found. Path:" + imagePath);
                imgB = new Bitmap(imagePath);
            } else {
                Image imageObj = image as Image;
                if (imageObj == null)
                    throw new Errors.ArgumentError("Invalid argument exception. A string or an Image object is required for the image argument.");
                imgB = imageObj.GetBitmap();
            }
            var imgA = _bitmap;
            Bitmap imgDiff;
            int error = CompareBitmaps(imgA, imgB, out imgDiff, center, offsetX, offsetY);
            if (autoDispose) {
                imgA.Dispose();
                imgB.Dispose();
            }
            return new Image(imgDiff) {
                _diffCount = error
            };
        }

        /// <summary>
        /// Returns the image CRC32 hash in hexadecimal.
        /// </summary>
        /// <returns>CRC32</returns>
        public string CRC {
            get {
                if (_bitmap == null)
                    return string.Empty;
                return this.GetCRC().ToString("X");
            }
        }

        private unsafe uint GetCRC() {
            if (_crc32 == 0 && _bitmap != null) {
                _crc32 = ComputeCRC32(_bitmap);
            }
            return _crc32;
        }

        /// <summary>
        /// Return true if images are the same.
        /// </summary>
        /// <param name="obj">Image object</param>
        /// <returns>True or False</returns>
        public override bool Equals(object obj) {
            Image img;
            if (obj == null || (img = obj as Image) == null)
                return false;
            return this.GetCRC() == img.GetCRC();
        }

        /// <summary>
        /// Returns object description
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "Image, CRC=" + this.CRC;
        }

        /// <summary>
        /// Returns the hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <summary>
        /// Insert the image in an Excel sheet.
        /// </summary>
        /// <param name="target">Excel address, worksheet, range or null to create a new sheet.</param>
        /// <param name="autoDispose">Release the image resources once done</param>
        /// <returns>Range</returns>
        public IRange ToExcel(object target = null, bool autoDispose = true) {
            IRange range = ExcelExt.GetRange(target);
            Clipboard.SetDataObject(_bitmap, false);
            range.Worksheet.Paste(range, _bitmap);
            Clipboard.Clear();
            if (autoDispose)
                _bitmap.Dispose();
            return range;
        }

        /// <summary>
        /// Returns a native Picture object
        /// </summary>
        /// <param name="autoDispose">Release the image resources once done</param>
        /// <returns></returns>
        Interop.IStdPicture ComInterfaces._Image.GetPicture(bool autoDispose) {
            Interop.IStdPicture picture = ImgExt.ImageToPictureDisp(_bitmap);
            if(autoDispose)
                _bitmap.Dispose();
            return picture;
        }

        #region Support

        /// <summary>
        /// Compare two images
        /// </summary>
        /// <param name="imageA">First image</param>
        /// <param name="imageB">Second image</param>
        /// <param name="imageDiff">Output - Image representing all the differences</param>
        /// <param name="center">Center the image B horizontally on image A</param>
        /// <param name="offsetX">Translates image B horizontally by the specified amount.</param>
        /// <param name="offsetY">Translates image B vertically by the specified amount.</param>
        /// <returns>Ratio of non matching pixels between 0 and 1</returns>
        private static unsafe int CompareBitmaps(Bitmap imageA, Bitmap imageB, out Bitmap imageDiff, bool center, int offsetX, int offsetY) {
            if (center)
                offsetX += (imageA.Width - imageB.Width) / 2;
            var rectA = new Rectangle(offsetX < 0 ? -offsetX : 0, offsetY < 0 ? -offsetY : 0, imageA.Width, imageA.Height);
            var rectB = new Rectangle(offsetX > 0 ? offsetX : 0, offsetY > 0 ? offsetY : 0, imageB.Width, imageB.Height);
            var rectIntersect = Rectangle.Intersect(rectA, rectB);
            var rectUnion = Rectangle.Union(rectA, rectB);
            imageDiff = new System.Drawing.Bitmap(rectUnion.Width, rectUnion.Height);

            int pixelDiffCount = 0;
            BitmapData dataA = imageA.LockBits(new Rectangle(System.Drawing.Point.Empty, rectA.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataB = imageB.LockBits(new Rectangle(System.Drawing.Point.Empty, rectB.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataDiff = imageDiff.LockBits(new Rectangle(System.Drawing.Point.Empty, rectUnion.Size), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            try {
                //Writes the difference between image A and B where they intersect
                int width = rectIntersect.Width;
                int height = rectIntersect.Height;
                byte* ptrA = (byte*)dataA.Scan0 + rectB.Top * dataA.Stride + rectB.Left * 3;
                byte* ptrB = (byte*)dataB.Scan0 + rectA.Top * dataB.Stride + rectA.Left * 3;
                byte* ptrDiff = (byte*)dataDiff.Scan0 + rectIntersect.Y * dataDiff.Stride + rectIntersect.X * 3;
                int padA = dataA.Stride - rectIntersect.Width * 3;
                int padB = dataB.Stride - rectIntersect.Width * 3;
                int padDiff = dataDiff.Stride - rectIntersect.Width * 3;
                for (int y = height; y-- > 0; ptrA += padA, ptrB += padB, ptrDiff += padDiff) {
                    for (int x = width; x-- > 0; ptrA += 3, ptrB += 3, ptrDiff += 3) {
                        if ((ptrDiff[0] = (byte)(ptrB[0] ^ ptrA[0]))
                            + (ptrDiff[1] = (byte)(ptrB[1] ^ ptrA[1]))
                            + (ptrDiff[2] = (byte)(ptrB[2] ^ ptrA[2])) > 50) {
                            pixelDiffCount++;
                        }
                    }
                }

                BitmapData dataR;
                Rectangle rectR;
                byte* ptrR;
                int padR;

                //Writes what's left on the top
                if (rectIntersect.Top != 0) {
                    dataR = rectA.Top == 0 ? dataA : dataB;
                    rectR = rectA.Top == 0 ? rectA : rectB;
                    width = rectR.Width;
                    height = rectIntersect.Y;
                    ptrR = (byte*)dataR.Scan0;
                    ptrDiff = (byte*)dataDiff.Scan0 + rectR.Left * 3;
                    padR = dataR.Stride - width * 3;
                    padDiff = dataDiff.Stride - width * 3;
                    pixelDiffCount += InvertColor(ptrR, padR, ptrDiff, padDiff, width, height);
                }

                //Writes what's left on the left
                if (rectIntersect.Left != rectUnion.Left) {
                    dataR = rectA.Left == 0 ? dataA : dataB;
                    rectR = rectA.Left == 0 ? rectA : rectB;
                    width = rectIntersect.X;
                    height = rectR.Height;
                    ptrR = (byte*)dataR.Scan0;
                    ptrDiff = (byte*)dataDiff.Scan0 + rectR.Top * dataDiff.Stride;
                    padR = dataR.Stride - width * 3;
                    padDiff = dataDiff.Stride - width * 3;
                    pixelDiffCount += InvertColor(ptrR, padR, ptrDiff, padDiff, width, height);
                }

                //Writes what's left on the right
                if (rectIntersect.Right != rectUnion.Right) {
                    dataR = rectA.Right == rectUnion.Right ? dataA : dataB;
                    rectR = rectA.Right == rectUnion.Right ? rectA : rectB;
                    width = (rectR.Right - rectIntersect.Right) * 3;
                    height = rectR.Bottom - rectIntersect.Top;
                    ptrR = (byte*)dataR.Scan0 + (rectIntersect.Top - rectR.Top) * dataR.Stride + (rectIntersect.Right - rectR.Left) * 3;
                    ptrDiff = (byte*)dataDiff.Scan0 + rectIntersect.Top * dataDiff.Stride + rectIntersect.Right * 3;
                    padR = dataR.Stride - width * 3;
                    padDiff = dataDiff.Stride - width * 3;
                    pixelDiffCount += InvertColor(ptrR, padR, ptrDiff, padDiff, width, height);
                }

                //Writes what's left on the bottom
                if (rectIntersect.Bottom != rectUnion.Bottom) {
                    dataR = rectA.Bottom == rectUnion.Bottom ? dataA : dataB;
                    rectR = rectA.Bottom == rectUnion.Bottom ? rectA : rectB;
                    width = rectIntersect.Width;
                    height = rectR.Bottom - rectIntersect.Bottom;
                    ptrR = (byte*)dataR.Scan0 + (rectIntersect.Bottom - rectR.Top) * dataR.Stride + (rectIntersect.Left - rectR.Left) * 3;
                    ptrDiff = (byte*)dataDiff.Scan0 + rectIntersect.Bottom * dataDiff.Stride + rectIntersect.Left * 3;
                    padR = dataR.Stride - width * 3;
                    padDiff = dataDiff.Stride - width * 3;
                    pixelDiffCount += InvertColor(ptrR, padR, ptrDiff, padDiff, width, height);
                }
            } finally {
                if (dataA != null)
                    imageA.UnlockBits(dataA);
                if (dataB != null)
                    imageB.UnlockBits(dataB);
                if (dataDiff != null)
                    imageDiff.UnlockBits(dataDiff);
            }
            return pixelDiffCount;
        }

        unsafe private static int InvertColor(byte* ptrR, int padR, byte* ptrW, int padW, int width, int height) {
            int unmatchingPixels = 0;
            for (int y = height; y-- > 0; ptrR += padR, ptrW += padW) {
                for (int x = width; x-- > 0; ptrR += 3, ptrW += 3) {
                    //Invert the color (255 - value)
                    if (((ptrW[0] = (byte)(ptrR[0] ^ 0xFF))
                        | (ptrW[1] = (byte)(ptrR[1] ^ 0xFF))
                        | (ptrW[2] = (byte)(ptrR[2] ^ 0xFF))
                        ) > 0) {
                        unmatchingPixels++;
                    }
                }
            }
            return unmatchingPixels;
        }



        /// <summary>
        /// Computes the Crc32 of a bitmap
        /// </summary>
        /// <param name="bitmap">Bitmap image</param>
        /// <returns>Crc32</returns>
        private static unsafe uint ComputeCRC32(Bitmap bitmap) {
            uint crc32 = uint.MaxValue;
            Rectangle rect = new Rectangle(System.Drawing.Point.Empty, bitmap.Size);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int height = bitmap.Height;
            int scanWidth = Math.Abs(data.Stride);
            int bytesWidth = bitmap.Width * (scanWidth / bitmap.Width);
            try {
                byte* ptr = (byte*)data.Scan0;
                fixed (uint* pTable = Crc32.CRCTABLE) {
                    for (int r = height; r-- > 0; ptr += scanWidth) {
                        for (int i = 0; i < bytesWidth; i++) {
                            crc32 = pTable[(crc32 ^ ptr[i]) & 0xFF] ^ (crc32 >> 8);
                        }
                    }
                }
            } finally {
                bitmap.UnlockBits(data);
            }
            return crc32 ^ uint.MaxValue;
        }

        #endregion

    }

}
