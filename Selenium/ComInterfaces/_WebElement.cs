using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-8B145197B76C")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _WebElement {

        #region Element State

        [DispId(901), Description("Whether the element would be visible to a user.")]
        bool IsDisplayed { get; }

        [DispId(903), Description("Whether the element is selected.")]
        bool IsSelected { get; }

        [DispId(905), Description("Returns the value of the attribute.")]
        object Attribute(string attributeName);

        [DispId(907), Description("Returns the value of a CSS property.")]
        object CssValue(string propertyName);

        [DispId(909), Description("Gets the text of the element.")]
        string Text();

        [DispId(912), Description("Gets this element’s tagName property.")]
        string TagName { get; }

        [DispId(937), Description("Whether the element is enabled.")]
        bool IsEnabled { get; }

        [DispId(943), Description("Returns the location of the element in the renderable canvas")]
        Point Location();

        [DispId(944), Description("Gets the location of an element relative to the origin of the view port.")]
        Point LocationInView();

        [DispId(947), Description("Returns the size of the element")]
        Size Size();

        [DispId(957), Description("Whether the element is still attached to the DOM.")]
        bool IsPresent { get; }

        [DispId(965), Description("Returns the value attribute")]
        object Value();

        #endregion


        #region Interactions

        [DispId(928), Description("Clears the text if it’s a text entry element.")]
        WebElement Clear();

        [DispId(932), Description("Drag and drop the element to an offset")]
        void DragAndDropToOffset(int offsetX, int offsetY);

        [DispId(934), Description("Drag and drop the element to another element")]
        void DragAndDropToWebElement([MarshalAs(UnmanagedType.Struct)]WebElement webelement);

        [DispId(919), Description("Clicks the element.")]
        void Click(string modifierKeys = null);

        [DispId(921), Description("Clicks at the element offset.")]
        void ClickByOffset(int offset_x, int offset_y);

        [DispId(923), Description("Clicks the element and hold.")]
        void ClickAndHold();

        [DispId(925), Description("Release a click previously hold")]
        void ReleaseMouse();

        [DispId(927), Description("Rigth clicks the element")]
        void ClickContext();

        [DispId(929), Description("Double clicks the element.")]
        void ClickDouble();

        [DispId(939), Description("Press a key and hold")]
        void HoldKeys(string modifierKeys);

        [DispId(941), Description("Release a key")]
        void ReleaseKeys(string modifierKeys);

        [DispId(945), Description("Simulates typing into the element.")]
        WebElement SendKeys(string keysOrModifier, string keys = null);

        [DispId(949), Description("Submits a form.")]
        void Submit();

        [DispId(951), Description("Scrolls the current element into the visible area of the browser window.")]
        WebElement ScrollIntoView(bool alignTop = false);

        #endregion


        #region Screenshots

        [DispId(953), Description("Takes a screenshot of the current element")]
        Image TakeScreenshot(int delayms = 0);

        #endregion


        #region Text methods

        [DispId(964), Description("Returns the first occurence matching the regular expression or null.")]
        string TextMatch(string pattern);

        [DispId(966), Description("Returns all the occurences matching the regular expression")]
        List TextMatches(string pattern);

        [DispId(971), Description("Return a number parsed from the text")]
        object TextAsNumber(string decimalCharacter = ".", object errorValue = null);

        #endregion


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


        #region Waiting

        [DispId(1040), Description("Wait for call to a procedure to return true. The procedure receives a WebElement as argument and returns a Boolean.")]
        object Until(object procedure, object argument = null, int timeout = -1);

        [DispId(1042), Description("Wait for an attribute")]
        WebElement WaitAttribute(string attribute, string pattern, int timeout = -1);

        [DispId(1043), Description("Wait for a CSS property")]
        WebElement WaitCssValue(string property, string value, int timeout = -1);

        [DispId(1047), Description("Wait for a web element to be displayed or not. Default is displayed.")]
        WebElement WaitDisplayed(bool displayed = true, int timeout = -1);

        [DispId(1050), Description("Wait for a web element to be enabled or not. Default is enabled.")]
        WebElement WaitEnabled(bool enabled = true, int timeout = -1);

        [DispId(1053), Description("Wait for a different attribute")]
        WebElement WaitNotAttribute(string attribute, string pattern, int timeout = -1);

        [DispId(1055), Description("Wait for a different CSS property")]
        WebElement WaitNotCssValue(string propertyName, string value, int timeout = -1);

        [DispId(1057), Description("Wait for a different text")]
        WebElement WaitNotText(string pattern, int timeout = -1);

        [DispId(1059), Description("Wait for a selection to true or false. Default is true.")]
        WebElement WaitSelection(bool selected = true, int timeout = -1);

        [DispId(1060), Description("Wait for text")]
        WebElement WaitText(string pattern, int timeout = -1);

        [DispId(1065), Description("Wait for the web element to be removed from the DOM.")]
        void WaitRemoval(int timeout = -1);

        #endregion


        #region Element casting

        [DispId(8300), Description("Cast the WebElement to a Select element")]
        SelectElement AsSelect();

        [DispId(8400), Description("Cast the WebElement to a Table element")]
        TableElement AsTable();

        #endregion


        #region Javascript

        [DispId(601), Description("Executes a piece of JavaScript in the context of the current element. Returns the value specified by the return statement.")]
        object ExecuteScript(string script, object arguments = null);

        [DispId(602), Description("Executes an asynchronous piece of JavaScript in the context of the current element. Returns the first argument of the callback function.")]
        object ExecuteAsyncScript(string script, object arguments = null, int timeout = -1);

        [DispId(605), Description("Waits for a piece of JavaScript to return true or not null")]
        object WaitForScript(string script, object arguments = null, int timeout = -1);

        #endregion


        [DispId(899), Description("Evaluate equality")]
        bool Equals(object obj);
    }

}
