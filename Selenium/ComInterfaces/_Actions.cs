using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-BBE48A6D09DB")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Actions {

        [DispId(401), Description("Waits the specified amount of time in milliseconds before executing the next action.")]
        Actions Wait(int timems);

        [DispId(403), Description("Clicks on an element or at the current position.")]
        Actions Click([MarshalAs(UnmanagedType.Struct)]WebElement element = null);

        [DispId(407), Description("Holds down the left mouse button on an element or at the current position.")]
        Actions ClickAndHold([MarshalAs(UnmanagedType.Struct)]WebElement element = null);

        [DispId(409), Description("Performs a context-click (right click) on an element or at the current position.")]
        Actions ClickContext([MarshalAs(UnmanagedType.Struct)]WebElement element = null);

        [DispId(412), Description("Double-clicks an element or at the current position.")]
        Actions ClickDouble([MarshalAs(UnmanagedType.Struct)]WebElement element = null);

        [DispId(417), Description("Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.")]
        Actions DragAndDrop([MarshalAs(UnmanagedType.Struct)]WebElement elementSource
            , [MarshalAs(UnmanagedType.Struct)]WebElement elementTarget);

        [DispId(419), Description("Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button. ")]
        Actions DragAndDropByOffset([MarshalAs(UnmanagedType.Struct)]WebElement elementSource, int offsetX, int offsetY);

        [DispId(421), Description("Sends a key press only, without releasing it. Should only be used with modifier keys (Control, Alt and Shift).")]
        Actions KeyDown(string modifierKeys, [MarshalAs(UnmanagedType.Struct)]WebElement element = null);

        [DispId(423), Description("Releases a modifier key.")]
        Actions KeyUp(string modifierKeys);

        [DispId(425), Description("Moving the mouse to an offset from current mouse position.")]
        Actions MoveByOffset(int offsetX, int offsetY);

        [DispId(427), Description("Moving the mouse to the middle of an element.")]
        Actions MoveToElement([MarshalAs(UnmanagedType.Struct)]WebElement element);

        [DispId(430), Description("Releasing a held mouse button.")]
        Actions Release([MarshalAs(UnmanagedType.Struct)]WebElement element = null);

        [DispId(433), Description("Sends keys to current focused element or provided element.")]
        Actions SendKeys(string keys, [MarshalAs(UnmanagedType.Struct)]WebElement element = null);

        [DispId(435), Description("Performs all stored Actions.")]
        void Perform();

    }

}