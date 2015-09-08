using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-ACE280CD7780")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Point {

        [DispId(43), Description("X")]
        int X { get; }

        [DispId(45), Description("Y")]
        int Y { get; }

    }

}
