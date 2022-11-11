using Selenium.Tests.Internals;
using System;
using System.ComponentModel;
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
            A.IsNotNull( ele, "Failed to find element by Id" );
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
            driver.Wait( 5000 );    // some ads appear
            WebElement ele;
            ele = driver.FindElementByXPath("//button[@type='button' and .='Click Me']");
            A.IsNotNull( ele, "Left click button" );
            driver.Actions
                .Click(ele)
                .Perform();
            WebElement e_scm = driver.FindElementById("dynamicClickMessage");
            A.IsNotNull( e_scm, "dynamic Click Message" );

            ele = driver.FindElementById("rightClickBtn");
            A.IsNotNull( ele, "Right click button" );
            driver.Actions
                .ClickContext(ele)
                .Perform();
            WebElement e_rcm = driver.FindElementById("rightClickMessage");
            A.IsNotNull( e_rcm, "rightClickMessage" );

            ele = driver.FindElementById("doubleClickBtn");
            A.IsNotNull( ele, "Right click button" );
            driver.Actions
                .ClickDouble(ele)
                .Perform();
            WebElement e_dcm = driver.FindElementById("doubleClickMessage");
            A.IsNotNull( e_dcm, "doubleClickMessage" );
        }

        [TestCase]
        public void DemoQASlider() {
            driver.Get("https://demoqa.com/slider");
            driver.Wait( 5000 );    // some ads appear
            WebElement ele;
            ele = driver.FindElementByXPath("//input[@type='range']");
            A.IsNotNull( ele, "range input" );
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
            driver.Wait( 5000 );    // some ads appear
            WebElement ele;
            ele = driver.FindElementById("toolTipButton");
            A.IsNotNull( ele, "toolTipButton" );
            driver.Actions
                .MoveToElement(ele)
                .Wait( 1000 )
                .Perform();
            A.IsNotNull( driver.FindElementById("buttonToolTip"), "buttonToolTip" );
        }

        [TestCase]
        public void DemoQADragDrop() {
            driver.Get("https://demoqa.com/dragabble");
            WebElement ele = driver.FindElementById("dragBox");
            A.IsNotNull( ele, "dragBox" );
            Point loc1 = ele.LocationInView();
            driver.Actions
                .DragAndDropByOffset(ele, 100, 50)
                .Wait( 1000 )
                .Perform();
            Point loc2 = ele.LocationInView();
            A.AreNotEqual( loc1.X, loc2.X );
            A.AreNotEqual( loc1.Y, loc2.Y );
            driver.Wait( 1000 );
            driver.Get("https://demoqa.com/droppable");
            WebElement ele1 = driver.FindElementById("draggable");
            WebElement ele2 = driver.FindElementById("droppable");
            A.IsNotNull( ele1 );
            A.IsNotNull( ele1 );
            driver.Actions
                .DragAndDrop(ele1, ele2)
                .Wait( 1000 )
                .Perform();
            A.AreEqual( "Dropped!", ele2.Text() );
        }

    }
}
