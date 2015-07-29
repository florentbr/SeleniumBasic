using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Selenium.Internal {

    class RunningObject : IDisposable {

        /// <summary>
        /// Register a COM instance in the running table
        /// </summary>
        /// <param name="name">Regitered name</param>
        /// <param name="instance">Object instance</param>
        /// <returns></returns>
        public static RunningObject Register(string name, object instance) {
            IRunningObjectTable table;
            if (NativeMethods.GetRunningObjectTable(0, out table) != 0)
                return null;

            IMoniker moniker;
            if (NativeMethods.CreateFileMoniker(name, out moniker) != 0)
                return null;

            int id = table.Register(1, instance, moniker);
            if (id == 0)
                return null;
            return new RunningObject(id);
        }


        private int _id;

        public RunningObject(int id) {
            _id = id;
        }

        ~RunningObject() {
            this.Dispose();
        }

        public void Dispose() {
            if (_id == 0)
                return;
            NativeMethods.RevokeActiveObject(_id, (IntPtr)0);
        }


        class NativeMethods {

            const string OLE32 = "ole32.dll";
            const string OLEAUT32 = "oleaut32.dll";

            [DllImport(OLE32)]
            public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

            [DllImport(OLE32)]
            public static extern int CreateFileMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszPathName
                , out IMoniker ppmk);

            [DllImport(OLEAUT32)]
            public static extern int RevokeActiveObject(int register, IntPtr reserved);

        }

    }

}
