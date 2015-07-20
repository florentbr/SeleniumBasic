using System;

namespace Selenium.Errors {

    /// <summary>
    /// 
    /// </summary>
    public class KeyboardInterruptError : SeleniumError {

        internal KeyboardInterruptError()
            : base("Code execution has been interrupted.") {
            base.Source = "Selenium";
            base.HResult = unchecked((int)0x80000007);
        }

    }

}
