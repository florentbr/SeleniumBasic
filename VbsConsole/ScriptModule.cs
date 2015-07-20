using Interop.MSScript;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace vbsc {

    class ScriptModule {

        private IMSScriptModule _msc_module;
        private Type _msc_type;

        public ScriptModule(IMSScriptModule msc_module) {
            _msc_module = msc_module;
            _msc_type = msc_module.GetType();
        }

        public object Run(string procedure, object[] arguments) {
            object[] invokeArgs;
            if (arguments != null && arguments.Length != 0) {
                invokeArgs = new object[1 + arguments.Length];
                Array.Copy(arguments, 0, invokeArgs, 1, arguments.Length);
            } else {
                invokeArgs = new[] { procedure };
            }
            invokeArgs[0] = procedure;
            return _msc_type.InvokeMember("Run", BindingFlags.InvokeMethod, null, _msc_module, invokeArgs);
        }

    }
}
