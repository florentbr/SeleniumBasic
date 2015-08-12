using Selenium.Core;
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Web driver for PhantomJS (Headless browser)
    /// </summary>
    /// <example>
    /// 
    /// VBScript:
    /// <code lang="vbs">	
    /// Class Script
    ///     Dim driver
    ///     
    ///     Sub Class_Initialize
    ///         Set driver = CreateObject("Selenium.PhantomJSDriver")
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
    ///   Dim driver As New PhantomJSDriver
    ///   driver.Get "http://www.google.com"
    ///   ...
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// 
    /// </example>
    [ProgId("Selenium.PhantomJSDriver")]
    [Guid("0277FC34-FD1B-4616-BB19-0809389E78C4")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class PhantomJSDriver : WebDriver, ComInterfaces._WebDriver {

        const string BROWSER_NAME = "phantomjs";

        public PhantomJSDriver()
            : base(BROWSER_NAME) { }

        internal static IDriverService StartService(WebDriver wd) {
            var svc = new DriverService(IPAddress.Loopback);
            svc.AddArgument("--webdriver=" + svc.IPEndPoint.ToString());
            svc.AddArgument("--webdriver-loglevel=ERROR");
            svc.AddArgument("--ignore-ssl-errors=true");
            svc.Start("phantomjs.exe");
            return svc;
        }

        internal static Capabilities ExtendCapabilities(WebDriver wd, bool remote = false) {
            var capa = wd.Capabilities;
            capa.BrowserName = "phantomjs";
            if (wd.Arguments.Count > 0)
                capa["phantomjs.cli.args"] = wd.Arguments;
            return capa;
        }

    }

}
