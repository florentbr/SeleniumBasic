using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using ExpectedException = NUnit.Framework.ExpectedExceptionAttribute;

namespace Selenium.Tests {
    [TestFixture(Browser.Firefox, Category="Firefox")]
    [TestFixture(Browser.Gecko, Category="Gecko")]
    [TestFixture(Browser.Chrome, Category="Chrome")]
    [TestFixture(Browser.Edge, Category="Edge")]
/*
    [TestFixture(Browser.Opera, Category="Opera")]
    [TestFixture(Browser.IE, Category="IE")]
    [TestFixture(Browser.PhantomJS, Category="PhantomJS")]
*/
    class TS_Alert : BaseBrowsers {

        public TS_Alert(Browser browser)
            : base(browser) { }

        [TestCase]
        public void ShouldCloseAlert() {
            driver.Get("/input.html");
            driver.ExecuteScript("window.setTimeout(function(){alert('test close');}, 100);");
            var alert = driver.SwitchToAlert();
            A.AreEqual("test close", alert.Text);
            alert.Accept();
            A.Null(driver.SwitchToAlert(0, false));
        }

        [TestCase]
        public void ShouldDismissAlert() {
            driver.Get("/input.html");
            driver.ExecuteScript("window.setTimeout(function(){window.res=window.confirm('test accept');}, 100);");
            var alert = driver.SwitchToAlert();
            A.AreEqual("test accept", alert.Text);
            alert.Accept();
            A.True((bool)driver.ExecuteScript("return window.res;"));
            A.Null(driver.SwitchToAlert(0, false));
        }

        [TestCase]
        public void ShouldAcceptAlert() {
            driver.Get("/input.html");
            driver.ExecuteScript("window.setTimeout(function(){window.res=window.confirm('test dismiss');}, 100);");
//            driver.Wait( 2000 );
            var alert = driver.SwitchToAlert();
            A.AreEqual("test dismiss", alert.Text);
            alert.Dismiss();
            A.False((bool)driver.ExecuteScript("return window.res;"));
            A.Null(driver.SwitchToAlert(0, false));
        }

        [TestCase]
        public void ShouldSendKeysToAlert() {
            driver.Get("/input.html");
            driver.ExecuteScript("window.setTimeout(function(){window.res=window.prompt('test prompt','defaultText');}, 100);");
            var alert = driver.SwitchToAlert();
            A.AreEqual("test prompt", alert.Text);
            alert.SendKeys("abcd");
            alert.Accept();
            A.AreEqual("abcd", (string)driver.ExecuteScript("return window.res;"));
            A.Null(driver.SwitchToAlert(0, false));
        }

        [TestCase]
        [ExpectedException(typeof(Selenium.Errors.NoAlertPresentError))]
        public void ShouldThrowMissingAlert() {
            driver.Get("/input.html");
            var alert = driver.SwitchToAlert(0);
        }

        [TestCase]
        [ExpectedException(typeof(Selenium.Errors.UnexpectedAlertOpenError))]
        public void ShouldThrowUnexpectedAlert() {
            driver.Get("/input.html");
            driver.ExecuteScript("window.setTimeout(function(){window.alert('na');}, 0);");
            driver.Wait( 1000 );
            var ele = driver.FindElementById("input__search");
            ele.Clear();
        }
    }

}
