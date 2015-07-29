using Interop.Excel;
using Selenium.Internal;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// DataTable object
    /// </summary>
    [ProgId("Selenium.Table")]
    [Guid("0277FC34-FD1B-4616-BB19-B719752452AA")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [DebuggerTypeProxy(typeof(Table.TableDebugView))]
    [DebuggerDisplay("Rows = {Count}")]
    public class Table : ComInterfaces._Table, IDisposable {

        private System.Data.DataTable _table;
        private System.Data.DataColumnCollection _columns;
        private System.Data.DataRowCollection _rowsall;
        private System.Data.DataRow[] _rowswhere;
        private object[] _rows;
        private ICells _cells;
        private bool _hasheaders;
        private System.Data.DataRow _cacheRow;
        private int _cacheIndex;

        /// <summary>
        /// Creates a new DataTable object.
        /// </summary>
        public Table() {
            _table = null;
            _rowsall = null;
            _rowswhere = null;
            _cells = null;
        }

        /// <summary>
        /// Release the resources.
        /// </summary>
        public void Dispose() {
            _table.Dispose();
            _table = null;
            _rowsall = null;
            _rowswhere = null;
            _cells = null;
        }

        /// <summary>
        /// Gets the row at the the given index (One based).
        /// </summary>
        /// <param name="index">Index starting at one.</param>
        /// <returns><see cref="TableRow"/></returns>
        public object this[int index] {
            get {
                try {
                    if (_rowswhere != null)
                        return new TableRow(this, _rowswhere[index - 1]);
                    return new TableRow(this, _rowsall[index - 1]);
                } catch (IndexOutOfRangeException) {
                    throw new Errors.ArgumentError("Index is out of range (One based).");
                }
            }
        }

        /// <summary>
        /// Loads the data from the source. Can be an excel address or a range.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hasHeaders"></param>
        /// <returns><see cref="Table"/></returns>
        public Table From(object source, bool hasHeaders = true) {
            try {
                IRange range = ExcelExt.GetRange(source);
                if (range.Count == 1)
                    range = range.CurrentRegion;

                var values = (object[,])range.Value2 ?? new object[1, 1];
                int clen = values.GetLength(1);
                int rlen = values.GetLength(0);

                _table = new System.Data.DataTable();
                _columns = _table.Columns;
                int irow = hasHeaders ? 2 : 1;
                for (var icol = 0; icol++ < clen; ) {
                    Type coltype = GetColumnType(values, irow, icol);

                    object colvalue = hasHeaders ?
                        values[1, icol]
                        : (icol + 1);

                    string colname = colvalue == null ?
                        string.Empty
                        : colvalue.ToString();

                    _columns.Add(new System.Data.DataColumn(colname, coltype));
                }

                _rowsall = _table.Rows;
                for (int r = hasHeaders ? 1 : 0; r++ < rlen; ) {
                    System.Data.DataRow row = _table.NewRow();
                    for (var c = 0; c < clen; c++)
                        row[c] = values[r, c + 1];

                    _rowsall.Add(row);
                }

                _hasheaders = hasHeaders;
                _cells = (ICells)range;
                return this;
            } catch (SeleniumException) {
                throw;
            } catch (Exception ex) {
                throw new SeleniumException(ex);
            }
        }

        /// <summary>
        /// Filter the rows using the given SQL like expression.
        /// </summary>
        /// <param name="filterExpression">SQL like expression</param>
        /// <param name="sortBy">Column name followed by ASC or DESC</param>
        /// <returns><see cref="Table"/></returns>
        public Table Where(string filterExpression, string sortBy = null) {
            try {
                _rowswhere = _table.Select(filterExpression, sortBy);
            } catch (Exception ex) {
                throw new FilterExpressionError("{0}\n\"{1}\", \"{2}\"", ex.Message, filterExpression, sortBy);
            }
            _rows = null;
            return this;
        }

        /// <summary>
        /// Returns the number of rows
        /// </summary>
        public int Count {
            get {
                if (_rowswhere != null)
                    return _rowswhere.Length;
                return _rowsall.Count;
            }
        }

        /// <summary>
        /// Returns the values in a 2dim array.
        /// </summary>
        public object[,] Values() {
            var rows = _rowswhere ?? _rowsall as ICollection;
            var rlen = rows.Count;
            var clen = _columns.Count;
            var values = new object[rlen + 1, clen];

            for (int c = 0; c < clen; c++)
                values[0, c] = _columns[c].ColumnName;

            var r = 1;
            foreach (System.Data.DataRow row in rows) {
                var items = row.ItemArray;
                for (int c = 0; c < clen; c++)
                    values[r, c] = items[c];
                r++;
            }
            return values;
        }

        /// <summary>
        ///  Copies the values to Excel. The target can be an address, a worksheet or a range.
        /// </summary>
        /// <param name="target">Excel address, worksheet or range or null to create a new sheet</param>
        /// <param name="clearFirst">Optional - If true, clears the cells first</param>
        public void ToExcel(object target = null, bool clearFirst = false) {
            IRange range = ExcelExt.GetRange(target);
            if (clearFirst) {
                var lastCell = range.SpecialCells(XlCellType.xlCellTypeLastCell);
                range[lastCell.Row, lastCell.Column].Clear();
            }
            var values = this.Values();
            var rlen = values.GetLength(0);
            var clen = values.GetLength(1);
            range[rlen, clen].Value2 = values;
        }

        #region Internal support nethods

        IEnumerator ComInterfaces._Table._NewEnum() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// For Each enumerator
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator() {
            if (_rows == null) {
                var rowssys = _rowswhere ?? _rowsall as ICollection;
                var cnt = rowssys.Count;
                _rows = new TableRow[cnt];
                int i = 0;
                foreach (var rowsys in rowssys)
                    _rows[i++] = new TableRow(this, (System.Data.DataRow)rowsys);
            }
            return _rows.GetEnumerator();
        }

        private Type GetColumnType(object[,] values, int firstrow, int column) {
            int rlen = values.GetLength(0);
            for (int r = firstrow; r < rlen; r++) {
                object value = values[r, column];
                if (value == null || value is double)
                    continue;
                return typeof(string);
            }
            return typeof(object);
        }

        internal object GetCell(int row, int column) {
            row += _hasheaders ? 2 : 1; //to base 1
            column += 1; //to base 1
            return _cells[row, column];
        }

        internal void SetCell(System.Data.DataRow datarow, int column, object value) {
            if (_cells == null)
                return;
            int row;
            if (datarow == _cacheRow) {
                row = _cacheIndex;
            } else {
                row = _rowsall.IndexOf(datarow);
                row += _hasheaders ? 2 : 1;   //convert to base 1
                _cacheRow = datarow;
                _cacheIndex = row;
            }
            column += 1;    //convert to base 1
            ((IRange)_cells[row, column]).Value2 = value;
        }

        #endregion


        internal class TableDebugView {
            private Table _table;

            public TableDebugView(Table table) {
                _table = table;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public object[,] Items {
                get {
                    return _table.Values();
                }
            }
        }

    }

    class FilterExpressionError : SeleniumError {
        public FilterExpressionError(string message, params object[] args)
            : base(55, message, args) { }
    }

}
