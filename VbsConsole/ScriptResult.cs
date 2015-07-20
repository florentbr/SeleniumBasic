using System.Text;
using System;

namespace vbsc {

    interface IScriptResult {

        Script Script { get; }

        bool Succeed { get; }

        string Source { get; }

    }

}
