using NUnit.Framework;
using System;
using System.Diagnostics;

namespace Selenium.Tests.Internals {

    public enum Browser {
        Firefox, Chrome, Opera, IE, PhantomJS, Gecko, Edge
    }

    abstract class BaseBrowsers {
        public static bool remote_driver = false;
        public readonly object Fixture;
        private Process driver_process;
        protected WebDriver driver;
        protected Keys Keys = new Keys();
        protected By By = new By();

        public BaseBrowsers(Browser browser) {
            WebServer.StartServer(@"..\..\Pages");
            if( remote_driver ) {
                this.driver = GetBrowserWithDriverStartedRemotely(browser);
            } else {
                this.driver = GetBrowserInstance(browser);
            }
            this.driver.BaseUrl = WebServer.BaseUri;
            this.Fixture = browser;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown() {
            driver.Quit();
            if( driver_process != null && !driver_process.HasExited ) {
                driver_process.Kill();
            }
        }

        private string GetBrowserTypeLib(Browser browser) {
            switch (browser) {
                case Browser.Gecko: return "Selenium.GeckoDriver";
                case Browser.Firefox: return "Selenium.FirefoxDriver";
                case Browser.Edge: return "Selenium.EdgeDriver";
                case Browser.Chrome: return "Selenium.ChromeDriver";
                case Browser.Opera: return "Selenium.OperaDriver";
                case Browser.IE: return "Selenium.IEDriver";
                case Browser.PhantomJS: return "Selenium.PhantomJSDriver";
            }
            throw new Exception("Browser not supported: " + browser.ToString());
        }

        private WebDriver GetBrowserInstance(Browser browser) {
            switch (browser) {
                case Browser.Gecko: return new Selenium.GeckoDriver();
                case Browser.Firefox: 
                    Selenium.FirefoxDriver ffd = new Selenium.FirefoxDriver();
                    ffd.SetBinary( @"C:\Utils\Firefox46\firefox.exe" );
                    return ffd;
                case Browser.Edge: return new Selenium.EdgeDriver();
                case Browser.Chrome: return new Selenium.ChromeDriver();
                case Browser.Opera: return new Selenium.OperaDriver();
                case Browser.IE: return new Selenium.IEDriver();
                case Browser.PhantomJS: return new Selenium.PhantomJSDriver();
            }
            throw new Exception("Browser not supported: " + browser.ToString());
        }

        private WebDriver GetBrowserWithDriverStartedRemotely(Browser browser) {
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
            default: throw new Exception("Browser not supported: " + browser.ToString());
            }
            driver_process = Process.Start(p_info); 
            NUnit.Framework.Assert.False( driver_process.HasExited, "Driver process cannot start: " + browser.ToString() );
            WebDriver s = new Selenium.WebDriver();
            s.StartRemotely( "http://localhost:4444/", browser.ToString().ToLower() );
            return s;
        }
    }
}
