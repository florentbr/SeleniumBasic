using System.Text;

namespace vbsc {

    class ScriptSuccess : IScriptResult {

        private readonly Script _script;
        private readonly ScriptProcedure _procedure;

        public ScriptSuccess(Script script, ScriptProcedure procedure = null) {
            _script = script;
            _procedure = procedure;
        }

        public Script Script {
            get { 
                return _script; 
            }
        }

        public bool Succeed {
            get { 
                return true; 
            }
        }

        public string Source {
            get {
                if (_procedure == null)
                    return string.Concat("in ", _script);
                return string.Concat(_procedure, " in ", _script);
            }
        }
    }

}
