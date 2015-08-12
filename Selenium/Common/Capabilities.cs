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
        /// Browser name
        /// </summary>
        public string BrowserName {
            get {
                return (string)base.Get(KEY_BROWSER_NAME, null);
            }
            set {
                base[KEY_BROWSER_NAME] = value;
            }
        }

        /// <summary>
        /// Browser version
        /// </summary>
        public string BrowserVersion {
            get {
                return (string)base.Get(KEY_BROWSER_VERSION, null);
            }
            set {
                base[KEY_BROWSER_VERSION] = value;
            }
        }

        /// <summary>
        /// Platform name
        /// </summary>
        public string PlatformName {
            get {
                return (string)base.Get(KEY_PLATFORM_NAME, "ANY");
            }
            set {
                base[KEY_PLATFORM_NAME] = value ?? "ANY";
            }
        }

        /// <summary>
        /// Platform version
        /// </summary>
        public string PlatformVersion {
            get {
                return (string)base.Get(KEY_PLATFORM_VERSION, null);
            }
            set {
                base[KEY_PLATFORM_VERSION] = value;
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
        /// Accept untrusted certificates
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
        /// Unexpected alert behaviour
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
