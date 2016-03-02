using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-7C9763568492")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _WebElements : System.Collections.IEnumerable {

        [DispId(0), Description("Get the WebElement at the provided index (One-based)")]
        WebElement this[int index] { get; }

        [DispId(118), Description("Number of web elements")]
        int Count { get; }

        [DispId(345), Description("Returns the first element")]
        WebElement First();

        [DispId(367), Description("Returns the last element")]
        WebElement Last();

        [DispId(67), Description("Return and array containing the attribute for each web element")]
        List Attribute(string attributeName, bool withAttributeOnly = true);

        [DispId(601), Description("Executes a script for each element and returns the results. Ex: return element.tagName;")]
        List ExecuteScript(string script, bool includeNullResults = false);

        [DispId(602), Description("Execute an asynchronous piece of JavaScript and returns the results.")]
        List ExecuteAsyncScript(string script, int timeout = -1);

        [DispId(86), Description("Returns a list containing the text for each element")]
        List Text(int offsetStart = 0, int offsetEnd = 0, bool trim = true);

        [DispId(92), Description("Returns a list containing the text parsed to a number for each element")]
        List Values(object defaultValue = null, int offsetStart = 0, int offsetEnd = 0);

        [DispId(-4)]
        System.Collections.IEnumerator _NewEnum();  //Has to be declared last

    }

}
