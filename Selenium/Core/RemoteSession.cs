using System;

namespace Selenium.Core {

    class RemoteSession {

        public readonly bool IsLocal;
        public readonly RemoteServer server;
        public readonly Mouse mouse;
        public readonly Keyboard keyboard;
        public readonly WindowContext windows;
        public readonly JavascriptContext javascript;
        public readonly Timeouts timeouts;
        public readonly Logs logs;
        public readonly FrameContext frame;
        public readonly Manage manage;
        public readonly TouchScreen touchscreen;
        public Dictionary capabilities;

        private string _id = null;
        private string _uri = null;

        public RemoteSession(string address, bool islocal, Timeouts timeouts) {
            this.IsLocal = islocal;
            this.windows = new WindowContext(this);
            this.mouse = new Mouse(this);
            this.keyboard = new Keyboard(this);
            this.javascript = new JavascriptContext(this);
            this.timeouts = timeouts;
            this.logs = new Logs(this);
            this.frame = new FrameContext(this);
            this.manage = new Manage(this);
            this.touchscreen = new TouchScreen(this);
            this.server = new RemoteServer(address, islocal, timeouts.Server);
        }

        public string Id {
            get { return _id; }
        }

        /// <summary>
        /// Starts a new session.
        /// </summary>
        /// <param name="desired_capabilities">An object describing the session's desired capabilities.</param>
        /// <param name="requiredCapabilities">An object describing the session's required capabilities (Optional).</param>
        /// <returns>{object} An object describing the session's capabilities.</returns>
        public void Start(Dictionary desired_capabilities, Dictionary requiredCapabilities = null) {
            var param = new Dictionary();
            param.Add("desiredCapabilities", desired_capabilities);
            if (requiredCapabilities != null)
                param.Add("requiredCapabilities", requiredCapabilities);

            var response = (Dictionary)server.Send(RequestMethod.POST, "/session", param);
            try {
                if( response.ContainsKey("sessionId") ) {
                    _id = (string)response["sessionId"];
                    this.capabilities = (Dictionary)response["value"];
                } else { 
                    Dictionary value = (Dictionary)response["value"];
                    _id = (string)value["sessionId"];
                    this.capabilities = (Dictionary)value["capabilities"];
                }
                _uri = "/session/" + _id;
            } catch (Errors.KeyNotFoundError ex) {
                throw new DeserializeException(typeof(RemoteSession), ex);
            }

            this.timeouts.SetSession(this);
        }

        /// <summary>
        /// Delete the session.
        /// </summary>
        public void Delete() {
            if (_id == null)
                return;
            Send(RequestMethod.DELETE);
            _id = null;
        }

        public string UploadFile(string localFile) {
            try {
                using (var zip = new Zip.ZipFile()) {
                    zip.AddFile(localFile);
                    string result = (string)Send(RequestMethod.POST, "/uploadFile", "file", zip);
                    return result;
                }
            } catch {
                return localFile;
            }
        }

        internal DateTime GetEndTime(int timeout) {
            DateTime time = DateTime.UtcNow.AddMilliseconds(
                timeout == -1 ? timeouts.timeout_implicitwait : timeout);
            return time;
        }

        /// <summary>
        /// Execute the method and relativeUri on the current session
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public object Send(RequestMethod method) {
            return Send(method, null, (Dictionary)null);
        }

        /// <summary>
        /// Execute the method and relativeUri on the current session
        /// </summary>
        /// <param name="method"></param>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        public object Send(RequestMethod method, string relativeUri = null) {
            return Send(method, relativeUri, (Dictionary)null);
        }

        /// <summary>
        /// Execute the method and relativeUri on the current session with 1 parameter
        /// </summary>
        /// <param name="method"></param>
        /// <param name="relativeUri"></param>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Send(RequestMethod method, string relativeUri, string param, object value) {
            Dictionary data = new Dictionary();
            data.Add(param, value);
            return Send(method, relativeUri, data);
        }

        /// <summary>
        /// Execute the method and relativeUri on the current session with 2 parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="relativeUri"></param>
        /// <param name="param1"></param>
        /// <param name="value1"></param>
        /// <param name="param2"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public object Send(RequestMethod method, string relativeUri, string param1, object value1, string param2, object value2) {
            Dictionary data = new Dictionary();
            data.Add(param1, value1);
            data.Add(param2, value2);
            return Send(method, relativeUri, data);
        }

        /// <summary>
        /// Execute the method and relativeUri on the current session with 3 parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="relativeUri"></param>
        /// <param name="param1"></param>
        /// <param name="value1"></param>
        /// <param name="paran2"></param>
        /// <param name="value2"></param>
        /// <param name="param3"></param>
        /// <param name="value3"></param>
        /// <returns></returns>
        public object Send(RequestMethod method, string relativeUri, string param1, object value1, string paran2, object value2, string param3, object value3) {
            Dictionary data = new Dictionary();
            data.Add(param1, value1);
            data.Add(paran2, value2);
            data.Add(param3, value3);
            return Send(method, relativeUri, data);
        }

        /// <summary>
        /// Execute the method and relativeUri on the current session with 4 parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="relativeUri"></param>
        /// <param name="param1"></param>
        /// <param name="value1"></param>
        /// <param name="param2"></param>
        /// <param name="value2"></param>
        /// <param name="param3"></param>
        /// <param name="value3"></param>
        /// <param name="param4"></param>
        /// <param name="value4"></param>
        /// <returns></returns>
        public object Send(RequestMethod method, string relativeUri, string param1, object value1, string param2, object value2, string param3, object value3, string param4, object value4) {
            Dictionary data = new Dictionary();
            data.Add(param1, value1);
            data.Add(param2, value2);
            data.Add(param3, value3);
            data.Add(param4, value4);
            return Send(method, relativeUri, data);
        }

        public object Send(RequestMethod method, string relativeUri, Dictionary data) {
            string uri = relativeUri == null ? _uri : (_uri + relativeUri);
            Dictionary result = server.Send(method, uri, data);
            if (result == null)
                return null;
            return result.Get("value", null);
        }

        public object SendAgain() {
            Dictionary result = server.SendAgain();
            return result.Get("value", null);
        }

        /// <summary>
        /// Repeatedly call the send function until the predicate function returns true
        /// or until the timeout is reached.
        /// </summary>
        /// <param name="timeout">Timeout in ms</param>
        /// <param name="sendfn">Send function</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns>Result of the send function</returns>
        internal T SendUntil<T>(int timeout, Func<T> sendfn, Func<T, bool> predicate) {
            DateTime endTime = GetEndTime(timeout);
            bool retry = false;
            while (true) {
                T result;
                if (retry) {
                    result = (T)SendAgain();
                } else {
                    retry = true;
                    result = sendfn();
                }
                if (predicate(result))
                    return result;
                if (DateTime.UtcNow > endTime)
                    throw new Errors.TimeoutError(timeout);
                SysWaiter.Wait();
            }
        }

    }

}
