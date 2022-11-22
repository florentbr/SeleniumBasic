using System;
using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using SetUp = NUnit.Framework.SetUpAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using ExpectedException = NUnit.Framework.ExpectedExceptionAttribute;

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
    class TS_SearchContext : BaseBrowsers {

        public TS_SearchContext(Browser browser)
            : base(browser) { }

        [SetUp]
        public void SetUp() {
            driver.Get("/findby.html");
        }

        [TestCase]
        public void ShouldFindElementById() {
            var ele = driver.FindElementById("id-4");
            A.AreEqual("Div4", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByClass() {
            var ele = driver.FindElementByClass("class");
            A.AreEqual("Div2", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByCss() {
            var ele = driver.FindElementByCss("#id-5");
            A.AreEqual("Div5", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByName() {
            var ele = driver.FindElementByName("name");
            A.AreEqual("Div3", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByLinkText() {
            var ele = driver.FindElementByLinkText("Link1");
            A.AreEqual("Link1", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByPartialLinkText() {
            var ele = driver.FindElementByPartialLinkText("ink1");
            A.AreEqual("Link1", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByTag() {
            var ele = driver.FindElementByTag("span");
            A.AreEqual("Div5", ele.Text());
        }

        [TestCase]
        public void ShouldFindElementByXPath() {
            var ele = driver.FindElementByXPath(@"//div[@id='id-4']");
            A.AreEqual("Div4", ele.Text());
        }        


        [TestCase]
        public void ShouldFindElementsById() {
            var elts = driver.FindElementsById("id-4");
            A.AreEqual(1, elts.Count);
            A.AreEqual("Div4", elts[0].Text());
        }

        [TestCase]
        public void ShouldFindElementsByClass() {
            var elts = driver.FindElementsByClass("class");
            A.AreEqual(1, elts.Count);
            A.AreEqual("Div2", elts[0].Text());
        }

        [TestCase]
        public void ShouldFindElementsByCss() {
            var elts = driver.FindElementsByCss("div a");
            A.AreEqual(2, elts.Count);
            A.AreEqual("Link2", elts[1].Text());
        }

        [TestCase]
        public void ShouldFindElementsByName() {
            var elts = driver.FindElementsByName("name");
            A.AreEqual(1, elts.Count);
            A.AreEqual("Div3", elts[0].Text());
        }

        [TestCase]
        public void ShouldFindElementsByLinkText() {
            var elts = driver.FindElementsByLinkText("Link1");
            A.AreEqual(1, elts.Count);
            A.AreEqual("Link1", elts[0].Text());
        }

        [TestCase]
        public void ShouldFindElementsByPartialLinkText() {
            var elts = driver.FindElementsByPartialLinkText("Link");
            A.AreEqual(2, elts.Count);
            A.AreEqual("Link2", elts[1].Text());
        }

        [TestCase]
        public void ShouldFindElementsByTag() {
            var elts = driver.FindElementsByTag("a");
            A.AreEqual(2, elts.Count);
            A.AreEqual("Link2", elts[1].Text());
        }

        [TestCase]
        public void ShouldFindElementsByXPath() {
            var elts = driver.FindElementsByXPath(@"//a");
            A.AreEqual(2, elts.Count);
            A.AreEqual("Link2", elts[1].Text());
        }

        [TestCase]
        public void ShouldFindElementsByAny() {
            var elts = driver.FindElements(By.Any(By.Class("class"), By.Tag("a")));
            A.AreEqual(3, elts.Count);
        }

        private void AddNewElementAfter( int delay ) {
            driver.ExecuteScript(@"setTimeout(function(){
                var e = document.createElement('div');
                e.id = 'id-append';
                e.innerHTML = 'DivAppend';
                document.body.appendChild(e);
            }, " + delay + ");");
        }

        [TestCase]
        public void ShouldFindWithExplicitWait() {
            AddNewElementAfter( 200 );
            var ele = driver.FindElementById("id-append", 1500, true);
        }

        [TestCase]
        public void ShouldFindWithImplicitWait() {
            AddNewElementAfter( 200 );
            var ele = driver.FindElementById("id-append");
            A.IsNotNull(ele);
        }

        [TestCase]
        [IgnoreFixture(Browser.Gecko, "Implicit timeout is always 0 and cannot be changed")]
        public void ShouldFindWithDriverImplicit() {
            const int implicit_timeout = 500;
            driver.Timeouts.Implicit = implicit_timeout;
            if( Fixture.Equals( Browser.Firefox ) )
                A.Ignore( "Timeout can be only set but not get" );
            else
                A.AreEqual( implicit_timeout, driver.Timeouts.Implicit, "Cannot set the implicit timeout to the driver" );
            AddNewElementAfter( 200 );
            var ele = driver.FindElementById("id-append", 1, true); 
        }

        [TestCase]
        [ExpectedException(typeof(Selenium.Errors.NoSuchElementError))]
        public void ShouldNotFind() {
            var ele = driver.FindElementById("id-missing", 5000, true);
        }

        [TestCase]
        public void ShouldFindWithNoException() {
            var ele = driver.FindElementById("id-missing", 0, false);
            A.Null(ele);
        }

        [TestCase]
        [IgnoreFixture(Browser.Firefox, "Shadow is not supported")]
        public void ShouldGetThroughShadow() {
            var sc = driver.FindElementByCss("div#shadow-ctr");
            var sr = sc.Shadow();
            A.IsNotNull( sr, "Element has no Shadow " );
            if( Fixture.Equals( Browser.Gecko ) )
                A.Ignore( "DNW in gecko. See https://bugzilla.mozilla.org/show_bug.cgi?id=1700097" );
            if( !( Fixture.Equals( Browser.Chrome ) || Fixture.Equals( Browser.Edge ) ) )
                A.Ignore("Old browsers are not supported");
            A.IsNotNull(sr.FindElementByCss("span"));
            A.IsNotNull(sr.FindElementByTag("span"));
            A.Throws<SeleniumError>( () => sr.FindElementByXPath(".//span") );  // apparently not supported
        }
    }
}
