using Selenium.Core;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Defines the interface through which the user can manipulate JavaScript alerts.
    /// </summary>
    /// <example>
    /// <code lang="vbs">	
    /// Set dlg = driver.SwitchToAlert()
    /// txt = dlg.Text
    /// dlg.Accept
    /// </code>
    /// </example>
    [ProgId("Selenium.Alert")]
    [Guid("0277FC34-FD1B-4616-BB19-2845E23A4514")]
    [Description("Defines the interface through which the user can manipulate JavaScript alerts.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Alert : ComInterfaces._Alert {

        #region Static methods

        internal static Alert SwitchToAlert(RemoteSession session, int timeout) {
            string text;
            try {
                text = (string)session.Send(RequestMethod.GET, "/alert_text");
            } catch (Errors.NoAlertPresentError) {
                if (timeout == 0)
                    throw;
                DateTime endTime = session.GetEndTime(timeout);
                while (true) {
                    SysWaiter.Wait();
                    try {
                        text = (string)session.SendAgain();
                        break;
                    } catch (Errors.NoAlertPresentError) {
                        if (DateTime.UtcNow > endTime)
                            throw;
                    }
                }
            }
            return new Alert(session, text);
        }

        #endregion


        private readonly RemoteSession _session;
        private readonly string _text;

        internal Alert(RemoteSession session, string text) {
            _session = session;
            _text = text;
        }

        /// <summary>
        /// Text displayed on the alert.
        /// </summary>
        public string Text {
            get { return _text; }
        }

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        public void Dismiss() {
            _session.Send(RequestMethod.POST, "/dismiss_alert");
        }

        /// <summary>
        /// Accepts the alert.
        /// </summary>
        public void Accept() {
            _session.Send(RequestMethod.POST, "/accept_alert");
        }

        /// <summary>
        /// Sends keys to the alert.
        /// </summary>
        /// <param name="keysToSend"></param>
        public void SendKeys(string keysToSend) {
            _session.Send(RequestMethod.POST, "/alert_text", "text", keysToSend);
        }

        /// <summary>
        /// Sets the user name and password in an alert prompting for credentials.
        /// </summary>
        /// <param name="user">User name</param>
        /// <param name="password">Password</param>
        public void SetCredentials(string user, string password){
            _session.Send(RequestMethod.POST, "/alert/credentials", "username", user, "password", password);
        }

    }

}
