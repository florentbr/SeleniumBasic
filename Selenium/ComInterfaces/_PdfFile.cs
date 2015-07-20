using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-F2A56C3A68D4")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _PdfFile {

        [DispId(407), Description("Adds an image with borders to the Pdf")]
        void AddImage([MarshalAs(UnmanagedType.Struct)]Image image, bool autoDispose = true);

        [DispId(412), Description("Adds a new page")]
        void AddPage();

        [DispId(416), Description("Adds a bookmark for the current position")]
        void AddBookmark(string name);

        [DispId(420), Description("Adds a vertical space")]
        void AddSpace(int size);

        [DispId(432), Description("Adds a clickable link")]
        void AddLink(string link, string text = null, int size = 10);

        [DispId(437), Description("Adds a text to the Pdf")]
        void AddText(string text, short size = 10, bool bold = false, bool italic = false, string color = null);

        [DispId(440), Description("Adds a centered text to the Pdf")]
        void AddTextCenter(string text, short size = 10, bool bold = false, bool italic = false, string color = null);

        [DispId(460), Description("Adds a title")]
        void AddTitle(string text, short size = 11, bool bold = true, bool italic = false);

        [DispId(465), Description("Saves the pdf")]
        void SaveAs(string filepath);

        [DispId(470), Description("Set the page margins (inch by default)")]
        void SetMargins(int left, int right, int top, int bottom, string metric = "in");

        [DispId(478), Description("Set the page size (inch by default)")]
        void SetPageSize(int width, int heigth, string metric = "in");

        [DispId(9999), Description("Releases the resources")]
        void Dispose();

    }

}
