using Interop.Excel;
using Selenium.Internal;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-54BA7C175990")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Image {

        [DispId(206), Description("Load an image file.")]
        Image Load(string filePath);

        [DispId(211), Description("Resize the image.")]
        Image Resize(float width = 0, float height = 0);

        [DispId(214), Description("Image width.")]
        short Width { get; }

        [DispId(218), Description("Image height.")]
        short Height { get; }

        [DispId(220), Description("Number of non matching pixels resulted from a comparison.")]
        int DiffCount { get; }

        [DispId(230), Description("Returns the image CRC32 hash value in hexa.")]
        string CRC { get; }

        [DispId(300), Description("Compare this image to another image(path or image) and return the result.")]
        Image CompareTo(object image, bool autoCenter = false, int offsetX = 0, int offsetY = 0, bool autoDispose = true);

        [DispId(320), Description("Copy the image to the clipboard.")]
        void Copy(bool autoDispose = true);

        [DispId(330), Description("Save the image to a file. Supported format: png, bmp, gif and jpg.")]
        string SaveAs(string filePath, bool autoDispose = true);

        [DispId(350), Description("Paste the image to excel and returns the range")]
        IRange ToExcel(object target = null, bool autoDispose = true);

        [DispId(389), Description("Returns a native StdPicture object")]
        Interop.IStdPicture GetPicture(bool autoDispose = true);

        [DispId(9999), Description("Dispose the image resources.")]
        void Dispose();

    }

}
