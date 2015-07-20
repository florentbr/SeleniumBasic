using Selenium.Tests.Internals;
using SetUp = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

namespace Selenium.Tests {

    [TestFixture(Browser.Firefox)]
    [TestFixture(Browser.Opera)]
    [TestFixture(Browser.Chrome)]
    [TestFixture(Browser.IE)]
    [TestFixture(Browser.PhantomJS)]
    class TS_Actions : BaseBrowsers {

        public TS_Actions(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/input.html");
        }

        [TestCase]
        public void ShouldPerformActionsWithoutFailures() {
            var ele = driver.FindElementById("input__search");
            driver.Actions
                .Click(ele)
                .Click()
                .ClickAndHold()
                .Release()
                .ClickAndHold(ele)
                .Release()
                .ClickDouble()
                .ClickDouble(ele)
                .DragAndDrop(ele, ele)
                .DragAndDropByOffset(ele, 10, 10)
                .KeyDown(Keys.Control, ele)
                .KeyUp(Keys.Control)
                .MoveByOffset(30, 87)
                .MoveToElement(ele)
                .SendKeys("abcd")
                .Wait(0)
                .ClickContext()
                .ClickContext(ele)
                .Perform();
        }

    }

}
