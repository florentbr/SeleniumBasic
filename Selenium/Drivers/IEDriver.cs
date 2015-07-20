using Selenium.Core;
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Web driver for Internet Explorer
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
    ///         Set driver = CreateObject("Selenium.IEDriver")
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
    ///   Dim driver As New IEDriver
    ///   driver.Get "http://www.mywebsite.com"
    ///   ...
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// 
    /// </example>
    [ProgId("Selenium.IEDriver")]
    [Guid("0277FC34-FD1B-4616-BB19-EED04A1E4CD1")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class IEDriver : WebDriver, ComInterfaces._WebDriver {

        internal override IDriverService StartService() {
            return IEDriver.StartService(this);
        }

        internal override Capabilities ExtendCapabilities() {
            return IEDriver.ExtendCapabilities(this);
        }

        internal static IDriverService StartService(WebDriver wd) {
            var svc = new DriverService(IPAddress.Loopback);
            svc.AddArgument("/host=" + svc.EndPoint.Address.ToString());
            svc.AddArgument("/port=" + svc.EndPoint.Port.ToString());
            svc.AddArgument("/log-level=ERROR");
            svc.AddArgument("/silent");
            svc.Start("iedriver.exe", true);
            return svc;
        }

        internal static Capabilities ExtendCapabilities(WebDriver wd, bool remote = false) {
            var capa = wd.Capabilities;
            capa.Browser = "internet explorer";
            capa["silent"] = true;
            capa["nativeEvents"] = true;
            //capa["requireWindowFocus"] = true;
            if (wd.Arguments.Count > 0)
                capa["ie.browserCommandLineSwitches"] = wd.Arguments;
            return capa;
        }

    }

}
