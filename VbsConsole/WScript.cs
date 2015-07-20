using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace vbsc {

    /// <summary>
    /// WScript object compiled with the user script
    /// </summary>
    [Guid("D4DC2448-9BB0-448D-BBF8-0709B82B82AF")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class WScript : IWscript {

        const string WSCRIPT_NAME = "Windows Script Host";

        public event Action<string> OnEcho;

        private readonly string _filepath;
        private readonly string[] _arguments;
        private readonly StringBuilder _buffer;
        private readonly EventWaitHandle _waiter;

        internal WScript(string filepath, string[] arguments) {
            _filepath = filepath;
            _arguments = arguments ?? new string[0];
            _waiter = new EventWaitHandle(false, EventResetMode.AutoReset);
            _buffer = new StringBuilder();
        }

        internal WScript() {
            _filepath = null;
            _arguments = null;
            _buffer = new StringBuilder();
        }

        public IWscript Application {
            get { return this; }
        }

        public object Arguments(int index = -1) {
            if (index == -1)
                return _arguments;
            return _arguments[index];
        }

        public object CreateObject(string strProgID, string strPrefix = null) {
            var type = Type.GetTypeFromProgID(strProgID);
            if (type == null)
                throw new Exception(string.Format("Application not registered: {0}", strProgID));
            return Activator.CreateInstance(type);
        }

        public object GetObject(string strPathname = null, string strProgID = null, string strPrefix = null) {
            return Marshal.GetActiveObject(strProgID);
        }

        public void Echo(object message) {
            if (OnEcho == null)
                return;
            _buffer.Length = 0;
            ICollection collection;
            if (message is string || (collection = message as ICollection) == null) {
                _buffer.Append(message ?? string.Empty);
                _buffer.Append('\n');
            } else {
                int i = 0;
                foreach (var item in collection) {
                    if (++i > 20) {
                        _buffer.Append("... (");
                        _buffer.Append(collection.Count - i + 1);
                        _buffer.Append(" more)\n");
                        break;
                    }
                    if (item is string) {
                        _buffer.Append('"');
                        _buffer.Append(item ?? string.Empty);
                        _buffer.Append("\"\n");
                    } else {
                        _buffer.Append(item ?? string.Empty);
                        _buffer.Append('\n');
                    }
                }
            }
            OnEcho(_buffer.ToString().CleanEnd());
        }

        public string Name {
            get {
                return WSCRIPT_NAME;
            }
        }

        public string FullName {
            get {
                return Assembly.GetExecutingAssembly().Location;
            }
        }

        public string CurrentDirectory {
            get {
                return Directory.GetCurrentDirectory();
            }
            set {
                Directory.SetCurrentDirectory(value);
            }
        }

        public string Path {
            get {
                var assembly_location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return System.IO.Path.GetDirectoryName(assembly_location);
            }
        }

        public string ScriptPath {
            get {
                if (_filepath == null)
                    return string.Empty;
                return System.IO.Path.GetDirectoryName(_filepath);
            }
        }

        public string ScriptName {
            get {
                if (_filepath == null)
                    return string.Empty;
                return System.IO.Path.GetFileName(_filepath);
            }
        }

        public string ScriptFullName {
            get {
                return _filepath ?? string.Empty;
            }
        }

        public TextWriter StdErr {
            get {
                return Console.Out;
            }
        }
        public TextReader StdIn {
            get {
                return Console.In;
            }
        }

        public TextWriter StdOut {
            get {
                return Console.Out;
            }
        }

        public void Sleep(int timems) {
            _waiter.WaitOne(timems);
        }

        public void Quit(int exitcode = 0) {
            throw new ExitException(exitcode);
        }

    }

}
