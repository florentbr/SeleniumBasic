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

            //the expiry must be in the future and within the 400 days accepted by Chrome
            string expiry = System.DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss");
            manage.AddCookie("ck_name", "ck_value", null, "/", expiry);
            var cookies = manage.Cookies;
            A.AreEqual(1, manage.Cookies.Count);

            var cookie = cookies[0];
            A.AreEqual("ck_name", cookie.Name);
            A.AreEqual("ck_value", cookie.Value);
            A.AreEqual("localhost", cookie.Domain);
            A.AreEqual("/", cookie.Path);
            A.AreEqual(expiry, cookie.Expiry);

            driver.Manage.DeleteAllCookies();
            A.AreEqual(0, manage.Cookies.Count);
        }

    }

}
