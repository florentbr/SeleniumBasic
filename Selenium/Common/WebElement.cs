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
    /// <seealso cref="ComInterfaces._WebElement"/>
    /// <seealso cref="SearchContext"/>
    [ProgId("Selenium.WebElement")]
    [Guid("0277FC34-FD1B-4616-BB19-A693991646BE")]
    [Description("Defines the interface through which the user controls elements on the page.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class WebElement : SearchContext, ComInterfaces._WebElement, IJsonObject {
        internal const string ELEMENT    = "ELEMENT";
        internal const string IDENTIFIER = "element-6066-11e4-a52e-4f735466cecf";

        /// <summary>
        /// Returns the element with focus, or BODY if nothing has focus.
        /// </summary>
        internal static WebElement GetActiveWebElement(RemoteSession session) {
            RequestMethod method = WebDriver.LEGACY ? RequestMethod.POST : RequestMethod.GET;
            var result = (Dictionary)session.Send(method, "/element/active");
            return new WebElement(session, result);
        }

        internal static bool TryParse(Dictionary dict, out string id) {
            if( WebDriver.LEGACY && dict.TryGetValue(ELEMENT, out id) )
                return true;
            return dict.TryGetValue(IDENTIFIER, out id);
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
            if( !WebDriver.LEGACY )
                dict.Add(IDENTIFIER, this.Id); // Selenium.NET sends both. Let's do the same.
            dict.Add( ELEMENT, this.Id);
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
        /// <returns>Point object</returns>
        public Point Location() {
            if( WebDriver.LEGACY ) {
                var dict = (Dictionary)Send(RequestMethod.GET, "/location");
                return new Point(dict);
            } else {
                var dict = (Dictionary)Send(RequestMethod.GET, "/rect");
                return new Point(dict);
            }
        }

        /// <summary>
        /// Gets the location of an element relative to the origin of the view port.
        /// </summary>
        /// <remarks>In the modern mode (not legacy), same as Location()</remarks>
        public Point LocationInView() {
            if( WebDriver.LEGACY ) {
                var dict = (Dictionary)Send(RequestMethod.GET, "/location_in_view");
                return new Point(dict);
            } else {
                var dict = (Dictionary)Send(RequestMethod.GET, "/rect");
                return new Point(dict);
            }
        }

        /// <summary>
        /// Returns the size of the element
        /// </summary>
        public Size Size() {
            if( WebDriver.LEGACY ) {
                var dict = (Dictionary)Send(RequestMethod.GET, "/size");
                return new Size(dict);
            } else {
                var dict = (Dictionary)Send(RequestMethod.GET, "/rect");
                return new Size(dict);
            }
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
        /// <returns>False if the element is is scrolled out of view, obscured and not visible to the user by any other reasons</returns>
        /// <remarks>
        /// The result does not relate to the visibility or display style properties!
        /// </remarks>
        public bool IsDisplayed {
            get {
                return (bool)Send(RequestMethod.GET, "/displayed");
            }
        }

        /// <summary>
        /// Whether the element is present
        /// </summary>
        public bool IsPresent {
            get {
                try{
                    Send(RequestMethod.GET, "/name");
                    return true;
                } catch (Errors.StaleElementReferenceError) {
                    return false;
                }
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
        /// Gets the property value.
        /// </summary>
        /// <param name="property">Property name</param>
        /// <returns>Property</returns>
        public object Property(string property) {
            var value = Send(RequestMethod.GET, "/property/" + property);
            return value;
        }

        /// <summary>
        /// Returns the value of a CSS property
        /// </summary>
        /// <param name="property">CSS property name</param>
        /// <returns>CSS value</returns>
        /// <example>
        /// <code lang="vbs">
        /// elem.CssValue("background-color")
        /// </code>
        /// </example>
        /// <remarks>
        /// Chromium based browsers return a color as "rgba(220, 53, 69, 1)"
        /// However, Gecko returns "rgb(220, 53, 69)"
        /// </remarks>
        public object CssValue(string property) {
            var value = (string)Send(RequestMethod.GET, "/css/" + property);
            return value;
        }

        /// <summary>
        /// Returns the value of an input control element
        /// </summary>
        /// <returns>Current value</returns>
        /// <remarks>
        /// Note: Legacy drivers return the current value of the "value" attribute. 
        /// Gecko returns the actual value which could be different from the "value" attribute.
        /// </remarks>
        public object Value() {
            if( WebDriver.LEGACY ) {
                var value = Send(RequestMethod.GET, "/attribute/value");
                return value;
            }
            return Send(RequestMethod.GET, "/property/value");
        }

        /// <summary>
        /// Returns the attached shadow root (if exists)
        /// Note: Searching elements from a shadow root is not supported in gecko.
        /// Also, the XPath strategy cannot be used.
        /// </summary>
        /// <returns>Shadow instance</returns>
        public Shadow Shadow() {
            var result = (Dictionary)Send(RequestMethod.GET, "/shadow");
            return new Shadow(session, result);
        }

        /// <summary>
        /// Gets the screenshot of the current element
        /// </summary>
        /// <param name="delayms"></param>
        /// <returns>Image</returns>
        public Image TakeScreenshot(int delayms = 0) {
            if (delayms != 0)
                SysWaiter.Wait(delayms);

            if(_session.capabilities.GetValue("takesElementScreenshot", false)){
                return (Image)Send(RequestMethod.GET, "/screenshot");
            }

            var dict = (Dictionary)_session.javascript.Execute(
                "return arguments[0].getBoundingClientRect();", new[] { this }, false);

            int left = Convert.ToInt32(dict["left"]);
            int top = Convert.ToInt32(dict["top"]);
            int width = Convert.ToInt32(dict["width"]);
            int height = Convert.ToInt32(dict["height"]);
            var rect = new System.Drawing.Rectangle(left, top, width, height);

            using (Image image = (Image)_session.Send(RequestMethod.GET, "/screenshot")) {
                var bitmap = image.GetBitmap();
                if (rect.Right > bitmap.Width || rect.Bottom > bitmap.Height)
                    throw new SeleniumError("Element outside of the screenshot.");

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
        /// <remarks>
        /// Note: In a Selenium based browser, this will not fire the element's onchange event 
        /// </remarks>
        public WebElement Clear() {
            Send(RequestMethod.POST, "/clear", "id", Id);
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
        /// <returns></returns>
        /// <example>
        /// To Send "mobile" to an element :
        /// <code lang="vbs">
        ///     driver.FindElementsById("id").sendKeys "mobile"
        /// </code>
        /// To Send ctrl+A to an element :
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
            Send(RequestMethod.POST, "/value", "value", new string[] { text }, "text", text);
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
        /// <exception cref="Exception">Throws if the element is blocked by another element</exception>
        public void Click(string keys = null) {
            if (keys != null) {
                if( WebDriver.LEGACY )
                    _session.keyboard.SendKeys(keys);
                else {
                    new Actions( _session )
                    .KeyDown(keys, this)
                    .Click(this)
                    .KeyUp(keys)
                    .Perform();
                    return;
                }
            }
            Send(RequestMethod.POST, "/click", "id", Id );
            if (keys != null) {
                _session.keyboard.SendKeys(keys);
            }
        }

        /// <summary>
        /// Scrolls the current element into the visible area of the browser window.
        /// </summary>
        /// <param name="alignTop">Optional - desired position of the element</param>
        /// <remarks>This method just executes the element's JavaScript method scrollIntoView(alignTop)
        /// Sometimes, calling this won't make the element visible (or clickable)
        /// because of other absolute positioned headers or footers.
        /// To scroll into the center, execute a script like "this.scrollIntoView({block: "center"});"
        /// </remarks>
        public WebElement ScrollIntoView(bool alignTop = false) {
            string script = "arguments[0].scrollIntoView("
                          + (alignTop ? "true);" : "false);");
            object[] arguments = { this };
            _session.javascript.Execute(script, arguments, false);
            return this;
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
        /// Waits until a function(element, argument) returns a true-castable result.
        /// </summary>
        /// <param name="procedure">Function reference. In VBScript use GetRef()</param>
        /// <param name="argument">Optional - the function's second argument</param>
        /// <param name="timeout">Optional. See <see cref="Timeouts.ImplicitWait"/></param>
        /// <returns>function's actual result</returns>
        /// <exception cref="Errors.TimeoutError">Throws when time out has reached</exception>
        /// <example>
        /// <code lang="vbs">
        /// function CheckButtonColor(element, wait_for)
        ///     CheckButtonColor = element.CssValue("color") = wait_for
        /// end function
        /// 
        /// driver.Get "https://demoqa.com/dynamic-properties"
        /// driver.FindElementByCss("button#colorChange").Until GetRef("CheckButtonColor"), "rgba(220, 53, 69, 1)", 8000
        /// </code>
        /// </example>
        object ComInterfaces._WebElement.Until(object procedure, object argument, int timeout) {
            if (timeout == -1)
                timeout = _session.timeouts.timeout_implicitwait;
            return COMExt.WaitUntilProc(procedure, this, argument, timeout);
        }

        /// <summary>
        /// Waits for an attribute
        /// </summary>
        /// <param name="attribute">Name</param>
        /// <param name="pattern">RegEx</param>
        /// <param name="timeout">Optional. See <see cref="Timeouts.ImplicitWait"/></param>
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
        /// Waits for a CSS property to change
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value expected to be changed to</param>
        /// <param name="timeout">Optional. See <see cref="Timeouts.ImplicitWait"/></param>
        /// <example>
        /// <code lang="vbs">
        /// if TypeName(driver) = "GeckoDriver" then red = "rgb(220, 53, 69)" else red = "rgba(220, 53, 69, 1)"
        /// ' wait until the color becomes red:
        /// driver.FindElementByCss("button#colorChange").WaitCssValue "color",red
        /// </code>
        /// </example>
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
        /// <param name="value">Original value expected to be changed</param>
        /// <param name="timeout">Optional. See <see cref="Timeouts.ImplicitWait"/></param>
        /// <returns></returns>
        /// <example>
        /// <code lang="vbs">
        /// if TypeName(driver) = "GeckoDriver" then white = "rgb(255, 255, 255)" else white = "rgba(255, 255, 255, 1)"
        /// ' originally the text is white, wait while it remains white:
        /// driver.FindElementByCss("button#colorChange").WaitNotCssValue "color",white
        /// </code>
        /// </example>
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

        /// <summary>
        /// Wait for the web element to be removed from the DOM.
        /// </summary>
        /// <param name="timeout"></param>
        public void WaitRemoval(int timeout = -1) {
            _session.SendUntil(timeout, () => this.IsPresent, (r) => r);
        }

        #endregion


        #region Casting properties

        /// <summary>
        /// Cast the WebElement to a <see cref="SelectElement"/>
        /// </summary>
        public SelectElement AsSelect() {
            return new SelectElement(this);
        }

        /// <summary>
        /// Cast the WebElement to a <see cref="TableElement"/>
        /// </summary>
        public TableElement AsTable() {
            return new TableElement(_session, this);
        }

        #endregion


        #region Javascript

        /// <summary>
        /// Executes a piece of JavaScript in the context of the current element.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="arguments">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        /// <example>
        /// <code lang="vbs">
        ///   Set element = driver.FindElementById("id")
        ///   Debug.Print element.ExecuteScript("return [this.tagName, this.scrollLeft, this.scrollTop];")
        /// </code>
        /// </example>
        public object ExecuteScript(string script, object arguments = null) {
            string script_ex = WrapScript(script);
            object[] args_ex = WrapArguments(this, arguments);
            var result = session.javascript.Execute(script_ex, args_ex, true);
            return result;
        }

        /// <summary>
        /// Execute an asynchronous piece of JavaScript in the context of the current element
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="arguments">Optional arguments for the script.</param>
        /// <param name="timeout">Optional timeout in milliseconds.</param>
        /// <returns>The first argument of the called function callback() .</returns>
        /// <example>
        /// <code lang="vb">
        ///     href = ele.ExecuteAsyncScript("callback(this.href);");"
        /// </code>
        /// </example>
        public object ExecuteAsyncScript(string script, object arguments = null, int timeout = -1) {
            string script_ex = WrapScript(script);
            object[] args_ex = WrapArguments(this, arguments);
            var result = session.javascript.ExecuteAsync(script_ex, args_ex, true, timeout);
            return result;
        }

        /// <summary>
        /// Waits for a script to return true or not null.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="arguments">The arguments to the script.</param>
        /// <param name="timeout">Optional timeout in milliseconds.</param>
        /// <returns>Value not null</returns>
        public object WaitForScript(string script, object arguments = null, int timeout = -1) {
            string script_ex = WrapScript(script);
            object[] args_ex = WrapArguments(this, arguments);
            var result = session.javascript.WaitFor(script_ex, args_ex, timeout);
            return result;
        }

        private static string WrapScript(string script) {
            return "return (function(){" + script + "}).apply(arguments[0],arguments[1]);";
        }

        private static object[] WrapArguments(object item, object arguments) {
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
        /// Determines whether the specified instances are considered equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object obj) {
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
