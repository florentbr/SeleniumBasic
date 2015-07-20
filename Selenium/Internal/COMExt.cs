using Selenium.Core;
using System;
using System.Runtime.InteropServices;

namespace Selenium.Internal {

    class COMExt {

        delegate int CallBack0([Out]out object result, ref object argument1, ref object argument2);

        delegate object CallBack1();
        delegate object CallBack2(ref object arg1);
        delegate object CallBack3(ref object arg1, ref object arg2);

        public static object WaitUntilProc(object function, object argument1, object argument2, int timeout) {
            CallBack0 callback0 = null;
            CallBack1 callback1 = null;
            CallBack2 callback2 = null;
            CallBack3 callback3 = null;
            DateTime endtime = DateTime.UtcNow.AddMilliseconds(timeout);
            int callbackIndex = 0; // argument1 == null ? 0 : (argument2 == null ? 1 : 2);
            if (function is int) {
                //handles AddressOf
                var ptr = new IntPtr((int)function);
                callback0 = (CallBack0)Marshal.GetDelegateForFunctionPointer(ptr, typeof(CallBack0));
            } else if (Marshal.IsComObject(function)) {
                //handles GetRef() or function(){}
                if (argument1 == null || argument1 == DBNull.Value) {
                    callbackIndex = 1;
                    callback1 = ((ICallBack1)function).Call;
                } else if (argument2 == null || argument2 == DBNull.Value) {
                    callbackIndex = 2;
                    callback2 = ((ICallBack2)function).Call;
                } else {
                    callbackIndex = 3;
                    callback3 = ((ICallBack3)function).Call;
                }
            } else {
                throw new Errors.ArgumentError("Invalid argument at position 0. A function is expected.");
            }
            try {
                while (true) {
                    object result = null;
                    switch (callbackIndex) {
                        case 0: callback0(out result, ref argument1, ref argument2); break;
                        case 1: result = callback1(); break;
                        case 2: result = callback2(ref argument1); break;
                        case 3: result = callback3(ref argument1, ref argument2); break;
                    }
                    if (result != null && !false.Equals(result))
                        return result;
                    if (DateTime.UtcNow > endtime)
                        throw new Errors.TimeoutError(timeout);
                    SysWaiter.Wait();
                }
            } catch (ArgumentException) {
                throw new Errors.ArgumentError("The procedure has an invalide signature.");
            } catch (COMException ex) {
                throw new SeleniumError("The procedure raised an error. " + ex.Message);
            }
        }


        #region interfaces

        [Guid("00000000-0000-0000-C000-000000000046"), InterfaceType(1)]
        private interface ICallBack1 {
            object Call();
        }

        [Guid("00000000-0000-0000-C000-000000000046"), InterfaceType(1)]
        private interface ICallBack2 {
            object Call(ref object arg1);
        }

        [Guid("00000000-0000-0000-C000-000000000046"), InterfaceType(1)]
        private interface ICallBack3 {
            object Call(ref object arg1, ref object arg2);
        }

        #endregion

    }

}
