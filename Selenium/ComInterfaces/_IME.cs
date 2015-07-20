using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    //TODO : Include IME

    [Guid("0277FC34-FD1B-4616-BB19-030859831269")]
    [ComVisible(false), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _IME {

        [DispId(234), Description("Make an engines that is available (appears on the list returned by getAvailableEngines) active.")]
        void Activate(string engine);

        [DispId(236), Description("Get the name of the active IME engine.")]
        string ActiveEngine { get; }

        [DispId(238), Description("All available engines on the machine.")]
        List AvailableEngines { get; }

        [DispId(240), Description("De-activate IME input (turns off the currently activated engine).")]
        void Deactivate();

        [DispId(244), Description("Indicates whether IME input active at the moment (not if it's available).")]
        bool IsActivated { get; }

    }

}
