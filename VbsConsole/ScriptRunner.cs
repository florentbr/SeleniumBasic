using Interop.MSScript;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace vbsc {

    class ScriptRunner : IDisposable {

        private List<IScriptResult> _scriptresults;
        private readonly ScriptControl _scriptcontrol;
        private bool _debug;

        public ScriptRunner(bool debug) {
            _scriptcontrol = new ScriptControl();
            _scriptcontrol.OnError = OnError;
            _scriptresults = new List<IScriptResult>();
            _debug = debug;
        }

        public void Dispose() {
            _scriptcontrol.Dispose();
        }

        public List<IScriptResult> Results {
            get {
                return _scriptresults;
            }
        }

        private void OnError(ScriptError error) {
            _scriptresults.Add(error);
            Logger.LogScriptError(error);
            if (_debug) {
                ScriptDebugger.Debug(_scriptcontrol, true);
            }
        }

        private void OnSuccess(ScriptSuccess succees) {
            _scriptresults.Add(succees);
        }

        private void OnInfo(Script script, string message) {
            if (message.Length != 0)
                Logger.LogScriptInfo(script.ToString(), message);
        }

        public void Execute(Script script, Regex procedure_pattern) {
            if (!_scriptcontrol.Build(script))
                return;
            var list_procedures = ListProcedures(script, procedure_pattern);
            if (_scriptcontrol.Run(list_procedures.ProcInitialize)) {
                foreach (var procedure in list_procedures) {
                    if (_scriptcontrol.Run(list_procedures.ProcSetup)) {
                        if (_scriptcontrol.Run(procedure, true)) {
                            OnSuccess(new ScriptSuccess(script, procedure));
                        } else {
                            var error = _scriptcontrol.Error;
                            if (list_procedures.ProcOnError != null) {
                                list_procedures.ProcOnError.Arguments = new[] { 
                                    procedure.Name,
                                    _scriptcontrol.Error.ToString() 
                                };
                                if (_scriptcontrol.Run(list_procedures.ProcOnError))
                                    error.AddInfo((string)_scriptcontrol.Result);
                            }
                            OnError(error);
                        }
                        _scriptcontrol.Run(list_procedures.ProcTearDown);
                    }
                }
                _scriptcontrol.Run(list_procedures.ProcTerminate);
            }
            if (list_procedures.Count == 0 && script.Succeed)
                OnSuccess(new ScriptSuccess(script));
            OnInfo(script, _scriptcontrol.WScriptEcho);
        }


        private ScriptProcedures ListProcedures(Script script, Regex pattern) {
            ScriptProcedures procedures = new ScriptProcedures();
            foreach (IMSScriptModule msc_module in _scriptcontrol.Modules) {
                ScriptModule module = new ScriptModule(msc_module);
                foreach (IMSScriptProcedure msc_procedure in msc_module.Procedures) {
                    var proc_name = msc_procedure.Name;
                    ScriptProcedure procedure = new ScriptProcedure(module, proc_name);
                    switch (proc_name.ToLower()) {
                        case "initialize": procedures.ProcInitialize = procedure; break;
                        case "terminate": procedures.ProcTerminate = procedure; break;
                        case "setup": procedures.ProcSetup = procedure; break;
                        case "teardown": procedures.ProcTearDown = procedure; break;
                        case "onerror": procedures.ProcOnError = procedure; break;
                        case "iif": break;
                        default:
                            if (msc_procedure.HasReturnValue)
                                continue;
                            if (!pattern.IsMatch(proc_name))
                                continue;
                            WithParams proc_params_str;
                            if (script.ScriptWithParams.TryGetValue(proc_name, out proc_params_str)) {
                                if (!_scriptcontrol.Eval("Array(" + proc_params_str.Params + ')')) {
                                    var error = new ScriptError(script, procedure, "Invalide array: " + proc_params_str.Params);
                                    error.AddTrace(script, proc_params_str.Line);
                                    OnError(error);
                                    continue;
                                }
                                var proc_params = (object[])_scriptcontrol.Result;
                                foreach (var proc_param in proc_params) {
                                    object[] args = CastToArray(proc_param);
                                    if (args.Length == 0) {
                                        var error = new ScriptError(script, procedure, string.Format(
                                            "Procedure {0} requires {1} argument(s). {2} provied.",
                                            proc_name, msc_procedure.NumArgs, args.Length));
                                        error.AddTrace(script, script.TextFormated.GetLineNumber("(Sub|Function).\b" + proc_name + "\b"));
                                        OnError(error);
                                        break;
                                    }
                                    procedure = new ScriptProcedure(module, proc_name, args);
                                    procedures.Add(procedure);
                                }
                            } else if (msc_procedure.NumArgs == 0) {
                                procedures.Add(procedure);
                            }
                            break;
                    }
                }
            }
            return procedures;
        }

        private object[] CastToArray(object obj) {
            if (obj == null)
                return new object[0];
            if (obj is object[])
                return (object[])obj;
            return new object[1] { obj };
        }

    }
}
