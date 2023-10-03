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
            driver.Wait( 5000 );
            var mouse = driver.Mouse;
            WebElement ele;
            ele = driver.FindElementByXPath("//button[@type='button' and .='Click Me']");
            A.IsNotNull( ele, "Left click button" );
            mouse.MoveTo(ele, 50);
            mouse.Click();
            WebElement e_scm = driver.FindElementById("dynamicClickMessage");
            A.IsNotNull( e_scm, "dynamicClickMessage" );

            ele = driver.FindElementById("rightClickBtn");
            A.IsNotNull( ele, "Right click button" );
            mouse.MoveTo(ele);
            mouse.Click(MouseButton.Right);
            WebElement e_rcm = driver.FindElementById("rightClickMessage");
            A.IsNotNull( e_rcm, "rightClickMessage" );

            ele = driver.FindElementById("doubleClickBtn");
            A.IsNotNull( ele, "Double click button" );
            mouse.MoveTo(ele);
            mouse.ClickDouble();
            WebElement e_dcm = driver.FindElementById("doubleClickMessage");
            A.IsNotNull( e_dcm, "doubleClickMessage" );
        }
    }

}
