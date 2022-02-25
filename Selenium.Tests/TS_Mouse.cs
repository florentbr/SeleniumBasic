using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using SetUp = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

namespace Selenium.Tests {

    [TestFixture(Browser.Firefox)]
    [TestFixture(Browser.Gecko)]
    [TestFixture(Browser.Chrome)]
    [TestFixture(Browser.Edge)]
/*
    [TestFixture(Browser.Opera)]
    [TestFixture(Browser.IE)]
    [TestFixture(Browser.PhantomJS)]
*/
    class TS_Mouse : BaseBrowsers {

        public TS_Mouse(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
        }

        [TestCase]
        public void ShouldDoubleClick() {
            this.driver.BaseUrl = WebServer.BaseUri;
            driver.Get("/input.html");
            var ele = driver.FindElementById("input__search");
            var mouse = driver.Mouse;

            mouse.MoveTo(ele, 50);
            mouse.Click();
            mouse.ClickAndHold();
            mouse.Release();
        }

        [TestCase]
        public void DemoQAButtons() {
            driver.Get("https://demoqa.com/buttons");
            var mouse = driver.Mouse;
            WebElement ele;
            ele = driver.FindElementByXPath("//button[@type='button' and .='Click Me']");
            Assert.NotEquals( ele, null, "Left click button" );
            mouse.MoveTo(ele, 50);
            mouse.Click();
            WebElement e_scm = driver.FindElementById("dynamicClickMessage");
            Assert.NotEquals( e_scm, null );

            ele = driver.FindElementById("rightClickBtn");
            Assert.NotEquals( ele, null, "Right click button" );
            mouse.MoveTo(ele);
            mouse.Click(MouseButton.Right);
            WebElement e_rcm = driver.FindElementById("rightClickMessage");
            Assert.NotEquals( e_rcm, null );

            ele = driver.FindElementById("doubleClickBtn");
            Assert.NotEquals( ele, null, "Double click button" );
            mouse.MoveTo(ele);
            mouse.ClickDouble();
            WebElement e_dcm = driver.FindElementById("doubleClickMessage");
            Assert.NotEquals( e_dcm, null );
        }
    }

}
