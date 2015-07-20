using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection;

namespace SScript {

    class RunnerVbNet : IRunner {

        private string[] _scripts;
        private string[] _arguments;
        private string _browser;
        private string _browser_str;
        private string _pattern;
        private Thread _thread;
        public List<Result> _results = new List<Result>();

        public RunnerVbNet(string[] scripts, string[] arguments, string browser, string pattern) {
            _scripts = scripts;
            _arguments = arguments;
            _browser = browser;
            _browser_str = browser != null ? "(" + _browser + ")" : "";
            _pattern = pattern;
            _thread = new Thread(RunScripts);
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
        }


        public List<Result> Results {
            get { return _results; }
        }

        public void Wait() {
            _thread.Join();
        }

        private void RunScripts() {
            foreach (var script in _scripts)
                RunVBSScript(script);
        }

        private string ReadScript(string filepath) {
            var script = System.IO.File.ReadAllText(filepath);
            if (_browser != null)
                script = Regex.Replace(script, @"(\.Start\(\s*)""\w+""", "$1\"" + _browser + "\"", RegexOptions.IgnoreCase);
            return script;
        }

        private void RunVBSScript(string filepath) {
            var starttime = DateTime.Now;
            var script = ReadScript(filepath);
            var references = new List<string> { "System.dll", "mscorlib.dll", "System.Core.dll", "System.Xml.dll", "System.Xml.Linq.dll", "System.Windows.Forms.dll" };
            references.AddRange(script.FindAll(@"^ *Reference ""(.*)""\s*$", 1));
            script = script.ReplaceAll(@"^ *Reference.*$", "");
            var source = Path.GetFileName(filepath) + _browser_str;

            Action<Exception, string> LogError = (ex, procedure) => {
                int line2 = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                var line = ex.ToString();
                _results.Add(new Result { Source = source, Procedure = procedure, ErrorLine = 0, ErrorDescription = ex.Message, IsError = true });
                Logger.LogError(procedure, source, 0, ex.Message.Trim('\r', '\n', ' '));
            };

            //Format parametrics methods
            script = script.ReplaceAll(@"\[With\((.*)\)\](\s+(Private |Public )?Sub )([\w_]+)", "Dim $4_dataset_ As Object() = {$1}$2$4");
            var mth_params_names = script.FindAll(@"([\w_]+)_dataset_", 1);

            var csc = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });

            var parameters = new CompilerParameters(references.ToArray());
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = true;

            var module_name = Regex.Match(script, @"^[\s\t]*Module\s+([\w_]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Groups[1].Value;
            var results = csc.CompileAssemblyFromSource(parameters, script);

            if (results.Errors.Count != 0) {
                foreach (CompilerError error in results.Errors)
                    Logger.LogError(null, source, error.Line, error.ErrorText);
                return;
            }

            //Parse script

            MethodInfo p_initialize = null;
            MethodInfo p_terminate = null;
            MethodInfo p_setup = null;
            MethodInfo p_teardown = null;

            var p_list = new ProcList<MethodInfo>();

            var reg_pattern = new Regex(_pattern, RegexOptions.IgnoreCase);
            var type = results.CompiledAssembly.GetType(module_name);
            var ass = type.Module.Assembly;

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)) {
                if (method.ReturnType.FullName != "System.Void") continue;
                switch (method.Name.ToLower()) {
                    case "initialize": p_initialize = method; break;
                    case "terminate": p_terminate = method; break;
                    case "setup": p_setup = method; break;
                    case "teardown": p_teardown = method; break;
                    default:
                        if (reg_pattern.IsMatch(method.Name)) {
                            if (Array.IndexOf(mth_params_names, method.Name) != -1) {
                                var mth_params_data = (object[])type.GetField(method.Name + "_dataset_", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                                foreach (var p in mth_params_data) {
                                    var mth_args = p is object[] ? (object[])p : new object[1] { p };
                                    if (mth_args.Length != method.GetParameters().Length) {
                                        Logger.LogError(method.Name, source, script.GetLineNumber("Sub.\b" + method.Name + "\b"), string.Format("Invalide argument. Method <{0}> requires {1} argument(s)", method.Name, method.GetParameters().Length, mth_args.Length));
                                        return;
                                    }
                                    p_list.Add(method, mth_args);
                                }
                            } else {
                                p_list.Add(method, new object[0]);
                            }
                        }
                        break;
                }
            }

            //Run
            if (p_list.Count == 0) {
                _results.Add(new Result { Source = source, Procedure = null });
            } else {
                var noparams = new object[0];
                try {
                    if (p_initialize != null)
                        p_initialize.Invoke(null, noparams);
                    foreach (var p in p_list) {
                        try {
                            var p_name = p.Proc.Name + p.ParamsToString();
                            if (p_setup != null)
                                p_setup.Invoke(null, noparams);
                            try {
                                p.Proc.Invoke(null, p.Params);
                                _results.Add(new Result { Source = source, Procedure = p_name });
                            } catch (Exception ex_r) { LogError(ex_r, p_name); }
                            try {
                                if (p_teardown != null)
                                    p_teardown.Invoke(null, noparams);
                            } catch (Exception ex_td) { LogError(ex_td, p_teardown.Name); }
                        } catch (Exception ex_su) { LogError(ex_su, p_setup.Name); }
                    }
                    try {
                        if (p_terminate != null)
                            p_terminate.Invoke(null, noparams);
                    } catch (Exception ex_ini) { LogError(ex_ini, p_terminate.Name); }
                } catch (Exception ex_ter) { LogError(ex_ter, p_initialize.Name); }
            }
            Logger.LogInfo(source, Logger.PopStdOut());

        }

    }
}
