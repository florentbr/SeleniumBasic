using System.Collections.Generic;

namespace vbsc {

    class ScriptProcedures : List<ScriptProcedure> {

        public ScriptProcedure ProcInitialize = null;
        public ScriptProcedure ProcTerminate = null;
        public ScriptProcedure ProcSetup = null;
        public ScriptProcedure ProcTearDown = null;
        public ScriptProcedure ProcOnError = null;

    }

}
