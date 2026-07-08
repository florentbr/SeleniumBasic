using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Selenium.Tests.Internals {

    public class WebServer {

        public static string BaseUri = @"http://localhost:19328";

        static WebServer _server = null;

        public static void StartServer(string folder) {
            if (_server == null) {
                _server = new WebServer(folder);
                _server.Start();
            }
        }

        public static void StopServer() {
            if (_server != null) {
                _server.Stop();
                _server = null;
            }
        }

        public static void AddPage(string name, string content) {
            _server.RegisterPage(name, content);
        }


        private HttpListener _listener;
        private Thread _thread;
        private Dictionary<string, byte[]> _cache;
        private string _folder;
        private bool _exit;

        public WebServer(string folder) {
            _folder = Path.GetFullPath(folder);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((s, e) => this.Stop());
            AppDomain.CurrentDomain.DomainUnload += new EventHandler((s, e) => this.Stop());
        }

        ~WebServer(){
            this.Stop();
        }

        public void Start() {
            _exit = false;
            _cache = new Dictionary<string, byte[]>();

            _listener = new HttpListener();
            _listener.Prefixes.Add(BaseUri + "/");
            _listener.Start();

            _thread = new Thread(Run);
            _thread.SetApartmentState(ApartmentState.MTA);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop() {
            _exit = true;
            if (_listener != null) {
                _listener.Abort();
                _listener.Close();
                _listener = null;
            }
            if (_thread != null) {
                _thread.Join();
                _thread = null;
            }
        }

        private void RegisterPage(string name, string content) {
            byte[] data = Encoding.UTF8.GetBytes(content);
            _cache.Add(name, data);
        }

        private void Run() {
            Debug.WriteLine("Webserver running...");
            try {
                while (!_exit) {
                    HttpListenerContext ctx = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(HandleRequest, ctx);
                }
            } catch { }
        }

        private void HandleRequest(object context) {
            var ctx = (HttpListenerContext)context;
            if (!_exit) {
                try {
                    string uri = ctx.Request.Url.AbsolutePath;
                    byte[] data;
                    if (TryGetPage(uri, out data)) {
                        ctx.Response.ContentLength64 = data.Length;
                        ctx.Response.OutputStream.Write(data, 0, data.Length);
                        ctx.Response.StatusCode = 200;
                    } else {
                        ctx.Response.StatusCode = 404;
                    }
                } catch {
                } finally {
                    ctx.Response.OutputStream.Close();
                }
            }
            if (ctx.Response != null)
                ctx.Response.Close();
        }

        private bool TryGetPage(string page, out byte[] data) {
            if (_cache.TryGetValue(page, out data))
                return true;

            string path = Path.Combine(_folder, page.TrimStart('/'));
            if (File.Exists(path)) {
                string text = File.ReadAllText(path);
                data = Encoding.UTF8.GetBytes(text);
                _cache.Add(page, data);
                return true;
            }

            data = null;
            return false;
        }

    }

}
