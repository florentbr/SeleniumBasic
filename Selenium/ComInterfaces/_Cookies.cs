using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-E6E7ED329824")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Cookies : System.Collections.IEnumerable {

        [DispId(0), Description("Get an item for a name or index (One-based)")]
        Cookie this[object identifier] { get; }

        [DispId(128), Description("Number of items")]
        int Count { get; }

        [DispId(-4)]
        System.Collections.IEnumerator _NewEnum();  //Has to be declared last

    }

}
