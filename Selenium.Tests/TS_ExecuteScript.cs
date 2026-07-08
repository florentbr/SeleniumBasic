using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
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
    class TS_ExecuteScript : BaseBrowsers {

        public TS_ExecuteScript(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/elements.html");
        }

        [TestCase]
        public void ShouldProcessWebElement() {
            //Webelement
            var ele = driver.FindElementById("input__text");
            var resEle = (WebElement)driver.ExecuteScript("return arguments[0];", ele);
            A.True(ele.Equals(resEle));
        }

        [TestCase]
        public void ShouldProcessWebElements() {
            //Webelements
            var eles = driver.FindElementsByTag("input");
            var resEles = (WebElements)driver.ExecuteScript("return arguments;", eles);
            A.AreEqual(eles.Count, resEles.Count);
            A.True(eles.First().Equals(resEles.First()));
        }

        [TestCase]
        public void ShouldProcessPrimitive() {
            //prinmitive int
            var res = driver.ExecuteScript("return arguments[0] + 3;", 8);
            A.AreEqual(11, res);

            //prinmitive double
            var valDouble = driver.ExecuteScript("return 1e-8;");
            A.AreEqual(1e-8, valDouble);
        }

        [TestCase]
        public void ShouldProcessDictionary() {
            //Dictionary
            var dict = new Selenium.Dictionary();
            dict.Add("a", 987);
            var resDict = (Dictionary)driver.ExecuteScript("return arguments[0];", dict);
            CollectionAssert.AreEquivalent(dict, resDict);
        }

        [TestCase]
        public void ShouldProcessList() {
            //List
            var list = new Selenium.List();
            list.Add(987);
            var resList = (Selenium.List)driver.ExecuteScript("return arguments;", list);
            CollectionAssert.AreEquivalent(list, resList);
        }

    }

}
