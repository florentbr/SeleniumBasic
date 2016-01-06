using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Selenium.Internal {

    /// <summary>
    /// A class to register hot keys globally with the RegisterHotKey API.
    /// </summary>
    class HotKeyGlobal {

        const int WM_HOTKEY = 0x0312;
        const int WM_USER_HOTKEY_REGISTER = 0x0401;
        const int WM_USER_HOTKEY_UNREGISTER_ALL = 0x0402;

        static Thread _thread_;
        static uint _threadId_;
        static List<int> _hotKeys_ = new List<int>(5);
        static List<Action> _actions_ = new List<Action>(5);

        public static void DefineHotKey(int modifier, int virtualKey, Action action) {
            if (_thread_ == null) {
                _thread_ = new Thread(RunMessagePump);
                _thread_.IsBackground = true;
                _thread_.Start();
                lock (_thread_) {
                    Monitor.Wait(_thread_);
                }
            }

            int hotKey = (virtualKey << 16) | (modifier & 0xFFFF);
            _hotKeys_.Add(hotKey);
            _actions_.Add(action);

            PostThreadMessage(WM_USER_HOTKEY_REGISTER, modifier, virtualKey);
        }

        public static void SubscribeAll() {
            foreach(int hotkey in _hotKeys_){
                int modifierCode = hotkey & 0xFFFF;
                int virtualKeyCode = hotkey >> 16;
                PostThreadMessage(WM_USER_HOTKEY_REGISTER, modifierCode, virtualKeyCode);
            }
        }

        public static void UnsubscribeAll() {
            PostThreadMessage(WM_USER_HOTKEY_UNREGISTER_ALL, 0, 0);
        }

        static void EvalHotKey(uint hotkey) {
            for (int i = _hotKeys_.Count; i-- > 0; ) {
                if (_hotKeys_[i] == hotkey) {
                    _actions_[i]();
                }
            }
        }

        static void RunMessagePump() {
            _threadId_ = Native.GetCurrentThreadId();

            // signal that the message pump is up and running
            lock (_thread_) {
                Monitor.Pulse(_thread_);
            }

            //run message pump
            try {
                DispatchThreadMessages();
            } catch { }
        }

        static void DispatchThreadMessages() {
            uint atom = ((_threadId_ ^ (_threadId_ >> 16)) & 0x0ffff) - 25;
            uint count = 0;
            var msg = new Native.Message();
            while (Native.GetMessage(ref msg, (IntPtr)0, 0, 0) > 0) {
                switch(msg.Msg){
                    case WM_USER_HOTKEY_REGISTER:
                        bool rhk = Native.RegisterHotKey(IntPtr.Zero,
                                                         atom + count,
                                                         (uint)msg.WParam,
                                                         (uint)msg.LParam);
                        if (rhk) {
                            count++;
                        }
                        break;
                    case WM_USER_HOTKEY_UNREGISTER_ALL:
                        while (count > 0) {
                            count--;
                            Native.UnregisterHotKey(IntPtr.Zero, atom + count);
                        }
                        break;
                    case WM_HOTKEY:
                        EvalHotKey((uint)msg.LParam); // lword=modifiers, hword=virtualKey
                        break;
                }
            }
        }

        static bool PostThreadMessage(int msg, int wParam, int lParam) {
            return Native.PostThreadMessage(_threadId_, msg, (IntPtr)wParam, (IntPtr)lParam);
        }


        #region Imports

        static class Native {

            public const string KERNEL32 = "kernel32.dll";
            public const string USER32 = "user32.dll";

            [DllImport(USER32)]
            public static extern bool RegisterHotKey(IntPtr hWnd, uint id, uint fsModifiers, uint vk);

            [DllImport(USER32)]
            public static extern bool UnregisterHotKey(IntPtr hWnd, uint id);

            [DllImport(KERNEL32, SetLastError = false)]
            public static extern uint GetCurrentThreadId();

            [StructLayout(LayoutKind.Sequential)]
            public struct Message {
                public IntPtr HWnd;
                public int Msg;
                public IntPtr WParam;
                public IntPtr LParam;
                public int Time;
                public int PointX;
                public int PointY;
            }

            [DllImport(USER32, SetLastError = false)]
            public static extern int GetMessage([In, Out] ref Message msg, IntPtr hWnd, int uMsgFilterMin, int uMsgFilterMax);

            [DllImport(USER32, SetLastError = false)]
            public static extern bool PostThreadMessage(uint threadId, int msg, IntPtr wParam, IntPtr lParam);

        }

        #endregion

    }

}
