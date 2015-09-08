using NUnit.Framework;
using System;
using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using Assert = Selenium.Assert;
using Selenium.ComInterfaces;

namespace Selenium.Tests {

    [TestFixture]
    public class TS_Assert {

        _Assert assert = new Assert();

        [Test]
        public void ShouldAssertTrue() {
            assert.True(true, "");
            try {
                assert.True(false, "msg");
                A.Fail();
            } catch (Exception ex) {
                A.AreEqual("Assert.True failed!\nmsg", ex.Message);
            }
        }

        [Test]
        public void ShouldAssertFalse() {
            assert.False(false, "");
            try {
                assert.False(true, "msg");
                A.Fail();
            } catch (Exception ex) {
                A.AreEqual("Assert.False failed!\nmsg", ex.Message);
            }
        }

        [Test]
        public void ShouldAssertEquals() {
            assert.Equals(true, true);
            try {
                assert.Equals(false, true);
                A.Fail();
            } catch (Exception ex) {
                A.AreEqual("Assert.Equals failed!\nexpected=False\nwas=True", ex.Message);
            }
        }

        [Test]
        public void ShouldAssertNotEquals() {
            assert.NotEquals(true, false);
            try {
                assert.NotEquals(false, false);
                A.Fail();
            } catch (Exception ex) {
                A.AreEqual("Assert.NotEquals failed!\nexpected!=False\nwas=False", ex.Message);
            }
        }

        [Test]
        public void ShouldAssertMatches() {
            assert.Matches(@"^\d+$", "1234");
            try {
                assert.Matches(@"^\d+$", "12a34");
                A.Fail();
            } catch (Exception ex) {
                A.AreEqual("Assert.Matches failed!\npattern=\"^\\d+$\"\nwas=\"12a34\"", ex.Message);
            }
        }

        [Test]
        public void ShouldAssertNotMatches() {
            assert.NotMatches(@"^\d+$", "12a34");
            try {
                assert.NotMatches(@"^\d+$", "1234");
                A.Fail();
            } catch (Exception ex) {
                A.AreEqual("Assert.NotMatches failed!\npattern!=\"^\\d+$\"\nwas=\"1234\"", ex.Message);
            }
        }

        [Test]
        public void ShouldAssertContains() {
            assert.Contains(@"a", "12a34");
            try {
                assert.Contains(@"a", "1234");
                A.Fail();
            } catch (Exception ex) {
                A.AreEqual("Assert.Contains failed!\ncontains=\"a\"\nwas=\"1234\"", ex.Message);
            }
        }

        [Test]
        public void ShouldAssertFail() {
            try {
                assert.Fail("msg");
                A.Fail();
            } catch (Exception ex) {
                A.AreEqual("Assert.Fail failed!\nmsg", ex.Message);
            }
        }

    }

}
