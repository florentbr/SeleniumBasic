using Selenium.Core;
using System;

namespace Selenium {

    /// <summary>
    /// Locators methods for WebDriver and WebElement
    /// </summary>
    public abstract class SearchContext {

        internal abstract RemoteSession session { get; }

        internal abstract string uri { get; }


        /// <summary>
        /// "Verifies that the specified element is somewhere on the page."
        /// </summary>
        /// <param name="by">An element loctor. string or By object</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns>true if the element is present, false otherwise</returns>
        public bool IsElementPresent(By by, int timeout = 0) {
            return FindElement(by, timeout, false) != null;
        }

        /// <summary>
        /// Waits for an element to be missing.
        /// </summary>
        /// <param name="by">Search mechanism</param>
        /// <param name="timeout">Optional - timeout in milliseconds</param>
        public void WaitNotElement(By by, int timeout = -1) {
            if (by.Strategy == Strategy.Any) {
                WaitAnyElementNotPresent(by, timeout);
            } else {
                WaitElementNotPresent(by.Strategy, (string)by.Value, timeout);
            }
        }

        /// <summary>
        /// Finds the first child element matching the given mechanism and value.
        /// </summary>
        /// <param name="strategy">The mechanism by which to find the elements.</param>
        /// <param name="value">The value to use to search for the elements.</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise"></param>
        /// <returns>WebElement</returns>
        public WebElement FindElementBy(Strategy strategy, string value, int timeout = -1, bool raise = true) {
            try {
                return FindFirstElement(strategy, value, timeout);
            } catch (Errors.NoSuchElementError) {
                if (raise)
                    throw new Errors.NoSuchElementError(strategy, value);
                return null;
            }
        }

        /// <summary>
        /// Find the first WebElement using the given method.
        /// </summary>
        /// <param name="by">Methode</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        /// <example>
        /// <code lang="vbs">
        /// Set elts = driver.FindElement(By.Any(By.Name("name"), By.Id("id")))
        /// </code>
        /// </example>
        public WebElement FindElement(By by, int timeout = -1, bool raise = true) {
            try {
                if (by.Strategy == Strategy.Any)
                    return FindAnyElement(by, timeout);
                return FindFirstElement(by.Strategy, (string)by.Value, timeout);
            } catch (Errors.NoSuchElementError) {
                if (raise)
                    throw new Errors.NoSuchElementError(by);
                return null;
            } catch (Errors.ElementNotVisibleError) {
                if (raise)
                    throw new Errors.ElementNotVisibleError(by);
                return null;
            }
        }

        /// <summary>
        /// Finds the first element matching the specified name.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        public WebElement FindElementByName(string name, int timeout = -1, bool raise = true) {
            return this.FindElementBy(Strategy.Name, name, timeout, raise);
        }

