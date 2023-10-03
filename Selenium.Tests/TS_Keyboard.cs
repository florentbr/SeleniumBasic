using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using SetUp = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

/*
 * Attention!
 * Gecko does not return the input control value via the Attribute.
 * Even when executed in the F12 console, the result of the following will be null:
 * input__search.getAttribute("value")
 * In this test all ele.Attribute("value") are replaced with ele.Value()
 * which now uses the driver's endpoint property/value instead of attribute/value
 * <see href="https://w3c.github.io/webdriver/#dfn-get-element-property"/>
*/

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
            var result = ele.Value();
            A.AreEqual(expected, result);
            driver.Wait( 1000 );
        }

        [TestCase]
        [IgnoreFixture(Browser.Edge, "MSEdgeDriver only supports characters in the BMP")]
        [IgnoreFixture(Browser.Chrome, "ChromeDriver only supports characters in the BMP")]
        [IgnoreFixture(Browser.Opera, "Not supported")]
        public void ShouldSendAndReadCharsSMP() {
            string expected = "🌍🍈👽💔";
            var ele = driver.FindElementById("input__search");
            ele.Clear().SendKeys(expected);
            var result = ele.Value();
            A.AreEqual(expected, result);
            driver.Wait( 1000 );
        }

        [TestCase]
        [IgnoreFixture(Browser.Edge, "MSEdgeDriver only supports characters in the BMP")]
        [IgnoreFixture(Browser.Chrome, "ChromeDriver only supports characters in the BMP")]
        [IgnoreFixture(Browser.Opera, "Not supported")]
        public void ShouldSendAndReadCharsSIP() {
            string expected = "𠀀𠀁𠀂𠀃𤁴𤁵𤁶𫜲𫜳𫜴";
            var ele = driver.FindElementById("input__search");
            ele.Clear().SendKeys(expected);
            var result = ele.Value();
            A.AreEqual(expected, result);
            driver.Wait( 1000 );
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleModifiersWithDriver() {
            //Delete with driver SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg1");
            A.AreEqual("abcdefg1", ele.Value());
            driver.SendKeys(Keys.Control, "a");
            driver.SendKeys(Keys.Delete);
            A.AreEqual("", ele.Value());
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleModifiersWithKeyboard() {
            //Delete with driver SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg2");
            A.AreEqual("abcdefg2", ele.Value());
            var kb = driver.Keyboard;
            kb.KeyDown(Keys.Control);
            kb.SendKeys("a");
            kb.KeyUp(Keys.Control);
            kb.SendKeys(Keys.Delete);
            A.AreEqual("", ele.Value());
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleModifiersWithElement() {
            //Delete with element SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg3");
            A.AreEqual("abcdefg3", ele.Value());
            ele.SendKeys(Keys.Control, "a" + Keys.Delete);
            A.AreEqual("", ele.Value());
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleModifiersWithActions() {
            //Delete with action SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg4");
            A.AreEqual("abcdefg4", ele.Value());
            driver.Actions
                .SendKeys(Keys.Control + "a", ele)
                .SendKeys(Keys.Delete)
                .Perform();
            A.AreEqual("", ele.Value());
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldHandleExplicitModifiersWithActions() {
            //Delete with action SendKeys
            var ele = driver.FindElementById("input__search");
            ele.SendKeys("abcdefg5");
            A.AreEqual("abcdefg5", ele.Value());
            driver.Actions
                .KeyDown(Keys.Control, ele)
                .SendKeys("a", ele)
                .KeyUp(Keys.Control)
                .SendKeys(Keys.Delete)
                .Perform();
            A.AreEqual("", ele.Value());
        }
    }
}
