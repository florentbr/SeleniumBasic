using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-384C7E50EFA8")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Waiter {

        [DispId(567), Description("Returns a boolean to continue looping and throws an exception if the timeout(ms) is reached.")]
        bool Not(object result, int timeout = -1, string timeoutMessage = null);

        [DispId(789), Description("Set the waiter timeout. Default is 30000 milliseconds")]
        int Timeout { get; set; }

        [DispId(798), Description("Waits the given time in milliseconds")]
        void Wait(int timems);

        [DispId(830), Description("Waits for a function(a,b) to return true.")]
        object Until(object function, object argument = null, int timeout = -1, string reserved = null);

        [DispId(876), Description("Waits for a file to be ready.")]
        void WaitForFile(string path, int timeout = -1);
    }

}
