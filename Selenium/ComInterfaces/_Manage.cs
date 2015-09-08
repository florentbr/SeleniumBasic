using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-11660D7615B7")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Manage {

        [DispId(43), Description("")]
        Storage StorageLocal { get; }

        [DispId(44), Description("")]
        Storage StorageSession { get; }

        [DispId(55), Description("Get the current geo location.")]
        Dictionary Location();

        [DispId(56), Description("Get the current geo location.")]
        void SetLocation(int latitude, int longitude, int altitude);

        [DispId(69), Description("Collection of cookies for the current page")]
        Cookies Cookies { get; }

        [DispId(72), Description("Adds a cookie to the current page.")]
        void AddCookie(string name, string value, string domain = null, string path = null, string expiry = null, bool secure = false, bool httpOnly = false);

        [DispId(75), Description("Deletes all cookies from the page.")]
        void DeleteAllCookies();

        [DispId(79), Description("Deletes the cookie with the specified name from the page.")]
        void DeleteCookieByName(string name);

        [DispId(82), Description("Gets a cookie with the specified name.")]
        Cookie FindCookieByName(string name);

        //TODO : Include LOGS
        //[DispId(105), Description("")]
        //Logs Logs { get; }

        //TODO : Include IME
        //[DispId(132), Description("")]
        //IME IME { get; }

    }

}
