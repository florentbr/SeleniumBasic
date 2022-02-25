﻿using Selenium;
using System;

namespace Selenium.Core {

    class WindowContext {

        #region Static

        internal static List GetWindowsHandles(RemoteSession session) {
            string endpoint = WebDriver.LEGACY ? "/window_handles" : "/window/handles";
            return (List)session.Send(RequestMethod.GET, endpoint);
        }

        internal static string GetCurrentTitle(RemoteSession session) {
            return (string)session.Send(RequestMethod.GET, "/title");
        }

        internal static string ActivateWindow(RemoteSession session, string handle) {
            string param = WebDriver.LEGACY ? "name" : "handle";
            session.Send(RequestMethod.POST, "/window", param, handle);
            string endpoint = WebDriver.LEGACY ? "/window_handle" : "/window";
            return (string)session.Send(RequestMethod.GET, endpoint);
        }

        internal static string ActivateWindowByName(RemoteSession session, string name) {
            if( !WebDriver.LEGACY )
                throw new Errors.NotImplementedError();
            session.Send(RequestMethod.POST, "/window", "name", name);
            return (string)session.Send(RequestMethod.GET, "/window_handle");
        }

        internal static string GetCurrentName(RemoteSession session) {
            string script = "return window.name;";
            object result = session.javascript.Execute(script, new object[0], true);
            if( result != null && result is string )
                return (string)result;
            return null;
        }

        #endregion

        private readonly RemoteSession _session;
        private List _cachedWindows;
        internal Window _previousWindow;
        internal Window _currentWindow;


        public WindowContext(RemoteSession session)
            : base() {
            _session = session;
            _currentWindow = new Window(session, this, null);
            _previousWindow = _currentWindow;
            _cachedWindows = new List(10);
            _cachedWindows.Add(_currentWindow);
        }

        /// <summary>
        /// Removes the specific window from all references and closes it
        /// </summary>
        public void Close(Window w)
        {
            if( w == null ) return;
            _cachedWindows.Remove(w);
            bool not_current = w != _currentWindow;
            if( not_current ) {
                ActivateWindow(_session, w.Handle);
            }
            _session.Send(RequestMethod.DELETE, "/window");
            if( not_current ) {
                ActivateWindow(_session, _currentWindow.Handle);
                if( _previousWindow == w )
                    _previousWindow = _currentWindow;
            } else if( _previousWindow != _currentWindow && _previousWindow != null ) {
                ActivateWindow(_session, _previousWindow.Handle);
                _currentWindow = _previousWindow;
            } else {
                _currentWindow = null;
                _previousWindow = null;
            }
        }

        public void ListWindows(out List windows, out List handles) {
            handles = WindowContext.GetWindowsHandles(_session);
            windows = new List(handles.Count);
            foreach (string handle in handles) {
                int hash = handle.GetHashCode();
                foreach (Window win in _cachedWindows) {
                    if (win.GetHashCode() == hash && handle == win.Handle) {
                        windows.Add(win);
                        goto nextHandle;
                    }
                }
                windows.Add(new Window(_session, this, handle));
            nextHandle:
                continue;
            }
            _cachedWindows = windows;
        }

        public Window CurrentWindow {
            get {
                return _currentWindow;
            }
        }

        /// <summary>
        /// Change focus to another window.
        /// </summary>
        /// <param name="title">The name of the window</param>
        /// <param name="timeout"></param>
        public Window SwitchToWindowByTitle(string title, int timeout = -1) {
            var endTime = _session.GetEndTime(timeout);
            while (true) {
                List windows, handles;
                this.ListWindows(out windows, out handles);
                foreach (Window win in windows) {
                    if (_currentWindow != null && win.Handle == _currentWindow.Handle)
                        continue;
                    WindowContext.ActivateWindow(_session, win.Handle);
                    string winTitle = GetCurrentTitle(_session);
                    if (winTitle == title) {
                        _previousWindow = _currentWindow;
                        _currentWindow = win;
                        return win;
                    }
                }

                if (DateTime.UtcNow > endTime)
                    throw new Errors.NoSuchWindowError(title);

                SysWaiter.Wait();
            }
        }

        /// <summary>
        /// Change focus to another window.
        /// </summary>
        /// <param name="name">The name of the window</param>
        /// <param name="timeout"></param>
        public Window SwitchToWindowByName(string name, int timeout = -1) {
            var endTime = _session.GetEndTime(timeout);
            while (true) {
                try {
                    if( WebDriver.LEGACY ) {
                        string handle = WindowContext.ActivateWindow(_session, name);
                        _previousWindow = _currentWindow;
                        foreach (Window win in _cachedWindows) {
                            if (win.Handle == handle) {
                                _currentWindow = win;
                                return _currentWindow;
                            }
                        }
                        _currentWindow = new Window(_session, this, handle);
                        _cachedWindows.Add(_currentWindow);
                        return _currentWindow;
                    } else {
                        List windows, handles;
                        this.ListWindows(out windows, out handles);
                        foreach (Window win in windows) {
                            if (_currentWindow != null && win.Handle == _currentWindow.Handle)
                                continue;
                            WindowContext.ActivateWindow(_session, win.Handle);
                            string winName = GetCurrentName(_session);
                            if (winName == name) {
                                _previousWindow = _currentWindow;
                                _currentWindow = win;
                                return win;
                            }
                        }
                    }
                } catch (Errors.NoSuchWindowError) { }

                if (DateTime.UtcNow > endTime)
                    throw new Errors.NoSuchWindowError(name);

                SysWaiter.Wait();
            }
        }

        /// <summary>
        /// Switch to the next available window
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Window SwitchToNextWindow(int timeout = -1) {
            DateTime endTime = _session.GetEndTime(timeout);
            string currentHandle = _currentWindow?.Handle;
            string previousHandle = _previousWindow?.Handle;

            while (true) {
                List windows, handles;
                this.ListWindows(out windows, out handles);
                int count = handles.Count;
                if (count > 1) { // need more than one window
                    //search index of the current windows
                    int i = count;
                    while (i-- > 0 && (string)handles[i] != currentHandle) ;

                    //search and activate the next window
                    for (int ii = count; ii-- > 0; ) {
                        if (++i == count)
                            i = 0;
                        var handle = (string)handles[i];
                        if (handle != currentHandle && handle != previousHandle)
                            return ActivateWindow((Window)windows[i]);
                    }
                }

                //handle time out
                if (DateTime.UtcNow > endTime)
                    throw new Errors.NoSuchWindowError();

                SysWaiter.Wait();
                handles = (List)_session.SendAgain();
            }
        }

        /// <summary>
        /// Switch to the previous focused window.
        /// </summary>
        /// <returns></returns>
        public Window SwitchToPreviousWindow() {
            if( _previousWindow == null )
                return null;
            return _previousWindow.Activate();
        }

        internal Window ActivateWindow(Window window) {
            if (_currentWindow != window) {
                ActivateWindow(_session, window.Handle);
                _previousWindow = _currentWindow;
                _currentWindow = window;
            }
            return window;
        }

    }

}
