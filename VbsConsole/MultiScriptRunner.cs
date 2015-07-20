using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace vbsc {

    class MultiScriptRunner {

        private Queue<Script> _queue_scripts = new Queue<Script>();
        private List<Thread> _pool_threads = new List<Thread>();
        private List<IScriptResult> _list_results = new List<IScriptResult>();
        private readonly int _number_of_thread;
        private string[] _script_arguments;
        private Regex _procedure_pattern;
        private bool _debug;

        public MultiScriptRunner(int nbthread = 1) {
            _number_of_thread = nbthread;
        }

        public List<IScriptResult> Run(string[] lst_scripts_paths, string[] lst_arguments, string[] lst_parameters, string procedure_pattern, bool debug) {
            _script_arguments = lst_arguments;
            _procedure_pattern = new Regex(procedure_pattern, RegexOptions.IgnoreCase);
            _debug = debug;

            var nbthread = Math.Min(_number_of_thread, lst_scripts_paths.Length);
            if (lst_parameters == null || lst_parameters.Length == 0)
                lst_parameters = new string[] { null };
            foreach (var param in lst_parameters) {
                foreach (var script_path in lst_scripts_paths)
                    _queue_scripts.Enqueue(new Script(script_path, lst_arguments, param));
            }
            for (int i = 0; i < nbthread; i++) {
                var thread = new Thread(RunScripts);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                _pool_threads.Add(thread);
            }
            foreach (var thread in _pool_threads)
                thread.Join();
            return _list_results;
        }

        private void RunScripts() {
            using (var runner = new ScriptRunner(_debug)) {
                Script script;
                while (true) {
                    lock(_queue_scripts){
                        if (_queue_scripts.Count == 0)
                            break;
                        script = _queue_scripts.Dequeue();
                    }
                    try {
                        runner.Execute(script, _procedure_pattern);
                    } catch (Exception ex) {
                        runner.Results.Add(new ScriptError(script, null, ex.Message));
                        Logger.LogScriptException(script, ex);
                    }
                }
                lock (_list_results) {
                    _list_results.AddRange(runner.Results);
                }
            }
        }

    }
}
