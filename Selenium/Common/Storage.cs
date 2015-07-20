using Selenium.Core;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Storage object
    /// </summary>
    [ProgId("Selenium.Storage")]
    [Guid("0277FC34-FD1B-4616-BB19-4425731F2305")]
    [Description("Storage object")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Storage : ComInterfaces._Storage {

        internal enum StorageType {
            SessionStorage,
            LocalStorage
        }

        private readonly RemoteSession _session;
        private readonly string _uri;

        /// <summary>
        /// </summary>
        /// <param name="session"></param>
        /// <param name="storageType"></param>
        internal Storage(RemoteSession session, StorageType storageType) {
            _session = session;
            _uri = storageType == StorageType.LocalStorage ? "/local_storage" : "/session_storage";
        }

        /// <summary>
        /// Get or set the storage item for the given key.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>object</returns>
        public string this[string key] {
            get {
                return (string)_session.Send(RequestMethod.GET, _uri + "/key/" + key);
            }
            set {
                this.Set(key, value);
            }
        }

        /// <summary>
        /// Sets the storage item
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Set(string key, string value) {
            _session.Send(RequestMethod.POST, _uri, "key", key, "value", value);
        }

        /// <summary>
        /// Get all keys of the storage.
        /// </summary>
        /// <returns>The list of keys.</returns>
        public List Keys() {
            return (List)_session.Send(RequestMethod.GET, _uri);
        }

        /// <summary>
        /// Get the number of items in the storage.
        /// </summary>
        /// <returns>{number} The number of items in the storage.</returns>
        public int Count() {
            return (int)_session.Send(RequestMethod.GET, _uri + "/size");
        }

        /// <summary>
        /// Clear the storage.
        /// </summary>
        public void Clear() {
            _session.Send(RequestMethod.DELETE, _uri);
        }

        /// <summary>
        /// Remove the storage item for the given key.
        /// </summary>
        /// <param name="key">Key of the item</param>
        public void Remove(string key) {
            _session.Send(RequestMethod.DELETE, _uri + "/key/" + key);
        }

    }

}
