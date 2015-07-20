using Selenium;
using System.Collections;

namespace Selenium.Core {

    /// <summary>
    /// Javascript object
    /// </summary>
    class JavascriptContext {

        private RemoteSession _session;

        internal JavascriptContext(RemoteSession session) {
            _session = session;
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
            object args;
            if (arguments == null) {
                args = new object[0];
            } else if (arguments is IEnumerable && !(arguments is string || arguments is Dictionary)) {
                args = arguments;
            } else {
                args = new object[] { arguments };
            }
            object result = _session.Send(RequestMethod.POST, "/execute", "script", script, "args", args);
            if (unbox)
                return Unbox(result);
            return result;
        }

        /// <summary>
        /// Waits for a script to return true or not null.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="arguments"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public object WaitFor(string script, object arguments, int timeout = -1) {
            if (!script.TrimStart().StartsWith("return"))
                script = "return " + script;

            if (!script.TrimEnd().EndsWith(";"))
                script = script + ";";

            object result = _session.SendUntil(timeout,
                () => Execute(script, arguments, false),
                (r) => r != null && !false.Equals(r)
            );
            return Unbox(result);
        }

        internal object Unbox(object value) {
            Dictionary valDict = value as Dictionary;
            if (valDict != null) {
                //try parse as web elementr
                string id;
                if (WebElement.TryParse(valDict, out id))
                    return new WebElement(_session, id);

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
