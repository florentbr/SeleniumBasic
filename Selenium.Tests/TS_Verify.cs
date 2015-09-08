using NUnit.Framework;
using Selenium.ComInterfaces;
using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;

namespace Selenium.Tests {

    [TestFixture]
    public class TS_Verify {

        _Verify verify = new Verify();

        [Test]
        public void ShouldVerifyTrueOK() {
            var res = verify.True(true);
            A.AreEqual("OK", res);
        }

        [Test]
        public void ShouldVerifyTrueKO() {
            var res = verify.True(false);
            A.AreEqual("KO expected=True was=False", res);
        }

        [Test]
        public void ShouldVerifyEnumEqual() {
            var res = verify.Equals("a;b;c", new string[] { "a", "b", "c" }, "");
            A.AreEqual("OK", res);
        }

        [Test]
        public void ShouldVerifyEnumNotEqual() {
            var res = verify.Equals("a;c;b", new string[] { "a", "b", "c" }, null);
            A.AreEqual("KO expected=\"a;c;b\" was=[a,b,c]", res);
        }

    }

}
