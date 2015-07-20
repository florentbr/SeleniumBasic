using Selenium.Core;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Timeouts object
    /// </summary>
    /// <example>
    /// Sets the implicit timout to 1 second
    /// <code lang="vbs">	
    /// driver.Timeouts.ImplicitWait = 1000
    /// </code>
    /// </example>
    [ProgId("Selenium.Timeouts")]
    [Guid("0277FC34-FD1B-4616-BB19-44A424DB3F50")]
    [Description("Timeouts management")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Timeouts : ComInterfaces._Timeouts {

        const string BASE_URI = "/timeouts";

        private readonly RemoteSession _session;

        internal int timeout_implicitwait = 3000;
        internal int timeout_pageload = -1;
        internal int timeout_script = -1;

        internal Timeouts(RemoteSession session) {
            _session = session;
        }

        /// <summary>
        /// Amount of time that Selenium will wait for waiting commands to complete
        /// </summary>
        public int ImplicitWait {
            get {
                return timeout_implicitwait;
            }
            set {
                //_session.Send(RequestMethod.POST, BASE_URI, "type", "implicit", "ms", ms);
                timeout_implicitwait = value;
            }
        }

        /// <summary>
        /// Amount of time the driver should wait while loading a page before throwing an exception.
        /// </summary>
        public int PageLoad {
            get {
                return timeout_pageload;
            }
            set {
                _session.Send(RequestMethod.POST, BASE_URI, "type", "page load", "ms", value);
                timeout_pageload = value;
            }
        }

        /// <summary>
        /// Amount of time the driver should wait while executing a script before throwing an exception.
        /// </summary>
        public int Script {
            get {
                return timeout_pageload;
            }
            set {
                _session.Send(RequestMethod.POST, BASE_URI, "type", "script", "ms", value);
                timeout_pageload = value;
            }
        }

        /// <summary>
        /// Maximum amount of time the driver should wait while waiting for a response from the server.
        /// </summary>
        public int Server {
            get {
                return _session.server.Timeout;
            }
            set {
                _session.server.Timeout = value;
            }
        }

    }

}
