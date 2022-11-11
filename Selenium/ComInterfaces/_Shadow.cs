using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    /// <summary>
    /// Methods available to get child elements of a ShadowRoot
    /// </summary>
    /// <remarks>
    /// Not all methods are supported. Different drivers vary the degree of support. Use caution.
    /// </remarks>
    /// 
    [Guid("0277FC34-FD1B-4616-BB19-939067ADF4F1")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Shadow {

        #region Find Elements

        [DispId(1176), Description("Find the first WebElement using the given method.")]
        WebElement FindElement([MarshalAs(UnmanagedType.Struct)]By by, int timeout = -1, bool raise = true);

        [DispId(1173), Description("Find the first WebElement for a given strategy and value.")]
        WebElement FindElementBy(Strategy strategy, string value, int timeout = -1, bool raise = true);

        [DispId(1178), Description("Finds the first element matching the specified CSS class.")]
        WebElement FindElementByClass(string classname, int timeout = -1, bool raise = true);

        [DispId(1180), Description("Finds the first element matching the specified CSS selector.")]
        WebElement FindElementByCss(string cssselector, int timeout = -1, bool raise = true);

        [DispId(1183), Description("Finds the first element matching the specified id.")]
        WebElement FindElementById(string id, int timeout = -1, bool raise = true);

        [DispId(1185), Description("Finds the first element matching the specified link text.")]
        WebElement FindElementByLinkText(string linktext, int timeout = -1, bool raise = true);

        [DispId(1187), Description("Finds the first element matching the specified name.")]
        WebElement FindElementByName(string name, int timeout = -1, bool raise = true);

        [DispId(1189), Description("Finds the first of elements that match the part of the link text supplied")]
        WebElement FindElementByPartialLinkText(string partiallinktext, int timeout = -1, bool raise = true);

        [DispId(1191), Description("Finds the first element matching the specified tag name.")]
        WebElement FindElementByTag(string tagname, int timeout = -1, bool raise = true);

        [DispId(1193), Description("Finds the first element matching the specified XPath query.")]
        WebElement FindElementByXPath(string xpath, int timeout = -1, bool raise = true);

        [DispId(1200), Description("Find all elements within the current context using the given mechanism.")]
        WebElements FindElements([MarshalAs(UnmanagedType.Struct)]By by, int minimum = 0, int timeout = 0);

        [DispId(1210), Description("Find all elements for a given strategy and value.")]
        WebElements FindElementsBy(Strategy strategy, string value, int minimum = 0, int timeout = 0);

        [DispId(1220), Description("Finds elements matching the specified class name.")]
        WebElements FindElementsByClass(string classname, int minimum = 0, int timeout = 0);

        [DispId(1221), Description("Finds elements matching the specified CSS selector.")]
        WebElements FindElementsByCss(string cssselector, int minimum = 0, int timeout = 0);

        [DispId(1223), Description("Finds elements matching the specified id.")]
        WebElements FindElementsById(string id, int minimum = 0, int timeout = 0);

        [DispId(1225), Description("Finds elements matching the specified link text.")]
        WebElements FindElementsByLinkText(string linktext, int minimum = 0, int timeout = 0);

        [DispId(1227), Description("Finds elements matching the specified name.")]
        WebElements FindElementsByName(string name, int minimum = 0, int timeout = 0);

        [DispId(1229), Description("Finds the first of elements that match the part of the link text supplied")]
        WebElements FindElementsByPartialLinkText(string partiallinktext, int minimum = 0, int timeout = 0);

        [DispId(1235), Description("Finds elements matching the specified tag name.")]
        WebElements FindElementsByTag(string tagname, int minimum = 0, int timeout = 0);

        [DispId(1239), Description("Finds elements matching the specified XPath query.")]
        WebElements FindElementsByXPath(string xpath, int minimum = 0, int timeout = 0);

        [DispId(1250), Description("Indicates whether a WebElement is present using the given method.")]
        bool IsElementPresent([MarshalAs(UnmanagedType.Struct)]By by, int timeout = 0);

        [DispId(1260), Description("Waits for an element to disappear from the page")]
        void WaitNotElement([MarshalAs(UnmanagedType.Struct)]By by, int timeout = -1);

        #endregion

        [DispId(899), Description("Evaluate equality")]
        bool Equals(object obj);
    }

}
