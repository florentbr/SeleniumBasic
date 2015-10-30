using Selenium.Serializer;
using System;
using System.IO;
using System.Net;
using System.Net.Cache;

namespace Selenium.Core {

    class RemoteServer {

        const string JSON_MIME_TYPE = "application/json";
        const string HEADER_CONTENT_TYPE = "application/json;charset=utf-8";
        const string REQUEST_ACCEPT_HEADER = "application/json;, image/png";

        private readonly int _response_timeout;
        private readonly string _server_uri;

        private RequestMethod _request_method;
        private string _request_uri;
        private JsonWriter _request_data;


        internal RemoteServer(string serverAddress, bool isLocal, int timeout) {
            _server_uri = serverAddress.TrimEnd('/');
            _response_timeout = timeout;

            HttpWebRequest.DefaultCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
            HttpWebRequest.DefaultMaximumResponseHeadersLength = -1;
            if (isLocal)
                HttpWebRequest.DefaultWebProxy = null;
            ServicePointManager.Expect100Continue = false;
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
            _request_method = method;
            _request_uri = uri;
            _request_data = data;

            HttpWebRequest request = CreateHttpWebRequest(method, uri, data, _response_timeout);
            SysWaiter.OnInterrupt = request.Abort;
            HttpWebResponse response = null;
            Dictionary responseDict = null;
            try {
                IAsyncResult asyncResult = request.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                responseDict = GetHttpWebResponseContent(response);
            } catch (WebException ex) {
                if (ex.Status == WebExceptionStatus.RequestCanceled) {
                    throw new Errors.KeyboardInterruptError();
                } else if (ex.Status == WebExceptionStatus.Timeout) {
                    throw new Errors.WebRequestTimeout(request);
                } else if ((response = ex.Response as HttpWebResponse) != null) {
                    try {
                        responseDict = GetHttpWebResponseContent(response);
                    } catch (Exception ex2) {
                        throw new SeleniumException(ex2);
                    }
                } else {
                    throw new Errors.WebRequestError(ex.Message);
                }
            } catch (Exception ex) {
                throw new SeleniumException(ex);
            } finally {
                SysWaiter.OnInterrupt = null;
                if (response != null)
                    response.Close();
            }

            //Evaluate the status and error
            int statusCode = (int)responseDict["status"];
            if (statusCode != 0) {
                var errorObject = responseDict["value"];
                var errorAsDict = errorObject as Dictionary;
                string errorMessage;
                if (errorAsDict != null) {
                    errorMessage = errorAsDict["message"] as string;
                } else {
                    errorMessage = errorObject as string;
                }
                var error = Errors.WebRequestError.Select(statusCode, errorMessage);
                error.ResponseData = responseDict;
                throw error;
            }
            return responseDict;
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
            using (Stream stream = response.GetResponseStream()) {
                if (IsJsonResponse(response)) {
                    Dictionary dict = (Dictionary)JsonReader.Deserialize(stream);
                    return dict;
                } else {
                    string bodyText = new StreamReader(stream).ReadToEnd();
                    switch ((int)response.StatusCode) {
                        case 400: throw new Exception("Missing Command Parameters: " + bodyText);
                        case 404: throw new Exception("Unknown Commands: " + bodyText);
                        case 405: throw new Exception("Invalid Command Method: " + bodyText);
                        case 501: throw new Exception("Unimplemented Commands: " + bodyText);
                        default: throw new Exception(response.StatusDescription);
                    }
                }
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
