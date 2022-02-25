using System;
using System.IO;
using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
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
    class TS_Capture : BaseBrowsers {

        public TS_Capture(Browser browser)
            : base(browser) { }

        [TestCase]
        public void ShouldCaptureToFile() {
            driver.Get("/elements.html");
            var imgname = string.Format("wd-capt-{0}-{1}.png"
                , Fixture.ToString().ToLower()
                , DateTime.Now.Ticks.ToString());

            driver.TakeScreenshot().SaveAs(@"%temp%\" + imgname);
            var file = new FileInfo(Environment.ExpandEnvironmentVariables(@"%temp%\" + imgname));
            A.Greater(file.Length, 10000);
            file.Delete();
        }

    }

}
