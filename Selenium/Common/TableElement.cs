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
        /// <param name="map">Optional - Javascript code to scrap each cell. Default: (e)=>e.textContent.trim()</param>
        /// <returns>Excel array</returns>
        public object[,] Data(int firstRowsToSkip = 0, int lastRowsToSkip = 0, string map = null) {
            const string JS_SCRAP_TABLE =
                "var rows=arguments[0].rows, values=[], columns=[];" +
                "for(var r=0; r<rows.length; r++){" +
                " for(var c=0, cells=rows[r].cells; c<cells.length; c++)" +
                "  values.push(map.call(cells[c], cells[c]));" +
                " columns.push(cells.length);" +
                "}" +
                "return {values: values, columns: columns};";

            // build the map function applied to each cell
            string js_map;
            if (string.IsNullOrEmpty(map)) {
                js_map = "var map=function(e){return e.textContent.trim()};";
            } else if (map.StartsWith("return")) {
                js_map = "var map=function(){" + map + "};";
            } else {
                js_map = "var map=" + map + ";";
            }

            // execute the script
            var response = (Dictionary)_session.javascript.Execute(js_map + JS_SCRAP_TABLE,
                                                                   new[] { _element },
                                                                   false);
            List values = (List)response["values"];
            List columns = (List)response["columns"];
            int rows_count = columns.Count;

            // find the maximum number of columns
            int cols_count = 0;
            for (int i = 0; i < rows_count; i++) {
                if ((int)columns[i] > cols_count)
                    cols_count = (int)columns[i];
            }

            // converts to a 2dim array
            object[,] table = (object[,])Array.CreateInstance(typeof(object),
                                                              new[] { rows_count, cols_count },
                                                              new[] { 1, 1 });
            int j = 0;
            for (int r = 1; r <= rows_count; r++) {
                int clen = (int)columns[r - 1];
                for (int c = 1; c <= clen; c++) {
                    table[r, c] = values[j++];
                }
            }

            return table;
        }

        /// <summary>
        /// Copies the values to Excel. The target can be an address, a worksheet or a range.
        /// </summary>
        /// <param name="target">Optional - Excel address, worksheet or range. New sheet if null</param>
        /// <param name="clear">Optional - If true, the cells will be cleared first</param>
        /// <param name="firstRowsToSkip">First row(s) to skip. Ex : 2 will skip the first two rows</param>
        /// <param name="lastRowsToSkip">Last row(s) to skip. Ex : 2 will skip the last two rows</param>
        /// <param name="map">Optional - Javascript code to scrap each cell. Default: (e)=>e.textContent.trim()</param>
        /// <example>
        /// Dim lst As New List
        /// lst.Add 43
        /// lst.ToExcel [Sheet1!A1]
        /// </example>
        public void ToExcel(object target = null, bool clear = false
            , int firstRowsToSkip = 0, int lastRowsToSkip = 0, string map = null) {

            IRange range = ExcelExt.GetRange(target);
            if (clear)
                range.CurrentRegion.Clear();
            object[,] values = Data(firstRowsToSkip, lastRowsToSkip, map);
            int rowlen = values.GetLength(0);
            int collen = values.GetLength(1);
            range[rowlen, collen].Value2 = values;
        }

    }

}
