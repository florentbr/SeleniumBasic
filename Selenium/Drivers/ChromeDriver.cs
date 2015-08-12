using Selenium.Core;
using Selenium.Internal;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Web driver for Chrome
    /// </summary>
    /// 
    /// <example>
    /// 
    /// VBScript:
    /// <code lang="vbs">	
    /// Class Script
    ///     Dim driver
    ///     
    ///     Sub Class_Initialize
    ///         Set driver = CreateObject("Selenium.ChromeDriver")
    ///         driver.Get "http://www.google.com"
    ///     End Sub
    /// 
    ///     Sub Class_Terminate
    ///         driver.Quit
    ///     End Sub
    /// End Class
    /// 
    /// Set s = New Script
    /// </code>
    /// 
    /// VBA:
    /// <code lang="vbs">	
    /// Public Sub Script()
    ///   Dim driver As New ChromeDriver
    ///   driver.Get "http://www.google.com"
    ///   ...
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// 
    /// </example>
    [ProgId("Selenium.ChromeDriver")]
    [Guid("0277FC34-FD1B-4616-BB19-5D556733E8C9")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class ChromeDriver : WebDriver, ComInterfaces._WebDriver {

        const string BROWSER_NAME = "chrome";

        public ChromeDriver()
            : base(BROWSER_NAME) { }

        internal static IDriverService StartService(WebDriver wd) {
            var svc = new DriverService(IPAddress.Loopback);
            svc.AddArgument("--port=" + svc.IPEndPoint.Port.ToString());
            svc.AddArgument("--silent");
            svc.Start("chromedriver.exe");
            return svc;
        }

        internal static Capabilities ExtendCapabilities(WebDriver wd, bool remote = false) {
            Capabilities capa = wd.Capabilities;

            Dictionary opts;
            if (!capa.TryGetValue("chromeOptions", out opts))
                capa["chromeOptions"] = opts = new Dictionary();

            capa.TryMove("debuggerAddress", opts);

            if (wd.Profile != null)
                wd.Arguments.Add("user-data-dir=" + ExpandProfile(wd.Profile, remote));

            if (wd.Arguments.Count != 0)
                opts.Add("args", wd.Arguments);

            if (wd.Extensions.Count != 0)
                opts.Add("extensions", wd.Extensions);

            if (wd.Preferences.Count != 0)
                opts.Add("prefs", wd.Preferences);

            if (wd.Binary != null)
                opts.Add("binary", wd.Binary);

            return capa;
        }

        private static string ExpandProfile(string profile, bool remote) {
            if (!remote) {
                if (IOExt.IsPath(profile)) {
                    profile = IOExt.ExpandPath(profile);
                } else {
                    profile = IOExt.AppDataFolder + @"\Google\Chrome\Profiles\" + profile;
                }
                Directory.CreateDirectory(profile);
            }
            return profile;
        }

    }

}
