using Selenium.Serializer;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Selenium.Core {

    class RemoteServer {

        const string JSON_MIME_TYPE = "application/json";
        const string HEADER_CONTENT_TYPE = "application/json;charset=UTF-8";
        const string HEADER_ACCEPT = "application/json";

        private readonly int _response_timeout;
        private readonly string _server_uri;

        private RequestMethod _request_method;
        private string _request_uri;
        private JSON _request_data;
        private NetworkCredential _credentials = null;


        internal RemoteServer(string serverAddress, bool isLocal, int timeout) {
            _server_uri = serverAddress.TrimEnd('/');
            _response_timeout = timeout;

            HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
            HttpWebRequest.DefaultMaximumResponseHeadersLength = -1;
            if (isLocal) {
                HttpWebRequest.DefaultWebProxy = null;
            } else {
                // Handle credentials for Basic authentication
                var creds = Regex.Match(_server_uri, @"://([^:/@]+):([^:/@]+)@");
                if (creds.Success) {
                    _credentials = new NetworkCredential(creds.Groups[1].Value, creds.Groups[2].Value);
                    _server_uri = _server_uri.Remove(creds.Index + 3, creds.Length - 3);
                }
            }
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

        public void ShutDown() {
            Send(RequestMethod.GET, @"/shutdown", null);
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
            JSON data = null;
            if (method == RequestMethod.POST) {
                if (param != null) {
                    data = JSON.Serialize(param);
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

        protected Dictionary SendRequest(RequestMethod method, string uri, JSON data) {
            _request_method = method;
            _request_uri = uri;
            _request_data = data;

            HttpWebRequest request = CreateHttpWebRequest(method, uri, data);
            SysWaiter.OnInterrupt = request.Abort;
            HttpWebResponse response = null;
            Dictionary responseDict = null;
            IAsyncResult asyncResult = null;

            try {
                asyncResult = request.BeginGetResponse(null, null);
                if(!asyncResult.AsyncWaitHandle.WaitOne(_response_timeout)){
                    request.Abort();
                    throw new WebException(null, WebExceptionStatus.Timeout);
                }
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

            if (responseDict != null) {
                //Evaluate the status and error
                int statusCode = (int)responseDict["status"];
                if (statusCode != 0) {
                    object errorObject = responseDict["value"];
                    Dictionary errorAsDict = errorObject as Dictionary;
                    string errorMessage;
                    if (errorAsDict != null) {
                        errorMessage = errorAsDict["message"] as string;
                    } else {
                        errorMessage = errorObject as string;
                    }
                    SeleniumError error = Errors.WebRequestError.Select(statusCode, errorMessage);
                    error.ResponseData = responseDict;
                    throw error;
                }
            }
            return responseDict;
        }

        private HttpWebRequest CreateHttpWebRequest(RequestMethod method, string url, JSON data) {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(url));
            request.Method = FormatRequestMethod(method);
            request.Accept = HEADER_ACCEPT;
            request.KeepAlive = true;

            if (_credentials != null) {
                request.Credentials = _credentials;
                _credentials = null;
            }

            if (method == RequestMethod.POST && data != null && data.Length != 0) {
                request.ContentType = HEADER_CONTENT_TYPE;
                request.ContentLength = data.Length;
                using (Stream rstream = request.GetRequestStream()) {
                    rstream.Write(data.GetBuffer(), 0, (int)data.Length);
                }
            } else {
                request.ContentLength = 0;
            }
            return request;
        }

        private static Dictionary GetHttpWebResponseContent(HttpWebResponse response) {
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;
            using (Stream stream = response.GetResponseStream()) {
                if (IsJsonResponse(response)) {
                    Dictionary dict = (Dictionary)JSON.Parse(stream);
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
            if (contentType != null && contentType.StartsWith(JSON_MIME_TYPE
                , StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            return false;
        }

    }

}
