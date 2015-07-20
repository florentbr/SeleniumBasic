using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-C6F450B6EE52")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Storage {

        [DispId(0), Description("Get or set the storage item for the given key.")]
        string this[string key] { get; set; }

        [DispId(128), Description("Get the number of items in the storage.")]
        int Count();

        [DispId(705), Description("Get all keys of the storage.")]
        List Keys();

        [DispId(709), Description("Clear the storage.")]
        void Clear();

        [DispId(713), Description("Remove the storage item for the given key.")]
        void Remove(string key);
    }
}
