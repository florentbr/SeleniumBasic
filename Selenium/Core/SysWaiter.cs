using Selenium.Internal;
using System;
using System.Threading;

namespace Selenium.Core {

    /// <summary>
    /// Provides waits methods that can be interrupted with a regitered hot key
    /// </summary>
    class SysWaiter {

        const int DEFAULT_WAIT_TIME = 50;

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

        public static Action OnInterrupt;

        static SysWaiter() {
            _signal_interrupt = new EventWaitHandle(false, EventResetMode.ManualReset);
            HotKeyGlobal.Subscribe(MOD_NONE, VK_ESCAPE, Interrupt);
            HotKeyGlobal.Subscribe(MOD_NONE, VK_PAUSE, Interrupt);
        }

        public static void Initialize() {
            HotKeyGlobal.SubscribeAgain();
        }

        private static void Interrupt() {
            _signal_interrupt.Set();
            if (OnInterrupt != null)
                OnInterrupt();
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
            _signal_interrupt.Reset();
            if (_signal_interrupt.WaitOne(timems, true))
                throw new Errors.KeyboardInterruptError();
        }

    }

}
