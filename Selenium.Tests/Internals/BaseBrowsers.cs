using NUnit.Framework;
using System;
using System.Diagnostics;
using OneTimeSetUp=NUnit.Framework.TestFixtureSetUpAttribute;
using OneTimeTearDown=NUnit.Framework.TestFixtureTearDownAttribute;

namespace Selenium.Tests.Internals
{
    public enum Browser {
        unknown, Firefox, Chrome, Opera, IE, PhantomJS, Gecko, Edge
    }

    abstract class BaseBrowsers {
        public static bool remote_driver = false;
        public readonly object FixtureParam;
        private Process driver_process;
        protected WebDriver driver;
        protected Keys Keys = new Keys();
        protected By By = new By();

        public BaseBrowsers() : this(GetBrowserFromCategory()) {
        }

        public BaseBrowsers(Browser browser) {
            WebServer.StartServer(@"..\..\Pages");
            if( remote_driver ) {
                this.driver = GetRemotelyStartedDriver(browser);
            } else {
                this.driver = GetDriverInstance(browser);
            }
            this.driver.BaseUrl = WebServer.BaseUri;
            this.FixtureParam = browser;
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp() {
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown() {
            driver.Quit();
            if( driver_process != null && !driver_process.HasExited ) {
                driver_process.Kill();
            }
        }

        private static Browser GetBrowserFromCategory() {
            if( NUnit.Core.TestExecutionContext.CurrentContext.CurrentTest.Categories.Contains("Edge") )
                return Browser.Edge;
            return Browser.unknown;
        }

        private string GetBrowserTypeLib(Browser browser) {
            switch (browser) {
                case Browser.Gecko:     return "Selenium.GeckoDriver";
                case Browser.Firefox:   return "Selenium.FirefoxDriver";
                case Browser.Edge:      return "Selenium.EdgeDriver";
                case Browser.Chrome:    return "Selenium.ChromeDriver";
                case Browser.Opera:     return "Selenium.OperaDriver";
                case Browser.IE:        return "Selenium.IEDriver";
                case Browser.PhantomJS: return "Selenium.PhantomJSDriver";
            }
            throw new Exception("Browser not supported: " + browser.ToString());
        }

        private WebDriver GetDriverInstance(Browser browser) {
            switch (browser) {
                case Browser.Gecko: return new Selenium.GeckoDriver();
                case Browser.Firefox: 
                    Selenium.FirefoxDriver ffd = new Selenium.FirefoxDriver();
                    ffd.SetBinary( @"C:\Utils\Firefox46\firefox.exe" );
                    return ffd;
                case Browser.Edge:      return new Selenium.EdgeDriver();
                case Browser.Chrome:    return new Selenium.ChromeDriver();
                case Browser.Opera:     return new Selenium.OperaDriver();
                case Browser.IE:        return new Selenium.IEDriver();
                case Browser.PhantomJS: return new Selenium.PhantomJSDriver();
            }
            throw new Exception("Browser not supported: " + browser.ToString());
        }

        private WebDriver GetRemotelyStartedDriver(Browser browser) {
            ProcessStartInfo p_info = new ProcessStartInfo();
            p_info.UseShellExecute = true;
            p_info.CreateNoWindow = false;
            p_info.WindowStyle = ProcessWindowStyle.Normal;
            switch (browser) {
                case Browser.Gecko: 
                    p_info.FileName = "geckodriver.exe";
                    p_info.Arguments = "-vv";
                    break;
                case Browser.Chrome:
                    p_info.FileName = "chromedriver.exe";
                    p_info.Arguments = "--port=4444 --verbose";
                    break;
                case Browser.Edge:
                    p_info.FileName = "edgedriver.exe";
                    p_info.Arguments = "--port=4444 --verbose";
                    break;
            default: throw new Exception("Browser not supported: " + browser.ToString());
            }
            try {
            driver_process = Process.Start(p_info); 
            NUnit.Framework.Assert.False( driver_process.HasExited, "Driver process cannot start: " + browser.ToString() );
            WebDriver s = new Selenium.WebDriver();
            s.StartRemotely( "http://localhost:4444/", browser.ToString().ToLower() );
            return s;
            } catch( Exception e ) {
                NUnit.Framework.Assert.Fail( "Thrown exception: " + e.Message );
            }
            return null;
        }
    }
}
