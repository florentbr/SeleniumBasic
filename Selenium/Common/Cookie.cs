using Selenium.Core;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Selenium {

    /// <summary>
    /// Cookie object
    /// </summary>
    [ProgId("Selenium.Cookie")]
    [Guid("0277FC34-FD1B-4616-BB19-2762FC4A1BF5")]
    [Description("Cookie object")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Cookie : ComInterfaces._Cookie {

        #region Static

        /// <summary>
        /// Get a cookie matching a name from the current page.
        /// </summary>
        internal static Cookie FindByName(RemoteSession session, string namePattern) {
            List cookies = Cookies.GetAll(session);
            Regex re = new Regex(namePattern, RegexOptions.IgnoreCase);
            foreach (Dictionary cookie in cookies) {
                if (re.IsMatch((string)cookie["name"]))
                    return new Cookie(session, cookie);
            }
            throw new Errors.NoSuchCookieError("Cookie not found. Pattern: " + namePattern);
        }

        /// <summary>
        /// Delete the cookie matching the name on the current page.
        /// </summary>
        /// <returns>object</returns>
        internal static void DeleteByName(RemoteSession session, string name) {
            session.Send(RequestMethod.DELETE, "/cookie/" + name);
        }

        internal static readonly DateTime BASE_TIME = new DateTime(1970, 1, 1, 0, 0, 0);

        #endregion


        RemoteSession _session;
        string _name;
        string _value;
        string _path;
        string _domain;
        double _expiry;
        bool _secure;

        internal Cookie(RemoteSession session, Dictionary dict) {
            _session = session;
            try {
                _name = dict.GetValue("name", string.Empty);
                _value = dict.GetValue("value", string.Empty);
                _path = dict.GetValue("path", string.Empty);
                _domain = dict.GetValue("domain", string.Empty);
                _secure = Convert.ToBoolean(dict.Get("secure", false));
                _expiry = Convert.ToDouble(dict.Get("expiry", 0));
            } catch (Errors.KeyNotFoundError ex) {
                throw new DeserializeException(typeof(Cookie), ex);
            } catch (Exception ex) {
                throw new SeleniumException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get { return _value; } }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get { return _path; } }

        /// <summary>
        /// 
        /// </summary>
        public string Domain { get { return _domain; } }

        /// <summary>
        /// 
        /// </summary>
        public bool Secure { get { return _secure; } }

        /// <summary>
        /// 
        /// </summary>
        public string Expiry {
            get {
                if (_expiry == 0)
                    return string.Empty;
                return BASE_TIME.AddSeconds(_expiry).ToString("s");
            }
        }

        /// <summary>
        /// Deletes the cookie from the browser.
        /// </summary>
        public void Delete() {
            DeleteByName(_session, _name);
        }

    }

}
