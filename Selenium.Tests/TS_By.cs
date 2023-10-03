using Selenium.ComInterfaces;
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
    class TS_By : BaseBrowsers {

        _By by = new By();

        public TS_By(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/findby.html");
        }

        [TestCase]
        public void ShouldFindElementById() {
            var ele = driver.FindElement(by.Id("id-4"));
            A.AreEqual("Div4", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByClass() {
            var ele = driver.FindElement(by.Class("class"));
            A.AreEqual("Div2", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByCss() {
            var ele = driver.FindElement(by.Css("#id-5"));
            A.AreEqual("Div5", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByName() {
            var ele = driver.FindElement(by.Name("name"));
            A.AreEqual("Div3", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByLinkText() {
            var ele = driver.FindElement(by.LinkText("Link1"));
            A.AreEqual("Link1", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByPartialLinkText() {
            var ele = driver.FindElement(by.PartialLinkText("ink1"));
            A.AreEqual("Link1", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByTag() {
            var ele = driver.FindElement(by.Tag("span"));
            A.AreEqual("Div5", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByXPath() {
            var ele = driver.FindElement(by.XPath(@"//div[@id='id-4']"));
            A.AreEqual("Div4", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByAny() {
            var ele = driver.FindElement(By.Any(By.Id("id_missing"), By.Id("id-4")));
            A.AreEqual("Div4", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementsById() {
            var elts = driver.FindElements(by.Id("id-4"));
            A.AreEqual(1, elts.Count);
            A.AreEqual("Div4", elts[0].Text());
        }

        [TestCase]
        public void ShouldFindElementsByClass() {
            var elts = driver.FindElements(by.Class("class"));
            A.AreEqual(1, elts.Count);
            A.AreEqual("Div2", elts[0].Text());
        }

        [TestCase]
        public void ShouldFindElementsByCss() {
            var elts = driver.FindElements(by.Css("div a"));
            A.AreEqual(2, elts.Count);
            A.AreEqual("Link2", elts[1].Text());
        }

        [TestCase]
        public void ShouldFindElementsByName() {
            var elts = driver.FindElements(by.Name("name"));
            A.AreEqual(1, elts.Count);
            A.AreEqual("Div3", elts[0].Text());
        }

        [TestCase]
        public void ShouldFindElementsByLinkText() {
            var elts = driver.FindElements(by.LinkText("Link1"));
            A.AreEqual(1, elts.Count);
            A.AreEqual("Link1", elts[0].Text());
        }

        [TestCase]
        public void ShouldFindElementsByPartialLinkText() {
            var elts = driver.FindElements(by.PartialLinkText("Link"));
            A.AreEqual(2, elts.Count);
            A.AreEqual("Link2", elts[1].Text());
        }

        [TestCase]
        public void ShouldFindElementsByTag() {
            var elts = driver.FindElements(by.Tag("a"));
            A.AreEqual(2, elts.Count);
            A.AreEqual("Link2", elts[1].Text());
        }

        [TestCase]
        public void ShouldFindElementsByXPath() {
            var elts = driver.FindElements(by.XPath(@"//a"));
            A.AreEqual(2, elts.Count);
            A.AreEqual("Link2", elts[1].Text());
        }

        [TestCase]
        public void ShouldFindElementsByAny() {
            var elts = driver.FindElements(By.Any(By.Class("class"), By.Tag("a")));
            A.AreEqual(3, elts.Count);
        }

    }

}
