using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Selenium.Internal {

    /// <summary>
    /// A class to register hot keys globally.
    /// Creates a message pump and uses RegisterHotKey from the windows API to register the hot key.
    /// Also creates a hidden window to track the thread as the same hot key can't be registered twice.
    /// A thread that successfully registered a hot key will once triggered, trigger all the other 
    /// threads having the same class name (hotkey952873).
    /// </summary>
    class HotKeyGlobal {

        static HotKeyGlobal _instance_;

        public static void Subscribe(int modifierCode, int virtualKeyCode, Action action) {
            if (_instance_ == null)
                _instance_ = new HotKeyGlobal();
            _instance_.RegisterHotKey(modifierCode, virtualKeyCode, action);
        }

        public static void SubscribeAgain() {
            _instance_.RegisterAgain();
        }

        public static void UnsubscribeAll() {
            _instance_.UnregisterAll();
        }


        private const string CLASS_NAME = "hotkey952873";
        private const int WM_USER_HOTKEY_REGISTER = 0x0401;
        private const int WM_USER_HOTKEY_UNREGISTER_ALL = 0x0402;

        private Thread _thread;
        private uint _threadId;
        private readonly object _threadLock = new object();
        private readonly List<int> _hotKeys = new List<int>(5);
        private readonly List<Action> _actions = new List<Action>(5);
        private volatile int _registeredCount = 0;
        private bool _disposed = false;

        public HotKeyGlobal() {
            _thread = new Thread(RunMessagePump);
            _thread.IsBackground = true;
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
            lock (_threadLock)
                Monitor.Wait(_threadLock);
        }

        ~HotKeyGlobal() {
            this.Dispose();
        }

        public void Dispose() {
            if (_disposed)
                return;
            _thread.Interrupt();
            _disposed = true;
        }

        private void RegisterHotKey(int modifierCode, int virtualKeyCode, Action action) {
            int hotKey = (virtualKeyCode << 16) | modifierCode;
            _hotKeys.Add(hotKey);
            _actions.Add(action);
            PostThreadMessage(WM_USER_HOTKEY_REGISTER, modifierCode, virtualKeyCode);
        }

        private void RegisterAgain() {
            _registeredCount = 0;
            for (int i = _actions.Count; i-- > 0; ) {
                int modifierCode = _hotKeys[i] & 0xFFFF;
                int virtualKeyCode = _hotKeys[i] >> 16;
                PostThreadMessage(WM_USER_HOTKEY_REGISTER, modifierCode, virtualKeyCode);
            }
        }

        private void UnregisterAll() {
            _hotKeys.Clear();
            _actions.Clear();
            PostThreadMessage(WM_USER_HOTKEY_UNREGISTER_ALL, 0, 0);
        }

        private void EvalHotKey(uint hotkey) {
            for (int i = _hotKeys.Count; i-- > 0; ) {
                if (_hotKeys[i] == hotkey)
                    _actions[i]();
            }
        }

        private bool PostThreadMessage(int msg, int wParam, int lParam) {
            return NativeMethods.PostThreadMessage(_threadId, msg, (IntPtr)wParam, (IntPtr)lParam);
        }

        private void RunMessagePump() {
            _threadId = NativeMethods.GetCurrentThreadId();

            //create the window to track sibling threads
            IntPtr user32 = NativeMethods.GetModuleHandle(NativeMethods.USER32);

            string name_DefWindowProc = Marshal.SystemDefaultCharSize == 1 ?
                "DefWindowProcA" : "DefWindowProcW";

            NativeMethods.WNDCLASSEX classEx = new NativeMethods.WNDCLASSEX();
            classEx.lpszClassName = CLASS_NAME;
            classEx.lpfnWndProc = NativeMethods.GetProcAddress(user32, name_DefWindowProc);
            classEx.cbSize = Marshal.SizeOf(typeof(NativeMethods.WNDCLASSEX));

            IntPtr classHandle = (IntPtr)NativeMethods.RegisterClassEx(classEx);
            IntPtr winHandle = NativeMethods.CreateWindowEx(0, classHandle, string.Empty
                , 0, 0, 0, 0, 0, (IntPtr)0, (IntPtr)0, (IntPtr)0, (IntPtr)0);

            //run message pump
            try {
                DispatchThreadMessages();
            } catch { }
        }

        private void DispatchThreadMessages() {
            lock (_threadLock)
                Monitor.Pulse(_threadLock);    //Signals the message pump is running
            var msg = new NativeMethods.Message();
            while (NativeMethods.GetMessage(ref msg, (IntPtr)0, 0, 0) > 0) {
                switch (msg.Msg) {
                    case NativeMethods.WM_HOTKEY:
                        if (_registeredCount > 0) {
                            //Notify the sibling processes as only one regitration per hot key is allowed
                            PostThreadsMessageToSibling(msg.Msg, (IntPtr)msg.WParam, (IntPtr)msg.LParam);
                        }
                        EvalHotKey((uint)msg.LParam); // lParam: LowerInt16=modifiers, UpperInt16=vkCode
                        break;
                    case WM_USER_HOTKEY_REGISTER:
                        if (NativeMethods.RegisterHotKey(IntPtr.Zero, _registeredCount + 1, (uint)msg.WParam, (uint)msg.LParam))
                            _registeredCount++;
                        break;
                    case WM_USER_HOTKEY_UNREGISTER_ALL:
                        while (_registeredCount > 0)
                            NativeMethods.UnregisterHotKey(IntPtr.Zero, _registeredCount--);
                        break;
                }
            }
        }

        /// <summary>
        /// Post a message to all the sibling threads:
        /// Loops through all the top windows to find the matching class.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        private void PostThreadsMessageToSibling(int msg, IntPtr wParam, IntPtr lParam) {
            int length = CLASS_NAME.Length + 1;
            var buffer = new StringBuilder(length);
            NativeMethods.EnumWindows((IntPtr hwnd, IntPtr param) => {
                NativeMethods.GetClassName(hwnd, buffer, length);
                if (CLASS_NAME.Equals(buffer.ToString())) {
                    uint threadId = NativeMethods.GetWindowThreadProcessId(hwnd, (IntPtr)0);
                    if (threadId != _threadId)
                        NativeMethods.PostThreadMessage(threadId, msg, wParam, lParam);
                }
                return true;
            }, IntPtr.Zero);
        }


        #region Imports

        static class NativeMethods {

            public const string KERNEL32 = "kernel32.dll";
            public const string USER32 = "user32.dll";

            public const int WM_CREATE = 0x01;
            public const int WM_DESTROY = 0x02;
            public const int WM_QUIT = 0x0012;
            public const int WM_CLOSE = 0x0010;
            public const int WM_HOTKEY = 0x0312;


            [DllImport(USER32, CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern IntPtr CreateWindowEx(
                int dwExStyle, IntPtr atomId, string lpszWindowName,
                int style, int x, int y, int width, int height,
                IntPtr hWndParent, IntPtr hMenu, IntPtr hInst, IntPtr pvParam);

            [DllImport(USER32)]
            public static extern bool DestroyWindow(IntPtr hWnd);

            [DllImport(USER32)]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [DllImport(USER32)]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

            [DllImport(USER32)]
            public static extern ushort RegisterClassEx(WNDCLASSEX wc_d);

            [DllImport(USER32)]
            public static extern int UnregisterClass(IntPtr atomId, IntPtr hInstance);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public class WNDCLASSEX {
                public int cbSize = 0;
                public int style = 0;
                public IntPtr lpfnWndProc;
                public int cbClsExtra = 0;
                public int cbWndExtra = 0;
                public IntPtr hInstance = IntPtr.Zero;
                public IntPtr hIcon = IntPtr.Zero;
                public IntPtr hCursor = IntPtr.Zero;
                public IntPtr hbrBackground = IntPtr.Zero;
                public string lpszMenuName = null;
                public string lpszClassName = null;
                public IntPtr hIconSm = IntPtr.Zero;
            }

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


            [DllImport(USER32, SetLastError = true, EntryPoint = "GetMessageW", ExactSpelling = true, CharSet = CharSet.Unicode)]
            public static extern int GetMessage([In, Out] ref Message msg, IntPtr hWnd, int uMsgFilterMin, int uMsgFilterMax);

            [DllImport(USER32, SetLastError = false)]
            public static extern bool PostThreadMessage(uint threadId, int msg, IntPtr wParam, IntPtr lParam);

            public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

            [DllImport(USER32, SetLastError = false)]
            public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

            [DllImport(USER32, SetLastError = false)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

            [DllImport(USER32, SetLastError = false)]
            public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

            [DllImport(KERNEL32, SetLastError = false)]
            public static extern uint GetCurrentThreadId();

            [DllImport(KERNEL32, SetLastError = false, CharSet = CharSet.Auto, BestFitMapping = false)]
            public static extern IntPtr GetModuleHandle(string modName);

            [DllImport(KERNEL32, SetLastError = false, CharSet = CharSet.Ansi, BestFitMapping = false)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        }

        #endregion

    }

}
