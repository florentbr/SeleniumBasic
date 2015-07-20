using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-D5DE929CF018")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _TouchActions {

        [DispId(6112), Description("")]
        TouchActions Wait(int timems);

        [DispId(6117), Description("")]

        TouchActions Tap([MarshalAs(UnmanagedType.Struct)]WebElement element);

        [DispId(6121), Description("")]
        TouchActions TapDouble([MarshalAs(UnmanagedType.Struct)]WebElement element);

        [DispId(6125), Description("")]
        TouchActions PressHold(int locationX, int locationY);

        [DispId(6129), Description("")]
        TouchActions PressRelease(int locationX, int locationY);

        [DispId(6131), Description("")]
        TouchActions TapLong([MarshalAs(UnmanagedType.Struct)]WebElement element);

        [DispId(6137), Description("")]
        TouchActions Flick(int speedX, int speedY);

        [DispId(6143), Description("")]
        TouchActions FlickFrom([MarshalAs(UnmanagedType.Struct)]WebElement element, int offsetX, int offsetY, int speed);

        [DispId(6148), Description("")]
        TouchActions Move(int locationX, int locationY);

        [DispId(6153), Description("")]
        TouchActions Scroll(int offsetX, int offsetY);

        [DispId(6159), Description("")]
        TouchActions ScrollFrom([MarshalAs(UnmanagedType.Struct)]WebElement element, int offsetX, int offsetY);

        [DispId(6165), Description("Performs all stored Actions.")]
        void Perform();

    }

}
