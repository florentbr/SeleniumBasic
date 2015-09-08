using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-7E2EBB6C82E9")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Size {

        [DispId(43), Description("Width")]
        int Width { get; }

        [DispId(45), Description("Height")]
        int Height { get; }

    }
}
