using Selenium;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Driver capabilities
    /// </summary>
    public class Capabilities : Dictionary, ComInterfaces._Dictionary {

        const string KEY_BROWSER_NAME = "browserName";
        const string KEY_BROWSER_VERSION = "browserVersion";
        const string KEY_PLATFORM = "platform";
        const string KEY_PLATFORM_NAME = "platformName";
        const string KEY_NATIVE_EVENTS = "nativeEvents";
        const string KEY_ACCEPT_SSL_CERTS = "acceptSslCerts";
        const string KEY_ACCEPT_INSECURE_CERTS = "acceptInsecureCerts";
        const string KEY_ALERT_BEHAVIOUR = "unexpectedAlertBehaviour";
        const string KEY_PAGE_LOAD_STRATEGY = "pageLoadStrategy";

        public Capabilities(){
            base.Add(KEY_PAGE_LOAD_STRATEGY, "normal");
        }

        public new object this[string key] {
            get {
                return base[key];
            }
            set {
                var range = value as Interop.Excel.IRange;
                if (range != null) {
                    base.Set(key, range.Text);
                } else {
                    base.Set(key, value);
                }
            }
        }

        /// <summary>
        /// Browser name
        /// </summary>
        public string BrowserName {
            get {
                return base.GetValue(KEY_BROWSER_NAME, string.Empty);
            }
            set {
                base[KEY_BROWSER_NAME] = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
        }

        /// <summary>
        /// Browser version
        /// </summary>
        public string BrowserVersion {
            get {
                return base.GetValue(KEY_BROWSER_VERSION, string.Empty);
            }
            set {
                base[KEY_BROWSER_VERSION] = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
        }

        /// <summary>
        /// Platform name
        /// </summary>
        public string Platform {
            get {
                return base.GetValue(WebDriver.LEGACY ? KEY_PLATFORM : KEY_PLATFORM_NAME, "ANY");
            }
            set {
                if( WebDriver.LEGACY )
                    base[KEY_PLATFORM] = string.IsNullOrEmpty(value) ? "ANY" : value;
                else
                    base[KEY_PLATFORM_NAME] = string.IsNullOrEmpty(value) ? "windows" : value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether native events are enabled (Firefox only).
        /// </summary>
        public bool EnableNativeEvents {
            get {
                return base.GetValue(KEY_NATIVE_EVENTS, false);
            }
            set {
                base[KEY_NATIVE_EVENTS] = value;
            }
        }

        /// <summary>
        /// Accept untrusted certificates
        /// </summary>
        public bool AcceptUntrustedCertificates {
            get {
                return base.GetValue(WebDriver.LEGACY ? KEY_ACCEPT_SSL_CERTS : KEY_ACCEPT_INSECURE_CERTS, false);
            }
            set {
                base[WebDriver.LEGACY ? KEY_ACCEPT_SSL_CERTS : KEY_ACCEPT_INSECURE_CERTS] = value;
            }
        }

        /// <summary>
        /// Unexpected alert behaviour. Legacy only
        /// </summary>
        public string UnexpectedAlertBehaviour {
            get {
                return base.GetValue(KEY_ALERT_BEHAVIOUR, string.Empty);
            }
            set {
                if( WebDriver.LEGACY )
                    base[KEY_ALERT_BEHAVIOUR] = value;
            }
        }

    }

}
