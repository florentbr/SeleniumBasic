using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-61DAD6C51012")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Keyboard {

        [DispId(728), Description("Returns a list of pressable keys.")]
        Keys Keys { get; }

        [DispId(730), Description("Sends a sequence of key strokes to the active element.")]
        Keyboard SendKeys(string keysOrModifiers, string keys = null);

        [DispId(732), Description("Presses and holds modifier keys (Control, Alt and Shift).")]
        Keyboard KeyDown(string modifiers);

        [DispId(734), Description("Release modifier keys (Control, Alt and Shift).")]
        Keyboard KeyUp(string modifiers);

    }

}
