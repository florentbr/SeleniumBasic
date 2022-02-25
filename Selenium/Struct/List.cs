using Interop.Excel;
using Selenium.Internal;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Represents a collection of values.
    /// </summary>
    [ProgId("Selenium.List")]
    [Guid("0277FC34-FD1B-4616-BB19-5DB46A739EEA")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [DebuggerTypeProxy(typeof(List.ListDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    public class List : ComInterfaces._List, System.Collections.IList, System.Collections.IEnumerable {

        private const int DEFAULT_CAPACITY = 4;
        const int CAPACITY_MULTIPLIER = 2;

        internal object[] _items;
        internal int _count = 0;

        /// <summary>
        /// Creates a new list object
        /// </summary>
        public List() {
            _items = new object[DEFAULT_CAPACITY];
        }

        /// <summary>
        /// Creates a new list object
        /// </summary>
        /// <param name="capacity">Capacity</param>
        public List(int capacity) {
            _items = new object[capacity];
        }

        /// <summary>
        /// Creates a new list object
        /// </summary>
        /// <param name="list"></param>
        public List(Array list) {
            _items = new object[list.Length];
            Array.Copy(list, _items, list.Length);
            _count = list.Length;
        }

        /// <summary>
        /// Returns the number of itens
        /// </summary>
        public int Count {
            get {
                return _count;
            }
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear() {
            Array.Clear(_items, 0, _count);
        }

        /// <summary>
        /// Return the item at index
        /// </summary>
        /// <param name="index">Zero based index</param>
        /// <returns></returns>
        public object this[int index] {
            get {
                return _items[index];
            }
            set {
                _items[index] = value;
            }
        }

        object ComInterfaces._List.this[int index] {
            get {
                return _items[index - 1];
            }
            set {
                _items[index - 1] = value;
            }
        }

        void ComInterfaces._List.Set(int index, object value) {
            _items[index - 1] = value;
        }

        object ComInterfaces._List.Get(int index) {
            return _items[index - 1];
        }

        /// <summary>
        /// Returns the List enumerator
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator() {
            return new ListEnumerator(_items, _count);
        }

        System.Collections.IEnumerator ComInterfaces._List._NewEnum() {
            return GetEnumerator();
        }

        /// <summary>
        ///  Adds a value in the list
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Index</returns>
        public int Add(object value) {
            if (_count == _items.Length) {
                IncreaseSize(_count * CAPACITY_MULTIPLIER);
            }
            _items[_count] = value;
            return _count++;
        }

        int ComInterfaces._List.Add(object value) {
            Add(value);
            return _count;
        }

        /// <summary>
        /// Adds multiple values at once in the list
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(object[] values) {
            int newSize = _count + values.Length;
            if (newSize > _items.Length) {
                IncreaseSize(newSize);
            }
            Array.Copy(values, 0, _items, _count, values.Length);
            _count = newSize;
        }

        internal void IncreaseSize(int size) {
            size = Math.Max(DEFAULT_CAPACITY, size);
            object[] newItems = new object[size];
            Array.Copy(_items, 0, newItems, 0, _count);
            _items = newItems;
        }

        /// <summary>
        /// Returns the first item
        /// </summary>
        public object First() {
            return _items[0];
        }

        /// <summary>
        /// Returns the last item
        /// </summary>
        public object Last() {
            return _items[_count - 1];
        }

        /// <summary>
        /// Returns an array with the values
        /// </summary>
        public object[] ToArray() {
            return this.Values();
        }

        /// <summary>
        /// Returns an array with the values
        /// </summary>
        public object[] Values() {
            object[] items = new object[_count];
            Array.Copy(_items, 0, items, 0, _count);
            return items;
        }

        /// <summary>
        /// Copies the values to Excel. The target can be an address, a worksheet or a range.
        /// </summary>
        /// <param name="target">Excel address, worksheet, range or null to create a new sheet.</param>
        /// <param name="title">Optional - Adds a title</param>
        /// <param name="clearFirst">Optional - If true, clears the cells first</param>
        /// <returns>Range</returns>
        /// <example>
        /// Dim lst As New List
        /// lst.Add 43
        /// lst.ToExcel [Sheet1!A1]
        /// </example>
        public IRange ToExcel(object target = null, string title = null, bool clearFirst = false) {
            IRange rg = ExcelExt.GetRange(target);
            
            if (clearFirst)
                rg[rg.SpecialCells(XlCellType.xlCellTypeLastCell).Row, 1].Clear();
            
            if (_count == 0)
                return rg;

            object[,] values;
            if (title == null) {
                values = new object[_count, 1];
                for (int i = 0; i < _count; i++)
                    values[i, 0] = _items[i];
            } else {
                _count++;
                values = new object[_count, 1];
                values[0, 0] = title;
                for (int i = 1; i < _count; i++)
                    values[i, 0] = _items[i - 1];
            }

            rg = rg[_count, 1];     //Resize range
            rg.Value2 = values;
            return rg;
        }

        /// <summary>
        /// Return true if the list contains the value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public bool Contains(object value) {
            for (int i = 0; i < _count; i++) {
                if (_items[i] == value)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The index of the value starting from the end.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int LastIndexOf(object value) {
            int i = _count;
            while (i-- > 0 && _items[i] != value) ;
            return i;
        }

        /// <summary>
        /// Returns the index of the value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public int IndexOf(object value) {
            int i = -1;
            while (++i < _count && _items[i] != value) ;
            return i == _count ? -1 : i;
        }

        /// <summary>
        /// Insert a value at index
        /// </summary>
        /// <param name="index">Index (Zero based)</param>
        /// <param name="value">Value</param>
        public void Insert(int index, object value) {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();
            int newSize = _count++;
            if (newSize > _items.Length) {
                object[] newItems = new object[newSize * 2];
                Array.Copy(_items, 0, newItems, 0, index);
                Array.Copy(_items, index, newItems, index + 1, _count - index);
                _items = newItems;
            } else {
                Array.Copy(_items, index, _items, index + 1, _count - index);
            }
            _items[index] = value;
        }

        void ComInterfaces._List.Insert(int index, object value) {
            Insert(index - 1, value);
        }

        /// <summary>
        /// Not available
        /// </summary>
        public bool IsFixedSize {
            get {
                return false;
            }
        }

        /// <summary>
        /// Not available
        /// </summary>
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <summary>
        /// Remove the value
        /// </summary>
        /// <param name="value">Value</param>
        public void Remove(object value) {
            int index = IndexOf(value);
            if (index == -1)
                throw new IndexOutOfRangeException();
            RemoveAt(index);
        }

        /// <summary>
        /// Remove the value at index
        /// </summary>
        /// <param name="index">Index (Zero based)</param>
        public void RemoveAt(int index) {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();
            if( index < _count - 1 )
                Array.Copy(_items, index + 1, _items, index, _count - index - 1);
            _items[--_count] = null;
        }

        void ComInterfaces._List.RemoveAt(int index) {
            RemoveAt(index - 1);
        }

        /// <summary>
        /// Converts each value.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="func">Conversion function</param>
        public void Convert<T>(Func<T, object> func) {
            for (int i = 0; i < _count; i++) {
                _items[i] = func((T)_items[i]);
            }
        }

        /// <summary>
        /// Copies the values to an array
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="index">Target index</param>
        public void CopyTo(Array array, int index) {
            Array.Copy(_items, 0, array, index, _count);
        }

        /// <summary>
        /// Sorts the elements
        /// </summary>
        /// <returns>Current list</returns>
        public List Sort() {
            Array.Sort(_items, 0, _count);
            return this;
        }

        /// <summary>
        /// Remove duplicates
        /// </summary>
        /// <param name="ignoreCase">Optional - Ignore case if true.</param>
        /// <returns></returns>
        public List Distinct(bool ignoreCase = false) {
            System.Collections.Hashtable table;
            if (ignoreCase) {
                table = CollectionsUtil.CreateCaseInsensitiveHashtable(_count);
            } else {
                table = new System.Collections.Hashtable(_count);
            }

            for (int i = 0; i < _count; i++) {
                object item = _items[i];
                if (item != null)
                    table[item] = null;
            }

            table.Keys.CopyTo(_items, 0);
            _count = table.Count;
            return this;
        }

        /// <summary>
        /// Retuns false
        /// </summary>
        public bool IsSynchronized {
            get {
                return false;
            }
        }

        /// <summary>
        /// Retuns null
        /// </summary>
        public object SyncRoot {
            get {
                return null;
            }
        }

        /// <summary>
        /// List enumerator
        /// </summary>
        public class ListEnumerator : System.Collections.IEnumerator {

            private object[] _items;
            private int _size;
            private int _index;
            private object _current;

            internal ListEnumerator(object[] list, int size) {
                _items = list;
                _size = size;
                _index = 0;
                _current = default(object);
            }

            /// <summary>
            /// Moves to the next item
            /// </summary>
            /// <returns></returns>
            public bool MoveNext() {
                if (_index < _size) {
                    _current = _items[_index];
                    _index++;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Returns the current item
            /// </summary>
            public object Current {
                get {
                    return _current;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position
            /// </summary>
            public void Reset() {
                _index = 0;
                _current = default(object);
            }

        }

        internal class ListDebugView {
            private List _list;

            public ListDebugView(List list) {
                _list = list;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Object[] Items {
                get {
                    return _list.ToArray();
                }
            }
        }

    }

}
