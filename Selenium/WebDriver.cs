using Selenium.Core;
using Selenium.Internal;
using Selenium.Serializer;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Object through which the user controls the browser.
    /// </summary>
    /// <example>
    /// VBScript:
    /// <code lang="vbs">	
    /// Class Script
    ///     Dim driver
    ///     
    ///     Sub Class_Initialize
    ///         Set driver = CreateObject("Selenium.WebDriver")
    ///         driver.Start "firefox", "http://www.google.com"
    ///         driver.Get "/"
    ///     End Sub
    /// 
    ///     Sub Class_Terminate
    ///         driver.Quit
    ///     End Sub
    /// End Class
    /// 
    /// Set s = New Script
    /// </code>
    /// 
    /// VBA:
    /// <code lang="vbs">	
    /// Public Sub Script()
    ///   Dim driver As New WebDriver
    ///   driver.Start "firefox", "http://www.google.com"
    ///   driver.Get "/"
    ///   ...
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// </example>
    [ProgId("Selenium.WebDriver")]
    [Guid("0277FC34-FD1B-4616-BB19-E3CCFFAB4234")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [Description("Defines the interface through which the user controls the browser using WebDriver")]
    public class WebDriver : SearchContext, ComInterfaces._WebDriver, IDisposable {

        const string RUNNING_OBJECT_NAME = "Selenium.WebDriver";

        internal Capabilities Capabilities = new Capabilities();
        internal Dictionary Preferences = new Dictionary();
        internal List Extensions = new List();
        internal List Arguments = new List();
        internal string Profile = null;
        internal string Binary = null;
        internal bool Persistant = false;

        private Timeouts timeouts = new Timeouts();
        private IDriverService _service = null;
        private RemoteSession _session = null;
        private string _baseUrl = null;
        private Proxy _proxy = null;
        private COMRunningObject _comRunningObj = null;

        /// <summary>
        /// Creates a new WebDriver object. 
        /// </summary>
        public WebDriver() {
            UnhandledException.Initialize();
            SysWaiter.Initialize();
            RegisterRunningObject();
            COMDisposable.Subscribe(this, typeof(ComInterfaces._WebDriver));
        }

        public WebDriver(string browser)
            : this() {
            this.Capabilities.BrowserName = browser;
        }

        ~WebDriver() {
            this.Dispose();
        }

        /// <summary>
        /// Release the resources.
        /// </summary>
        public void Dispose() {
            if (_comRunningObj != null) {
                _comRunningObj.Dispose();
                _comRunningObj = null;
            }
            if (_service != null) {
                _service.Dispose();
                _service = null;
            }
            if (_session != null) {
                _session = null;
            }
        }

        private void RegisterRunningObject(){
            if (_comRunningObj == null)
                _comRunningObj = new COMRunningObject(this, RUNNING_OBJECT_NAME);
        }

        #region Setup


        /// <summary>
        /// 
        /// </summary>
        public Proxy Proxy {
            get {
                return _proxy ?? (_proxy = new Proxy(Capabilities));
            }
        }


        /// <summary>
        /// Set a specific profile for the firefox webdriver
        /// </summary>
        /// <param name="nameOrDirectory">Profil name (Firefox only) or directory (Firefox and Chrome)</param>
        /// <param name="persistant">If true, the browser will be launched without a copy the profile (Firefox only)</param>
        /// <remarks>The profile directory can be copied from the user temp folder (run %temp%) before the WebDriver is stopped. It's also possible to create a new Firefox profile by launching firefox with the "-p" switch (firefox.exe -p).</remarks>
        /// <example>
        /// <code lang="vbs">
        ///   Dim driver As New Selenium.FirefoxDriver
        ///   driver.SetProfile "Selenium"  'Firefox only. Profile created by running "..\firefox.exe -p"
        ///   driver.Get "http://www.google.com"
        ///   ...
        /// </code>
        /// <code lang="vbs">
        ///   Dim driver As New Selenium.FirefoxDriver
        ///   driver.SetProfile "C:\MyProfil"   'For Chrome and Firefox only
        ///   driver.Get "http://www.google.com"
        ///   ...
        /// </code>
        /// </example>
        public void SetProfile(string nameOrDirectory, bool persistant = false) {
            Profile = nameOrDirectory;
            Persistant = persistant;
        }

        /// <summary>
        /// Set a specific preference for the firefox webdriver
        /// </summary>
        /// <param name="key">Preference key</param>
        /// <param name="value">Preference value</param>
        public void SetPreference(string key, object value) {
            Preferences[key] = JsonReader.Parse(value);
        }

        /// <summary>
        /// Set a specific capability for the webdriver
        /// </summary>
        /// <param name="key">Capability key</param>
        /// <param name="value">Capability value</param>
        public void SetCapability(string key, object value) {
            Capabilities[key] = JsonReader.Parse(value);
        }

        /// <summary>
        /// Add an extension to the browser (For Firefox and Chrome only)
        /// </summary>
        /// <param name="extensionPath">Path to the extension</param>
        public void AddExtension(string extensionPath) {
            Extensions.Add(new DriverExtension(extensionPath));
        }

        /// <summary>
        /// Add an argument to be appended to the command line to launch the browser.
        /// </summary>
        /// <param name="argument">Argument</param>
        public void AddArgument(string argument) {
            Arguments.Add(argument);
        }

        /// <summary>
        /// Set the path to the browser executable to use
        /// </summary>
        /// <param name="path">Full path</param>
        public void SetBinary(string path) {
            if (!File.Exists(path))
                throw new Errors.FileNotFoundError(path);
            this.Binary = path;
        }

        #endregion


        #region Session

        internal override RemoteSession session {
            get {
                if (_session == null)
                    throw new Errors.BrowserNotStartedError();
                return _session;
            }
        }

        internal override string uri {
            get {
                return string.Empty;
            }
        }

        /// <summary>
        /// Starts a new Selenium testing session
        /// </summary>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, phantomjs</param>
        /// <param name="baseUrl">The base URL</param>
        /// <example>
        /// <code lang="vbs">	
        ///     Dim driver As New WebDriver()
        ///     driver.Start "firefox", "http://www.google.com"
        ///     driver.Get "/"
        /// </code>
        /// </example>
        public void Start(string browser = null, string baseUrl = null) {
            try {
                browser = ExpendBrowserName(browser);
                switch (browser) {
                    case "firefox":
                        _service = FirefoxDriver.StartService(this);
                        break;
                    case "chrome":
                        _service = ChromeDriver.StartService(this);
                        break;
                    case "phantomjs":
                        _service = PhantomJSDriver.StartService(this);
                        break;
                    case "internet explorer":
                        _service = IEDriver.StartService(this);
                        break;
                    case "MicrosoftEdge":
                        _service = EdgeDriver.StartService(this);
                        break;
                    case "opera":
                        _service = OperaDriver.StartService(this);
                        break;
                    default:
                        throw new Errors.ArgumentError("Invalid browser name: {0}", browser);
                }

                this.Capabilities.BrowserName = browser;

                RegisterRunningObject();

                _session = new RemoteSession(_service.Uri, true, this.timeouts);
                _session.Start(this.Capabilities);

                if (!string.IsNullOrEmpty(baseUrl))
                    this.BaseUrl = baseUrl;
            } catch (SeleniumException) {
                throw;
            } catch (Exception ex) {
                throw new SeleniumException(ex);
            }
        }

        /// <summary>
        /// Starts remotely a new Selenium testing session
        /// </summary>
        /// <param name="executorUri">Remote executor address (ex : "http://localhost:4444/wd/hub")</param>
        /// <param name="browser">Name of the browser : firefox, ie, chrome, phantomjs, htmlunit, htmlunitwithjavascript, android, ipad, opera</param>
        /// <param name="version">Browser version</param>
        /// <param name="platform">Platform: WINDOWS, LINUX, MAC, ANDROID...</param>
        /// <example>
        /// <code lang="vbs">	
        ///     Dim driver As New WebDriver()
        ///     driver.StartRemotely "http://localhost:4444/wd/hub", "ie", 11
        ///     driver.Get "/"
        /// </code>
        /// </example>
        public void StartRemotely(string executorUri, string browser = null, string version = null, string platform = null) {
            try {
                browser = ExpendBrowserName(browser);
                switch (browser) {
                    case "firefox":
                        FirefoxDriver.ExtendCapabilities(this, true);
                        break;
                    case "chrome":
                        ChromeDriver.ExtendCapabilities(this, true);
                        break;
                    case "phantomjs":
                        PhantomJSDriver.ExtendCapabilities(this, true);
                        break;
                    case "internet explorer":
                        IEDriver.ExtendCapabilities(this, true);
                        break;
                    case "MicrosoftEdge":
                        EdgeDriver.ExtendCapabilities(this, true);
                        break;
                    case "opera":
                        OperaDriver.ExtendCapabilities(this, true);
                        break;
                }

                this.Capabilities.Platform = platform;
                this.Capabilities.BrowserName = browser;
                if (!string.IsNullOrEmpty(version))
                    this.Capabilities.BrowserVersion = version;

                _session = new RemoteSession(executorUri, false, this.timeouts);
                _session.Start(this.Capabilities);

                RegisterRunningObject();

            } catch (SeleniumException) {
                throw;
            } catch (Exception ex) {
                throw new SeleniumException(ex);
            }
        }

        /// <summary>
        /// Close the Browser and Dispose of WebDriver
        /// </summary>
        public virtual void Quit() {
            if (_session != null) {
                try {
                    if (_session.IsLocal) {
                        if (!(_service is FirefoxService))
                            _session.Delete();
                        _service.Quit(_session.server);
                    } else {
                        _session.Delete();
                    }
                } catch { }
            }

            this.Dispose();
        }

        private string ExpendBrowserName(string browser) {
            if (string.IsNullOrEmpty(browser)) {
                browser = this.Capabilities.BrowserName;
                if (string.IsNullOrEmpty(browser))
                    throw new Errors.ArgumentError("Browser not defined");
            }
            string browserName = browser.ToLower();
            switch (browserName) {
                case "firefox":
                case "ff":
                    return "firefox";
                case "chrome":
                case "cr":
                    return "chrome";
                case "phantomjs":
                case "pjs":
                    return "phantomjs";
                case "internet explorer":
                case "internetexplorer":
                case "iexplore":
                case "ie":
                    return "internet explorer";
                case "microsoft edge":
                case "microsoftedge":
                case "edge":
                    return "MicrosoftEdge";
                case "opera":
                case "op":
                    return "opera";
                default:
                    return browserName;
            }
        }

        #endregion


        #region Interfaces

        /// <summary>
        /// Manage the browser settings. Need to be defined before the browser is launched
        /// </summary>
        public Timeouts Timeouts {
            get {
                return this.timeouts;
            }
        }

        /// <summary>
        /// Instructs the driver to change its settings.
        /// </summary>
        public Manage Manage {
            get {
                return this.session.manage;
            }
        }

        /// <summary>
        /// Get the actions class
        /// </summary>
        /// <example>
        /// <code lang="vbs">	
        ///     WebDriver driver = New WebDriver()
        ///     driver.start "firefox", "http://www.google.com"
        ///     driver.get "/"
        ///     driver.Actions.keyDown(Keys.Control).sendKeys("a").perform
        /// </code>
        /// </example>
        public Actions Actions {
            get {
                return new Actions(this.session);
            }
        }

        /// <summary>
        /// TouchActions
        /// </summary>
        public TouchActions TouchActions {
            get {
                RemoteSession session = this.session;
                return new TouchActions(session, this.TouchScreen);
            }
        }

        /// <summary>
        /// Keyboard
        /// </summary>
        public Keyboard Keyboard {
            get {
                return this.session.keyboard;
            }
        }

        /// <summary>
        /// Mouse
        /// </summary>
        public Mouse Mouse {
            get {
                return this.session.mouse;
            }
        }

        /// <summary>
        /// TouchScreen
        /// </summary>
        public TouchScreen TouchScreen {
            get {
                return this.session.touchscreen;
            }
        }

        /// <summary>
        /// Keys
        /// </summary>
        public Keys Keys {
            get {
                return this.session.keyboard.Keys;
            }
        }

        #endregion


        #region Navigation

        /// <summary>
        /// Base URL to use a relative URL with Get
        /// </summary>
        public string BaseUrl {
            get {
                return _baseUrl;
            }
            set {
                _baseUrl = value.TrimEnd('/') + '/';
            }
        }

        /// <summary>
        /// Loads a web page in the current browser session. Same as Open method.
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="timeout">Optional timeout in milliseconds. Infinite=-1</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns>Return true if the url was openned within the timeout, false otherwise</returns>
        public bool Get(string url, int timeout = -1, bool raise = true) {
            if (_session == null)
                this.Start();
            RemoteSession session = _session;

            if (string.IsNullOrEmpty(url))
                throw new Errors.ArgumentError("Argument 'url' cannot be null.");

            if (timeout > 0)
                session.timeouts.PageLoad = timeout;

            int idx = url.IndexOf(":");
            if (idx == -1) {
                //relative url
                if (_baseUrl == null)
                    throw new Errors.ArgumentError("Base URL not defined. Define a base URL or use a full URL.");
                url = string.Concat(_baseUrl, url.TrimStart('/'));
            } else {
                //absolute url
                idx = url.IndexOf("://");
                if (idx != -1) {
                    idx = url.IndexOf('/', idx + 3);
                    if (idx != -1) {
                        _baseUrl = url.Substring(0, idx).TrimEnd('/') + '/';
                    } else {
                        _baseUrl = url.TrimEnd('/') + '/';
                    }
                }
            }

            try {
                session.Send(RequestMethod.POST, "/url", "url", url);
                return true;
            } catch {
                if (raise)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Get the URL the browser is currently displaying.
        /// </summary>
        /// <returns>Current URL</returns>
        public string Url {
            get {
                return (string)session.Send(RequestMethod.GET, "/url");
            }
        }

        /// <summary>
        /// Gets the title of the current browser window.
        /// </summary>
        /// <returns>Title of the window</returns>
        public string Title {
            get {
                return session.windows.CurrentWindow.Title;
            }
        }

        /// <summary>
        /// Goes one step backward in the browser history.
        /// </summary>
        public void GoBack() {
            this.session.Send(RequestMethod.POST, "/back");
        }

        /// <summary>
        /// Goes one step forward in the browser history.
        /// </summary>
        public void GoForward() {
            this.session.Send(RequestMethod.POST, "/forward");
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        public void Refresh() {
            this.session.Send(RequestMethod.POST, "/refresh");
        }

        #endregion


        #region Windows

        /// <summary>
        /// Gets an object allowing the user to manipulate the currently-focused browser window.
        /// </summary>
        public Window Window {
            get {
                return session.windows.CurrentWindow;
            }
        }

        /// <summary>
        /// Gets the window handles of open browser windows.
        /// </summary>
        /// <returns><see cref="List" /></returns>
        public List Windows {
            get {
                List windows, handles;
                session.windows.ListWindows(out windows, out handles);
                return windows;
            }
        }

        /// <summary>
        /// Closes the current window.
        /// </summary>
        public void Close() {
            this.Window.Close();
        }

        #endregion


        #region Interaction

        /// <summary>
        /// Sends a sequence of keystrokes to the browser.
        /// </summary>
        /// <param name="keysOrModifier">Sequence of keys or a modifier key(Control, Shift or Alt) if the sequence is in keysToSendEx</param>
        /// <param name="keys">Optional - Sequence of keys if keysToSend contains modifier key(Control,Shift...)</param>
        /// <example>
        /// To Send mobile to the window :
        /// <code lang="vbs">
        ///     driver.SendKeys "mobile"
        /// </code>
        /// To Send ctrl+a to the window :
        /// <code lang="vbs">
        ///     driver.SendKeys Keys.Control, "a"
        /// </code>
        /// </example>
        public void SendKeys(string keysOrModifier, string keys = null) {
            this.session.keyboard.SendKeys(keysOrModifier, keys);
        }

        #endregion


        #region Screenshot

        /// <summary>
        /// Takes the screenshot of the current window
        /// </summary>
        /// <param name="delay">Time to wait before taking the screenshot in milliseconds</param>
        /// <returns><see cref="Image" /></returns>
        public Image TakeScreenshot(int delay = 0) {
            if (delay != 0)
                SysWaiter.Wait(delay);
            Image image = (Image)session.Send(RequestMethod.GET, "/screenshot");
            return image;
        }

        #endregion


        #region Javascript

        /// <summary>
        /// Executes JavaScript in the context of the currently selected frame or window
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="arguments">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        public object ExecuteScript(string script, object arguments = null) {
            object args = FormatArguments(arguments);
            object result = session.javascript.Execute(script, args, true);
            return result;
        }

        /// <summary>
        /// Waits for a script to return true or not null.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="arguments"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public object WaitForScript(string script, object arguments, int timeout = -1) {
            object args = FormatArguments(arguments);
            object result = session.javascript.WaitFor(script, args, timeout);
            return result;
        }

        private static object FormatArguments(object arguments) {
            if (arguments == null) {
                return new object[0];
            } else if (arguments is IEnumerable && !(arguments is string || arguments is Dictionary)) {
                return arguments;
            } else {
                return new object[] { arguments };
            }
        }

        #endregion


        #region Find Elements

        /// <summary>
        /// Returns the element with focus, or BODY if nothing has focus.
        /// </summary>
        /// <returns></returns>
        public WebElement ActiveElement() {
            return WebElement.GetActiveWebElement(session);
        }

        #endregion


        #region PageSource

        /// <summary>
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        public string PageSource() {
            var result = session.Send(RequestMethod.GET, "/source");
            return (string)result;
        }

        /// <summary>
        /// Returns the first occurence matching the regular expression.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="group">Optional - Group number (Zero based)</param>
        /// <returns>String</returns>
        public string PageSourceMatch(string pattern, short group = 0) {
            var result = this.PageSource().Match(pattern, group);
            return result;
        }

        /// <summary>
        /// Returns all the occurences matching the regular expression.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="group">Optional - Group number (Zero based)</param>
        /// <returns>Array of strings or null</returns>
        public List PageSourceMatches(string pattern, short group = 0) {
            var result = this.PageSource().Matches(pattern, group);
            return result;
        }

        #endregion


        #region Context

        /// <summary>
        /// Select either the first frame on the page or the main document when a page contains iFrames.
        /// </summary>
        /// <returns>A WebDriver instance focused on the default frame.</returns>
        public void SwitchToDefaultContent() {
            this.session.frame.SwitchToDefaultContent();
        }

        /// <summary>
        /// Switch focus to the specified window by name.
        /// </summary>
        /// <param name="name">The name of the window to activate</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns>Current web driver</returns>
        public Window SwitchToWindowByName(string name, int timeout = -1, bool raise = true) {
            try {
                return session.windows.SwitchToWindowByName(name, timeout);
            } catch (Errors.NoSuchWindowError) {
                if (raise)
                    throw new Errors.NoSuchWindowError(name);
                return null;
            }
        }

        /// <summary>
        /// Switch focus to the specified window by title.
        /// </summary>
        /// <param name="title">The title of the window to activate</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns>Current web driver</returns>
        public Window SwitchToWindowByTitle(string title, int timeout = -1, bool raise = true) {
            try {
                return session.windows.SwitchToWindowByTitle(title, timeout);
            } catch (Errors.NoSuchWindowError) {
                if (raise)
                    throw new Errors.NoSuchWindowError(title);
                return null;
            }
        }

        /// <summary>
        /// Switch the focus to the next window.
        /// </summary>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true. Default is true.</param>
        /// <returns>Window</returns>
        public Window SwitchToNextWindow(int timeout = -1, bool raise = true) {
            try {
                return session.windows.SwitchToNextWindow(timeout);
            } catch (Errors.NoSuchWindowError) {
                if (raise)
                    throw;
                return null;
            }
        }

        /// <summary>
        /// Switch the focus to the previous window
        /// </summary>
        /// <returns>Window</returns>
        public Window SwitchToPreviousWindow() {
            return session.windows.SwitchToPreviousWindow();
        }

        /// <summary>
        /// Switch focus to the specified frame, by index(zero based), name or WebElement.
        /// </summary>
        /// <param name="identifier">The name, index(zero based) or WebElement</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns>Current web driver</returns>
        public bool SwitchToFrame(object identifier, int timeout = -1, bool raise = true) {
            try {
                this.session.frame.SwitchToFrame(identifier, timeout);
            } catch (Errors.NoSuchFrameError) {
                if (raise)
                    throw new Errors.NoSuchFrameError(identifier);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Select the parent frame of the currently selected frame.
        /// </summary>
        /// <returns>The WebDriver instance focused on the specified frame.</returns>
        public void SwitchToParentFrame() {
            this.session.frame.SwitchToParentFrame();
        }

        /// <summary>
        /// Switch focus to an alert on the page.
        /// </summary>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <returns>Focused alert</returns>
        public Alert SwitchToAlert(int timeout = -1, bool raise = true) {
            try {
                return Alert.SwitchToAlert(session, timeout);
            } catch (Errors.NoAlertPresentError) {
                if (raise)
                    throw;
                return null;
            }
        }

        #endregion


        #region Wait methods

        /// <summary>
        /// Wait the specified time in millisecond before executing the next command
        /// </summary>
        /// <param name="timems">Time to wait in millisecond</param>
        public void Wait(int timems) {
            SysWaiter.Wait(timems);
        }

        /// <summary>
        /// Waits for the delegate function to return not null or true
        /// </summary>
        /// <typeparam name="T">Returned object or boolean</typeparam>
        /// <param name="func">Delegate taking the web driver instance as argument</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns>Variant or boolean</returns>
        public T Until<T>(Func<WebDriver, T> func, int timeout = -1) {
            if (timeout == -1)
                timeout = session.timeouts.timeout_implicitwait;
            return Waiter.WaitUntil(func, this, timeout, 80);
        }

        /// <summary>
        /// Waits for a function to return true. VBScript: Function WaitEx(webdriver), VBA: Function WaitEx(webdriver As WebDriver) As Boolean 
        /// </summary>
        /// <param name="procedure">Function reference.  VBScript: wd.WaitFor GetRef(\"WaitEx\")  VBA: wd.WaitFor AddressOf WaitEx)</param>
        /// <param name="argument">Optional - Argument to send to the function</param>
        /// <param name="timeout">Optional - timeout in milliseconds</param>
        /// <returns>Current WebDriver</returns>
        /// VBA example:
        /// <example><code lang="vbs">	
        /// Sub WaitForTitle(driver, argument, result)
        ///     result = driver.Title = argument
        /// End Sub
        /// 
        /// Sub testSimple()
        ///     Dim driver As New FirefoxDriver
        ///     driver.Get "http://www.google.com"
        ///     driver.Until AddressOf WaitForTitle, "Google"
        ///     ...
        /// End Sub
        /// </code>
        /// </example>        
        /// 
        /// VBScript example:
        /// <example><code lang="vbs">	
        /// Function WaitForTitle(driver, argument)
        ///     WaitForTitle = driver.Title = argument
        /// End Function
        /// 
        /// Sub testSimple()
        ///     Dim driver As New FirefoxDriver
        ///     driver.Get "http://www.google.com"
        ///     driver.Until GetRef("WaitForTitle"), "Google", 1000
        ///     ...
        /// End Sub
        /// </code>
        /// </example>
        object ComInterfaces._WebDriver.Until(object procedure, object argument, int timeout) {
            if (timeout == -1)
                timeout = session.timeouts.timeout_implicitwait;
            return COMExt.WaitUntilProc(procedure, this, argument, timeout);
        }

        #endregion


        #region Cache

        /// <summary>
        /// Get the status of the html5 application cache.
        /// </summary>
        /// <returns>{number} Status code for application cache: {UNCACHED = 0, IDLE = 1, CHECKING = 2, DOWNLOADING = 3, UPDATE_READY = 4, OBSOLETE = 5}</returns>
        public CacheState CacheStatus() {
            return (CacheState)session.Send(RequestMethod.GET, "/application_cache/status");
        }

        #endregion


        #region Clipboard

        /// <summary>
        /// Sets the text in the Clipboard
        /// </summary>
        /// <param name="text">Text</param>
        public void SetClipBoard(string text) {
            ClipboardExt.SetText(text);
        }

        /// <summary>
        /// Returns the text from the Clipboard
        /// </summary>
        public string GetClipBoard() {
            return ClipboardExt.GetText();
        }

        #endregion
    }

}
