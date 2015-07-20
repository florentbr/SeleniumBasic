using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Selenium.Internal {

    /// <summary>
    /// A class to register global hot keys. Turns out that it only works with elevated privileges.
    /// Installs a low level hook on the keyboad to intercept the key down event.
    /// </summary>
    class HotKeyGlobalLL : SafeHandle {

        private static HotKeyGlobalLL _instance_;

        static HotKeyGlobalLL() {
            _instance_ = new HotKeyGlobalLL();
        }


        public static bool Subscribe(int modifierCode, int virtualKeyCode, Action action) {
            _instance_.RegisterHotKey(modifierCode, virtualKeyCode, action);
            return !_instance_.IsInvalid;
        }

        public static void UnsubscribeAll() {
            _instance_.UnregisterAll();
        }



        private NativeMethods.LowLevelKeyboardProc _callback;
        private List<int> _hotKeys = new List<int>();
        private List<Action> _actions = new List<Action>();
        private int _modifiers;

        private HotKeyGlobalLL()
            : base(IntPtr.Zero, true) {

            _callback = new NativeMethods.LowLevelKeyboardProc(LowLevelKeyboardCallBack);
            this.handle = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, _callback, IntPtr.Zero, 0);
        }

        protected override bool ReleaseHandle() {
            if (!this.IsInvalid)
                NativeMethods.UnhookWindowsHookEx(this.handle);
            return true;
        }

        public override bool IsInvalid {
            get {
                return this.handle == IntPtr.Zero;
            }
        }

        private void RegisterHotKey(int modifierCode, int virtualKeyCode, Action action) {
            int hotKeyCode = GetHotKeyCode(modifierCode, virtualKeyCode);
            _hotKeys.Add(hotKeyCode);
            _actions.Add(action);
        }

        private void UnregisterAll() {
            _hotKeys.Clear();
            _actions.Clear();
        }

        private IntPtr LowLevelKeyboardCallBack(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0) {
                int vkCode = Marshal.ReadInt32(lParam);
                switch ((int)wParam) {
                    case NativeMethods.WM_SYSKEYDOWN:
                    case NativeMethods.WM_KEYDOWN:
                        switch (vkCode) {
                            case 0xA0:
                            case 0xA1:
                                _modifiers |= 0x0004; break; //Shift
                            case 0xA2:
                            case 0xA3:
                                _modifiers |= 0x0002; break; //Control
                            case 0xA4:
                            case 0xA5:
                                _modifiers |= 0x0001; break; //Alt
                            default:
                                int hotkey = GetHotKeyCode(_modifiers, vkCode);
                                for (int i = _hotKeys.Count; i-- > 0; ) {
                                    if (_hotKeys[i] == hotkey) {
                                        _actions[i].BeginInvoke(null, null);
                                    }
                                }
                                break;
                        }
                        break;
                    case NativeMethods.WM_SYSKEYUP:
                    case NativeMethods.WM_KEYUP:
                        switch (vkCode) {
                            case 0xA0:
                            case 0xA1:
                                _modifiers &= 0xFFFB; break; //Shift
                            case 0xA2:
                            case 0xA3:
                                _modifiers &= 0xFFFD; break; //Control
                            case 0xA4:
                            case 0xA5:
                                _modifiers &= 0xFFFE; break; //Alt
                        }
                        break;
                }
            }
            return NativeMethods.CallNextHookEx(this.handle, nCode, wParam, lParam);
        }

        private int GetHotKeyCode(int modifierCode, int virtualKeyCode) {
            return (virtualKeyCode << 16) | modifierCode; //LowerInt16=modifiers, UpperInt16=vkCode
        }


        #region Imports

        static class NativeMethods {

            public const string USER32 = "user32.dll";

            public const int WH_KEYBOARD_LL = 13;
            public const int WM_KEYDOWN = 0x100;
            public const int WM_KEYUP = 0x101;
            public const int WM_SYSKEYDOWN = 0x104;
            public const int WM_SYSKEYUP = 0x105;

            public delegate IntPtr LowLevelKeyboardProc(int code, IntPtr wParam, IntPtr lParam);

            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            [DllImport(USER32, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, int dwThreadId);

            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            [DllImport(USER32, SetLastError = true)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            [DllImport(USER32, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        }

        #endregion

    }

}
