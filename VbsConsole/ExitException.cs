using Interop.MSScript;
using System;

namespace vbsc {

    class ExitException : Exception {

        const string SOURCE_NAME = "WScript.Exit";

        public ExitException(int code)
            : base(string.Concat("Exit code: ", code)) {
            base.Source = SOURCE_NAME;
            base.HResult = unchecked((int)0x80000000U | code);
        }

        public static bool Is(IMSScriptError error) {
            return error.Source == SOURCE_NAME;
        }

        public static bool Is(Exception ex) {
            return ex.Source == SOURCE_NAME;
        }

        public static int ExitCode(IMSScriptError err) {
            return unchecked((int)0x7FFFFFFFU & err.Number);
        }
    }

}
