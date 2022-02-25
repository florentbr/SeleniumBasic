using Selenium.Tests.Internals;
using System.ComponentModel;
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
    class TS_Actions : BaseBrowsers {
        public TS_Actions(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
        }

        [TestCase]
        public void LocalShouldNotFail() {
            this.driver.BaseUrl = WebServer.BaseUri;
            driver.Get("/input.html");
            WebElement ele = driver.FindElementById("input__search");
            Assert.NotEquals( ele, null, "Failed to find element by Id" );
            driver.Actions
                .Click(ele)
                .Click()
                .ClickAndHold()
                .Release()
                .ClickAndHold(ele)
                .Release()
                .ClickDouble()
                .ClickDouble(ele)
                .KeyDown(Keys.Control, ele)
                .KeyUp(Keys.Control)
                .MoveByOffset(30, 87)
                .MoveToElement(ele)
                .SendKeys("abcd")
                .Wait(0)
                .ClickContext()
                .Wait(1000)
                .ClickContext(ele)
                .Wait(1000)
                .Perform();
        }

        [TestCase]
        public void DemoQAButtons() {
            driver.Get("https://demoqa.com/buttons");
            WebElement ele;
            ele = driver.FindElementByXPath("//button[@type='button' and .='Click Me']");
            Assert.NotEquals( ele, null, "Left click button" );
            driver.Actions
                .Click(ele)
                .Perform();
            WebElement e_scm = driver.FindElementById("dynamicClickMessage");
            Assert.NotEquals( e_scm, null );

            ele = driver.FindElementById("rightClickBtn");
            Assert.NotEquals( ele, null, "Right click button" );
            driver.Actions
                .ClickContext(ele)
                .Perform();
            WebElement e_rcm = driver.FindElementById("rightClickMessage");
            Assert.NotEquals( e_rcm, null );

            ele = driver.FindElementById("doubleClickBtn");
            Assert.NotEquals( ele, null, "Right click button" );
            driver.Actions
                .ClickDouble(ele)
                .Perform();
            WebElement e_dcm = driver.FindElementById("doubleClickMessage");
            Assert.NotEquals( e_dcm, null );
        }

        [TestCase]
        public void DemoQASlider() {
            driver.Get("https://demoqa.com/slider");
            WebElement ele;
            ele = driver.FindElementByXPath("//input[@type='range']");
            Assert.NotEquals( ele, null );
            driver.Actions
                .ClickAndHold(ele)
                .MoveByOffset( 20, 0 )
                .Release()
                .Wait( 2000 )
                .ClickAndHold()
                .MoveByOffset( -40, 0 )
                .Release()
                .Wait( 1000 )
                .Perform();
        }

        [TestCase]
        public void DemoQAHover() {
            driver.Get("https://demoqa.com/tool-tips");
            WebElement ele;
            ele = driver.FindElementById("toolTipButton");
            Assert.NotEquals( ele, null );
            driver.Actions
                .MoveToElement(ele)
                .Wait( 1000 )
                .Perform();
            Assert.NotEquals( driver.FindElementById("buttonToolTip"), null );
        }

        [TestCase]
        public void DemoQADragDrop() {
            driver.Get("https://demoqa.com/dragabble");
            WebElement ele = driver.FindElementById("dragBox");
            Assert.NotEquals( ele, null );
            driver.Actions
                .DragAndDropByOffset(ele, 100, 50)
                .Wait( 1000 )
                .Perform();
            driver.Wait( 1000 );
            driver.Get("https://demoqa.com/droppable");
            WebElement ele1 = driver.FindElementById("draggable");
            WebElement ele2 = driver.FindElementById("droppable");
            Assert.NotEquals( ele1, null );
            Assert.NotEquals( ele1, null );
            driver.Actions
                .DragAndDrop(ele1, ele2)
                .Wait( 1000 )
                .Perform();
            Assert.Equals( ele2.Text(), "Dropped!" );
        }

    }

}
