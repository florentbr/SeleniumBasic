using System;
using System.Threading;

namespace Selenium.Internal {

    class ThreadExt {

        /// <summary>
        /// Invoke a function on a new single thread.
        /// </summary>
        /// <typeparam name="T">Returned value</typeparam>
        /// <param name="func">Function</param>
        /// <param name="timeout">Execution timeout</param>
        /// <returns></returns>
        public static T RunSTA<T>(Func<T> func, int timeout) {
            T result = default(T);
            Exception exception = null;
            using (var evt = new EventWaitHandle(false, EventResetMode.ManualReset)) {
                Thread thread = new Thread(() => {
                    try {
                        result = func();
                    } catch (Exception ex) {
                        exception = ex;
                    }
                    evt.Set();
                });
                thread.TrySetApartmentState(ApartmentState.STA);
                thread.Start();
                evt.WaitOne(timeout, true);
            }
            if (exception != null)
                throw new SeleniumException(exception);
            return result;
        }

    }

}
