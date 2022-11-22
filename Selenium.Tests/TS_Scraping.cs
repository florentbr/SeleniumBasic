using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

namespace Selenium.Tests {

    [TestFixture(Browser.Firefox)]
    [TestFixture(Browser.Gecko, Category = "InFocus")]
    [TestFixture(Browser.Chrome)]
    [TestFixture(Browser.Edge)]
/*
    [TestFixture(Browser.Opera)]
    [TestFixture(Browser.IE)]
    [TestFixture(Browser.PhantomJS)]
*/
    class TS_Scraping : BaseBrowsers {

        public TS_Scraping(Browser browser)
            : base(browser) { }

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

        [TestCase]
//        [IgnoreFixture(Browser.Gecko, "Not supported")]
        public void ShouldGetBrowserLog() {
            driver.Get("/notexisting.html");
            Logs l = driver.Manage.Logs;
            List sl = l.Browser;
            A.IsNotNull( sl );
        }
        [TestCase]
        [IgnoreFixture(Browser.Gecko, "Not supported")]
        public void ShouldGetDriverLog() {
            driver.Get("/notexisting.html");
            Logs l = driver.Manage.Logs;
            List sl = l.Driver;
            A.IsNotNull( sl );
        }
    }

}
