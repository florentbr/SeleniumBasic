using System;

namespace Selenium.Errors {

    /// <summary>
    /// 
    /// </summary>
    public class KeyboardInterruptError : SeleniumError {

        internal KeyboardInterruptError()
            : base(null) {
            base.Source = "Selenium";
            base.HResult = FACILITY_CONTROL_ERROR | 0x4004;
        }

        public override string Message {
            get {
                return "Code execution has been interrupted.";
            }
        }

    }

}
