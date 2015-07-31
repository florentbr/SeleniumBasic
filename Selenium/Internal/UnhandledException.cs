using Selenium.Internal;
using System;
using System.Threading;

namespace Selenium.Core {

    static class UnhandledException {

        private static UnhandledExceptionEventHandler _callback = null;
        private static Thread _thread = null;

        public static void Initialize() {
            if (_callback != null)
                return;

            _callback = new UnhandledExceptionEventHandler(On_UnhandledException);
            AppDomain.CurrentDomain.UnhandledException += _callback;
        }

        private static void On_UnhandledException(object sender, UnhandledExceptionEventArgs ex_arg) {
            if (ex_arg.ExceptionObject is ThreadAbortException)
                return;
            //Display the exception message box on another thread
            _thread = new Thread(new ParameterizedThreadStart((ex) =>
                ExceptionDialog.ShowDialog((Exception)ex)
            ));
            _thread.IsBackground = true;
            SysWaiter.Signal();
            _thread.Start(ex_arg.ExceptionObject);
        }

    }

}
