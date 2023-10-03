using NUnit.Framework;
using System;
using A = NUnit.Framework.Assert;

namespace Selenium.Tests.Internals {

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple=true)]
    public class IgnoreFixtureAttribute : Attribute, ITestAction {

        private object _fixture;
        private string _reason;

        public IgnoreFixtureAttribute(object fixture, string reason) {
            _fixture = fixture;
            _reason = reason;
        }

        public void BeforeTest(TestDetails testDetails) {
            BaseBrowsers tb = testDetails.Fixture as BaseBrowsers;
            if (tb != null && _fixture.Equals(tb.FixtureParam))
                A.Ignore(string.Format("{0}: {1}", _fixture, _reason));
        }

        public void AfterTest(TestDetails testDetails) {

        }

        public ActionTargets Targets {
            get { 
                return ActionTargets.Test | ActionTargets.Suite;
            }
        }

    }

}
