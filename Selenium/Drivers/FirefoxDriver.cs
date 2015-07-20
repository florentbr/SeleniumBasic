using Selenium.Core;
using System;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Web driver for Firefox
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
    ///         Set driver = CreateObject("Selenium.FirefoxDriver")
    ///         driver.Get "http://www.google.com"
    ///     End Sub
    /// 
    ///     Sub Class_Terminate
    ///         driver.Quit
    ///         ...
    ///     End Sub
    /// End Class
    /// 
    /// Set s = New Script
    /// </code>
    /// 
    /// VBA:
    /// <code lang="vbs">	
    /// Public Sub Script()
    ///   Dim driver As New FirefoxDriver
    ///   driver.Get "http://www.mywebsite.com"
    ///   ...
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// 
    /// </example>
    [ProgId("Selenium.FirefoxDriver")]
    [Guid("0277FC34-FD1B-4616-BB19-14DB1E4916D4")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class FirefoxDriver : WebDriver, ComInterfaces._WebDriver {

        internal override IDriverService StartService() {
            return FirefoxDriver.StartService(this);
        }

        internal override Capabilities ExtendCapabilities() {
            return FirefoxDriver.ExtendCapabilities(this);
        }

        internal static IDriverService StartService(WebDriver wd) {
            FirefoxService svc = new FirefoxService();
            svc.Start(wd.Arguments, wd.Preferences, wd.Extensions, wd.Capabilities, wd.Profile, wd.Persistant);
            return svc;
        }

        internal static Capabilities ExtendCapabilities(WebDriver wd, bool remote = false) {
            var capa = wd.Capabilities;
            capa.Browser = "firefox";
            capa["webdriver.logging.profiler.enabled"] = false;
            capa["loggingPrefs"] = new Dictionary {
                {"profiler", "OFF"},
                {"driver", "OFF"},
                {"browser", "OFF"}
            };
            return capa;
        }

    }

}
