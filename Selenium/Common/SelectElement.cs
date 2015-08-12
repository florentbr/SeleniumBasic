using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium {

    /// <summary>
    /// Web element of type Select
    /// </summary>
    /// <example>
    /// Select an element by text
    /// <code lang="vbs">	
    /// driver.FindElementById("select").AsSelect.SelectByText "Option Two"
    /// </code>
    /// </example>
    [ProgId("Selenium.SelectElement")]
    [Guid("0277FC34-FD1B-4616-BB19-336CEF3B4EFC")]
    [Description("Select web element")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class SelectElement : ComInterfaces._SelectElement {

        internal readonly WebElement _element;

        internal SelectElement(WebElement element) {
            string tag = element.TagName.ToLowerInvariant();
            if ("select" != tag)
                throw new Errors.UnexpectedTagNameError("select", tag);
            _element = element;
            object attribute = element.Attribute("multiple");
            this.IsMultiple = attribute != null && attribute.ToString().ToLowerInvariant() != "false";
        }


        /// <summary>
        /// Gets a value indicating whether the parent element supports multiple selections.
        /// </summary>
        public bool IsMultiple { get; private set; }

        /// <summary>
        /// Gets the list of options for the select element.
        /// </summary>
        public WebElements Options {
            get {
                return _element.FindElementsByTag("option");
            }
        }

        /// <summary>
        /// Gets the selected item within the select element.
        /// </summary>
        /// <remarks>If more than one item is selected this will return the first item.</remarks>
        /// <exception cref="Errors.NoSuchElementError">Thrown if no option is selected.</exception>
        public WebElement SelectedOption {
            get {
                foreach (WebElement option in this.Options) {
                    if (option.IsSelected)
                        return option;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets all of the selected options within the select element.
        /// </summary>
        public WebElements AllSelectedOptions {
            get {
                var returnValue = new WebElements();
                foreach (WebElement option in this.Options) {
                    if (option.IsSelected)
                        returnValue.Add(option);
                }
                return returnValue;
            }
        }

        /// <summary>
        /// Select all options by the text displayed.
        /// </summary>
        /// <param name="text">The text of the option to be selected. If an exact match is not found,
        /// this method will perform a substring match.</param>
        /// <remarks>When given "Bar" this method would select an option like:
        /// <para>
        /// &lt;option value="foo"&gt;Bar&lt;/option&gt;
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.NoSuchElementError">Thrown if there is no element with the given text present.</exception>
        public void SelectByText(string text) {
            if (text == null)
                throw new Errors.ArgumentError("text argument must not be null");
            var options = _element.FindElementsByXPath(".//option[normalize-space(.) = " + EscapeString(text) + "]");

            bool matched = false;
            foreach (WebElement option in options) {
                SetSelected(option);
                if (!this.IsMultiple)
                    return;
                matched = true;
            }

            if (options.Count == 0 && text.Contains(" ")) {
                string substringWithoutSpace = GetLongestToken(text);
                WebElements candidates;
                if (string.IsNullOrEmpty(substringWithoutSpace)) {
                    candidates = _element.FindElementsByTag("option");
                } else {
                    candidates = _element.FindElementsByXPath(".//option[contains(., " + EscapeString(substringWithoutSpace) + ")]");
                }
                foreach (WebElement option in candidates) {
                    if (text == option.Text()) {
                        SetSelected(option);
                        if (!this.IsMultiple)
                            return;
                        matched = true;
                    }
                }
            }

            if (!matched)
                throw new Errors.NoSuchElementError("Cannot locate element with text: " + text);
        }

        /// <summary>
        /// Select an option by the value.
        /// </summary>
        /// <param name="value">The value of the option to be selected.</param>
        /// <remarks>When given "foo" this method will select an option like:
        /// <para>
        /// &lt;option value="foo"&gt;Bar&lt;/option&gt;
        /// </para>
        /// </remarks>
        /// <exception cref="Errors.NoSuchElementError">Thrown when no element with the specified value is found.</exception>
        public void SelectByValue(string value) {
            string xpath = ".//option[@value = " + EscapeString(value) + "]";
            var options = _element.FindElementsByXPath(xpath);
            bool matched = false;
            foreach (WebElement option in options) {
                SetSelected(option);
                if (!this.IsMultiple)
                    return;
                matched = true;
            }
            if (!matched)
                throw new Errors.NoSuchElementError("Cannot locate option with value: " + value);
        }

        /// <summary>
        /// Select the option by the index, as determined by the "index" attribute of the element.
        /// </summary>
        /// <param name="index">The value of the index attribute of the option to be selected.</param>
        /// <exception cref="Errors.NoSuchElementError">Thrown when no element exists with the specified index attribute.</exception>
        public void SelectByIndex(int index) {
            string match = index.ToString(CultureInfo.InvariantCulture);
            bool matched = false;
            foreach (WebElement option in this.Options) {
                if (match.Equals(option.Attribute("index"))) {
                    SetSelected(option);
                    if (!this.IsMultiple)
                        return;
                    matched = true;
                }
            }
            if (!matched)
                throw new Errors.NoSuchElementError("Cannot locate option with index: " + index);
        }

        /// <summary>
        /// Clear all selected entries. This is only valid when the SELECT supports multiple selections.
        /// </summary>
        /// <exception cref="Errors.InvalidOperationError">Thrown when attempting to deselect all options from a SELECT 
        /// that does not support multiple selections.</exception>
        public void DeselectAll() {
            if (!this.IsMultiple)
                throw new Errors.InvalidOperationError("You may only deselect all options if multi-select is supported");
            foreach (WebElement option in this.Options)
                UnsetSelected(option);
        }

        /// <summary>
        /// Deselect the option by the text displayed.
        /// </summary>
        /// <param name="text">The text of the option to be deselected.</param>
        /// <remarks>When given "Bar" this method would deselect an option like:
        /// <para>
        /// &lt;option value="foo"&gt;Bar&lt;/option&gt;
        /// </para>
        /// </remarks>
        public void DeselectByText(string text) {
            string xpath = ".//option[normalize-space(.) = " + EscapeString(text) + "]";
            var options = _element.FindElementsByXPath(xpath);
            foreach (WebElement option in options)
                UnsetSelected(option);
        }

        /// <summary>
        /// Deselect the option having value matching the specified text.
        /// </summary>
        /// <param name="value">The value of the option to deselect.</param>
        /// <remarks>When given "foo" this method will deselect an option like:
        /// <para>
        /// &lt;option value="foo"&gt;Bar&lt;/option&gt;
        /// </para>
        /// </remarks>
        public void DeselectByValue(string value) {
            string xpath = ".//option[@value = " + EscapeString(value) + "]";
            var options = _element.FindElementsByXPath(xpath);
            foreach (WebElement option in options)
                UnsetSelected(option);
        }

        /// <summary>
        /// Deselect the option by the index, as determined by the "index" attribute of the element.
        /// </summary>
        /// <param name="index">The value of the index attribute of the option to deselect.</param>
        public void DeselectByIndex(int index) {
            string match = index.ToString(CultureInfo.InvariantCulture);
            foreach (WebElement option in this.Options) {
                if (match.Equals(option.Attribute("index")) && option.IsSelected)
                    option.Click();
            }
        }

        #region Support

        private static void SetSelected(WebElement option) {
            if (!option.IsSelected)
                option.Click();
        }

        private static void UnsetSelected(WebElement option) {
            if (option.IsSelected)
                option.Click();
        }

        private static string EscapeString(string txt) {
            var hasQuote = txt.IndexOf('\'') != -1;
            var hasDblQuote = txt.IndexOf('"') != -1;
            if (hasQuote && hasDblQuote) {
                var sb = new StringBuilder("concat(", 100);
                var substr = txt.Split('"');
                for (int i = 0, len = substr.Length - 1; i < len; i++) {
                    sb.Append('"').Append(substr[i]).Append('"');
                    sb.Append(", '\"', ");
                }
                if (txt[txt.Length - 1] == '"')
                    sb.Append(", '\"'");
                return sb.Append(')').ToString();
            }
            if (hasDblQuote)
                return "'" + txt + "'";
            return "\"" + txt + "\"";
        }

        private static string GetLongestToken(string txt) {
            var result = string.Empty;
            var substr = txt.Split(' ');
            foreach (string str in substr) {
                if (str.Length > result.Length)
                    result = str;
            }
            return result;
        }

        #endregion
    }

}
