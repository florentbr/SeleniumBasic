using System;
using Selenium.Core;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Timeouts used in in waiting
    /// </summary>
    /// <example>
    /// Sets the deault operational time out to 1 second:
    /// <code lang="vb">
    /// driver.Timeouts.ImplicitWait = 1000
    /// </code>
    /// Tells the driver process to set its element location timeout to 1 second:
    /// <code lang="vb">
    /// driver.Timeouts.Implicit = 1000
    /// </code>
    /// </example>
    [ProgId("Selenium.Timeouts")]
    [Guid("0277FC34-FD1B-4616-BB19-44A424DB3F50")]
    [Description("Timeouts management")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Timeouts : ComInterfaces._Timeouts {
        private const String ENDPOINT = "/timeouts";

        internal static void SendTimeoutScript(RemoteSession session, int timeout) {
            session.Send(RequestMethod.POST, ENDPOINT, "type", "script", "ms", timeout);
        }

        internal static void SendTimeoutPageLoad(RemoteSession session, int timeout) {
            session.Send(RequestMethod.POST, ENDPOINT, "type", "page load", "ms", timeout);
        }

        internal static void SendTimeoutImplicit(RemoteSession session, int timeout) {
            session.Send(RequestMethod.POST, ENDPOINT, "type", "implicit", "ms", timeout);
        }


        private RemoteSession _session;

        internal int timeout_server = 90000;       // 90 seconds
        internal int timeout_pageload = 60000;     // 60 seconds
        internal int timeout_implicitwait = 3000;  // 3  seconds
        internal int timeout_script = 15000;       // 15 seconds

        /// <summary>
        /// Amount of time that Selenium will wait for commands to complete. Default is 3000ms
        /// </summary>
        /// <remarks>
        /// Note: 
        /// This timeout is not related to the driver process'
        /// <see href="https://w3c.github.io/webdriver/#timeouts">implicit timeout</see>.
        /// It just a SeleniumBasic's default time for an element search or wait operation would continue repeating a requests
        /// until the timeout is reached.
        /// </remarks>
        /// <remarks>Default is 3000ms</remarks>
        /// <seealso cref="Timeouts.Implicit"/>
        public int ImplicitWait {
            get {
                return timeout_implicitwait;
            }
            set {
                if (value == timeout_implicitwait) return;
                timeout_implicitwait = value;
            }
        }

        /// <summary>
        /// Amount of time the driver process should wait to complete when locating an element.
        /// </summary>
        /// <remarks>
        /// Default is 0ms. See <see href="https://w3c.github.io/webdriver/#timeouts">implicit timeout</see>
        /// </remarks>
        public int Implicit {
            get {
                try {
                    Dictionary dict = (Dictionary)_session.Send(RequestMethod.GET, ENDPOINT);
                    if( dict != null )
                        return Convert.ToInt32(dict["implicit"]);
                } catch { }
                return -1;
            }
            set {
                if (_session != null)
                    SendTimeoutImplicit(_session, value);
            }
        }

        /// <summary>
        /// Amount of time the driver should wait while loading a page before throwing an exception.
        /// </summary>
        /// <remarks>Default is 60000ms</remarks>
        public int PageLoad {
            get {
                Dictionary dict = (Dictionary)_session.Send(RequestMethod.GET, ENDPOINT);
                if( dict != null ) {
                    timeout_pageload = Convert.ToInt32(dict["pageLoad"]);
                }
                return timeout_pageload;
            }
            set {
                if (value == timeout_pageload) return;
                if (_session != null)
                    SendTimeoutPageLoad(_session, value);
                timeout_pageload = value;
            }
        }

        /// <summary>
        /// Amount of time the driver should wait while executing an asynchronous script before throwing an error.
        /// </summary>
        /// <remarks>Default is 15000ms</remarks>
        public int Script {
            get {
                Dictionary dict = (Dictionary)_session.Send(RequestMethod.GET, ENDPOINT);
                if( dict != null ) {
                    timeout_script = Convert.ToInt32(dict["script"]);
                }
                return timeout_script;
            }
            set {
                if (value == timeout_script) return;
                if (_session != null)
                    SendTimeoutScript(_session, value);
                timeout_script = value;
            }
        }

        /// <summary>
        /// Maximum amount of time the driver should wait while waiting for a response from the server. 
        /// </summary>
        /// <remarks>Default is 90000ms</remarks>
        public int Server {
            get {
                return timeout_server;
            }
            set {
                if (value == timeout_server) return;
                timeout_server = value;
            }
        }

        internal void SetSession(RemoteSession session) {
            _session = session;
            SendTimeoutPageLoad(_session, timeout_pageload);
            SendTimeoutScript(_session, timeout_script);
        }

    }

}
