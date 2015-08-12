using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Selenium.Internal {

    /// <summary>
    /// Adds a call on the Dispose method of a COM object when the reference count reaches 0.
    /// </summary>
    /// <example>
    /// class MyClass : IMyClass, IDisposable {
    /// 
    ///     public MyClass() {
    ///         COMDisposable.Subscribe(this, typeof(IMyClass));
    ///     }
    ///     
    ///     public void Dispose() {
    ///     
    ///     }
    ///     
    /// }
    /// </example>
    class COMDisposable {

        delegate int ReleaseDelegate(IntPtr pUnk);

        static readonly int RELEASE_PTR_OFFSET = 2 * IntPtr.Size;
        static readonly Hashtable ITEMS = new Hashtable();
        static readonly ReleaseDelegate ReleaseNew = new ReleaseDelegate(Release);
        static readonly IntPtr ReleaseNewPtr = Marshal.GetFunctionPointerForDelegate(ReleaseNew);

        private ReleaseDelegate ReleaseOri;
        private int RefCountStart;

        public static void Subscribe(IDisposable instance, Type interfaceCOM) {
            IntPtr pUnk = Marshal.GetComInterfaceForObject(instance, interfaceCOM);
            IntPtr vtable = Marshal.ReadIntPtr(pUnk);
            COMDisposable item = (COMDisposable)ITEMS[vtable];
            if (item == null) {
                //gets the the Release function
                IntPtr releaseOriPtr = Marshal.ReadIntPtr(vtable, RELEASE_PTR_OFFSET);
                ReleaseDelegate releaseOri = (ReleaseDelegate)Marshal.GetDelegateForFunctionPointer(
                    releaseOriPtr, typeof(ReleaseDelegate));

                //saves the original function and reference count
                item = new COMDisposable();
                item.ReleaseOri = releaseOri;
                item.RefCountStart = releaseOri(pUnk);
                ITEMS.Add(vtable, item);

                //overrides the Release function
                Marshal.WriteIntPtr(vtable, RELEASE_PTR_OFFSET, ReleaseNewPtr);
            } else {
                item.ReleaseOri(pUnk);
            }
        }

        private static int Release(IntPtr pUnk) {
            IntPtr vtable = Marshal.ReadIntPtr(pUnk);
            COMDisposable item = (COMDisposable)ITEMS[vtable];

            int refCount = item.ReleaseOri(pUnk);
            if (refCount < item.RefCountStart) {
                IDisposable obj = (IDisposable)Marshal.GetObjectForIUnknown(pUnk);
                item.ReleaseOri(pUnk);
                obj.Dispose();
            }
            return refCount;
        }

    }

}
