using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    //TODO : Include logs

    [Guid("0277FC34-FD1B-4616-BB19-F5671F493AAE")]
    [ComVisible(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Logs {

        [DispId(47), Description("Get the log entries for the browser")]
        List Browser { get; }

        [DispId(49), Description("Get the log entries for the client")]
        List Client { get; }

        [DispId(51), Description("Get the log entries for the driver")]
        List Driver { get; }

        [DispId(53), Description("Get the log entries for the server")]
        List Server { get; }

        [DispId(55), Description("Get available log types.")]
        List Types { get; }

    }

}
