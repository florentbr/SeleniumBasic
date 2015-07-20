using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-A3DE5685A27E")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _By {

        [DispId(5), Description("Current strategy.")]
        string Strategy { get; }

        [DispId(7), Description("Value of the current strategy.")]
        string Value { get; }

        [DispId(20), Description("Finds elements based on the HTML class name attribute.")]
        By ClassName(string className);

        [DispId(21), Description("Finds elements using a CSS expression.")]
        By CssSelector(string cssSelector);

        [DispId(22), Description("Finds elements based on the HTML id attribute.")]
        By Id(string id);

        [DispId(23), Description("Finds elements matching the given text.")]
        By LinkText(string linkText);

        [DispId(24), Description("Finds elements based on the HTML name attribute.")]
        By Name(string name);

        [DispId(25), Description("Finds elements containing the given text.")]
        By PartialLinkText(string partialLinkText);

        [DispId(26), Description("Finds elements based on the HTML tag name")]
        By TagName(string tagName);

        [DispId(27), Description("Finds elements using an XPATH expression.")]
        By XPath(string xpath);

        [DispId(50), Description("Finds elements by using multiple strategies")]
        By Any([MarshalAs(UnmanagedType.Struct)]By by1
            , [MarshalAs(UnmanagedType.Struct)]By by2
            , [MarshalAs(UnmanagedType.Struct)]By by3 = null
            , [MarshalAs(UnmanagedType.Struct)]By by4 = null
            , [MarshalAs(UnmanagedType.Struct)]By by5 = null
            , [MarshalAs(UnmanagedType.Struct)]By by6 = null);

    }

}
