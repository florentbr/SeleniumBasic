using Selenium.Core;
using Selenium;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    //TODO : Include logs

    /// <summary>
    /// Logs object
    /// </summary>
    [ProgId("Selenium.Logs")]
    [Guid("0277FC34-FD1B-4616-BB19-E9C0E4941F8F")]
    [Description("Logs object")]
    [ComVisible(false), ClassInterface(ClassInterfaceType.None)]
    public class Logs : ComInterfaces._Logs {

        const string BASE_URI = "/log";

        private RemoteSession _session;

        internal Logs(RemoteSession session) {
            _session = session;
        }

        /// <summary>
        /// Get the log entries for the server
        /// </summary>
        /// <returns>The list of log entries.</returns>
        public List Server {
            get {
                return (List)_session.Send(RequestMethod.POST, BASE_URI, "type", "server");
            }
        }

        /// <summary>
        /// Get the log entries for the browser
        /// </summary>
        /// <returns>The list of log entries.</returns>
        public List Browser {
            get {
                return (List)_session.Send(RequestMethod.POST, BASE_URI, "type", "browser");
            }
        }

        /// <summary>
        /// Get the log entries for the client
        /// </summary>
        /// <returns>The list of log entries.</returns>
        public List Client {
            get {
                return (List)_session.Send(RequestMethod.POST, BASE_URI, "type", "client");
            }
        }

        /// <summary>
        /// Get the log entries for the driver
        /// </summary>
        /// <returns>The list of log entries.</returns>
        public List Driver {
            get {
                return (List)_session.Send(RequestMethod.POST, BASE_URI, "type", "driver");
            }
        }

        /// <summary>
        /// Get available log types.
        /// </summary>
        /// <returns>The list of available log types.</returns>
        public List Types {
            get {
                return (List)_session.Send(RequestMethod.GET, BASE_URI + "/types");
            }
        }

    }
}
