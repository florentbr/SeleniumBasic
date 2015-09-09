using Selenium.Core;
using Selenium;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Collection of web elements
    /// </summary>
    [ProgId("Selenium.WebElements")]
    [Guid("0277FC34-FD1B-4616-BB19-460D25F4D2B3")]
    [Description("Collection of Web elements (One-based)")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class WebElements : List, ComInterfaces._WebElements {

        internal WebElements() {

        }

        internal WebElements(List webElements)
            : base(webElements.Values()) {
        }

        internal WebElements(RemoteSession session, List rawElements)
            : base(rawElements.Count) {
            _count = _items.Length;
            for (int i = 0; i < _count; i++) {
                _items[i] = new WebElement(session, (Dictionary)rawElements[i]);
            }
        }

        internal void Add(RemoteSession session, List rawElements) {
            int count = rawElements.Count;
            int size = _count + count;
            if (size > _items.Length)
                IncreaseSize(size);
            for (int i = 0; i < count; i++) {
                _items[_count++] = new WebElement(session, (Dictionary)rawElements[i]);
            }
        }

        /// <summary>
        /// Returns the first item
        /// </summary>
        public new WebElement First() {
            return (WebElement)base.First();
        }

        /// <summary>
        /// Returns the last item
        /// </summary>
        public new WebElement Last() {
            return (WebElement)base.Last();
        }

        /// <summary>
        /// Adds a web element
        /// </summary>
        /// <param name="element"></param>
        public void Add(WebElement element) {
            base.Add(element);
        }

        /// <summary>
        /// Get the WebElement at the provided index
        /// </summary>
        /// <param name="index">Base zero index</param>
        /// <returns>WebElement</returns>
        public new WebElement this[int index] {
            get {
                return (WebElement)base[index];
            }
        }

        /// <summary>
        /// Get the WebElement at the provided index
        /// </summary>
        /// <param name="index">Base one index</param>
        /// <returns>WebElement</returns>
        WebElement ComInterfaces._WebElements.this[int index] {
            get {
                return (WebElement)base[index - 1];
            }
        }

        /// <summary>
        /// Returns a list with the attribute for each element
        /// </summary>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="withAttributeOnly">True to skip elements without the attribute</param>
        /// <returns></returns>
        public List Attribute(string attributeName, bool withAttributeOnly = true) {
            if (this.Count == 0)
                return new List();
            var session = ((WebElement)base[0])._session;
            var args = new object[] { this, attributeName };
            string script = @"var e=arguments[0],p=arguments[1],a=[],r;"
                + "for(var i=0;i<e.length;i++){r=e[i].getAttribute(p);"
                + (withAttributeOnly ? "if(r!=null)a.push(r);" : "a.push(r);")
                + "};return a;";
            var attributes = (List)session.javascript.Execute(script, args, false);
            return attributes;
        }

        /// <summary>
        /// Execute a script against each web element and returns all the results;
        /// </summary>
        /// <param name="script">Javascript script</param>
        /// <param name="ignoreNulls">Null elements are skiped</param>
        /// <returns>List</returns>
        /// <example>
        /// <code lang="vbs">
        /// Set driver = CreateObject("Selenium.FirefoxDriver")
        /// driver.Get "https://www.google.co.uk/search?q=Eiffel+tower"
        /// Set links = driver.FindElementsByTagName("a").ExecuteScript("return this.getAttribute('href');")
        /// WScript.Echo "Count:" &amp; links.Count &amp; " Values:" &amp; vbCr &amp; Join(links.Values, vbCr)
        /// </code>
        /// </example>
        public List ExecuteScript(string script, bool ignoreNulls = true) {
            if (this.Count == 0)
                return null;
            var session = ((WebElement)base[0])._session;
            object[] args = new object[] { this };
            string script2 = "var f=function(){" + script + "};"
                + @"var e=arguments[0],r=[];"
                + "for(var i=0;i<e.length;i++){"
                + (ignoreNulls ? "v=f.apply(e[i]);if(v!=null)r.push(v);" : "r.push(f.apply(e[i]));")
                + "}return r;";
            var results = (List)session.javascript.Execute(script2, args, true);
            return results;
        }

        /// <summary>
        /// Returns a list containing the text for each element
        /// </summary>
        /// <param name="offsetStart"></param>
        /// <param name="offsetEnd"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public List Text(int offsetStart = 0, int offsetEnd = 0, bool trim = true) {
            int size = base.Count;
            int imax = size + offsetEnd - 1;
            int imin = offsetStart;
            List lst = new List(size - offsetStart + offsetEnd);
            int i = -1;
            foreach (WebElement ele in this) {
                if (++i < imin || i > imax)
                    continue;
                var text = ele.Text();
                if (trim) {
                    lst.Add(text.Trim());
                } else {
                    lst.Add(text);
                }
            }
            return lst;
        }

        /// <summary>
        /// Returns a list containing the text parsed to a number for each element
        /// </summary>
        /// <param name="defaultValue">Default value in case the text can't be parsed.</param>
        /// <param name="offsetStart"></param>
        /// <param name="offsetEnd"></param>
        /// <returns></returns>
        public List Values(object defaultValue = null, int offsetStart = 0, int offsetEnd = 0) {
            int size = base.Count;
            int imin = offsetStart;
            int imax = size - 1 + offsetEnd;
            List lst = new List(size - offsetStart + offsetEnd);
            int i = -1;
            foreach (WebElement ele in this) {
                if (++i < imin || i > imax)
                    continue;
                string text = ele.Text().Trim();
                double num;
                if (double.TryParse(text, out num)) {
                    lst.Add(num);
                } else {
                    lst.Add(defaultValue);
                }
            }
            return lst;
        }


        IEnumerator ComInterfaces._WebElements._NewEnum() {
            return GetEnumerator();
        }

    }

}
