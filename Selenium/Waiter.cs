﻿using Selenium.Core;
using Selenium.Internal;
using Selenium.ComInterfaces;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Waiting functions
    /// </summary>
    [ProgId("Selenium.Waiter")]
    [Guid("0277FC34-FD1B-4616-BB19-7D30CBC3F6BB")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [Description("Waiting functions to keep the visual basic editor from not responding")]
    public class Waiter : _Waiter {

        private DateTime? _endtime;
        private int _timeout = 5000;
        private int _delayms = 60;

        /// <summary>
        /// Creates a new Waiter object
        /// </summary>
        public Waiter() {

        }

        /// <summary>
        /// Waiter timeout in milliseconds. Default is 5000 milliseconds
        /// </summary>
        public int Timeout {
            get { return (int)_timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Delay in millisecond before trying again. Default is 60 milliseconds
        /// </summary>
        public int RetryDelay {
            get { return (int)_delayms; }
            set { _delayms = value; }
        }

        /// <summary>
        /// Waits the given time in milliseconds
        /// </summary>
        /// <param name="timems">Time to wait in milliseconde</param>
        void _Waiter.Wait(int timems) {
            SysWaiter.Wait(timems);
        }

        /// <summary>
        /// Returns a boolean to continue waiting and throws an exception if the timeout is reached
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="timeout">Optional - Timeout in millisecond</param>
        /// <param name="timeoutMessage">Optional - Message in case of timeout</param>
        /// <returns>Returns false if the result is true, false otherwise.</returns>
        /// <example>
        /// 
        /// <code lang="vbs">	
        /// Public Sub Script()
        ///   Dim driver As New FirefoxDriver, Waiter As New Waiter
        ///   driver.Get "http://www.mywebsite.com/"
        ///   While Waiter.Not(driver.Title = "MyTitle"): Wend
        ///   ...
        /// End Sub
        /// </code>
        /// </example>
        public bool Not(object result, int timeout = -1, string timeoutMessage = null) {
            if (_endtime == null) {
                _endtime = DateTime.UtcNow.AddMilliseconds(timeout == -1 ? _timeout : timeout);
            } else {
                if (DateTime.UtcNow > _endtime) {
                    _endtime = null;
                    if (timeoutMessage != null)
                        throw new Errors.TimeoutError(timeoutMessage);
                    throw new Errors.TimeoutError(timeout == -1 ? _timeout : timeout);
                }
                try {
                    SysWaiter.Wait(_delayms);
                } catch {
                    _endtime = null;
                    throw;
                }
            }
            if (result == null || false.Equals(result))
                return true;
            _endtime = null;
            return false;
        }

        /// <summary>
        /// Waits for the delegate function to return not null or true
        /// </summary>
        /// <typeparam name="T">Returned object or boolean</typeparam>
        /// <param name="func">Delegate function</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns>Variant or boolean</returns>
        public T Until<T>(Func<T> func, int timeout = -1) {
            if (timeout == -1)
                timeout = _timeout;
            return Waiter.WaitUntil(func, timeout, _delayms);
        }

        /// <summary>
        /// Waits for a function(a,b) to return a true-castable result.
        /// </summary>
        /// <param name="procedure">Function's reference. In VBScript use GetRef()</param>
        /// <param name="argument">Function's first argument</param>
        /// <param name="timeout">Timeout in ms</param>
        /// <param name="reserved">Not in use</param>
        /// <returns>function's actual result</returns>
        /// <exception cref="Errors.TimeoutError">Throws when time out has reached</exception>
        /// <example>
        /// <code lang="vbs">
        /// Dim driver  ' the called function has access to global variables
        /// 
        /// Private Function WaitForTitle(expected_title, always_null)
        ///   WaitForTitle = driver.Title = expected_title
        /// End Function
        ///  
        /// Public Sub Script()
        ///   Set Waiter = CreateObject("Selenium.Waiter")
        ///   Set driver = CreateObject("Selenium.FirefoxDriver")
        ///   driver.Get "http://www.google.com/"
        ///   Waiter.Until GetRef("WaitForTitle"), "Google", 5000
        /// End Sub
        /// </code>
        /// </example>
        object _Waiter.Until(object procedure, object argument, int timeout, string reserved) {
            if (timeout == -1)
                timeout = _timeout;
            return COMExt.WaitUntilProc(procedure, argument, null, timeout);
        }

        /// <summary>
        /// Waits the given time in milliseconds
        /// </summary>
        /// <param name="timems">Time to wait in milliseconde</param>
        public static void Wait(int timems) {
            SysWaiter.Wait(timems);
        }

        /// <summary>
        /// Waits for a delegate function to be executed without exception
        /// </summary>
        /// <param name="func">Delegate action</param>
        /// <param name="timeoums">Timeout in millisecond</param>
        /// <param name="sleepms">Delay between each attempt</param>
        /// <returns>Delegate result</returns>
        public static T WaitNoException<T>(Func<T> func, int timeoums, int sleepms) {
            var endTime = DateTime.UtcNow.AddMilliseconds(timeoums);
            while (true) {
                try {
                    return func();
                } catch {
                    if (DateTime.UtcNow > endTime)
                        throw;
                }
                SysWaiter.Wait(sleepms);
            }
        }

        /// <summary>
        /// Waits for a delegate action to be executed without exception
        /// </summary>
        /// <param name="action">Delegate action</param>
        /// <param name="timeoums">Timeout in millisecond</param>
        /// <param name="sleepms">Delay between each attempt</param>
        public static void WaitNoException(Action action, int timeoums, int sleepms) {
            var endTime = DateTime.UtcNow.AddMilliseconds(timeoums);
            while (true) {
                try {
                    action();
                    return;
                } catch {
                    if (DateTime.UtcNow > endTime)
                        throw;
                }
                SysWaiter.Wait(sleepms);
            }
        }

        /// <summary>
        /// Waits for a delegate function to return true or not null
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="args"></param>
        /// <param name="timeoums"></param>
        /// <param name="sleepms"></param>
        /// <returns></returns>
        public static T WaitUntil<U, T>(Func<U, T> func, U args, int timeoums, int sleepms) {
            var endTime = DateTime.UtcNow.AddMilliseconds(timeoums);
            while (true) {
                T result = func(args);
                if (IsNotNullOrFalse(result))
                    return result;
                if (DateTime.UtcNow > endTime)
                    throw new Errors.TimeoutError(timeoums);
                SysWaiter.Wait(sleepms);
            }
        }

        /// <summary>
        /// Waits for a delegate function to return true or not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="timeoums"></param>
        /// <param name="sleepms"></param>
        /// <returns></returns>
        public static T WaitUntil<T>(Func<T> func, int timeoums, int sleepms) {
            var endTime = DateTime.UtcNow.AddMilliseconds(timeoums);
            while (true) {
                T result = func();
                if (IsNotNullOrFalse(result))
                    return result;
                if (DateTime.UtcNow > endTime)
                    throw new Errors.TimeoutError(timeoums);
                SysWaiter.Wait(sleepms);
            }
        }


        /// <summary>
        /// Waits for a delegate function to return true
        /// </summary>
        /// <param name="func">Delegate action</param>
        /// <param name="timeoums">Timeout in millisecond</param>
        /// <param name="sleepms">Delay between each attempt</param>
        /// <returns>True if succeed, false otherwise</returns>
        public static bool WaitTrue(Func<bool> func, int timeoums, int sleepms) {
            var endTime = DateTime.UtcNow.AddMilliseconds(timeoums);
            while (true) {
                if (func())
                    return true;
                if (DateTime.UtcNow > endTime)
                    return false;
                SysWaiter.Wait(sleepms);
            }
        }

        private static bool IsNotNullOrFalse<T>(T value) {
            return !(value == null || value is bool && (bool)(object)value == false);
        }



        /// <summary>
        /// Waits for a file to be ready.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="timeout">Timeout in milliseonds</param>
        public void WaitForFile(string path, int timeout = -1) {
            Waiter.WaitUntil(() => {
                if (File.Exists(path)) {
                    try {
                        File.Move(path, path);
                        return true;
                    } catch (System.IO.IOException) { }
                }
                return false;
            }, timeout, 100);
        }

    }

}
