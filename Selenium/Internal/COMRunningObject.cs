using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Selenium.Internal {

    class COMRunningObject : IDisposable {

        private IMoniker _moniker;
        private IRunningObjectTable _table;
        private int _id;

        /// <summary>
        /// Register a COM instance in the running table
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <param name="name">Registration name</param>
        /// <returns></returns>
        public COMRunningObject(object instance, string name) {
            if (NativeMethods.GetRunningObjectTable(0, out _table) != 0)
                return;

            if (NativeMethods.CreateFileMoniker(name, out _moniker) != 0)
                return;

            _id = _table.Register(1, instance, _moniker);
        }

        ~COMRunningObject() {
            this.Dispose();
        }

        public void Dispose() {
            if (_id == 0)
                return;
            _table.Revoke(_id);
            _id = 0;
        }


        class NativeMethods {

            const string OLE32 = "ole32.dll";

            [DllImport(OLE32)]
            public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

            [DllImport(OLE32)]
            public static extern int CreateFileMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszPathName
                , out IMoniker ppmk);

        }

    }

}
