using Microsoft.Win32;
using Selenium.Core;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium {

    /// <summary>
    /// Web driver for Gecko driver
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
    ///         Set driver = CreateObject("Selenium.GeckoDriver")
    ///         driver.Get "http://www.google.com"
    ///         ...
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
    ///   Dim driver As New GeckoDriver
    ///   driver.Get "http://www.mywebsite.com"
    ///   ...
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// 
    /// </example>
    [ProgId("Selenium.GeckoDriver")]
    [Guid("0277FC34-FD1B-4616-BB19-19E724A03AE2")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class GeckoDriver : WebDriver, ComInterfaces._WebDriver {

        const string BROWSER_NAME = "Gecko";

        /// <summary>
        /// 
        /// </summary>
        public GeckoDriver()
            : base(BROWSER_NAME) { 
            WebDriver.LEGACY = false;   // assuming only one driver is used at a time
        }

        internal static IDriverService StartService(WebDriver wd) {
            ExtendCapabilities(wd, false);

            var svc = new DriverService();
            svc.AddArgument("--host=127.0.0.1");
            svc.AddArgument("--port=" + svc.IPEndPoint.Port.ToString());
            svc.Start("geckodriver.exe", true);
            return svc;
        }

        internal static void ExtendCapabilities(WebDriver wd, bool remote) {
            WebDriver.LEGACY = false;
            Capabilities capa = wd.Capabilities;

            Dictionary opts;
            if (!capa.TryGetValue("GeckoOptions", out opts))
                capa["GeckoOptions"] = opts = new Dictionary();

        }

    }

}
