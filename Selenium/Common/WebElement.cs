using Selenium.Core;
using Selenium.Internal;
using Selenium.Serializer;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Selenium {

    /// <summary>
    /// Defines the interface through which the user controls elements on the page. 
    /// </summary>
    [ProgId("Selenium.WebElement")]
    [Guid("0277FC34-FD1B-4616-BB19-A693991646BE")]
    [Description("Defines the interface through which the user controls elements on the page.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class WebElement : SearchContext, ComInterfaces._WebElement, IJsonObject {

        /// <summary>
        /// Returns the element with focus, or BODY if nothing has focus.
        /// </summary>
        internal static WebElement GetActiveWebElement(RemoteSession session) {
            var result = (Dictionary)session.Send(RequestMethod.POST, "/element/active");
            return new WebElement(session, result);
        }

        internal static bool TryParse(Dictionary dict, out string id) {
            return dict.TryGetValue("ELEMENT", out id);
            //TODO: Change the key once implemented on the server side
            //return dict.TryGetValue("ELEMENT-6066-11e4-a52e-4f735466cecf", out id);
        }


        internal readonly RemoteSession _session;
        internal readonly string Id;

        internal WebElement(RemoteSession session, string id) {
            _session = session;
            this.Id = id;
        }

        internal WebElement(RemoteSession session, Dictionary dict) {
            _session = session;
            if (!WebElement.TryParse(dict, out this.Id))
                throw new SeleniumException("Failed to extact the WebElement from the dictionary. Missing key.");
        }

        internal override RemoteSession session {
            get {
                return _session;
            }
        }

        internal override string uri {
            get {
                return "/element/" + this.Id;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Dictionary SerializeJson() {
            var dict = new Dictionary();
            dict.Add("ELEMENT", this.Id);
            //TODO: Change the key once implemented on the server side
            //dict.Add("ELEMENT-6066-11e4-a52e-4f735466cecf", id);
            return dict;
        }

        private object Send(RequestMethod method, string relativeUri) {
            return _session.Send(method, this.uri + relativeUri);
        }

        private object Send(RequestMethod method, string relativeUri, string key, object value) {
            return _session.Send(method, this.uri + relativeUri, key, value);
        }

        private object Send(RequestMethod method, string relativeUri, string key1, object value1, string key2, object value2) {
            return _session.Send(method, this.uri + relativeUri, key1, value1, key2, value2);
        }


        #region Element State

        /// <summary>
        /// Gets this element’s tagName property.
        /// </summary>
        public string TagName {
            get {
                return (string)Send(RequestMethod.GET, "/name");
            }
        }

        /// <summary>
        /// Gets the text of the element.
        /// </summary>
        public string Text() {
            return (string)Send(RequestMethod.GET, "/text");
        }

        /// <summary>
        /// Whether the element is enabled.
        /// </summary>
        public bool IsEnabled {
            get {
                return (bool)Send(RequestMethod.GET, "/enabled");
            }
        }

        /// <summary>
        /// Whether the element is selected.
        /// </summary>
        public bool IsSelected {
            get {
                return (bool)Send(RequestMethod.GET, "/selected");
            }
        }

        /// <summary>
        /// Returns the location of the element in the renderable canvas
        /// </summary>
        /// <returns>Point</returns>
        public Point Location() {
            var dict = (Dictionary)Send(RequestMethod.GET, "/location");
            return new Point(dict);
        }

        /// <summary>
        /// Gets the location of an element relative to the origin of the view port.
        /// </summary>
        public Point LocationInViewport() {
            var dict = (Dictionary)Send(RequestMethod.GET, "/location_in_view");
            return new Point(dict);
        }

        /// <summary>
        /// Returns the size of the element
        /// </summary>
        public Size Size() {
            var dict = (Dictionary)Send(RequestMethod.GET, "/size");
            return new Size(dict);
        }

        /// <summary>
        /// Returns the size of the element
        /// </summary>
        public object Rect {
            get {
                var dict = (Dictionary)Send(RequestMethod.GET, "/rect");
                return null;
            }
        }

        /// <summary>
        /// Whether the element would be visible to a user
        /// </summary>
        public bool IsDisplayed {
            get {
                return (bool)Send(RequestMethod.GET, "/displayed");
            }
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="attribute">Attribute name</param>
        /// <returns>Attribute</returns>
        public object Attribute(string attribute) {
            var value = Send(RequestMethod.GET, "/attribute/" + attribute);
            return value;
        }

        /// <summary>
        /// Returns the value of a CSS property
        /// </summary>
        /// <param name="property">Property name</param>
        /// <returns>CSS value</returns>
        public object CssValue(string property) {
            var value = (string)Send(RequestMethod.GET, "/css/" + property);
            return value;
        }

        /// <summary>
        /// Gets the screenshot of the current element
        /// </summary>
        /// <param name="delayms"></param>
        /// <returns>Image</returns>
        public Image TakeScreenshot(int delayms = 0) {
            if (delayms != 0)
                SysWaiter.Wait(delayms);

            var dict = (Dictionary)_session.javascript.Execute(
                "return arguments[0].getBoundingClientRect()", new[] { this }, false);

            var rect = new System.Drawing.Rectangle(
                Convert.ToInt32(dict["left"]), Convert.ToInt32(dict["top"]),
                Convert.ToInt32(dict["width"]), Convert.ToInt32(dict["height"]));

            using (Image image = (Image)_session.Send(RequestMethod.GET, "/screenshot")) {
                var bitmap = image.GetBitmap();
                var bmpCrop = bitmap.Clone(rect, bitmap.PixelFormat);
                return new Image(bmpCrop);
            }
        }

        /// <summary>
        /// Compares if two web elements are equal
        /// </summary>
        /// <param name="other">WebElement to compare against</param>
        /// <returns>A boolean if it is equal or not</returns>
        public bool Equals(WebElement other) {
            if (Id == other.Id)
                return true;
            return (bool)Send(RequestMethod.GET, "/equals/" + other.Id);
        }

        #endregion


        #region Interactions

        /// <summary>
        /// Clears the text if it’s a text entry element.
        /// </summary>
        public WebElement Clear() {
            Send(RequestMethod.POST, "/clear");
            return this;
        }

        /// <summary>
        /// Clicks at the element offset.
        /// </summary>
        /// <param name="offset_x">Offset X</param>
        /// <param name="offset_y">Offset Y</param>
        public void ClickByOffset(int offset_x, int offset_y) {
            _session.mouse.MoveTo(this, offset_x, offset_y);
            _session.mouse.Click();
        }

        /// <summary>
        /// Clicks the element and hold.
        /// </summary>
        public void ClickAndHold() {
            _session.mouse.moveTo(this);
            _session.mouse.ClickAndHold();
        }

        /// <summary>
        /// Release a click
        /// </summary>
        public void ReleaseMouse() {
            _session.mouse.moveTo(this);
            _session.mouse.Release();
        }

        /// <summary>
        /// Rigth clicks the element
        /// </summary>
        public void ClickContext() {
            _session.mouse.moveTo(this);
            _session.mouse.Click(MouseButton.Right);
        }

        /// <summary>
        /// Double clicks the element.
        /// </summary>
        public void ClickDouble() {
            _session.mouse.moveTo(this);
            _session.mouse.ClickDouble();
        }

        /// <summary>
        /// Drag and drop the element to another element
        /// </summary>
        /// <param name="element">Target WebElement</param>
        public void DragAndDropToWebElement(WebElement element) {
            _session.mouse.moveTo(this);
            _session.mouse.ClickAndHold();
            _session.mouse.moveTo(element);
            _session.mouse.Release();
        }

        /// <summary>
        /// Drag and drop the element to an offset
        /// </summary>
        /// <param name="offsetX">Offset X</param>
        /// <param name="offsetY">Offset Y</param>
        public void DragAndDropToOffset(int offsetX, int offsetY) {
            _session.mouse.moveTo(this);
            _session.mouse.ClickAndHold();
            _session.mouse.MoveTo(this, offsetX, offsetY);
            _session.mouse.Release();
        }

        /// <summary>
        /// Press a key and hold
        /// </summary>
        /// <param name="modifiers">Key</param>
        public void HoldKeys(string modifiers) {
            _session.mouse.moveTo(this);
            _session.mouse.Click();
            _session.keyboard.KeyDown(modifiers);
        }

        /// <summary>
        /// Release a key
        /// </summary>
        /// <param name="modifiers">Key</param>
        public void ReleaseKeys(string modifiers) {
            _session.keyboard.KeyUp(modifiers);
        }

        /// <summary>
        /// Simulates typing into the element.
        /// </summary>
        /// <param name="keysOrModifier">Sequence of keys or a modifier key(Control,Shift...) if the sequence is in keysToSendEx</param>
        /// <param name="keys">Optional - Sequence of keys if keysToSend contains modifier key(Control,Shift...)</param>
        /// <example>
        /// To Send mobile to an element :
        /// <code lang="vbs">
        ///     driver.FindElementsById("id").sendKeys "mobile"
        /// </code>
        /// To Send ctrl+a to an element :
        /// <code lang="vbs">
        ///     driver.FindElementsById("id").sendKeys Keys.Control, "a"
        /// </code>
        /// </example>
        public WebElement SendKeys(string keysOrModifier, string keys = null) {
            var text = string.Concat(keysOrModifier, keys);
            if (text == null)
                throw new ArgumentNullException("text", "text cannot be null");

            if (!_session.IsLocal && text.IndexOf(":/") != -1 && File.Exists(text)) {
                text = _session.UploadFile(text);
            }
            Send(RequestMethod.POST, "/value", "value", new string[] { text });
            return this;
        }

        /// <summary>
        /// Submits a form.
        /// </summary>
        public void Submit() {
            Send(RequestMethod.POST, "/submit");
        }

        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <param name="keys">Optional - Modifier Keys to press</param>
        public void Click(string keys = null) {
            if (keys != null)
                _session.keyboard.SendKeys(keys);
            Send(RequestMethod.POST, "/click");
            if (keys != null)
                _session.keyboard.SendKeys(keys);
        }

        #endregion


        #region Text methods

        /// <summary>
        /// Searches the specified input string for an occurrence of the regular expression supplied in the pattern parameter.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Returns an array of values if the regex has a group, a single value or empty</returns>
        public string TextMatch(string pattern) {
            return this.Text().Match(pattern);
        }

        /// <summary>
        /// Returns all the occurences matching the regular expression.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>Array of strings or null</returns>
        public List TextMatches(string pattern) {
            return this.Text().Matches(pattern);
        }

        /// <summary>
        /// Return a number parsed from the text
        /// </summary>
        /// <param name="decimalCharacter">Decimal character. Default is "."</param>
        /// <param name="errorValue">Default value in case of failure</param>
        /// <returns>Number or defualt value</returns>
        public object TextAsNumber(string decimalCharacter = ".", object errorValue = null) {
            double number;
            if (this.Text().TryExtractNumber(out number, decimalCharacter[0]))
                return number;
            return errorValue;
        }

        #endregion


        #region Waiter methods


        /// <summary>
        /// Waits for the delegate function to return not null or true
        /// </summary>
        /// <typeparam name="T">Returned object or boolean</typeparam>
        /// <param name="func">Delegate taking the web element instance as argument</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns>Variant or boolean</returns>
        public T Until<T>(Func<WebElement, T> func, int timeout = -1) {
            if (timeout == -1)
                timeout = _session.timeouts.timeout_implicitwait;
            return Waiter.WaitUntil(func, this, timeout, 80);
        }

        /// <summary>
        /// Waits for a procesult to set result to true. VBScript: Function WaitEx(webdriver), VBA: Function WaitEx(webdriver As WebDriver) As Boolean 
        /// </summary>
        /// <param name="procedure">Function reference.  Sub Procedure(element, result)  waitFor AddressOf Procedure</param>
        /// <param name="argument">Optional - Argument: Sub Procedure(element, argument, result) waitFor AddressOf Procedure, "argument"</param>
        /// <param name="timeout">Optional - timeout in milliseconds</param>
        /// <returns>Current WebDriver</returns>
        object ComInterfaces._WebElement.Until(object procedure, object argument, int timeout) {
            if (timeout == -1)
                timeout = _session.timeouts.timeout_implicitwait;
            return COMExt.WaitUntilProc(procedure, this, argument, timeout);
        }

        /// <summary>
        /// Waits for an attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="pattern"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitAttribute(string attribute, string pattern, int timeout = -1) {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            _session.SendUntil(timeout,
                () => this.Attribute(attribute),
                (r) => regex.IsMatch((string)r)
            );
            return this;
        }

        /// <summary>
        /// Waits for a different attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="pattern"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitNotAttribute(string attribute, string pattern, int timeout = -1) {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            _session.SendUntil(timeout,
                () => this.Attribute(attribute),
                (r) => !regex.IsMatch((string)r)
            );
            return this;
        }

        /// <summary>
        /// Waits for a CSS property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
        /// <param name="timeout"></param>
        public WebElement WaitCssValue(string propertyName, string value, int timeout = -1) {
            _session.SendUntil(timeout,
                () => this.CssValue(propertyName),
                (r) => string.Compare((string)r, value, true) == 0
            );
            return this;
        }

        /// <summary>
        /// Waits for a different CSS property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitNotCssValue(string propertyName, string value, int timeout = -1) {
            _session.SendUntil(timeout,
                () => this.CssValue(propertyName),
                (r) => string.Compare((string)r, value, true) != 0
            );
            return this;
        }

        /// <summary>
        /// Waits for text
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitText(string pattern, int timeout = -1) {
            if (this.TagName.ToLower() == "input") {
                this.WaitAttribute("value", pattern, timeout);
            } else {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                _session.SendUntil(timeout,
                    this.Text,
                    (r) => regex.IsMatch(r)
                );
            }
            return this;
        }

        /// <summary>
        /// Waits for a different text
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitNotText(string pattern, int timeout = -1) {
            if (this.TagName.ToLower() == "input") {
                this.WaitNotAttribute("value", pattern, timeout);
            } else {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                _session.SendUntil(timeout,
                    this.Text,
                    (r) => !regex.IsMatch(r)
                );
            }
            return this;
        }

        /// <summary>
        /// Waits for the element to be selected or not.
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitSelection(bool selected = true, int timeout = -1) {
            _session.SendUntil(timeout,
                () => this.IsSelected,
                (r) => r == selected
            );
            return this;
        }

        /// <summary> 
        /// Waits for the element to be enabled.
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitEnabled(bool enabled = true, int timeout = -1) {
            _session.SendUntil(timeout,
                () => this.IsEnabled,
                (r) => r == enabled
            );
            return this;
        }

        /// <summary>
        /// Waits for the element to be displayed.
        /// </summary>
        /// <param name="displayed"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public WebElement WaitDisplayed(bool displayed = true, int timeout = -1) {
            _session.SendUntil(timeout,
                () => this.IsDisplayed,
                (r) => r == displayed
            );
            return this;
        }

        #endregion


        #region Casting properties

        /// <summary>
        /// Cast the WebElement to a Select element
        /// </summary>
        public SelectElement AsSelect() {
            return new SelectElement(this);
        }

        /// <summary>
        /// Cast the WebElement to a Select element
        /// </summary>
        public TableElement AsTable() {
            return new TableElement(_session, this);
        }

        #endregion


        #region Javascript

        /// <summary>
        /// Executes a JavaScript function in the context of the current element and returns the return value of the function.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="arguments">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        public object ExecuteScript(string script, object arguments = null) {
            string newscript = "return (function(){" + script + "}).apply(arguments[0],arguments[1]);";
            object[] newargs = FormatArguments(this, arguments);
            var result = session.javascript.Execute(newscript, newargs, true);
            return result;
        }

        /// <summary>
        /// Waits for a script to return true or not null.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="arguments"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public object WaitForScript(string script, object arguments, int timeout = -1) {
            string newscript = "return (function(){" + script + "}).apply(arguments[0],arguments[1]);";
            object[] newargs = FormatArguments(this, arguments);
            var result = session.javascript.WaitFor(newscript, newargs, timeout);
            return result;
        }

        private static object[] FormatArguments(object item, object arguments) {
            if (arguments == null) {
                return new[] { item, new object[0] };
            } else if (arguments is IEnumerable && !(arguments is string || arguments is Dictionary)) {
                return new[] { item, arguments };
            } else {
                return new[] { item, new object[] { arguments } };
            }
        }

        #endregion


        #region Protected support methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if (obj == null)
                return false;
            WebElement element = obj as WebElement;
            if (element == null)
                return false;
            return Id == element.Id;
        }

        /// <summary>
        /// Returns the hash code for this element
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        #endregion

    }
}
