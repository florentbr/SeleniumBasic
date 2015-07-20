using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// DataRow object
    /// </summary>
    [ProgId("Selenium.TableRow")]
    [Guid("0277FC34-FD1B-4616-BB19-760FA0360E53")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class TableRow : ComInterfaces._TableRow {

        private System.Data.DataRow _datarow;
        private Table _datatable;

        internal TableRow(Table datatable, System.Data.DataRow datarow) {
            _datatable = datatable;
            _datarow = datarow;
        }

        /// <summary>
        /// Gets or sets the value associated with the column identifier
        /// </summary>
        /// <param name="identifier">Name of the colum or index</param>
        /// <returns></returns>
        public object this[object identifier] {
            get {
                int c = GetColumn(identifier);
                return _datarow[c];
            }
            set {
                int c = GetColumn(identifier);
                _datarow[c] = value;
                _datatable.SetCell(_datarow, c, value);
            }
        }

        /// <summary>
        /// Gets the value associated with the column identifier
        /// </summary>
        /// <param name="identifier">Name of the colum or index</param>
        /// <returns></returns>
        object ComInterfaces._TableRow.Get(object identifier) {
            return this[identifier];
        }

        /// <summary>
        /// Sets the value associated with the column identifier
        /// </summary>
        /// <param name="identifier">Name of the colum or index</param>
        /// <param name="value">Value to set</param>
        void ComInterfaces._TableRow.Set(object identifier, object value) {
            this[identifier] = value;
        }

        /// <summary>
        /// Gets the Excel range if any asscociated with the column identifier
        /// </summary>
        /// <param name="identifier">Name of the colum or index</param>
        /// <returns></returns>
        public object Cell(object identifier) {
            var table = _datarow.Table;
            int r = table.Rows.IndexOf(_datarow);
            int c;
            if (identifier is string) {
                c = table.Columns[(string)identifier].Ordinal;
            } else if (identifier is Int16) {
                c = (short)identifier - 1;
            } else if (identifier is Int32) {
                c = (int)identifier - 1;
            } else {
                throw new Errors.ArgumentError("Invalid data type for argument identifier. Allowed: string, integer");
            }
            return _datatable.GetCell(r, c);
        }

        /// <summary>
        /// Gets an array containing all the values of the row
        /// </summary>
        public object Values {
            get {
                return _datarow.ItemArray;
            }
        }

        #region Internal support methods

        private int GetColumn(object identifier) {
            if (identifier is string)
                return _datarow.Table.Columns[(string)identifier].Ordinal;

            if (identifier is Int16)
                return (int)(Int16)identifier;

            if (identifier is int)
                return (int)identifier;

            throw new Errors.ArgumentError("Invalid data type for argument identifier. Allowed: string, integer");
        }

        #endregion

    }

}
