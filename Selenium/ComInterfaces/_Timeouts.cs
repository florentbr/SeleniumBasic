using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-74F5D5680428")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Timeouts {

        [DispId(476), Description("Amount of time that Selenium will wait for waiting commands to complete")]
        int ImplicitWait { get; set; }

        [DispId(459), Description("Amount of time the driver should wait while loading a page before throwing an exception.")]
        int PageLoad { get; set; }

        [DispId(465), Description("Amount of time the driver should wait while executing a script before throwing an exception.")]
        int Script { get; set; }

        [DispId(489), Description("Maximum amount of time the driver should wait while waiting for a response from the server.")]
        int Server { get; set; }

    }
}
