using Microsoft.Win32;
using Selenium.Core;
using Selenium.Internal;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium {

    /// <summary>
    /// Web driver client for Microsoft Edge driver process
    /// </summary>
    /// <remarks>
    /// Note, the driver process image name is expected to be edgedriver.exe , not msedgedriver.exe
    /// </remarks>
    /// 
    /// <example>
    /// <code lang="vbs">	
    ///         Dim driver
    ///         Set driver = CreateObject("Selenium.EdgeDriver")
    ///         driver.Get "http://www.google.com"
    ///         ...
    ///         driver.Quit
    /// </code>
    /// <code lang="VB">	
    /// Public Sub Script()
    ///   Dim driver As New EdgeDriver
    ///   driver.Get "http://www.mywebsite.com"
    ///   ...
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// 
    /// </example>
    [ProgId("Selenium.EdgeDriver")]
    [Guid("0277FC34-FD1B-4616-BB19-3C406728F1A2")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class EdgeDriver : WebDriver, ComInterfaces._WebDriver {

        const string BROWSER_NAME = "MicrosoftEdge";

        /// <summary>
        /// 
        /// </summary>
        public EdgeDriver()
            : base(BROWSER_NAME) { }

        internal static IDriverService StartService(WebDriver wd) {
            ExtendCapabilities(wd, false);

            var svc = new DriverService();
            svc.AddArgument("--host=localhost");
            svc.AddArgument("--port=" + svc.IPEndPoint.Port.ToString());
            svc.Start("edgedriver.exe", true);
            return svc;
        }

        internal static void ExtendCapabilities(WebDriver wd, bool remote) {
            Capabilities capa = wd.Capabilities;

            Dictionary opts;
            if (!capa.TryGetValue("ms:edgeOptions", out opts))
                capa["ms:edgeOptions"] = opts = new Dictionary();

            if (wd.Profile != null)
                wd.Arguments.Add("--user-data-dir=" + ExpandProfile(wd.Profile, remote));

            if (wd.Arguments.Count != 0)
                opts["args"] = wd.Arguments;

        }

        private static string ExpandProfile(string profile, bool remote) {
            if (!remote) {
                if (IOExt.IsPath(profile)) {
                    profile = IOExt.ExpandPath(profile);
                } else {
                    profile = IOExt.AppDataFolder + @"\Microsoft\Edge\Profiles\" + profile;
                }
                Directory.CreateDirectory(profile);
            }
            return profile;
        }
    }

}
