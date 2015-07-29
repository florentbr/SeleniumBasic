using Selenium.Serializer;
using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;

namespace Selenium.Core {

    class RemoteServer {

        const string JSON_MIME_TYPE = "application/json";
        const string HEADER_CONTENT_TYPE = "application/json;charset=utf-8";
        const string REQUEST_ACCEPT_HEADER = "application/json;, image/png";

        private int _response_timeout = 30000;
        private readonly string _server_uri;

        private HttpWebRequest _request;
        private RequestMethod _request_method;
        private string _request_uri;
        private JsonWriter _request_data;
        private Dictionary _response_content;
        private Exception _response_exception;

        private EventWaitHandle _event_send;
        private EventWaitHandle _event_received;
        private Thread _thread;
        private bool _disposed;


        internal RemoteServer(string serverAddress, bool isLocal) {
            _server_uri = serverAddress.TrimEnd('/');
            HttpWebRequest.DefaultCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
            HttpWebRequest.DefaultMaximumResponseHeadersLength = -1;
            if (isLocal)
                HttpWebRequest.DefaultWebProxy = null;
            ServicePointManager.Expect100Continue = false;

            _event_send = new EventWaitHandle(false, EventResetMode.ManualReset);
            _event_received = new EventWaitHandle(false, EventResetMode.ManualReset);
            _thread = new Thread(RunRequests);
            _thread.IsBackground = true;
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
        }

        ~RemoteServer() {
            this.Dispose();
        }

        public void Dispose() {
            _disposed = true;
            if (_request != null){
                _request.Abort();
                _request = null;
            }
            if(_thread != null){
                _thread.Interrupt();
                _thread = null;
            }
            _event_send.Close();
        }

        /// <summary>
        /// Sets the server timeout.
        /// </summary>
        public int Timeout {
            get {
                return _response_timeout;
            }
            set {
                _response_timeout = value;
            }
        }

        /// <summary>
        /// Query the server's current status.
        /// </summary>
        /// <returns>{object} An object describing the general status of the server.</returns>
        public Dictionary GetStatus() {
            var response = Send(RequestMethod.GET, "/status", (Dictionary)null);
            return (Dictionary)response["value"];
        }

        /// <summary>
        /// Returns a list of the currently active sessions.
        /// </summary>
        /// <returns>A list of the currently active sessions.</returns>
        public List GetSessions() {
            var response = Send(RequestMethod.GET, "/sessions");
            List sessions = (List)response["value"];
            sessions.Convert((Dictionary d) => new RemoteSession(this, (string)d["id"], (Dictionary)d["capabilities"]));
            return sessions;
        }

        /// <summary>
        /// Sends a request to the server
        /// </summary>
        /// <param name="method"></param>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        public Dictionary Send(RequestMethod method, string relativeUri) {
            //adds the server url
            var uri = _server_uri + relativeUri;
            return SendRequest(method, uri, null);
        }

        /// <summary>
        /// Sends a requests with parameters to the server
        /// </summary>
        /// <param name="method"></param>
        /// <param name="relativeUri"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Dictionary Send(RequestMethod method, string relativeUri, Dictionary param) {
            //Serialise the parameters
            JsonWriter data = null;
            if (method == RequestMethod.POST) {
                if (param != null) {
                    data = JsonWriter.Serialize(param);
                }
            }

            //adds the server url
            string uri = _server_uri;
            if (relativeUri != null)
                uri += relativeUri;

            return SendRequest(method, uri, data);
        }

        /// <summary>
        /// Execute again the previous execution
        /// </summary>
        /// <returns>Object</returns>
        public Dictionary SendAgain() {
            return SendRequest(_request_method, _request_uri, _request_data);
        }

        protected Dictionary SendRequest(RequestMethod method, string uri, JsonWriter data) {
            //save the request for resend
            _request_method = method;
            _request_uri = uri;
            _request_data = data;

            //Send the request
            _event_received.Reset();
            _event_send.Set();
            _event_received.WaitOne();

            //Evaluate exception
            if (_response_exception != null)
                throw _response_exception;

            //Evaluate the status and error
            int statusCode = (int)_response_content["status"];
            if (statusCode != 0) {
                var errorObject = _response_content["value"];
                var errorAsDict = errorObject as Dictionary;
                string errorMessage;
                if (errorAsDict != null) {
                    errorMessage = errorAsDict["message"] as string;
                } else {
                    errorMessage = errorObject as string;
                }
                var error = Errors.WebRequestError.Select(statusCode, errorMessage);
                error.ResponseData = _response_content;
                throw error;
            }
            return _response_content;
        }

        private void RunRequests() {
            while (!_disposed) {
                HttpWebResponse response = null;
                try {
                    _event_send.WaitOne();
                    _event_send.Reset();
                    if (_disposed)
                        return;

                    _response_exception = null;
                    _response_content = null;
                    _request = CreateHttpWebRequest(_request_method, _request_uri, _request_data, _response_timeout);
                    SysWaiter.OnInterrupt = _request.Abort;
                    response = (HttpWebResponse)_request.GetResponse();
                    _response_content = GetHttpWebResponseContent(response);
                } catch (ThreadInterruptedException) {
                    return;
                } catch (ThreadAbortException) {
                    return;
                } catch (WebException ex) {
                    if (ex.Status == WebExceptionStatus.RequestCanceled) {
                        _response_exception = new Errors.KeyboardInterruptError();
                    } else if (ex.Status == WebExceptionStatus.Timeout) {
                        _response_exception = new Errors.WebRequestTimeout(_request);
                    } else if ((response = ex.Response as HttpWebResponse) != null) {
                        _response_content = GetHttpWebResponseContent(response);
                    } else {
                        _response_exception = new Errors.WebRequestError(ex.Message);
                    }
                } catch (Exception ex) {
                    _response_exception = new SeleniumException(ex);
                } finally {
                    SysWaiter.OnInterrupt = null;
                    _event_received.Set();
                }
            }
        }


        private static HttpWebRequest CreateHttpWebRequest(RequestMethod method, string url, JsonWriter data, int timeout) {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(url));
            request.Method = FormatRequestMethod(method);
            request.Timeout = timeout;
            request.Accept = REQUEST_ACCEPT_HEADER;
            request.KeepAlive = true;
            request.ServicePoint.ConnectionLimit = 100;
            if (method == RequestMethod.POST && data != null && data.Length != 0) {
                request.ContentType = HEADER_CONTENT_TYPE;
                request.ContentLength = data.Length;
                using (Stream rstream = request.GetRequestStream()) {
                    data.CopyTo(rstream);
                }
            } else {
                request.ContentLength = 0;
            }
            return request;
        }

        private static Dictionary GetHttpWebResponseContent(HttpWebResponse response) {
            try {
                using (Stream stream = response.GetResponseStream()) {
                    if (IsJsonResponse(response)) {
                        Dictionary dict = (Dictionary)JsonReader.Deserialize(stream);
                        return dict;
                    } else {
                        string msg = new StreamReader(stream).ReadToEnd();
                        throw new SeleniumError(msg);
                    }
                }
            } catch (Exception ex) {
                throw new SeleniumException(ex);
            }
        }

        private static string FormatRequestMethod(RequestMethod method) {
            switch (method) {
                case RequestMethod.POST: return "POST";
                case RequestMethod.GET: return "GET";
                case RequestMethod.DELETE: return "DELETE";
                default:
                    throw new SeleniumException("Method not supported {0}", method);
            }
        }

        private static bool IsJsonResponse(WebResponse response) {
            var contentType = response.ContentType;
            if (contentType != null && contentType.StartsWith(JSON_MIME_TYPE, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

    }

}
