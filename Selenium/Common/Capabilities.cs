
using Selenium;
namespace Selenium {

    /// <summary>
    /// Driver capabilities
    /// </summary>
    public class Capabilities : Dictionary, ComInterfaces._Dictionary {

        const string KEY_BROWSER_NAME = "browserName";
        const string KEY_BROWSER_VERSION = "browserVersion";
        const string KEY_PLATFORM_NAME = "platformName";
        const string KEY_PLATFORM_VERSION = "platformVersion";
        const string KEY_NATIVE_EVENTS = "nativeEvents";
        const string KEY_ACCEPT_SSL_CERTIFICATES = "acceptSslCerts";
        const string KEY_ALERT_BEHAVIOUR = "unexpectedAlertBehaviour";

        /// <summary>
        /// 
        /// </summary>
        public Capabilities() {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browserName"></param>
        /// <param name="browserVersion"></param>
        /// <param name="platformName"></param>
        /// <param name="platformVersion"></param>
        internal Capabilities(string browserName, string browserVersion = null, string platformName = null, string platformVersion = null) {
            Add(KEY_BROWSER_NAME, browserName);
            Add(KEY_BROWSER_VERSION, browserVersion ?? null);
            Add(KEY_PLATFORM_NAME, platformName ?? "ANY");
            Add(KEY_PLATFORM_VERSION, platformVersion ?? null);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Browser {
            get {
                return (string)base.Get(KEY_BROWSER_NAME, null);
            }
            set {
                base[KEY_BROWSER_NAME] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether native events are enabled (Firefox only).
        /// </summary>
        public bool EnableNativeEvents {
            get {
                return (bool)base.Get(KEY_NATIVE_EVENTS, false);
            }
            set {
                base[KEY_NATIVE_EVENTS] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AcceptUntrustedCertificates {
            get {
                return (bool)base.Get(KEY_ACCEPT_SSL_CERTIFICATES, false);
            }
            set {
                base[KEY_ACCEPT_SSL_CERTIFICATES] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UnexpectedAlertBehaviour {
            get {
                return (string)base.Get(KEY_ALERT_BEHAVIOUR, false);
            }
            set {
                base[KEY_ALERT_BEHAVIOUR] = value;
            }
        }

    }

}
