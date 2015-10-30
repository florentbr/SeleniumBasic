using Selenium.Core;
using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Manage object
    /// </summary>
    [ProgId("Selenium.Manage")]
    [Guid("0277FC34-FD1B-4616-BB19-AC0E93DB0DE2")]
    [Description("Manage object")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Manage : ComInterfaces._Manage {

        private RemoteSession _session;
        private Logs _logs;
        private Storage _storageLocal;
        private Storage _storageSession;
        //private IME _ime;

        internal Manage(RemoteSession session) {
            _session = session;
        }

        /// <summary>
        /// 
        /// </summary>
        public Timeouts Timeouts {
            get {
                return _session.timeouts;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Logs Logs {
            get {
                return _logs ?? (_logs = new Logs(_session));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Storage StorageLocal {
            get {
                return _storageLocal ?? (_storageLocal = new Storage(_session, Storage.StorageType.LocalStorage));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Storage StorageSession {
            get {
                return _storageSession ?? (_storageSession = new Storage(_session, Storage.StorageType.SessionStorage));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary Capabilities {
            get {
                return _session.capabilities;
            }
        }

        /*
        public IME IME {
            get { 
                return _ime ?? (_ime = new IME(_session)); 
            }
        }
        */


        #region Location

        /// <summary>
        /// Get the current geo location.
        /// </summary>
        /// <returns>The current geo location. {latitude: number, longitude: number, altitude: number} </returns>
        public Dictionary Location() {
            return (Dictionary)_session.Send(RequestMethod.GET, "/location");
        }

        /// <summary>
        /// Set the current geo location.
        /// </summary>
        public void SetLocation(int latitude, int longitude, int altitude) {
            Dictionary location = new Dictionary();
            location.Add("latitude", latitude);
            location.Add("longitude", longitude);
            location.Add("altitude", altitude);
            _session.Send(RequestMethod.POST, "/location", "location", location);
        }

        #endregion


        #region Cookies

        /// <summary>
        /// Collection of cookies for the current page
        /// </summary>
        /// <returns>CookieCollection</returns>
        public Cookies Cookies {
            get {
                List rawCookies = Cookies.GetAll(_session);
                return new Cookies(_session, rawCookies);
            }
        }

        /// <summary>
        /// Adds a cookie to the current page.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <param name="expiry"></param>
        /// <param name="secure"></param>
        /// <param name="httpOnly"></param>
        public void AddCookie(string name, string value, string domain = null, string path = null, string expiry = null, bool secure = false, bool httpOnly = false) {
            Cookies.AddOne(_session, name, value, path, domain, expiry, secure, httpOnly);
        }

        /// <summary>
        /// Deletes all cookies from the page.
        /// </summary>
        public void DeleteAllCookies() {
            Cookies.DeleteAll(_session);
        }

        /// <summary>
        /// Deletes the cookie with the specified name from the page.
        /// </summary>
        /// <param name="name">The name of the cookie to be deleted.</param>
        public void DeleteCookieByName(string name) {
            Cookie.DeleteByName(_session, name);
        }

        /// <summary>
        /// Gets a cookie with the specified name.
        /// </summary>
        /// <param name="namePattern">The name of the cookie to retrieve.</param>
        /// <returns>The Cookie containing the name. Returns null if no cookie with the specified name is found.</returns>
        public Cookie FindCookieByName(string namePattern) {
            return Cookie.FindByName(_session, namePattern);
        }

        #endregion

    }

}
