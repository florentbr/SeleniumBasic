using Interop.Excel;
using Selenium.Internal;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Represents a collection of keys and values.
    /// Light dictionary using chaining disigned for small list of items. 
    /// Complexity: O(n)
    /// </summary>
    [ProgId("Selenium.Dictionary")]
    [Guid("0277FC34-FD1B-4616-BB19-CEA7D8FD6954")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [DebuggerTypeProxy(typeof(Dictionary.DictionaryDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    public class Dictionary : ComInterfaces._Dictionary, System.Collections.IEnumerable {

        DictionaryItem _head;
        internal int _count = 0;

        /// <summary>
        /// Creates a new Dictionary object
        /// </summary>
        public Dictionary() {

        }

        /// <summary>
        /// Creates a new Dictionary object
        /// </summary>
        /// <param name="d">System dictionary</param>
        public Dictionary(System.Collections.Generic.IDictionary<string, object> d) {
            DictionaryItem last = null;
            foreach (System.Collections.Generic.KeyValuePair<string, object> item in d) {
                DictionaryItem newNode = new DictionaryItem(item.Key, item.Value);
                if (last == null) {
                    _head = last = newNode;
                } else {
                    last.next = newNode;
                    last = newNode;
                }
            }
            _count = d.Count;
        }

        /// <summary>
        /// Adds a new value to the dictionary
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, object value) {
            int hash = key.GetHashCode();

            DictionaryItem last = null;
            DictionaryItem node;
            for (node = _head; node != null; node = node.next) {
                if (hash == node.hash && key.Equals(node.key))
                    throw new ArgumentException("Duplicated key");
                last = node;
            }

            DictionaryItem newNode = new DictionaryItem(hash, key, value);
            _count++;
            if (last == null) {
                _head = newNode;
            } else {
                last.next = newNode;
            }
        }

        /// <summary>
        /// Returns the number of items
        /// </summary>
        public int Count {
            get {
                return _count;
            }
        }

        /// <summary>
        /// Clears the dictionary
        /// </summary>
        public void Clear() {
            _head = null;
            _count = 0;
        }

        /// <summary>
        /// Returns the value for the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public object this[string key] {
            get {
                int hash = key.GetHashCode();
                for (DictionaryItem node = _head; node != null; node = node.next) {
                    if (hash == node.hash && key.Equals(node.key))
                        return node.value;
                }
                throw new Errors.KeyNotFoundError(key);
            }
            set {
                this.Set(key, value);
            }
        }

        /// <summary>
        /// Sets the value for the given key. The key is created if missing.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value) {
            int hash = key.GetHashCode();

            DictionaryItem last = null;
            DictionaryItem node;
            for (node = _head; node != null; node = node.next) {
                if (hash == node.hash && key.Equals(node.key)) {
                    node.value = value;
                    return;
                }
                last = node;
            }

            DictionaryItem newNode = new DictionaryItem(hash, key, value);
            if (last == null) {
                _head = newNode;
            } else {
                last.next = newNode;
            }
            _count++;
        }

        /// <summary>
        /// Returns the value associated with the specified key or the default value if the key doesn't exist.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object Get(string key, object defaultValue) {
            int hash = key.GetHashCode();
            for (DictionaryItem node = _head; node != null; node = node.next) {
                if (hash == node.hash && key.Equals(node.key))
                    return node.value;
            }
            return defaultValue;
        }

        internal bool TryGetValue<T>(string key, out T value) {
            int hash = key.GetHashCode();
            for (DictionaryItem node = _head; node != null; node = node.next) {
                if (hash == node.hash && key.Equals(node.key)) {
                    value = (T)node.value;
                    return true;
                }
            }
            value = default(T);
            return false;
        }


        /// <summary>
        /// Moves an item to a target dictionary.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="target">Target dictionary</param>
        /// <returns>True if the key was found, false otherwise</returns>
        internal bool TryMove(string key, Dictionary target) {
            int hash = key.GetHashCode();
            DictionaryItem last = null;
            for (DictionaryItem node = _head; node != null; node = node.next) {
                if (hash == node.hash && key.Equals(node.key)) {
                    if (last == null) {
                        _head = node.next;
                    } else {
                        last.next = node.next;
                    }
                    target.Add(key, node.value);
                    return true;
                }
                last = node;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the key is present, false otherwise.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public bool ContainsKey(string key) {
            int hash = key.GetHashCode();
            for (DictionaryItem node = _head; node != null; node = node.next) {
                if (hash == node.hash && key.Equals(node.key))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the value for the given key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if succeed, false otherwise</returns>
        public bool Remove(string key) {
            int hash = key.GetHashCode();
            DictionaryItem last = null;
            for (DictionaryItem node = _head; node != null; node = node.next) {
                if (hash == node.hash && key.Equals(node.key)) {
                    if (last == null) {
                        _head = node.next;
                    } else {
                        last.next = node.next;
                    }
                    return true;
                }
                last = node;
            }
            return false;
        }

        /// <summary>
        /// Returms an array containing the keys
        /// </summary>
        /// <returns></returns>
        public string[] Keys {
            get {
                string[] items = new string[_count];
                int i = 0;
                for (DictionaryItem node = _head; node != null; node = node.next) {
                    items[i++] = node.key;
                }
                return items;
            }
        }

        string[] ComInterfaces._Dictionary.Keys() {
            return this.Keys;
        }

        /// <summary>
        /// Returms an array containing the values
        /// </summary>
        /// <returns></returns>
        public object[] Values {
            get {
                object[] items = new object[_count];
                int i = 0;
                for (DictionaryItem node = _head; node != null; node = node.next) {
                    items[i++] = node.value;
                }
                return items;
            }
        }

        /// <summary>
        /// Items
        /// </summary>
        public DictionaryItem[] Items {
            get {
                DictionaryItem[] items = new DictionaryItem[_count];
                int i = 0;
                for (DictionaryItem node = _head; node != null; node = node.next) {
                    items[i++] = node;
                }
                return items;
            }
        }

        object[] ComInterfaces._Dictionary.Values() {
            return this.Values;
        }

        /// <summary>
        /// Writes the values in an Excel sheet.
        /// </summary>
        /// <param name="target">Excel address, worksheet, range or null to create a new sheet.</param>
        /// <param name="clearFirst">Optional - If true, clears the cells first</param>
        public void ToExcel(object target = null, bool clearFirst = false) {
            IRange rg = ExcelExt.GetRange(target);
            if (clearFirst)
                rg[rg.SpecialCells(XlCellType.xlCellTypeLastCell).Row, 1].Clear();

            if (_count == 0)
                return;

            var values = new object[_count, 2];

            int i = 0;
            for (DictionaryItem node = _head; node != null; node = node.next, i++) {
                values[i, 0] = node.key;
                values[i, 1] = node.value;
            }

            rg[_count, 2].Value2 = values;
        }

        /// <summary>
        /// Explicite casting to a Dictionary&lt;string, object&gt; type
        /// </summary>
        /// <param name="d">Dictionary</param>
        /// <returns></returns>
        public static explicit operator System.Collections.Generic.Dictionary<string, object>(Dictionary d) {
            var dict = new System.Collections.Generic.Dictionary<string, object>(d.Count);
            for (DictionaryItem node = d._head; node != null; node = node.next) {
                dict.Add(node.key, node.value);
            }
            return dict;
        }

        /// <summary>
        /// Explicite casting from a Dictionary&lt;string, object&gt; type
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static explicit operator Dictionary(System.Collections.Generic.Dictionary<string, object> d) {
            return new Dictionary(d);
        }

        /// <summary>
        /// Return the dictionary enumerator
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator() {
            return new DictionaryEnumerator(_head);
        }

        System.Collections.IEnumerator ComInterfaces._Dictionary._NewEnum() {
            return GetEnumerator();
        }

        /// <summary>
        /// Dictionary enumerator
        /// </summary>
        public class DictionaryEnumerator : System.Collections.IEnumerator {

            private readonly DictionaryItem _head;
            private DictionaryItem _current;

            internal DictionaryEnumerator(DictionaryItem node) {
                _head = node;
                _current = null;
            }

            /// <summary>
            /// Moves to the next item
            /// </summary>
            /// <returns></returns>
            public bool MoveNext() {
                if (_current == null) {
                    _current = _head;
                    return _current != null;
                }
                if (_current.next == null)
                    return false;

                _current = _current.next;
                return true;
            }

            /// <summary>
            /// Returns the current item
            /// </summary>
            public DictionaryItem Current {
                get {
                    return _current;
                }
            }

            /// <summary>
            /// Returns the current item
            /// </summary>
            object System.Collections.IEnumerator.Current {
                get {
                    return _current;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position
            /// </summary>
            public void Reset() {
                _current = null;
            }

        }


        internal class DictionaryDebugView {
            private Dictionary _dict;

            public DictionaryDebugView(Dictionary dictionary) {
                _dict = dictionary;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public DictionaryItem[] Items {
                get {
                    return _dict.Items;
                }
            }

        }

    }

}
