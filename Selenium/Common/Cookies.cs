using Selenium.Core;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Cookie collection
    /// </summary>
    [ProgId("Selenium.Cookies")]
    [Guid("0277FC34-FD1B-4616-BB19-313080DBB4DA")]
    [Description("Collection of cookies (One-based)")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Cookies : List, ComInterfaces._Cookies {

        #region Static methods

        /// <summary>
        /// Get all cookies visible to the current page.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        internal static List GetAll(RemoteSession session) {
            return (List)session.Send(RequestMethod.GET, "/cookie");
        }

        /// <summary>
        /// Adds a new cookie.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <param name="expiry"></param>
        /// <param name="secure"></param>
        /// <param name="httpOnly"></param>
        internal static void AddOne(RemoteSession session, string name, string value, string path, string domain, string expiry, bool secure, bool httpOnly) {
            var dict = new Dictionary();
            dict.Add("name", name);
            dict.Add("value", value);
            if (path != null)
                dict.Add("path", path);
            if (domain != null)
                dict.Add("domain", domain);
            if (expiry != null) {
                var dt = (long)(DateTime.Parse(expiry) - Cookie.BASE_TIME).TotalSeconds;
                dict.Add("expiry", dt);
            }
            if (secure == true)
                dict.Add("secure", true);
            if (httpOnly == true)
                dict.Add("httpOnly", true);
            session.Send(RequestMethod.POST, "/cookie", "cookie", dict);
        }

        /// <summary>
        /// Delete all cookies visible to the current page.
        /// </summary>
        internal static void DeleteAll(RemoteSession session) {
            session.Send(RequestMethod.DELETE, "/cookie");
        }

        #endregion


        internal Cookies(RemoteSession session, List rawCookies)
            : base(rawCookies.Count) {

            _count = _items.Length;
            for (int i = 0; i < _count; i++) {
                _items[i] = new Cookie(session, (Dictionary)rawCookies[i]);
            }
        }

        /// <summary>
        /// Returns the cookie at index (Zero based)
        /// </summary>
        /// <param name="index">Zero based index</param>
        /// <returns></returns>
        public new Cookie this[int index] {
            get {
                return (Cookie)base[index];
            }
        }

        /// <summary>
        /// Returns the cookie for the given name
        /// </summary>
        /// <param name="name">Cookie name</param>
        /// <returns></returns>
        public Cookie this[string name] {
            get {
                for (int i = _count; i-- > 0; ) {
                    Cookie item = (Cookie)_items[i];
                    if (string.Compare(name, item.Name, true) == 0)
                        return item;
                }
                throw new SeleniumError("Cookie not found: " + name);
            }
        }

        /// <summary>
        /// Gets an item at the provided index (One based).
        /// </summary>
        /// <param name="identifier">Name or index (based one)</param>
        /// <returns>Cookie</returns>
        Cookie ComInterfaces._Cookies.this[object identifier] {
            get {
                if (identifier is string)
                    return this[(string)identifier];
                if (identifier is short)
                    return this[(short)identifier - 1];
                if (identifier is int)
                    return this[(int)identifier - 1];
                throw new Errors.ArgumentError("Invalid type for argument identifier.");
            }
        }

        IEnumerator ComInterfaces._Cookies._NewEnum() {
            return base.GetEnumerator();
        }

    }

}
