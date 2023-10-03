using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
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
    class TS_Scraping : BaseBrowsers {

        public TS_Scraping(Browser b)
            : base(b) { }

        [TestCase]
        public void ShouldScrapTextFromTable() {
            driver.Get("/table.html");
            var element = driver.FindElementByCss("#table table");
            TableElement table = element.AsTable();
            var data = table.Data();
            A.AreEqual(data[1, 1], "Table Heading 1");
            A.AreEqual(data[6, 5], "Table Footer 5");
        }

        [TestCase]
        public void ShouldScrapTextFromElements() {
            driver.Get("/table.html");
            var elements = driver.FindElementsByCss("thead th");
            var data = elements.Text();
            A.AreEqual(data[0], "Table Heading 1");
        }
    }

}
