using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-FBDA3A91C82B")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Window {

        [DispId(500), Description("Set the context to this window.")]
        Window Activate();

        [DispId(533), Description("Gets the size of the outer browser window")]
        Size Size();

        [DispId(536), Description("Switch to the window with the provided name")]
        Window SwitchToWindowByName(string name, int timeout = -1);

        [DispId(537), Description("Switch to the window with the provided title")]
        Window SwitchToWindowByTitle(string title, int timeout = -1);

        [DispId(538), Description("Switch to the next window")]
        Window SwitchToNextWindow(int timeout = -1);

        [DispId(540), Description("Switch to the previous window")]
        Window SwitchToPreviousWindow();

        [DispId(544), Description("Sets the size of the outer browser window, including title bars and window borders.")]
        void SetSize(int width, int height);

        [DispId(550), Description("Get the title of the window")]
        string Title { get; }

        [DispId(554), Description("Gets the position of the browser window relative to the upper-left corner of the screen.")]
        Point Position();

        [DispId(557), Description("Sets the position of the browser window relative to the upper-left corner of the screen.")]
        void SetPosition(int x, int y);

        [DispId(562), Description("Maximizes the current window if it is not already maximized")]
        void Maximize();

        [DispId(567), Description("Set the current window full screen")]
        void FullScreen();

        [DispId(574), Description("Closes the current window")]
        void Close();

    }

}
