using Selenium.Core;
using Selenium;
using System;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Window object
    /// </summary>
    /// <example>
    /// <code lang="vbs">	
    /// Set win1 = driver.Window
    /// driver.SwitchToNextWindow().Close
    /// win1.Activate();
    /// txt = dlg.Text
    /// dlg.Accept
    /// </code>
    /// </example>
    [ProgId("Selenium.Window")]
    [Guid("0277FC34-FD1B-4616-BB19-BC4304C23087")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Window : ComInterfaces._Window {

        internal static string GetWindowHandle(RemoteSession session) {
            if( WebDriver.LEGACY )
                return (string)session.Send(RequestMethod.GET, "/window_handle");
            else
                return (string)session.Send(RequestMethod.GET, "/window");
        }

        private readonly RemoteSession _session;
        private readonly WindowContext _windows;
        private string _handle = null;
        internal int _hash = 0;

        internal Window(RemoteSession session, WindowContext windows, string handle) {
            _session = session;
            _windows = windows;
            _handle = handle;
            if (handle != null)
                _hash = handle.GetHashCode();
        }

        private string uri() {
            if (_windows.CurrentWindow == this)
                return "/window/current";
            return "/window/" + _handle;
        }

        /// <summary>
        /// Returns the hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            if (_hash == 0) {
                _handle = Window.GetWindowHandle(_session);
                _hash = _handle.GetHashCode();
            }
            return _hash;
        }

        /// <summary>
        /// Returns the handle of the current window.
        /// </summary>
        public string Handle {
            get {
                if (_handle == null) {
                    _handle = Window.GetWindowHandle(_session);
                    _hash = _handle.GetHashCode();
                }
                return _handle;
            }
        }

        /// <summary>
        /// Determines whether the specified instances are considered equal.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            Window win = obj as Window;
            if (win == null)
                return false;
            return this.GetHashCode() == win.GetHashCode() && this.Handle == win.Handle;
        }

        /// <summary>
        /// Change focus to another window.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        public Window SwitchToWindowByTitle(string title, int timeout = -1) {
            return _windows.SwitchToWindowByTitle(title, timeout);
        }

        /// <summary>
        /// Change focus to another window.
        /// </summary>
        /// <param name="name">Name of the window</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        public Window SwitchToWindowByName(string name, int timeout = -1) {
            return _windows.SwitchToWindowByName(name, timeout);
        }

        /// <summary>
        /// Switch to the next available window
        /// </summary>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <returns></returns>
        public Window SwitchToNextWindow(int timeout = -1) {
            return _windows.SwitchToNextWindow(timeout);
        }

        /// <summary>
        /// Switch to the previous focused window.
        /// </summary>
        /// <returns></returns>
        public Window SwitchToPreviousWindow() {
            return _windows.SwitchToPreviousWindow();
        }

        /// <summary>
        /// Change focus to this window.
        /// </summary>
        /// <returns>Self</returns>
        public Window Activate() {
            _windows.ActivateWindow(this);
            return this;
        }

        /// <summary>
        /// Get the current page title.
        /// </summary>
        /// <returns>The current page title.</returns>
        public string Title {
            get {
                _windows.ActivateWindow(this);
                return (string)_session.Send(RequestMethod.GET, "/title");
            }
        }

        /// <summary>
        /// Gets the position of the browser window relative to the upper-left corner of the screen.
        /// </summary>
        /// <remarks>When setting this property, it should act as the JavaScript window.moveTo() method.</remarks>
        public Point Position() {
            string endpoint = WebDriver.LEGACY ? uri() + "/position" : "/window/rect";
            var result = (Dictionary)_session.Send(RequestMethod.GET, endpoint);
            return new Point(result);
        }

        /// <summary>
        /// Sets the position of the browser window relative to the upper-left corner of the screen.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void SetPosition(int x, int y) {
            string endpoint = WebDriver.LEGACY ? uri() + "/position" : "/window/rect";
            _session.Send(RequestMethod.POST, endpoint, "x", x, "y", y);
        }

        /// <summary>
        /// Gets the size of the outer browser window, including title bars and window borders.
        /// </summary>
        /// <remarks>When setting this property, it should act as the JavaScript window.resizeTo() method.</remarks>
        public Size Size() {
            string endpoint = WebDriver.LEGACY ? uri() + "/size" : "/window/rect";
            var dict = (Dictionary)_session.Send(RequestMethod.GET, endpoint);
            return new Size(dict);
        }

        /// <summary>
        /// Sets the size of the outer browser window, including title bars and window borders.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(int width, int height) {
            string endpoint = WebDriver.LEGACY ? uri() + "/size" : "/window/rect";
            _session.Send(RequestMethod.POST, endpoint, "width", width, "height", height);
        }

        /// <summary>
        /// Maximizes the current window if it is not already maximized.
        /// </summary>
        public void Maximize() {
            string endpoint = WebDriver.LEGACY ? uri() + "/maximize" : "/window/maximize";
            _session.Send(RequestMethod.POST, endpoint, new Dictionary());
        }

        /// <summary>
        /// Set the current window full screen.
        /// </summary>
        public void FullScreen() {
            string endpoint = WebDriver.LEGACY ? uri() + "/fullscreen" : "/window/fullscreen";
            _session.Send(RequestMethod.POST, endpoint, new Dictionary());
        }

        /// <summary>
        /// Closes the current window.
        /// </summary>
        public void Close() {
            _windows.Close( this );
        }

    }

}
