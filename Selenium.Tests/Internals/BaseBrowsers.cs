using NUnit.Framework;
using System;

namespace Selenium.Tests.Internals {

    public enum Browser {
        Firefox, Chrome, Opera, IE, PhantomJS
    }

    abstract class BaseBrowsers {

        public readonly object Fixture;

        protected WebDriver driver;
        protected Keys Keys = new Keys();
        protected By By = new By();

        public BaseBrowsers(Browser browser) {
            WebServer.StartServer(@"..\..\Pages");
            this.driver = GetBrowserInstance(browser);
            this.driver.BaseUrl = WebServer.BaseUri;
            this.Fixture = browser;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown() {
            driver.Quit();
        }

        private string GetBrowserTypeLib(Browser browser) {
            switch (browser) {
                case Browser.Firefox: return "Selenium.FirefoxDriver";
                case Browser.Chrome: return "Selenium.ChromeDriver";
                case Browser.Opera: return "Selenium.OperaDriver";
                case Browser.IE: return "Selenium.IEDriver";
                case Browser.PhantomJS: return "Selenium.PhantomJSDriver";
            }
            throw new Exception("Browser not supported: " + browser.ToString());
        }

        private WebDriver GetBrowserInstance(Browser browser) {
            switch (browser) {
                case Browser.Firefox: return new Selenium.FirefoxDriver();
                case Browser.Chrome: return new Selenium.ChromeDriver();
                case Browser.Opera: return new Selenium.OperaDriver();
                case Browser.IE: return new Selenium.IEDriver();
                case Browser.PhantomJS: return new Selenium.PhantomJSDriver();
            }
            throw new Exception("Browser not supported: " + browser.ToString());
        }

    }

}
