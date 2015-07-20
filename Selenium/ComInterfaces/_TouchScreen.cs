using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-FFD6FAEF290A")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _TouchScreen {

        [DispId(3423), Description("")]
        bool IsPortrait();

        [DispId(3427), Description("")]
        void ToLandscape();

        [DispId(3429), Description("")]
        void ToPortrait();

        [DispId(3432), Description("")]
        TouchScreen FlickFrom([MarshalAs(UnmanagedType.Struct)]WebElement element, int xoffset, int yoffset, int speed);

        [DispId(3435), Description("")]
        TouchScreen Flick(int xspeed, int yspeed);

        [DispId(3438), Description("")]
        TouchScreen Move(int x, int y);

        [DispId(3442), Description("")]
        TouchScreen PressHold(int x, int y);

        [DispId(3445), Description("")]
        TouchScreen PressRelease(int x, int y);

        [DispId(3447), Description("")]
        TouchScreen ScrollFrom([MarshalAs(UnmanagedType.Struct)]WebElement element, int xoffset, int yoffset);

        [DispId(3449), Description("")]
        TouchScreen Scroll(int xoffset, int yoffset);

        [DispId(3453), Description("")]
        TouchScreen Tap([MarshalAs(UnmanagedType.Struct)]WebElement element);

        [DispId(3459), Description("")]
        TouchScreen TapDouble([MarshalAs(UnmanagedType.Struct)]WebElement element);

        [DispId(3465), Description("")]
        TouchScreen TapLong([MarshalAs(UnmanagedType.Struct)]WebElement element);

    }

}
