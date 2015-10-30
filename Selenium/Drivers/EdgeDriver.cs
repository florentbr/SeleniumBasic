using Microsoft.Win32;
using Selenium.Core;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium {

    /// <summary>
    /// Web driver for Microsoft Edge driver
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
    ///         Set driver = CreateObject("Selenium.EdgeDriver")
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
            if (!capa.TryGetValue("edgeOptions", out opts))
                capa["edgeOptions"] = opts = new Dictionary();

        }

    }

}
