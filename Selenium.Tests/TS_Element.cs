using System;
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
    class TS_Element : BaseBrowsers {

        public TS_Element(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/element.html");
        }
/*
        [TestCase]
        public void ShouldReturnDisplayState() {
            var ele1 = driver.FindElementById("input__search");
            A.True(ele1.IsDisplayed);

            var ele2 = driver.FindElementById("input__search_hidden");
            A.False(ele2.IsDisplayed);
        }

        [TestCase]
        public void ShouldReturnSelectedState() {
            var ele1 = driver.FindElementById("checkbox1");
            A.False(ele1.IsSelected);

            var ele2 = driver.FindElementById("checkbox2");
            A.True(ele2.IsSelected);

            var ele3 = driver.FindElementById("select_item2");
            A.True(ele2.IsSelected);
        }

        [TestCase]
        public void ShouldReturnAttribute() {
            var ele1 = driver.FindElementById("input__search");
            A.AreEqual("search", ele1.Attribute("type"));
        }

        [TestCase]
        public void ShouldReturnCssValue() {
            var ele1 = driver.FindElementById("input__search");
            A.AreEqual("300px", ele1.CssValue("width"));
        }

        [TestCase]
        public void ShouldReturnText() {
            var ele1 = driver.FindElementByCss("#forms__text label");
            A.AreEqual("Search", ele1.Text());
        }

        [TestCase]
        public void ShouldReturnTagName() {
            var ele1 = driver.FindElementById("forms__text");
            A.AreEqual("fieldset", ele1.TagName);
        }

        [TestCase]
        public void ShouldReturnIsEnabled() {
            var ele1 = driver.FindElementById("input__search");
            A.True(ele1.IsEnabled);

            var ele2 = driver.FindElementById("input__search_disabled");
            A.False(ele2.IsEnabled);
        }

        [TestCase]
        public void ShouldReturnLocation() {
            var ele1 = driver.FindElementById("txt_div");

            Point expected = new Point(20, 156);
            Point actual = ele1.Location();

            int diffX = Math.Abs(actual.X - expected.X);
            int diffY = Math.Abs(actual.Y - expected.Y);

            A.True(diffX < 2 && diffY < 20
                , string.Format("Expected {0} but was {1}", expected, actual));
        }

        [TestCase]
        public void ShouldReturnSize() {
            var ele1 = driver.FindElementById("txt_div");
            var size = ele1.Size();
            A.AreEqual(308, size.Width);
            A.AreEqual(58, size.Height);
        }
*/
        [TestCase]
        public void ShouldReturnActiveElement() {
            var ele1 = driver.FindElementById("input__search");
            ele1.SendKeys("");

            var ele2 = driver.ActiveElement();
            A.True(ele1.Equals(ele2));
        }
/*
        [TestCase]
        public void ShouldClearElement() {
            var ele1 = driver.FindElementById("input__search");
            ele1.SendKeys("abc");
            A.AreEqual("abc", ele1.Attribute("value"));
            ele1.Clear();
            A.AreEqual("", ele1.Attribute("value"));
        }

        [TestCase]
        public void ShouldClickElement() {
            var ele1 = driver.FindElementById("input__search");
            ele1.SendKeys("abc");
            A.AreEqual("abc", ele1.Attribute("value"));

            var ele2 = driver.FindElementById("bt_reset");
            ele2.Click();
            A.AreEqual("", ele1.Attribute("value"));
        }
*/
    }

}
