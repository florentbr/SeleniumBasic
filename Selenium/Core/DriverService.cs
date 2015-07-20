using Selenium.Internal;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Selenium.Core {

    class DriverService : IDriverService {

        const string TEMP_FOLDER = @"Selenium";

        internal static string GetTempFolder() {
            var tmp_dir = Path.Combine(Path.GetTempPath(), TEMP_FOLDER);
            Directory.CreateDirectory(tmp_dir);
            return tmp_dir;
        }


        private string _library_dir;
        private IPEndPoint _endpoint;
        private Process _process;
        private StringBuilder _arguments;
        private Socket _socketLock;
        private string _temp_folder;

        public DriverService(IPAddress address) {
            _library_dir = IOExt.GetAssemblyDirectory();
            _arguments = new StringBuilder();
            _temp_folder = DriverService.GetTempFolder();
            NetExt.LockNewEndPoint(address, out _socketLock, out _endpoint);
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose() {
            if (_socketLock != null) {
                _socketLock.Close();
                _socketLock = null;
            }

            if (_process != null) {
                if (!_process.HasExited) {
                    _process.Kill();
                }
                _process.Close();
                _process = null;
            }
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Quit() {
            if (_process == null || _process.HasExited)
                return;
            try {
                var request = (HttpWebRequest)WebRequest.Create(this.Uri + @"/shutdown");
                request.KeepAlive = false;
                WebResponse resp = request.GetResponse();
                resp.Close();
            } catch (WebException) { }

            if (!_process.WaitForExit(5000)) {
                _process.Close();
                _process = null;
            }
        }

        public string Uri {
            get {
                return "http://" + _endpoint.ToString();
            }
        }

        public IPEndPoint EndPoint {
            get {
                return _endpoint;
            }
        }

        public void AddArgument(string argument) {
            _arguments.Append(' ');
            _arguments.Append(argument);
        }

        public void Start(string filename, bool hide = true) {
            string servicePath = Path.Combine(_library_dir, filename);

            //Define the process
            _process = new Process();
            ProcessStartInfo si = _process.StartInfo;
            si.FileName = servicePath;
            si.WindowStyle = hide ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;
            si.Arguments = _arguments.ToString().Trim();
            si.UseShellExecute = false;
            si.CreateNoWindow = hide;
            si.EnvironmentVariables["TEMP"] = _temp_folder;
            si.EnvironmentVariables["TMP"] = _temp_folder;
            si.RedirectStandardError = false;
            si.RedirectStandardOutput = false;

            //Starts the server
            _process.Start();

            //release the lock on the socket
            _socketLock.Close();    //release the lock on the socket

            //Waits for the port to be listening
            if (!NetExt.WaitForLocalPortListening(_endpoint.Port, 10000, 150))
                //if (!NetExt.WaitForPortConnectable(_endpoint, 10000, 100))
                throw new Errors.TimeoutError("The driver failed to open the listening port {0} within 10s", _endpoint);
        }

    }

}
