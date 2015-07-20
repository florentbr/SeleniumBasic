using System.Collections.Generic;
using System.Text;

namespace vbsc {

    class ScriptError : IScriptResult {

        private readonly Script _script;
        private readonly ScriptProcedure _proc_item;
        private List<TraceLine> _stack_trace = new List<TraceLine>();
        private StringBuilder _message = new StringBuilder();
        private StringBuilder _info = new StringBuilder();
        private int _err_line = 0;

        public ScriptError(Script script, ScriptProcedure procedure, string message) {
            _script = script;
            _proc_item = procedure;
            _message.Append(message.CleanEnd());
            if(script != null)
                _script.Succeed = false;
        }

        public Script Script {
            get {
                return _script;
            }
        }

        public bool Succeed {
            get {
                return false;
            }
        }

        public string Message {
            get {
                return _message.ToString();
            }
        }

        public string Source {
            get {
                if (_proc_item == null)
                    return string.Format("line {0} in {1}", this.LineNumber, _script).Trim();
                return string.Format("{0} line {1} in {2}", _proc_item, this.LineNumber, _script).Trim();
            }
        }

        public int LineNumber {
            get {
                if (_err_line == 0) {
                    string scriptPath = _script.Path;
                    foreach (var trace in _stack_trace) {
                        if (trace.Script.Path == scriptPath) {
                            _err_line = trace.LineNumber;
                            break;
                        }
                    }
                }
                return _err_line;
            }
        }

        internal void AddTrace(TraceLine traceLine) {
            _stack_trace.Add(traceLine);
        }

        internal void AddTrace(Script script, int line_number) {
            var traceline = new TraceLine(script, line_number);
            _stack_trace.Add(traceline);
        }

        internal void FillParentTrace() {
            var last_trace = _stack_trace.LastItem();
            if (last_trace == null)
                return;
            var script = last_trace.Script;
            while (script.ParentScript != null) {
                AddTrace(script.ParentScript, script.ParentLineNumber);
                script = script.ParentScript;
            }
        }

        internal void AddInfo(string info) {
            _info.AppendLine(info.CleanEnd());
        }

        public override string ToString() {
            var buffer = new StringBuilder();
            buffer.Append(_message);
            buffer.Append('\n');
            foreach (TraceLine trace in _stack_trace) {
                buffer.AppendFormat(" at {0} line {1}:\n", trace.Script.Path, trace.LineNumber);
                buffer.AppendFormat("  {0}\n", trace.LineOfCode);
            }
            if (_info.Length != 0)
                buffer.Append(_info.ToString());
            return buffer.ToString();
        }

    }

}
