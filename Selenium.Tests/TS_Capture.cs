using System;
using System.IO;
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
    class TS_Capture : BaseBrowsers {

        public TS_Capture(Browser browser)
            : base(browser) { }

        [TestCase]
        public void ShouldCaptureToFile() {
            driver.Get("/elements.html");
            var imgname = string.Format("wd-capt-{0}-{1}.png"
                , FixtureParam.ToString().ToLower()
                , DateTime.Now.Ticks.ToString());

            driver.TakeScreenshot().SaveAs(@"%temp%\" + imgname);
            var file = new FileInfo(Environment.ExpandEnvironmentVariables(@"%temp%\" + imgname));
            A.Greater(file.Length, 10000);
            file.Delete();
        }

    }

}
