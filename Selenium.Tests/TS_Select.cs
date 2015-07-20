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
    class TS_Select : BaseBrowsers {

        public TS_Select(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/select.html");
        }

        [TestCase]
        public void ShouldListItems() {
            var ele = driver.FindElementById("select").AsSelect();
            List lst = ele.Options.Text();
            A.AreEqual("Option One", lst.First());
            A.AreEqual("Option Three", lst.Last());
        }

        [TestCase]
        public void ShouldSelectByIndex() {
            var ele = driver.FindElementById("select").AsSelect();
            ele.SelectByIndex(1);
            A.AreEqual("Option Two", ele.SelectedOption.Text());
        }

        [TestCase]
        public void ShouldSelectByText() {
            var ele = driver.FindElementById("select").AsSelect();
            ele.SelectByText("Option Two");
            A.AreEqual("Option Two", ele.SelectedOption.Text());
        }

        [TestCase]
        public void ShouldSelectByValue() {
            var ele = driver.FindElementById("select").AsSelect();
            ele.SelectByValue("opt2");
            A.AreEqual("opt2", ele.SelectedOption.Attribute("value"));
        }

    }

}
