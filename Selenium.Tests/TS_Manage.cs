using Selenium.Tests.Internals;
using System;
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

    }

}
