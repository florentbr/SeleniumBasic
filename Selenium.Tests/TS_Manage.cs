using Selenium.Tests.Internals;
using System;
using A = NUnit.Framework.Assert;
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
    class TS_Manage : BaseBrowsers {

        public TS_Manage(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/input.html");
        }

        [TestCase]
        [IgnoreFixture(Browser.IE, "Not supported")]
        public void ShouldAddAndDeleteCookies() {
            var manage = driver.Manage;
            A.AreEqual(0, manage.Cookies.Count);

            DateTime now = DateTime.Now;
            DateTime exp = now.AddDays( 10 );
            string exp_s = exp.ToString("yyyy-MM-ddTHH:mm:ss");
            manage.AddCookie("ck_name", "ck_value", null, "/", exp_s);
            var cookies = manage.Cookies;
            A.AreEqual(1, manage.Cookies.Count);

            var cookie = cookies[0];
            A.AreEqual("ck_name", cookie.Name);
            A.AreEqual("ck_value", cookie.Value);
            A.AreEqual("localhost", cookie.Domain);
            A.AreEqual("/", cookie.Path);
            A.AreEqual(exp_s, cookie.Expiry);

            driver.Manage.DeleteAllCookies();
            A.AreEqual(0, manage.Cookies.Count);
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
