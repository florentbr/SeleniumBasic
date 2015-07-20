using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Selenium.Tests.Internals {

    class COM {

        public static object CreateObject(string app) {
            Type type = Type.GetTypeFromProgID(app);
            if (type == null)
                throw new Exception(string.Format("Application not registered: {0}", app));
            return Activator.CreateInstance(type);
        }

        public static void Dispose(object obj) {
            if (Marshal.IsComObject(obj)) {
                while (Marshal.ReleaseComObject(obj) > 0) ;
            }
        }

        private object _obj;
        private Type _type;

        public COM(object obj) {
            if ((_obj = obj) != null)
                _type = obj.GetType();
        }

        public object Value {
            get { 
                return _obj;
            }
        }

        public COM invoke(string method, params object[] args) {
            var result = _type.InvokeMember(method, BindingFlags.InvokeMethod, null, _obj, args);
            return new COM(result);
        }

        public COM get(string prop, params object[] args) {
            var result = _type.InvokeMember(prop, BindingFlags.GetProperty, null, _obj, args);
            return new COM(result);
        }

        public void set(string prop, params object[] args) {
            _type.InvokeMember(prop, BindingFlags.SetProperty, null, _obj, args);
        }

        public void Dispose() {
            try {
                Marshal.ReleaseComObject(_obj);
            } catch { }
        }

    }

}
