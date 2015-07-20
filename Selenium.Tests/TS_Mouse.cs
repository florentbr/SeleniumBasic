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
    class TS_Mouse : BaseBrowsers {

        public TS_Mouse(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/input.html");
        }

        [TestCase]
        public void ShouldDoubleClick() {
            var ele = driver.FindElementById("input__search");
            var mouse = driver.Mouse;

            mouse.MoveTo(ele, 50);
            mouse.Click();
            mouse.ClickAndHold();
            mouse.Release();
        }

    }

}
