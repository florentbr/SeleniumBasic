using Selenium;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Proxy options
    /// </summary>
    /// <example>
    /// <code lang="vbs">	
    /// Dim driver As New FirefoxDriver
    /// driver.Proxy.SetHttpProxy "http://myproxy.com:8273"
    /// driver.Get "http://www..."
    /// </code>
    /// </example>
    [ProgId("Selenium.Proxy")]
    [Guid("0277FC34-FD1B-4616-BB19-8FEA46200810")]
    [Description("Proxy options")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Proxy : ComInterfaces._Proxy {

        const string KEY_PROXY = "proxy";
        const string KEY_TYPE = "proxyType";
        const string VAL_MANUAL = "manual";

        private Dictionary _capabilities;
        private Dictionary _proxy;

        internal Proxy(Dictionary capabilities) {
            _capabilities = capabilities;
            _proxy = null;
        }

        private void Set(string key, object value) {
            if (_proxy == null) {
                _proxy = new Dictionary();
                _capabilities.Add(KEY_PROXY, _proxy);
            }
            _proxy.Set(key, value);
        }

        /// <summary>
        /// Use proxy with automatic detection.
        /// </summary>
        public void SetAutodetect() {
            Set(KEY_TYPE, "autodetect");
            Set("autodetect", true);
        }

        /// <summary>
        /// Proxy with automatic configuration from URL.
        /// </summary>
        /// <param name="url"></param>
        public void SetAutoConfigure(string url) {
            Set(KEY_TYPE, "pac");
            Set("proxyAutoconfigUrl", url);
        }

        /// <summary>
        /// Use an HTTP proxy
        /// </summary>
        /// <param name="value"></param>
        public void SetHttpProxy(string value) {
            Set(KEY_TYPE, VAL_MANUAL);
            Set("httpProxy", value);
        }

        /// <summary>
        /// Use a FTP proxy
        /// </summary>
        /// <param name="value"></param>
        public void SetFTPProxy(string value) {
            Set(KEY_TYPE, VAL_MANUAL);
            Set("ftpProxy", value);
        }

        /// <summary>
        /// Use a SSL proxy
        /// </summary>
        /// <param name="value"></param>
        public void SetSSLProxy(string value) {
            Set(KEY_TYPE, VAL_MANUAL);
            Set("sslProxy", value);
        }

        /// <summary>
        /// Use a Sock proxy
        /// </summary>
        /// <param name="value"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SetSocksProxy(string value, string username = null, string password = null) {
            Set(KEY_TYPE, VAL_MANUAL);
            Set("socksProxy", value);
            if (username != null)
                Set("socksUsername", username);
            if (password != null)
                Set("socksPassword", password);
        }

        IEnumerator ComInterfaces._Proxy._NewEnum() {
            if (_proxy == null)
                return null;
            return _proxy.GetEnumerator();
        }

    }

}
