using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using SetUp = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

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
    class TS_Frame : BaseBrowsers {

        public TS_Frame(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/frame1.html");
        }

        [TestCase]
        public void ShouldSwitchToFrameByIdName() {
            A.AreEqual("Frame1", driver.FindElementById("info").Text());

            driver.SwitchToFrame("frm2"); //by name
            A.AreEqual("Frame2", driver.FindElementById("info").Text());

            driver.SwitchToFrame("frm3"); //by id
            A.AreEqual("Frame3", driver.FindElementById("info").Text());

            driver.SwitchToDefaultContent();
            A.AreEqual("Frame1", driver.FindElementById("info").Text());
        }

        [TestCase]
        public void ShouldSwitchToFrameByIndex() {
            A.AreEqual("Frame1", driver.FindElementById("info").Text());

            driver.SwitchToFrame(0);
            A.AreEqual("Frame2", driver.FindElementById("info").Text());

            driver.SwitchToFrame(0);
            A.AreEqual("Frame3", driver.FindElementById("info").Text());

            driver.SwitchToDefaultContent();
            A.AreEqual("Frame1", driver.FindElementById("info").Text());
        }

        [TestCase]
        public void ShouldSwitchToFrameByWebElement() {
            A.AreEqual("Frame1", driver.FindElementById("info").Text());

            driver.SwitchToFrame(driver.FindElementByName("frm2"));
            A.AreEqual("Frame2", driver.FindElementById("info").Text());

            driver.SwitchToFrame(driver.FindElementById("frm3"));
            A.AreEqual("Frame3", driver.FindElementById("info").Text());

            driver.SwitchToDefaultContent();
            A.AreEqual("Frame1", driver.FindElementById("info").Text());
        }

        [TestCase]
        [IgnoreFixture(Browser.PhantomJS, "Not supported")]
        public void ShouldSwitchToParentFrame() {
            A.AreEqual("Frame1", driver.FindElementById("info").Text());

            driver.SwitchToFrame(driver.FindElementByName("frm2"));
            A.AreEqual("Frame2", driver.FindElementById("info").Text());

            driver.SwitchToFrame(driver.FindElementById("frm3"));
            A.AreEqual("Frame3", driver.FindElementById("info").Text());

            driver.SwitchToParentFrame();
            A.AreEqual("Frame2", driver.FindElementById("info").Text());
        }
    }
}
