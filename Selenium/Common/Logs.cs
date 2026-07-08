using Selenium.Core;
using Selenium;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    //TODO : Include logs

    /// <summary>
    /// Logs object
    /// </summary>
    /// <remarks><para>
    /// It's possible to get the SeleniumBasic own log messages to be displayed or saved to a file.
    /// To achieve that please create a file in the folder where the .exe file located and name it
    /// [process name].exe.nlog</para><para>
    /// For an example, if it is executed by the 64 bit Windows Scripting host, create the following file
    /// C:\Windows\System32\cscript.exe.nlog</para><para>
    /// The <see href="https://github.com/NLog/NLog/wiki/Configuration-file">content</see>
    /// of the file could be like in the following example:</para>
    /// </remarks>
    /// <example>
    /// <code lang="XML"><![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
    ///       xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    ///     <targets>
    ///         <target name="logfile" xsi:type="File" fileName="D:\temp\selenium.log" />
    ///         <target name="logconsole" xsi:type="Console" />
    ///     </targets>
    ///     <rules>
    ///         <logger name="*" minlevel="Info" writeTo="logconsole" />
    ///         <logger name="*" minlevel="Debug" writeTo="logfile" />
    ///     </rules>
    /// </nlog>
    /// ]]></code>
    /// </example>
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
