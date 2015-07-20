using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-D0E30A5D0697")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Proxy {

        [DispId(615), Description("")]
        void SetAutoConfigure(string url);

        [DispId(617), Description("")]
        void SetAutodetect();

        [DispId(620), Description("")]
        void SetFTPProxy(string value);

        [DispId(622), Description("")]
        void SetHttpProxy(string value);

        [DispId(625), Description("")]
        void SetSocksProxy(string value, string username = null, string password = null);

        [DispId(628), Description("")]
        void SetSSLProxy(string value);

        [DispId(-4)]
        System.Collections.IEnumerator _NewEnum();  //Has to be declared last

    }

}
