using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-A398E67A519B")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface _DictionaryItem {

        [DispId(50)]
        string Key { get; }

        [DispId(57)]
        object Value { get; }

    }

}
