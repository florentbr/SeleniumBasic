using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-63F894CA99E9")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Mouse {

        [DispId(231), Description("")]
        Mouse Click(MouseButton button = MouseButton.Left);

        [DispId(234), Description("")]
        Mouse ClickDouble();

        [DispId(237), Description("")]
        Mouse ClickAndHold(MouseButton button = MouseButton.Left);

        [DispId(239), Description("")]
        Mouse Release(MouseButton button = MouseButton.Left);

        [DispId(243), Description("")]
        Mouse MoveTo([MarshalAs(UnmanagedType.Struct)]WebElement element, int xoffset = 0, int yoffset = 0);

    }

}
