using Interop.MSScript;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace vbsc {

    class ScriptControl : IDisposable {

        public Action<ScriptError> OnError;

        private IMSScriptControl _msc_compiler;
        private Script _script = null;
        private ScriptProcedure _procedure = null;
        private ScriptError _error = null;
        private object _result = null;
        private int _err_line = 0;
        private WScript _wscript;
        private StringBuilder _wscriptEcho = new StringBuilder();

        public ScriptControl() {
            _msc_compiler = (IMSScriptControl)new MSScriptControl();
            _msc_compiler.Timeout = -1;
            _msc_compiler.Language = "VBScript";
            _msc_compiler.UseSafeSubset = false;
            _msc_compiler.AllowUI = true;
            _msc_compiler.ErrorEvent += OnError_Event;
        }

        public void Dispose() {
            while (Marshal.ReleaseComObject(_msc_compiler) > 0) ;
        }

        private void Clean() {
            _error = null;
            _err_line = 0;
            _procedure = null;
            _result = null;
        }

        public string WScriptEcho {
            get {
                return _wscriptEcho.ToString().CleanEnd();
            }
        }

        public ScriptError Error {
            get {
                return _error;
            }
        }

        public WScript WScript {
            get {
                return _wscript;
            }
        }

        public object Result {
            get {
                return _result;
            }
        }

        private void OnError_Event() {
            var err = _msc_compiler.Error;
            try {
                if (ExitException.Is(err)) {
                    if (ExitException.ExitCode(err) == 0 || _error != null)
                        return;
                }
                if (_error == null) {
                    _error = new ScriptError(_script, _procedure, err.Description);
                }
                var line = err.Line;
                if (_script != null && line != 0 && line != _err_line)
                    _error.AddTrace(_script.GetTraceLineAt(line));
                _err_line = line;
            } finally {
                err.Clear();
            }
        }

        public void SetLanguage(Script script) {
            if (script == null)
                return;

            if (script.Path.EndsWith(".js")) {
                _msc_compiler.Language = "JScript";
            } else if (script.Path.EndsWith(".vbs")) {
                _msc_compiler.Language = "VBScript";
            } else {
                throw new ArgumentException("Invalide extention: " + script.Path);
            }
        }

        public bool Build(Script script = null) {
            if (_wscript != null) {
                _msc_compiler.Reset();
                _wscriptEcho.Length = 0;
            }
            SetLanguage(script);
            if (script == null) {
                _wscript = new WScript();
            } else {
                _wscript = new WScript(script.Path, script.Arguments);
            }
            _wscript.OnEcho += (m) => _wscriptEcho.AppendLine(m);
            _script = script;
            _msc_compiler.AddObject("WScript", _wscript, false);
            //_compiler.AddCode("Function IIf(Expression, TruePart, FalsePart)\r\nIf Expression Then IIf = truepart Else IIf = falsepart\r\nEnd Function\r\n");
            if (script != null)
                return Execute(script.GetCode());
            return true;
        }

        public bool Execute(string code, bool noevent = false) {
            Clean();
            try {
                _msc_compiler.AddCode(code);
            } catch (Exception ex) {
                if (_error == null && !ExitException.Is(ex))
                    throw;
            }
            if (_error == null)
                return true;
            _error.FillParentTrace();
            if (OnError != null && !noevent)
                OnError(_error);
            return false;
        }

        public bool Eval(string code, out object result) {
            Clean();
            try {
                result = _msc_compiler.Eval(code);
                return true;
            } catch (Exception ex) {
                if (_error == null && !ExitException.Is(ex))
                    throw;
                _error.FillParentTrace();
                result = null;
                return false;
            }
        }

        public bool Run(ScriptProcedure procedure, bool noevent = false) {
            Clean();
            if (procedure == null)
                return true;
            _procedure = procedure;
            try {
                procedure.Run();
            } catch (Exception ex) {
                if (_error == null && !ExitException.Is(ex))
                    throw;
            }
            if (_error == null)
                return true;
            if (OnError != null && !noevent)
                OnError(_error);
            return false;
        }

        public bool Eval(string expression) {
            Clean();
            try {
                _result = _msc_compiler.Eval(expression);
                return true;
            } catch {
                if (_error == null)
                    throw;
                return false;
            }
        }

        public IMSScriptProcedures Procedures {
            get {
                return _msc_compiler.Procedures;
            }
        }

        public IMSScriptModules Modules {
            get {
                return _msc_compiler.Modules;
            }
        }

    }

}
