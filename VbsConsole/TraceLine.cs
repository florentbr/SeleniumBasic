
namespace vbsc {

    class TraceLine {

        public readonly Script Script;
        public readonly int LineNumber;
        public readonly string LineOfCode;

        public TraceLine(Script script, int line) {
            Script = script;
            LineNumber = line;
            LineOfCode = script.TextOriginal.GetLineAt(line);
        }

        public override string ToString() {
            return string.Format("at {0} line {1}\r\n {2}", this.Script.Path, this.LineNumber, this.LineOfCode);
        }
    }

}
