using Selenium.Core;
using Selenium.Internal;
using Selenium.Serializer;
using Selenium.ComInterfaces;
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
    ///   Dim driver
    ///   Set driver = CreateObject("Selenium.WebDriver")
    ///   driver.Start "firefox", "http://www.google.com"
    ///   driver.Get "/"
    ///   ...
    ///   driver.Quit
    /// </code>
    /// 
    /// <code lang="VB">	
    ///   Dim driver As New WebDriver
    ///   driver.Start "firefox", "http://www.google.com"
    ///   driver.Get "/"
    ///   ...
    ///   driver.Quit
    /// </code>
    /// </example>
    [ProgId("Selenium.WebDriver")]
    [Guid("0277FC34-FD1B-4616-BB19-E3CCFFAB4234")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [Description("Defines the interface through which the user controls the browser using WebDriver")]
    public class WebDriver : SearchContext, ComInterfaces._WebDriver, IDisposable {

        const string RUNNING_OBJECT_NAME = "Selenium.WebDriver";

        // New API https://w3c.github.io/webdriver/#endpoints are used with
        // Gecko driver which does not support many endpoints and payloads used in previous implementation
        // Other drivers still used in the legacy mode
        internal static bool LEGACY = true; 

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
            WebDriver.LEGACY = true;
            UnhandledException.Initialize();
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
        /// Set a specific profile directory or a named profile for the firefox webdriver
        /// </summary>
        /// <param name="nameOrDirectory">Profile name (Firefox only) or directory (Edge, Chrome)</param>
        /// <param name="persistant">If true, the browser will be launched without a copy the profile (Firefox only)</param>
        /// <remarks>
        /// The profile directory cannot be a symlink or an UNC path. It has to be a real physical local directory.
        /// </remarks>
        /// <remarks>
        /// It's possible to pre-create a new Firefox profile by launching firefox with the "-p" switch (firefox.exe -p).
        /// </remarks>
        /// <example>
        /// <code lang="vb">
        ///   Dim driver As New Selenium.FirefoxDriver
        ///   driver.SetProfile "C:\MyProfile"   ' the directory of a persistant profile
        ///   driver.Get "http://www.google.com"
        ///   ...
        /// </code>
        /// <code lang="vb">
        ///   Dim driver As New Selenium.FirefoxDriver
        ///   driver.SetProfile "Selenium"  'Firefox only. Profile created by running "..\firefox.exe -p"
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
            Preferences[key] = JSON.Parse(value);
        }

        /// <summary>
        /// Set a specific capability for the webdriver
        /// </summary>
        /// <param name="key">Capability key</param>
        /// <param name="value">Capability value</param>
        public void SetCapability(string key, object value) {
            Capabilities[key] = JSON.Parse(value);
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
        /// Starts a new Selenium session
        /// </summary>
        /// <param name="browser">Name of the browser: chrome, edge, gecko, firefox, ie, phantomjs, opera</param>
        /// <param name="baseUrl">The base URL</param>
        /// <exception cref="SeleniumException">When the session start failed</exception>
        /// <example>
        /// <code lang="VB">	
        ///     Dim driver As New WebDriver()
        ///     driver.Start "gecko", "http://www.google.com"
        ///     driver.Get "/"
        /// </code>
        /// <code lang="vbs">	
        ///     Dim driver
        ///     ﻿Set driver = CreateObject("Selenium.WebDriver")
        ///     driver.Start "gecko"
        ///     driver.Get "http://www.google.com/"
        /// </code>
        /// </example>
        /// <remarks>
        /// This method executes the driver process. The driver executable has to be in the same folder as 
        /// the registered SeleniumBasic.dll assembly
        /// 
        /// firefox is an old browser version driver which works via the plugin firefoxdriver.xpi
        /// gecko is for a modern FireFox browser.
        /// 
        /// edge and chrome require the driver version to be exactly matched the current browser version
        /// 
        /// ie, phantomjs, opera drivers were not tested in this release and thus could be considered as not supported
        /// </remarks>
        public void Start(string browser = null, string baseUrl = null) {
            try {
                browser = ExpendBrowserName(browser);
                switch (browser) {
                    case "gecko":
                        _service = GeckoDriver.StartService(this);
                        break;
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
                if (string.IsNullOrEmpty(this.Capabilities.BrowserName))
                    this.Capabilities.BrowserName = browser;
                if( LEGACY )
                    this.Capabilities.UnexpectedAlertBehaviour = "ignore";

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
        /// Starts a new Selenium session attached to a remotely started driver process
        /// </summary>
        /// <param name="executorUri">Remote executor address (ex : "http://localhost:4444/wd/hub")</param>
        /// <param name="browser">Name of the browser: gecko, firefox, chrome, edge, ie, phantomjs, htmlunit, htmlunitwithjavascript, android, ipad, opera</param>
        /// <param name="version">Optional Browser version</param>
        /// <param name="platform">Optional Platform: WINDOWS, LINUX, MAC, ANDROID...</param>
        /// <example>
        /// <code lang="vb">
        ///     Dim driver As New WebDriver()
        ///     driver.StartRemotely "http://localhost:4444/wd/hub"
        ///     driver.Get "/"
        /// </code>
        /// </example>
        /// <remarks>
        /// This could be useful for debugging. Start the driver process manually in the verbose mode, like:
        /// geckodriver.exe -vv
        /// or
        /// chromedriver.exe --verbose
        /// A custom connection port also could be specified as a driver's command line parameter
        /// </remarks>
        public void StartRemotely(string executorUri, string browser = null, string version = null, string platform = null) {
            try {
                browser = ExpendBrowserName(browser);
                switch (browser) {
                    case "firefox":
                        FirefoxDriver.ExtendCapabilities(this, true);
                        break;
                    case "gecko":
                        GeckoDriver.ExtendCapabilities(this, true);
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
                if (string.IsNullOrEmpty(this.Capabilities.BrowserName))
                    this.Capabilities.BrowserName = browser;
                if (!string.IsNullOrEmpty(version))
                    this.Capabilities.BrowserVersion = version;
                this.Capabilities.UnexpectedAlertBehaviour = "ignore";

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
                case "gecko":
                    return "gecko";
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

        /// <summary>
        /// Sends a custom command to the driver process
        /// </summary>
        /// <param name="method">POST, GET or DELETE</param>
        /// <param name="relativeUri">Relative URI. Ex: "/screenshot"</param>
        /// <param name="param1">Optional</param>
        /// <param name="value1"></param>
        /// <param name="param2">Optional</param>
        /// <param name="value2"></param>
        /// <param name="param3">Optional</param>
        /// <param name="value3"></param>
        /// <param name="param4">Optional</param>
        /// <param name="value4"></param>
        /// <returns>Result</returns>
        /// <example>
        /// <code lang="vbs">
        ///     ' collect all links of the page
        ///     Set links = driver.Send("POST", "/elements", "using", "css selector", "value", "a")
        /// </code>
        /// <code lang="vbs">
        ///     ' "print" the page and get a binary as a result (works only in gecko)
        ///     dim base64
        ///     base64 = driver.Send( "POST", "/print", "shrinkToFit", true )
        /// </code>
        /// </example>
        public object Send(string method, string relativeUri,
                                 string param1 = null, object value1 = null,
                                 string param2 = null, object value2 = null,
                                 string param3 = null, object value3 = null,
                                 string param4 = null, object value4 = null) {

            RequestMethod mth = 0;
            switch (method.ToUpper()) {
                case "POST": mth = RequestMethod.POST; break;
                case "GET": mth = RequestMethod.GET; break;
                case "DELETE": mth = RequestMethod.DELETE; break;
                default:
                    throw new Errors.ArgumentError("Unhandled method: " + method);
            }

            Dictionary data = null;
            if (param1 != null) {
                data = new Dictionary();
                data.Add(param1, value1);
                if (param2 != null) {
                    data.Add(param2, value2);
                    if (param3 != null) {
                        data.Add(param3, value3);
                        if (param4 != null) {
                            data.Add(param4, value3);
                        }
                    }
                }
            }
            object result = session.Send(mth, relativeUri, data);
            return result;
        }


        #endregion


        #region Interfaces

        /// <summary>
        /// Manage the <see cref="Selenium.Timeouts"/>. Need to be defined before the browser is launched
        /// </summary>
        public Timeouts Timeouts {
            get {
                return this.timeouts;
            }
        }

        /// <summary>
        /// Instructs the driver to <see cref="Selenium.Manage"/> its settings.
        /// </summary>
        public Manage Manage {
            get {
                return this.session.manage;
            }
        }

        /// <summary>
        /// Get the <see cref="Selenium.Actions"/> class
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
        /// Get the <see cref="Selenium.TouchActions"/> class
        /// </summary>
        public TouchActions TouchActions {
            get {
                RemoteSession session = this.session;
                return new TouchActions(session, this.TouchScreen);
            }
        }

        /// <summary>
        /// <see cref="Selenium.Keyboard"/>Keyboard
        /// </summary>
        public Keyboard Keyboard {
            get {
                return this.session.keyboard;
            }
        }

        /// <summary>
        /// <see cref="Selenium.Mouse"/>
        /// </summary>
        public Mouse Mouse {
            get {
                return this.session.mouse;
            }
        }

        /// <summary>
        /// <see cref="Selenium.TouchScreen"/>
        /// </summary>
        public TouchScreen TouchScreen {
            get {
                return this.session.touchscreen;
            }
        }

        /// <summary>
        /// <see cref="Selenium.Keys"/>
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
                _baseUrl = value.TrimEnd('/');
            }
        }

        /// <summary>
        /// Performs a GET request to load a web page in the current browser session. 
        /// </summary>
        /// <param name="url">URL (could be relative, see <see cref="BaseUrl"/>)</param>
        /// <param name="timeout">Optional timeout in milliseconds. see </param>
        /// <param name="raise">Optional - Raise an exception after the timeout when true</param>
        /// <exception cref="Exception">Thrown when the timeout has reached</exception>
        /// <returns>Return true if the url was openned within the timeout, false otherwise (when the raise param was false)</returns>
        public bool Get(string url, int timeout = -1, bool raise = true) {
            if (_session == null)
                this.Start();
            RemoteSession session = _session;

            if (string.IsNullOrEmpty(url))
                throw new Errors.ArgumentError("Argument 'url' cannot be null.");
            int idx = url.IndexOf("/");
            if (idx == 0) {
                //relative url
                if (_baseUrl == null)
                    throw new Errors.ArgumentError("Base URL not defined. Define a base URL or use a full URL.");
                url = string.Concat(_baseUrl, url);
            } else {
                //absolute url
                idx = url.IndexOf('/', idx + 3);
                if (idx != -1) {
                    _baseUrl = url.Substring(0, idx - 1);
                } else {
                    _baseUrl = url;
                }
            }

            if (timeout > 0){
                Timeouts.SendTimeoutPageLoad(session, timeout);
            }
            try {
                session.Send(RequestMethod.POST, "/url", "url", url);
                return true;
            } catch {
                if (raise)
                    throw;
                return false;
            } finally {
                if (timeout > 0){
                    Timeouts.SendTimeoutPageLoad(session, session.timeouts.timeout_pageload);
                }
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
            if( WebDriver.LEGACY )
                this.session.Send(RequestMethod.POST, "/back");
            else
                this.session.Send(RequestMethod.POST, "/back", new Dictionary());
        }

        /// <summary>
        /// Goes one step forward in the browser history.
        /// </summary>
        public void GoForward() {
            if( WebDriver.LEGACY )
                this.session.Send(RequestMethod.POST, "/forward");
            else
                this.session.Send(RequestMethod.POST, "/forward", new Dictionary());
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        public void Refresh() {
            if( WebDriver.LEGACY )
                this.session.Send(RequestMethod.POST, "/refresh");
            else
                this.session.Send(RequestMethod.POST, "/refresh", new Dictionary());
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
        /// <remarks>Try to not overuse, since the windows enumeration is a heavy process.</remarks>
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
        /// Execute a piece of JavaScript in the context of the currently selected frame or window
        /// </summary>
        /// <param name="script">The JavaScript code to execute in the page context.</param>
        /// <param name="arguments">The arguments array available to the script.</param>
        /// <returns>The value specified by the script's return statement.</returns>
        /// <example>
        /// <code lang="vb">
        ///     txt = driver.ExecuteScript("return 'xyz' + arguments[0];", "123")
        /// </code>
        /// </example>
        public object ExecuteScript(string script, object arguments = null) {
            object args_ex = FormatArguments(arguments);
            object result = session.javascript.Execute(script, args_ex, true);
            return result;
        }

        /// <summary>
        /// Execute an asynchronous piece of JavaScript in the context of the current frame or window.
        /// </summary>
        /// <param name="script">JavaScript code which involves asynchronous operations such as promises, etc.</param>
        /// <param name="arguments">Optional arguments array available for the script.</param>
        /// <param name="timeout">Optional timeout in milliseconds.</param>
        /// <returns>The first argument of the function callback() the script should call to return the result.</returns>
        /// <example>
        /// <code lang="vb">
        ///     txt = driver.ExecuteAsyncScript("callback('xyz')");"
        /// </code>
        /// </example>
        public object ExecuteAsyncScript(string script, object arguments = null, int timeout = -1) {
            object args_ex = FormatArguments(arguments);
            object result = session.javascript.ExecuteAsync(script, args_ex, true, timeout);
            return result;
        }

        /// <summary>
        /// Waits for a piece of JavaScript to return true or not null.
        /// </summary>
        /// <param name="script">Piece of JavaScript code to execute.</param>
        /// <param name="arguments">Optional arguments for the script.</param>
        /// <param name="timeout">Optional timeout in milliseconds.</param>
        /// <returns>Value not null</returns>
        /// <example lang="vb">
        /// driver.WaitForScript "document.readyState == 'complete';"
        /// </example>
        public object WaitForScript(string script, object arguments, int timeout = -1) {
            object args_ex = FormatArguments(arguments);
            object result = session.javascript.WaitFor(script, args_ex, timeout);
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
            const string JS = "return document.body.innerHTML.match(/{0}/)[{1}]";
            string code = string.Format(JS, pattern, group);
            object result = session.javascript.Execute(code, null, false);
            return (string)result;
        }

        /// <summary>
        /// Returns all the occurences matching the regular expression.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="group">Optional - Group number (Zero based)</param>
        /// <returns>Array of strings or null</returns>
        public List PageSourceMatches(string pattern, short group = 0) {
            const string JS = "var r=/{0}/g,s=document.body.innerHTML,a=[],m;"
                            + "while(m=r.exec(s))a.push(m[{1}]);return a;";
            string code = string.Format(JS, pattern, group);
            object result = session.javascript.Execute(code, null, false);
            return (List)result;
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
        /// <returns><see cref="Selenium.Window"/></returns>
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
        /// <returns><see cref="Selenium.Window"/></returns>
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

                if( !WebDriver.LEGACY ) {
                    var name = identifier as string;
                    if( name != null ) {
                        string css = "frame[name='" + name + "'],iframe[name='" + name + "'],frame#" + name + ",iframe#" + name;
                        WebElement fr_elem = FindElementBy(Strategy.Css, css, timeout, raise);
                        if( fr_elem == null )
                            throw new Errors.NoSuchFrameError();
                        identifier = fr_elem;
                    }
                }

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
        /// <example>
        /// <code lang="VB">	
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
        /// <code lang="vbs">	
        /// Function WaitForTitle(driver, argument)
        ///     WaitForTitle = driver.Title = argument
        /// End Function
        /// 
        ///     driver.Get "http://www.google.com"
        ///     driver.Until GetRef("WaitForTitle"), "Google", 1000
        ///     ...
        /// </code>
        /// </example>
        object _WebDriver.Until(object procedure, object argument, int timeout) {
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
