using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Selenium.Internal {

    class COMRunningObject : IDisposable {

        private int _obj_id;

        /// <summary>
        /// Register a COM instance in the running table
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <param name="name">Registration name</param>
        /// <returns></returns>
        public COMRunningObject(object instance, string name) {
            IRunningObjectTable rot;
            if (Native.GetRunningObjectTable(0, out rot) != 0)
                return;

            IMoniker moniker;
            int hresult = Native.CreateFileMoniker(name, out moniker);
            if (hresult == 0) {
                _obj_id = rot.Register(Native.ROTFLAGS_REGISTRATIONKEEPSALIVE
                    , instance, moniker);
                Marshal.ReleaseComObject(moniker);
            }

            Marshal.ReleaseComObject(rot);
        }

        public void Dispose() {
            if (_obj_id == 0)
                return;

            try {
                Native.RevokeActiveObject(_obj_id, IntPtr.Zero);
            } catch { }

            _obj_id = 0;
        }


        class Native {

            const string OLE32 = "ole32.dll";
            const string OLEAUT32 = "oleaut32.dll";

            public const int ROTFLAGS_REGISTRATIONKEEPSALIVE = 1;

            [DllImport(OLE32)]
            public static extern int GetRunningObjectTable(
                int reserved, out IRunningObjectTable prot);

            [DllImport(OLE32)]
            public static extern int CreateFileMoniker(
                [MarshalAs(UnmanagedType.LPWStr)] string lpszPathName, out IMoniker ppmk);

            [DllImport(OLEAUT32)]
            public static extern int RevokeActiveObject(int handle, IntPtr reserved);
        }

    }

}
