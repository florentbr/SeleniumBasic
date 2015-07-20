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
    class TS_Keyboard : BaseBrowsers {

        public TS_Keyboard(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/input.html");
        }

        [TestCase]
        public void ShouldSendAndReadCharsBMP() {
            string expected = "éáéú€ΑΒΓΔΕΖໝ‱ㄓ龜龜契金喇网ꬤ￦";
            var ele = driver.FindElementById("input__search");
            ele.Clear();
            ele.SendKeys(expected);
            var result = ele.Attribute("value");
            A.AreEqual(expected, result);
        }

        [TestCase]
        [IgnoreFixture(Browser.Chrome, "Not supported")]
        [IgnoreFixture(Browser.Opera, "Not supported")]
        public void ShouldSendAndReadCharsSMP() {
            string expected = "🌍🍈👽💔";
            var ele = driver.FindElementById("input__search");
            ele.Clear().SendKeys(expected);
            var result = ele.Attribute("value");
            A.AreEqual(expected, result);
        }

        [TestCase]
        [IgnoreFixture(Browser.Chrome, "Not supported")]
        [IgnoreFixture(Browser.Opera, "Not supported")]
        public void ShouldSendAndReadCharsSIP() {
            string expected = "𠀀𠀁𠀂𠀃𤁴𤁵𤁶𫜲𫜳𫜴";
            var ele = driver.FindElementById("input__search");
            ele.Clear().SendKeys(expected);
            var result = ele.Attribute("value");
            A.AreEqual(expected, result);
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleModifiersWithDriver() {
            //Delete with driver SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg");
            A.AreEqual("abcdefg", ele.Attribute("value"));
            driver.SendKeys(Keys.Control, "a");
            driver.SendKeys(Keys.Delete);
            A.AreEqual("", ele.Attribute("value"));
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleModifiersWithKeyboard() {
            //Delete with driver SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg");
            A.AreEqual("abcdefg", ele.Attribute("value"));
            var kb = driver.Keyboard;
            kb.KeyDown(Keys.Control);
            kb.SendKeys("a");
            kb.KeyUp(Keys.Control);
            kb.SendKeys(Keys.Delete);
            A.AreEqual("", ele.Attribute("value"));
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleModifiersWithElement() {
            //Delete with element SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg");
            A.AreEqual("abcdefg", ele.Attribute("value"));
            ele.SendKeys(Keys.Control, "a" + Keys.Delete);
            A.AreEqual("", ele.Attribute("value"));
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleModifiersWithActions() {
            //Delete with action SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg");
            A.AreEqual("abcdefg", ele.Attribute("value"));
            driver.Actions
                .SendKeys(Keys.Control + "a", ele)
                .SendKeys(Keys.Delete)
                .Perform();
            A.AreEqual("", ele.Attribute("value"));
        }

    }

}
