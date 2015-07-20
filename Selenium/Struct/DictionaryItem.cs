using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// DictionaryItem object
    /// </summary>
    [ProgId("Selenium.DictionaryItem")]
    [Guid("0277FC34-FD1B-4616-BB19-92FC0C76E59B")]
    [Description("Item object from a Dictionary")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [DebuggerTypeProxy(typeof(DictionaryItem.DictionaryItemDebugView))]
    [DebuggerDisplay("{Key}: {Value}")]
    public class DictionaryItem : ComInterfaces._DictionaryItem, System.Collections.IEnumerable {

        internal readonly int hash;
        internal readonly string key;
        internal object value;
        internal DictionaryItem next = null;

        internal DictionaryItem(string key, object value) {
            this.hash = key.GetHashCode();
            this.key = key;
            this.value = value;
        }

        internal DictionaryItem(int hash, string key, object value) {
            this.hash = hash;
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Dictionary key
        /// </summary>
        public string Key {
            get {
                return this.key;
            }
        }

        /// <summary>
        /// Dictionary value
        /// </summary>
        public object Value {
            get {
                return this.value;
            }
            set {
                this.value = value;
            }
        }


        /// <summary>
        /// Returns the KeyValue enumerator
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator() {
            return new DictionaryItemEnumerator(this);
        }


        /// <summary>
        /// KeyValue enumerator
        /// </summary>
        public class DictionaryItemEnumerator : System.Collections.IEnumerator {

            private DictionaryItem _dict;
            private int _index;
            private object _current;

            internal DictionaryItemEnumerator(DictionaryItem dict) {
                _dict = dict;
                _index = 0;
                _current = null;
            }

            /// <summary>
            /// Moves to the next item.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext() {
                switch (_index) {
                    case 0: _current = _dict.key; break;
                    case 1: _current = _dict.value; break;
                    default: return false;
                }
                _index++;
                return true;
            }

            /// <summary>
            /// Returns the current item.
            /// </summary>
            public object Current {
                get {
                    return _current;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position.
            /// </summary>
            public void Reset() {
                _index = 0;
                _current = null;
            }

        }

        internal class DictionaryItemDebugView {

            private DictionaryItem _item;

            public DictionaryItemDebugView(DictionaryItem item) {
                _item = item;
            }

            public string Key {
                get {
                    return _item.key;
                }
            }

            public object Value {
                get {
                    return _item.value;
                }
            }

        }

    }

}
