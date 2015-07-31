using Interop.Excel;
using Selenium.Core;
using Selenium.Internal;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Web element Table 
    /// </summary>
    /// <example>
    /// Select an element by text
    /// <code lang="vbs">	
    /// data = driver.FindElementById("tbl").AsTable.Data
    /// driver.FindElementById("tbl").AsTable.ToExcel [Sheet1!A1]
    /// </code>
    /// </example>
    [ProgId("Selenium.TableElement")]
    [Guid("0277FC34-FD1B-4616-BB19-851CEFB2F7B6")]
    [Description("Table web element")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class TableElement : ComInterfaces._TableElement {

        private RemoteSession _session;
        private WebElement _element;

        internal TableElement(RemoteSession session, WebElement element) {
            _session = session;
            _element = element;
            var tag = _element.TagName.ToLowerInvariant();
            if ("table" != tag && "tbody" != tag)
                throw new Errors.UnexpectedTagNameError("table,tbody", tag);
        }

        /// <summary>
        /// Get the data from an HTML table
        /// </summary>
        /// <param name="firstRowsToSkip">First row(s) to skip. Ex : 2 will skip the first two rows</param>
        /// <param name="lastRowsToSkip">Last row(s) to skip. Ex : 2 will skip the last two rows</param>
        /// <returns>Excel array</returns>
        public object[,] Data(int firstRowsToSkip = 0, int lastRowsToSkip = 0) {
            const string script = "return (function(a){for(var d=[],b=0,g=a.length;b<g;b++){for(var e=a[b].cells,f=[],c=0,h=e.length;c<h;c++)f.push(e[c].textContent);d.push(f)}return d;})(arguments[0].rows);";
            object response = _session.javascript.Execute(script, new[] { _element }, false);
            ICollection data = response as ICollection;
            if (data == null)
                return null;
            int dim1 = data.Count;
            int dim2 = 0;
            int lastRow = dim1 - lastRowsToSkip;
            int firstRow = firstRowsToSkip + 1;
            object[,] table = null;
            int r = 0, c;
            foreach (ICollection row in data) {
                r++;
                if (r < firstRow || r > lastRow)
                    continue;
                if (table == null) {
                    dim2 = row.Count;
                    table = (object[,])Array.CreateInstance(typeof(object), new[] { dim1, dim2 }, new[] { 1, 1 });
                }
                c = 0;
                foreach (string cell in row)
                    table[r, ++c] = (object)cell.Trim();
            }
            return table;
        }

        /// <summary>
        /// Copies the values to Excel. The target can be an address, a worksheet or a range.
        /// </summary>
        /// <param name="target">Excel address, worksheet or range.</param>
        /// <param name="clear">Optional - If true, the cells will be cleared first</param>
        /// <example>
        /// Dim lst As New List
        /// lst.Add 43
        /// lst.ToExcelRange Range("Sheet1!A1")
        /// </example>
        public void ToExcel(object target, bool clear = false) {
            IRange range = ExcelExt.GetRange(target);
            if (clear)
                range.CurrentRegion.Clear();
            object[,] values = Data();
            int rowlen = values.GetLength(0);
            int collen = values.GetLength(1);
            range[values, collen].Value2 = values;
        }

    }

}
