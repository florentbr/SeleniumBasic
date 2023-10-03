using Selenium.Internal;
using System;
using System.Security.AccessControl;
using System.Threading;

namespace Selenium.Core {

    /// <summary>
    /// Provides waits methods that can be interrupted with a registered hot key
    /// </summary>
    class SysWaiter {
        private static readonly NLog.Logger _l = NLog.LogManager.GetCurrentClassLogger();

        const int DEFAULT_WAIT_TIME = 50;
        const int TIMEOUT_DIVIDER   = 10;

        //Modifier keys
        const int MOD_NONE = 0x00;
        const int MOD_ALT = 0x01;
        const int MOD_CONTROL = 0x02;
        const int MOD_SHIFT = 0x04;
        const int MOD_WIN = 0x08;

        //Virtual key codes
        const int VK_ESCAPE = 0x1B;
        const int VK_PAUSE = 0x13;

        static readonly EventWaitHandle _signal_interrupt = null;

        static Action _interrupt_delegate;

        static SysWaiter() {
            string user = Environment.UserDomainName + "\\" + Environment.UserName;
            var rule = new EventWaitHandleAccessRule(user,
                                                     EventWaitHandleRights.FullControl,
                                                     AccessControlType.Allow);
            var security = new EventWaitHandleSecurity();
            security.AddAccessRule(rule);

            bool createdNew;
            _signal_interrupt = new EventWaitHandle(false,
                                                    EventResetMode.ManualReset,
                                                    @"Global\InterruptKey"+ Environment.UserName,
                                                    out createdNew,
                                                    security);

            HotKeyGlobal.DefineHotKey(MOD_ALT, VK_ESCAPE, ProcInterrupt);
        }

        public static Action OnInterrupt {
            set {
                if (value == null && _interrupt_delegate != null) {
                    HotKeyGlobal.UnsubscribeAll();
                } else {
                    HotKeyGlobal.SubscribeAll();
                }
                _interrupt_delegate = value;
            }
        }

        private static void ProcInterrupt() {
            _signal_interrupt.Set();
            if (_interrupt_delegate != null){
                _interrupt_delegate();
                _interrupt_delegate = null;
            }
            HotKeyGlobal.UnsubscribeAll();
        }

        public static EventWaitHandle WaitHandle {
            get {
                return _signal_interrupt;
            }
        }

        public static void Signal() {
            _signal_interrupt.Set();
        }

        internal static void Wait() {
            Wait(DEFAULT_WAIT_TIME);
        }

        public static void Wait(int timems) {
            HotKeyGlobal.SubscribeAll();
            _signal_interrupt.Reset();
            bool signaled = _signal_interrupt.WaitOne(timems, true);
            if (signaled)
                throw new Errors.KeyboardInterruptError();
            HotKeyGlobal.UnsubscribeAll();
        }

        internal static int GetTimeChunk( int full_timeout ) {
            int time_chunk = full_timeout / SysWaiter.TIMEOUT_DIVIDER;
            if( time_chunk < DEFAULT_WAIT_TIME ) time_chunk = DEFAULT_WAIT_TIME;
            return time_chunk;
        }

    }

}
