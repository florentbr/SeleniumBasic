using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-CC6284398AA5")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _WebDriver {

        #region Setup

        [DispId(200), Description("")]
        void AddArgument(string argument);

        [DispId(202), Description("")]
        void AddExtension(string extensionPath);

        [DispId(205), Description("")]
        void SetCapability(string key, object value);

        [DispId(209), Description("")]
        void SetPreference(string key, object value);

        [DispId(303), Description("")]
        void SetProfile(string nameOrDirectory, bool persistant = false);

        [DispId(308), Description("")]
        Proxy Proxy { get; }

        [DispId(310), Description("")]
        void SetBinary(string path);

        #endregion


        #region Session

        [DispId(12), Description("Starts a new Selenium testing session")]
        void Start(string browser = null, string baseUrl = null);

        [DispId(15), Description("Starts remotely a new Selenium session")]
        void StartRemotely(string executorUri, string browser = null, string version = null, string platform = "ANY");

        [DispId(20), Description("Quits this driver, closing every associated window. Same as stop.")]
        void Quit();

        #endregion


        #region Interfaces

        [DispId(78), Description("")]
        Timeouts Timeouts { get; }

        [DispId(80), Description("")]
        Manage Manage { get; }

        [DispId(82), Description("")]
        Actions Actions { get; }

        [DispId(86), Description("")]
        TouchActions TouchActions { get; }

        [DispId(105), Description("")]
        Keyboard Keyboard { get; }

        [DispId(106), Description("")]
        Keys Keys { get; }

        [DispId(110), Description("")]
        Mouse Mouse { get; }

        [DispId(115), Description("")]
        TouchScreen TouchScreen { get; }

        #endregion


        #region Navigation

        [DispId(30), Description("Base URL to use a relative URL with Get")]
        string BaseUrl { get; set; }

        [DispId(32), Description("Loads a new web page with a relative or absolute URL")]
        bool Get(string url, int timeout = -1, bool raise = true);

        [DispId(95), Description("Returns the page URL")]
        string Url { get; }

        [DispId(96), Description("Returns the page title")]
        string Title { get; }

        [DispId(98), Description("Goes one step backward in the browser history.")]
        void GoBack();

        [DispId(102), Description("Goes one step forward in the browser history.")]
        void GoForward();

        [DispId(107), Description("Refreshes the current page.")]
        void Refresh();

        #endregion


        #region Windows

        [DispId(159), Description("Returns the the current window.")]
        Window Window { get; }

        [DispId(163), Description("Returns the handles of all windows within the current session.")]
        List Windows { get; }

        [DispId(43), Description("Closes the current window")]
        void Close();

        #endregion


        #region Interaction

        [DispId(404), Description("Sends a sequence of keystrokes to the browser.")]
        void SendKeys(string keysOrModifier, string keys = null);

        #endregion


        #region Screenshot

        [DispId(503), Description("Capture a screenshot")]
        Image TakeScreenshot(int delayms = 100);

        #endregion


        #region Javascript

        [DispId(601), Description("Execute JavaScrip on the page")]
        object ExecuteScript(string script, object arguments = null);

        [DispId(605), Description("Waits for the Javascript engine to return true or not null")]
        object WaitForScript(string script, object arguments, int timeout = -1);

        #endregion


        #region  Find Elements

        [DispId(1170), Description("Gets the currently active element on the document")]
        WebElement ActiveElement();

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

        [DispId(1220), Description("Finds elements matching the specified CSS class.")]
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


        //TODO: remove in a later release
        #region Deprecated
        
        [DispId(1179), Description("Deprecated, use FindElementByClass instead")]
        WebElement FindElementByClassName(string classname, int timeout = -1, bool raise = true);
        
        [DispId(1181), Description("Deprecated, use FindElementByCss instead")]
        WebElement FindElementByCssSelector(string cssselector, int timeout = -1, bool raise = true);
        
        [DispId(1192), Description("Deprecated, use FindElementByTag instead")]
        WebElement FindElementByTagName(string tagname, int timeout = -1, bool raise = true);
        
        [DispId(1222), Description("Deprecated, use FindElementsByClass instead")]
        WebElements FindElementsByClassName(string classname, int minimum = 0, int timeout = 0);

        [DispId(1224), Description("Deprecated, use FindElementsByCss instead")]
        WebElements FindElementsByCssSelector(string cssselector, int minimum = 0, int timeout = 0);
        
        [DispId(1236), Description("Deprecated, use FindElementsByTag instead")]
        WebElements FindElementsByTagName(string tagname, int minimum = 0, int timeout = 0);
        
        #endregion
        
        #endregion


        #region PageSource

        [DispId(1302), Description("Returns the page source")]
        string PageSource();

        [DispId(1307), Description("Returns the first occurence matching the regular expression.")]
        string PageSourceMatch(string pattern, short group = 0);

        [DispId(1309), Description("Returns all the occurences matching the regular expression.")]
        List PageSourceMatches(string pattern, short group = 0);

        #endregion


        #region Context

        [DispId(1604), Description("Select either the first frame on the page or the main document when a page contains iFrames.")]
        void SwitchToDefaultContent();

        [DispId(1609), Description("Switche the focus to the specified frame, by index, name or WebElement.")]
        bool SwitchToFrame(object identifier, int timeout = -1, bool raise = true);

        [DispId(1611), Description("Switche the focus to the specified window.")]
        Window SwitchToWindowByName(string name, int timeout = -1, bool raise = true);

        [DispId(1612), Description("Switche the focus to the specified window.")]
        Window SwitchToWindowByTitle(string title, int timeout = -1, bool raise = true);

        [DispId(1614), Description("Switche the focus to the next window.")]
        Window SwitchToNextWindow(int timeout = -1, bool raise = true);

        [DispId(1617), Description("Switche the focus to the previous window.")]
        Window SwitchToPreviousWindow();

        [DispId(1619), Description("Switche the focus to the parent frame.")]
        void SwitchToParentFrame();

        [DispId(1623), Description("Switch the focus to an alert on the page.")]
        Alert SwitchToAlert(int timeout = -1, bool raise = true);

        #endregion


        #region Waiter

        [DispId(2037), Description("Waits the specified time in millisecond before executing the next command")]
        void Wait(int timems);

        [DispId(2039), Description("Waits for a procedure to return true")]
        object Until(object procedure, object argument = null, int timeout = -1);

        #endregion


        #region Cache

        [DispId(3456), Description("Get the status of the html5 application cache.")]
        CacheState CacheStatus();

        #endregion


        #region Clipboard

        [DispId(356), Description("Set text in the Clipboard")]
        void SetClipBoard(string text);

        [DispId(378), Description("Get text from the Clipboard")]
        string GetClipBoard();

        #endregion

    }

}
