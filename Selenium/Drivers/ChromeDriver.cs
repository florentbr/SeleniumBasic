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

        internal override IDriverService StartService() {
            return ChromeDriver.StartService(this);
        }

        internal override Capabilities ExtendCapabilities() {
            return ChromeDriver.ExtendCapabilities(this);
        }

        internal static IDriverService StartService(WebDriver wd) {
            var svc = new DriverService(IPAddress.Loopback);
            svc.AddArgument("--port=" + svc.EndPoint.Port.ToString());
            svc.AddArgument("--silent");
            svc.Start("chromedriver.exe");
            return svc;
        }

        internal static Capabilities ExtendCapabilities(WebDriver wd, bool remote = false) {
            var capa = wd.Capabilities;
            capa.Browser = "chrome";

            var opts = new Dictionary();

            if (wd.Profile != null)
                wd.Arguments.Add("user-data-dir=" + ExpandProfile(wd.Profile, remote));

            if (wd.Arguments.Count != 0)
                opts.Add("args", wd.Arguments);

            if (wd.Extensions.Count != 0)
                opts.Add("extensions", wd.Extensions);

            if (wd.Preferences.Count != 0)
                opts.Add("prefs", wd.Preferences);

            capa["chromeOptions"] = opts;

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
