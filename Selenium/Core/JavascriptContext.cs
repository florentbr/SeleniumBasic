using Selenium;
using System.Collections;

namespace Selenium.Core {

    /// <summary>
    /// Javascript object
    /// </summary>
    class JavascriptContext {

        private readonly RemoteSession session;

        internal JavascriptContext(RemoteSession session) {
            this.session = session;
        }

        /// <summary>
        /// Inject a snippet of JavaScript into the page for execution in the context
        /// of the currently selected frame.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="arguments">Optional - The script arguments.</param>
        /// <param name="unbox">Optional - Converts web elements to objects</param>
        /// <returns></returns>
        public object Execute(string script, object arguments = null, bool unbox = true) {
            object result = this.session.Send(RequestMethod.POST, "/execute", "script", script, "args", arguments);
            if (unbox)
                return Unbox(result);
            return result;
        }

        /// <summary>
        /// Inject a snippet of JavaScript into the page for execution in the context
        /// of the currently selected frame.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="arguments">Optional - The script arguments.</param>
        /// <param name="unbox">Optional - Converts web elements to objects</param>
        /// <param name="timeout">Optional - Optional timeout in milliseconds.</param>
        /// <returns></returns>
        public object ExecuteAsync(string script, object arguments = null, bool unbox = true, int timeout = -1) {
            if (timeout != -1)
                session.timeouts.Script = timeout;

            string script_ex = "var callback=arguments[--arguments.length];" + script;

            object result = this.session.Send(RequestMethod.POST, "/execute_async", "script", script_ex, "args", arguments);
            if (unbox)
                return Unbox(result);
            return result;
        }

        /// <summary>
        /// Waits for a script to return true or not null.
        /// </summary>
        /// <param name="script">Piece of JavaScript code to execute.</param>
        /// <param name="arguments">Optional arguments for the script.</param>
        /// <param name="timeout">Optional timeout in milliseconds.</param>
        /// <returns></returns>
        public object WaitFor(string script, object arguments = null, int timeout = -1) {
            if (timeout != -1)
                session.timeouts.Script = timeout;

            bool has_return = (script.IndexOf("return") & -4) == 0;
            string script_ex
                = "var callback=arguments[--arguments.length];"
                + "var evl=function(){" + (has_return ? string.Empty : "return ") + script + "};"
                + "var tst=function(){"
                + " var res=evl();"
                + " if(res) callback(res); else setTimeout(tst, 60);"
                + "};"
                + "tst();";

            object result = this.session.Send(RequestMethod.POST, "/execute_async", "script", script_ex, "args", arguments);
            return Unbox(result);
        }

        internal object Unbox(object value) {
            Dictionary valDict = value as Dictionary;
            if (valDict != null) {
                //try parse as web elementr
                string id;
                if (WebElement.TryParse(valDict, out id))
                    return new WebElement(this.session, id);

                foreach (DictionaryItem item in valDict)
                    item.Value = Unbox(item.Value);
                return valDict;
            }

            List valArr = value as List;
            if (valArr != null) {
                bool isElementCollection = true;
                for (int i = valArr.Count; i-- > 0; ) {
                    object parsedItem = Unbox(valArr[i]);
                    valArr[i] = parsedItem;
                    if (!(parsedItem is WebElement))
                        isElementCollection = false;
                }
                if (valArr.Count > 0 && isElementCollection)
                    return new WebElements(valArr);
                return valArr;
            }

            return value;
        }

    }

}
