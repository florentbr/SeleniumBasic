using System.Globalization;
using System.Threading;
using Selenium.Tests.Internals;
using A = NUnit.Framework.Assert;
using Test = NUnit.Framework.TestAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using TestFixtureSetUp = NUnit.Framework.TestFixtureSetUpAttribute;
using TestFixtureTearDown = NUnit.Framework.TestFixtureTearDownAttribute;

namespace Selenium.Tests {

//    [TestFixture]
    class TS_Excel : BaseExcel {

        [Test]
        public void TC_ShouldReadAndWriteDataTable() { 
            addSheet();
            setValue("A1:C5", new object[,]{
                {"A", "B", "C"},
                {0, 10.7, "val0"},
                {1, 10, "val1"},
                {2, 18.7, "val2"},
                {3, 18.8, "val3"}
            });
            var dt = new Table();

            var data = dt.From("A1", true).Where("B > 11", null);
            A.AreEqual(2, dt.Count);

            addSheet();
            data.ToExcel("A1");
            A.AreEqual("A", getValue("A1"));
            A.AreEqual("C", getValue("C1"));
            A.AreEqual(3, getValue("A3"));
            A.AreEqual("val3", getValue("C3"));
        }

        [Test]
        public void TC_ShouldWriteList() {  
            addSheet();
            var target = getRange("A1");

            var list = new List();
            list.AddRange(new object[] { 665, 987897, 909 });
            list.ToExcel(target, "Mytitle", true);

            A.AreEqual("Mytitle", getValue("A1"));
            A.AreEqual(665, getValue("A2"));
            A.AreEqual(909, getValue("A4"));
            A.AreEqual(null, getValue("A5"));
        }

    }

}
