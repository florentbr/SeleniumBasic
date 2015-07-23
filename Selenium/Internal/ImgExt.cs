using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Selenium.Internal {

    [ComImport(), Guid("7BF80981-BF32-101A-8BBB-00AA00300CAB")]
    public interface IStdPicture { }

    class ImgExt {

        public static IStdPicture ImageToPictureDisp(Bitmap bitmap) {
            var pictDescBitmap = new NativeMethods.PictDescBitmap(bitmap);
            object ppVoid = null;
            Guid guid = typeof(IStdPicture).GUID;
            NativeMethods.OleCreatePictureIndirect(pictDescBitmap, ref guid, true, out ppVoid);
            return (IStdPicture)ppVoid;
        }

        public static Bitmap RemoveAlphaChannel(Bitmap bitmapSrc) {
            Rectangle rect = new Rectangle(0, 0, bitmapSrc.Width, bitmapSrc.Height);
            Bitmap bitmapDest = (Bitmap)new Bitmap(bitmapSrc.Width, bitmapSrc.Height, PixelFormat.Format24bppRgb);
            BitmapData dataSrc = bitmapSrc.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataDest = bitmapDest.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            NativeMethods.CopyMemory(dataDest.Scan0, dataSrc.Scan0, (uint)dataSrc.Stride * (uint)dataSrc.Height);
            bitmapSrc.UnlockBits(dataSrc);
            bitmapDest.UnlockBits(dataDest);
            return bitmapDest;
        }

        static class NativeMethods {

            const string KERNEL32 = "Kernel32.dll";
            const string OLEAUTH32 = "oleaut32.dll";

            [DllImport(KERNEL32)]
            public extern static void CopyMemory(IntPtr dest, IntPtr src, uint length);

            [StructLayout(LayoutKind.Sequential)]
            public class PictDescBitmap {
                internal int cbSizeOfStruct = Marshal.SizeOf(typeof(PictDescBitmap));
                internal int pictureType = 1;
                internal IntPtr hBitmap = IntPtr.Zero;
                internal IntPtr hPalette = IntPtr.Zero;
                internal int unused = 0;

                internal PictDescBitmap(Bitmap bitmap) {
                    this.hBitmap = bitmap.GetHbitmap();
                }
            }

            [DllImport(OLEAUTH32, PreserveSig = true)]
            public static extern int OleCreatePictureIndirect(
                [In] PictDescBitmap pictdesc, ref Guid iid, bool fOwn,
                [MarshalAs(UnmanagedType.Interface)] out object ppVoid);

        }

    }

}