        /// <summary>
        /// Finds the first element matching the specified XPath query.
        /// </summary>
        /// <param name="xpath">XPath</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        public WebElement FindElementByXPath(string xpath, int timeout = -1, bool raise = true) {
            return this.FindElementBy(Strategy.XPath, xpath, timeout, raise);
        }

        /// <summary>
        /// Finds the first element matching the specified id.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        public WebElement FindElementById(string id, int timeout = -1, bool raise = true) {
            if( WebDriver.LEGACY )
                return this.FindElementBy(Strategy.Id, id, timeout, raise);
            else {
                return this.FindElementBy(Strategy.Css, "#" + id, timeout, raise);
            }
        }

        /// <summary>
        /// Finds the first element matching the specified CSS class.
        /// </summary>
        /// <param name="classname">Classname</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        public WebElement FindElementByClass(string classname, int timeout = -1, bool raise = true) {
            return this.FindElementBy(Strategy.Class, classname, timeout, raise);
        }

        /// <summary>
        /// Finds the first element matching the specified CSS selector.
        /// </summary>
        /// <param name="cssselector">CSS selector</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        public WebElement FindElementByCss(string cssselector, int timeout = -1, bool raise = true) {
            return this.FindElementBy(Strategy.Css, cssselector, timeout, raise);
        }

        /// <summary>
        /// Finds the first element matching the specified link text.
        /// </summary>
        /// <param name="linktext">Link text</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        public WebElement FindElementByLinkText(string linktext, int timeout = -1, bool raise = true) {
            return this.FindElementBy(Strategy.LinkText, linktext, timeout, raise);
        }

        /// <summary>
        /// Finds the first of elements that match the part of the link text supplied
        /// </summary>
        /// <param name="partiallinktext">Partial link text</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        public WebElement FindElementByPartialLinkText(string partiallinktext, int timeout = -1, bool raise = true) {
            return this.FindElementBy(Strategy.PartialLinkText, partiallinktext, timeout, raise);
        }

        /// <summary>
        /// Finds the first element matching the specified tag name.
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns><see cref="WebElement" /> or null</returns>
        public WebElement FindElementByTag(string tagname, int timeout = -1, bool raise = true) {
            return this.FindElementBy(Strategy.Tag, tagname, timeout, raise);
        }


        /// <summary>
        /// Finds all child elements matching the given mechanism and value.
        /// </summary>
        /// <param name="strategy">The mechanism by which to find the elements.</param>
        /// <param name="value">The value to use to search for the elements.</param>
        /// <param name="minimum">Minimum of elements expected</param>
        /// <param name="timeout">Timeout in millisecond</param>
        /// <returns></returns>
        public WebElements FindElementsBy(Strategy strategy, string value, int minimum = 0, int timeout = 0) {
            return this.FindAllElements(strategy, (string)value, minimum, timeout);
        }

        /// <summary>
        /// Find all elements within the current context using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns>A list of all WebElements, or an empty list if nothing matches</returns>
        /// <example>
        /// <code lang="vbs">
        /// Set elts = driver.FindElements(By.Any(By.Name("name"), By.Id("id")))
        /// </code>
        /// </example>
        public WebElements FindElements(By by, int minimum = 0, int timeout = 0) {
            if (by.Strategy == Strategy.Any)
                return this.FindAnyElements(by, minimum, timeout);
            return this.FindElementsBy(by.Strategy, (string)by.Value, minimum, timeout);
        }

        /// <summary>
        /// Finds elements matching the specified name.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns><see cref="WebElements" /></returns>
        public WebElements FindElementsByName(string name, int minimum = 0, int timeout = 0) {
            return this.FindElementsBy(Strategy.Name, name, minimum, timeout);
        }

        /// <summary>
        /// Finds elements matching the specified XPath query.
        /// </summary>
        /// <param name="xpath">XPath</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns><see cref="WebElements" /></returns>
        public WebElements FindElementsByXPath(string xpath, int minimum = 0, int timeout = 0) {
            return this.FindElementsBy(Strategy.XPath, xpath, minimum, timeout);
        }

        /// <summary>
        /// Finds elements matching the specified id.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns><see cref="WebElements" /></returns>
        public WebElements FindElementsById(string id, int minimum = 0, int timeout = 0) {
            return this.FindElementsBy(Strategy.Id, id, minimum, timeout);
        }

        /// <summary>
        /// Finds elements matching the specified CSS class.
        /// </summary>
        /// <param name="classname">Class name</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns><see cref="WebElements" /></returns>
        public WebElements FindElementsByClass(string classname, int minimum = 0, int timeout = 0) {
            return this.FindElementsBy(Strategy.Class, classname, minimum, timeout);
        }

        /// <summary>
        /// Finds elements matching the specified CSS selector.
        /// </summary>
        /// <param name="cssselector">CSS selector</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns><see cref="WebElements" /></returns>
        public WebElements FindElementsByCss(string cssselector, int minimum = 0, int timeout = 0) {
            return this.FindElementsBy(Strategy.Css, cssselector, minimum, timeout);
        }

        /// <summary>
        /// Finds elements matching the specified link text.
        /// </summary>
        /// <param name="linktext">Link text</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns><see cref="WebElements" /></returns>
        public WebElements FindElementsByLinkText(string linktext, int minimum = 0, int timeout = 0) {
            return this.FindElementsBy(Strategy.LinkText, linktext, minimum, timeout);
        }

        /// <summary>
        /// Finds the first of elements that match the part of the link text supplied
        /// </summary>
        /// <param name="partiallinktext">Partial link text</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns><see cref="WebElements" /></returns>
        public WebElements FindElementsByPartialLinkText(string partiallinktext, int minimum = 0, int timeout = 0) {
            return this.FindElementsBy(Strategy.PartialLinkText, partiallinktext, minimum, timeout);
        }

        /// <summary>
        /// Finds elements matching the specified tag name.
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="minimum">Minimum number of elements to wait for</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns><see cref="WebElements" /></returns>
        public WebElements FindElementsByTag(string tagname, int minimum = 0, int timeout = 0) {
            return this.FindElementsBy(Strategy.Tag, tagname, minimum, timeout);
        }

        /// <summary>
        /// Returns a web element matching the given method and value or null if no element found
        /// </summary>
        private WebElement FindFirstElement(Strategy strategy, string value, int timeout) {
            RemoteSession session = this.session;
            string relativeUri = this.uri + "/element";
            Dictionary element;
            try {
                if( !WebDriver.LEGACY )
                    TranslateStrategy( ref strategy, ref value );
                string method = By.FormatStrategy(strategy);
                element = (Dictionary)session.Send(RequestMethod.POST, relativeUri, "using", method, "value", value);
            } catch (Errors.NoSuchElementError) {
                if (timeout == 0)
                    throw;
                var endTime = session.GetEndTime(timeout);
                while (true) {
                    SysWaiter.Wait();
                    try {
                        element = (Dictionary)session.SendAgain();
                        break;
                    } catch (Errors.NoSuchElementError) {
                        if (DateTime.UtcNow > endTime)
                            throw;
                    }
                }
            }
            return new WebElement(session, element);
        }

        private bool TranslateStrategy( ref Strategy strategy, ref string value ) {
            switch( strategy ) {
            case Strategy.Id:
                strategy = Strategy.Css;
                value = "#" + value;
                return true;
            case Strategy.Class:
                strategy = Strategy.Css;
                value = "." + value;
                return true;
            case Strategy.Name:
                strategy = Strategy.XPath;
                value = ( this is WebElement ? "." : "" ) + "//*[@name='" + value + "']";
                return true;
            }
            return false;
        }

        private WebElement FindAnyElement(By byAny, int timeout) {
            RemoteSession session = this.session;
            string relativeUri = this.uri + "/element";
            Dictionary element;
            DateTime endTime = session.GetEndTime(timeout);
            while (true) {
                foreach (By by in (By[])byAny.Value) {
                    if (by == null)
                        break;
                    try {
                        Strategy strategy = by.Strategy;
                        string value = by.Value.ToString();
                        if( !WebDriver.LEGACY )
                            TranslateStrategy( ref strategy, ref value );
                        string method = By.FormatStrategy(strategy);
                        element = (Dictionary)session.Send(RequestMethod.POST, relativeUri, "using", method, "value", value);
                        return new WebElement(session, element);
                    } catch (Errors.NoSuchElementError) { }
                }
                if (DateTime.UtcNow > endTime)
                    throw new Errors.NoSuchElementError(byAny);
                SysWaiter.Wait();
            }
        }

        /// <summary>
        /// Returns all the web elements matching the given method and value
        /// </summary>
        private WebElements FindAllElements(Strategy strategy, string value, int minimum, int timeout) {
            RemoteSession session = this.session;
            string uri = this.uri + "/elements";
            try {
                if( !WebDriver.LEGACY )
                    TranslateStrategy( ref strategy, ref value );
                var method = By.FormatStrategy(strategy);
                List elements = session.SendUntil(timeout,
                    () => (List)session.Send(RequestMethod.POST, uri, "using", method, "value", value),
                    (r) => r.Count >= minimum
                );
                return new WebElements(session, elements);
            } catch (Errors.TimeoutError) {
                throw new Errors.NoSuchElementError(strategy, value);
            }
        }

        private WebElements FindAnyElements(By byAny, int minimum, int timeout) {
            RemoteSession session = this.session;
            string uri = this.uri + "/elements";
            WebElements webelements = new WebElements();
            DateTime endTime = session.GetEndTime(timeout);
            while (true) {
                foreach (By by in (By[])byAny.Value) {
                    if (by == null)
                        break;
                    Strategy strategy = by.Strategy;
                    var value = (string)by.Value;
                    if( !WebDriver.LEGACY )
                        TranslateStrategy( ref strategy, ref value );
                    var method = By.FormatStrategy(strategy);
                    List elements = (List)session.Send(RequestMethod.POST, uri, "using", method, "value", value);
                    webelements.Add(session, elements);
                }
                if (webelements.Count >= minimum)
                    return webelements;
                if (DateTime.UtcNow > endTime)
                    throw new Errors.NoSuchElementError(byAny);
                SysWaiter.Wait();
            }
        }

        /// <summary>
        /// Returns a web element matching the given method and value
        /// </summary>
        private void WaitElementNotPresent(Strategy strategy, string value, int timeout) {
            RemoteSession session = this.session;
            string uri = this.uri + "/element";
            if( !WebDriver.LEGACY )
                TranslateStrategy( ref strategy, ref value );
            string method = By.FormatStrategy(strategy);
            DateTime endTime = session.GetEndTime(timeout);
            try {
                session.Send(RequestMethod.POST, uri, "using", method, "value", value);
                while (true) {
                    SysWaiter.Wait();
                    session.SendAgain();
                    if (DateTime.UtcNow > endTime)
                        throw new Errors.ElementPresentError(strategy, value);
                }
            } catch (Errors.NoSuchElementError) { }
        }

        /// <summary>
        /// Returns a web element matching the given method and value
        /// </summary>
        private void WaitAnyElementNotPresent(By byAny, int timeout) {
            RemoteSession session = this.session;
            string uri = this.uri + "/element";
            DateTime endTime = session.GetEndTime(timeout);
            foreach (By by in (By[])byAny.Value) {
                if (by == null)
                    break;
                try {
                    Strategy strategy = by.Strategy;
                    string value = by.Value.ToString();
                    if( !WebDriver.LEGACY )
                        TranslateStrategy( ref strategy, ref value );
                    string method = By.FormatStrategy(strategy);
                    session.Send(RequestMethod.POST, uri, "using", method, "value", value);
                    while (true) {
                        if (DateTime.UtcNow > endTime)
                            throw new Errors.ElementPresentError(byAny);
                        SysWaiter.Wait();
                        session.SendAgain();
                    }
                } catch (Errors.NoSuchElementError) { }
            }
        }

    }

}
