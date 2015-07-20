using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using SetUp = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

namespace Selenium.Tests {

    [TestFixture(Browser.Firefox)]
    [TestFixture(Browser.Opera)]
    [TestFixture(Browser.Chrome)]
    [TestFixture(Browser.IE)]
    [TestFixture(Browser.PhantomJS)]
    class TS_Window : BaseBrowsers {

        public TS_Window(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/win1.html");
        }

        [TestCase]
        public void ShouldDisplayMaximized() {
            var win = driver.Window;
            win.SetSize(800, 600);

            win.Maximize();
            var size = win.Size();
            A.Greater(size.Width, 900);
            A.Greater(size.Height, 700);
        }

        [TestCase]
        public void ShouldGetAndSetSize() {
            var win = driver.Window;
            win.SetSize(800, 600);
            var size = win.Size();
            A.AreEqual(800, size.Width);
            A.AreEqual(600, size.Height);
        }

        [TestCase]
        [IgnoreFixture(Browser.PhantomJS, "Not supported")]
        public void ShouldGetAndSetPosition() {
            var win = driver.Window;
            win.SetPosition(17, 23);
            var pos = win.Position();
            A.AreEqual(17, pos.X);
            A.AreEqual(23, pos.Y);
        }

        [TestCase]
        public void ShouldGetTitle() {
            var win = driver.Window;
            A.AreEqual("Window1",  win.Title);
        }

        [TestCase]
        [IgnoreFixture(Browser.PhantomJS, "Not supported")]
        public void ShouldCloseWindow() {
            var win1 = driver.Window;
            driver.FindElementByLinkText("Window2").Click();
            driver.SwitchToNextWindow().Close();
            win1.Activate();
            A.AreEqual(1, driver.Windows.Count);
        }

    }

}
