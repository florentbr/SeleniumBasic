using Selenium.Core;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Selenium.Internal {

    class COMExt {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int CallBack([Out]out object result, ref object argument1, ref object argument2);

        public static object WaitUntilProc(object function, object argument1, object argument2, int timeout) {
            DateTime endtime = DateTime.UtcNow.AddMilliseconds(timeout);
            try {
                if (function is int) {
                    //handles AddressOf
                    var ptr = new IntPtr((int)function);
                    CallBack callback0 = (CallBack)Marshal.GetDelegateForFunctionPointer(ptr, typeof(CallBack));
                    while (true) {
                        object result = null;
                        callback0(out result, ref argument1, ref argument2);
                        if (result != null && !false.Equals(result))
                            return result;
                        if (DateTime.UtcNow > endtime)
                            throw new Errors.TimeoutError(timeout);
                        SysWaiter.Wait();
                    }
                } else if (Marshal.IsComObject(function)) {
                    Type t = function.GetType();
                    object[] args = new object[] { argument1, argument2 };
                    while (true) {
                        object result = t.InvokeMember(string.Empty, BindingFlags.InvokeMethod, null, function, args);
                        if (result != null && !false.Equals(result))
                            return result;
                        if (DateTime.UtcNow > endtime)
                            throw new Errors.TimeoutError(timeout);
                        SysWaiter.Wait();
                    }
                } else {
                    throw new Errors.ArgumentError("Invalid argument at position 0. A function is expected.");
                }
            } catch (ArgumentException) {
                throw new Errors.ArgumentError("The procedure has an invalide signature.");
            } catch (COMException ex) {
                throw new SeleniumError(ex.Message);
            }
        }

    }

}
