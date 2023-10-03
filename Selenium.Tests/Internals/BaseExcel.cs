using System;
using System.Globalization;
using System.Threading;
using OneTimeSetUp=NUnit.Framework.TestFixtureSetUpAttribute;
using OneTimeTearDown=NUnit.Framework.TestFixtureTearDownAttribute;

namespace Selenium.Tests.Internals {

    abstract class BaseExcel {

        protected COM excel;
        protected COM workbooks;
        protected COM workbook;
        protected COM worksheets;
        protected COM worksheet;

        [OneTimeSetUp]
        public void TestFixtureSetUp() {

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");   
            var type = Type.GetTypeFromProgID("Excel.Application");
            var instance = Activator.CreateInstance(type);
            excel = new COM(instance);
            //_xl.set("Visible", true);
            workbooks = excel.get("Workbooks");
            workbook = workbooks.invoke("Add", Type.Missing);
            worksheets = workbook.get("Sheets");
            worksheet = workbook.get("ActiveSheet");
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown() {
            workbook.invoke("Close", false, Type.Missing, Type.Missing);
            excel.invoke("Quit");
            worksheet.Dispose();
            workbook.Dispose();
            workbooks.Dispose();
            excel.Dispose();
            foreach (var p in System.Diagnostics.Process.GetProcessesByName("excel")) {
                p.Kill();
                p.Dispose();
            }
        }

        public void addSheet() {
            worksheet = worksheets.invoke("Add", Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        public void setValue(string range, object[,] values) {
            var rg = worksheet.get("Range", range, Type.Missing);
            rg.set("Value", values);
        }

        public object getValue(string range) {
            var rg = worksheet.get("Range", range, Type.Missing);
            return rg.get("Value").Value;
        }

        public object getRange(string range) {
            return worksheet.get("Range", range, Type.Missing).Value;
        }

    }

}
