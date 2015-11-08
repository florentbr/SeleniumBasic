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
        const string KEY_NATIVE_EVENTS = "nativeEvents";
        const string KEY_ACCEPT_SSL_CERTIFICATES = "acceptSslCerts";
        const string KEY_ALERT_BEHAVIOUR = "unexpectedAlertBehaviour";
        const string KEY_PAGE_LOAD_STRATEGY = "pageLoadStrategy";

        public Capabilities(){
            base.Add(KEY_PAGE_LOAD_STRATEGY, "normal");
            base.Add(KEY_ALERT_BEHAVIOUR, "ignore");
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
                return base.GetValue(KEY_PLATFORM, "ANY");
            }
            set {
                base[KEY_PLATFORM] = string.IsNullOrEmpty(value) ? "ANY" : value;
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
                return base.GetValue(KEY_ACCEPT_SSL_CERTIFICATES, false);
            }
            set {
                base[KEY_ACCEPT_SSL_CERTIFICATES] = value;
            }
        }

        /// <summary>
        /// Unexpected alert behaviour
        /// </summary>
        public string UnexpectedAlertBehaviour {
            get {
                return base.GetValue(KEY_ALERT_BEHAVIOUR, string.Empty);
            }
            set {
                base[KEY_ALERT_BEHAVIOUR] = value;
            }
        }

    }

}
