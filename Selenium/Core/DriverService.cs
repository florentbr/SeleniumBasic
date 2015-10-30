using Selenium.Internal;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace Selenium.Core {

    class DriverService : IDriverService {

        const string TEMP_FOLDER = @"Selenium";

        internal static string GetTempFolder() {
            var tmp_dir = Path.Combine(Path.GetTempPath(), TEMP_FOLDER);
            Directory.CreateDirectory(tmp_dir);
            return tmp_dir;
        }


        private string _library_dir;
        private ProcessExt _process;
        private List<string> _arguments;
        protected EndPointExt _endpoint;
        private string _temp_folder;

        public DriverService() {
            _library_dir = IOExt.GetAssemblyDirectory();
            _arguments = new List<string>();
            _temp_folder = GetTempFolder();
            _endpoint = EndPointExt.Create(IPAddress.Loopback, false);
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose() {
            _endpoint.Dispose();

            if (_process != null) {
                if (!_process.HasExited) {
                    _process.Kill();
                }
                _process.Dispose();
                _process = null;
            }
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Quit(RemoteServer server) {
            if (_process == null || _process.HasExited)
                return;

            server.ShutDown();

            if (_process.WaitForExit(5000)) {
                _process.Dispose();
                _process = null;
            }
        }

        public virtual string Uri {
            get {
                return "http://localhost:" + _endpoint.IPEndPoint.Port.ToString();
            }
        }

        public IPEndPoint IPEndPoint {
            get {
                return _endpoint.IPEndPoint;
            }
        }

        public void AddArgument(string argument) {
            _arguments.Add(argument);
        }

        public void Start(string filename, bool hide = true) {
            Hashtable env = ProcessExt.GetStdEnvironmentVariables();
            env["TEMP"] = _temp_folder;
            env["TMP"] = _temp_folder;

            string servicePath = Path.Combine(_library_dir, filename);
            if (!File.Exists(servicePath))
                throw new Errors.FileNotFoundError(servicePath);

            //Start the process
            _process = ProcessExt.Start(servicePath, _arguments, null, env, true, true);

            //Waits for the port to be listening
            if (!_endpoint.WaitForListening(10000, 150))
                throw new Errors.TimeoutError("The driver failed to open the listening port {0} within 10s", _endpoint);

        }

    }

}
