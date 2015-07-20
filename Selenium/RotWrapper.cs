using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium {

    class RotWrapper : IDisposable {
        /// <summary>
        /// Returns a pointer to the IRunningObjectTable
        /// interface on the local running object table (ROT).
        /// </summary>
        /// <param name="reserved">This parameter is reserved and must be 0.</param>
        /// <param name="prot">The address of an IRunningObjectTable* pointer variable
        /// that receives the interface pointer to the local ROT. When the function is
        /// successful, the caller is responsible for calling Release on the interface
        /// pointer. If an error occurs, *pprot is undefined.</param>
        /// <returns>This function can return the standard return values E_UNEXPECTED and S_OK.</returns>
        [DllImport("ole32.dll")]
        static extern int GetRunningObjectTable(uint reserved, out System.Runtime.InteropServices.ComTypes.IRunningObjectTable pprot);

        [DllImport("ole32.dll")]
        static extern int CreateFileMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszPathName, out System.Runtime.InteropServices.ComTypes.IMoniker ppmk);

        [DllImport("oleaut32.dll")]
        static extern int RevokeActiveObject(int register, IntPtr reserved);

        public int RegisterObject(object obj, string stringId) {
            int regId = -1;

            System.Runtime.InteropServices.ComTypes.IRunningObjectTable pROT = null;
            System.Runtime.InteropServices.ComTypes.IMoniker pMoniker = null;

            int hr;

            if ((hr = GetRunningObjectTable((uint)0, out pROT)) != 0) {
                return (hr);
            }

            // File Moniker has to be used because in VBS GetObject only works with file monikers in the ROT
            if ((hr = CreateFileMoniker(stringId, out pMoniker)) != 0) {

                return hr;
            }

            int ROTFLAGS_REGISTRATIONKEEPSALIVE = 1;
            regId = pROT.Register(ROTFLAGS_REGISTRATIONKEEPSALIVE, obj, pMoniker);

            _RegisteredObjects.Add(new ObjectInRot(obj, regId));

            return 0;
        }


        class ObjectInRot {
            public ObjectInRot(object obj, int regId) {
                this.obj = obj;
                this.regId = regId;
            }
            public object obj;
            public int regId;
        };
        List<ObjectInRot> _RegisteredObjects = new List<ObjectInRot>();

        #region IDisposable Members

        public void Dispose() {
            foreach (ObjectInRot obj in _RegisteredObjects) {
                RevokeActiveObject(obj.regId, IntPtr.Zero);
            }
            _RegisteredObjects.Clear();
        }

        #endregion
    }
}
