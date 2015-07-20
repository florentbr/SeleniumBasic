using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace vbsc {

    class Script {

        public readonly string Path;
        public readonly string[] Arguments;
        public readonly string Param;
        public readonly string Directory;
        public readonly string TextOriginal;
        public readonly int LineCount;

        public bool Succeed = true;
        public string TextFormated;

        public List<Script> ChildenScripts;
        public Script ParentScript;
        public int ParentLineNumber;

        public Dictionary<string, WithParams> ScriptWithParams;


        public Script(string script_path, string[] arguments, string param = null)
            : this(script_path) {
            this.Arguments = arguments;
            this.Param = param;
            FormatScript(null, 0);
        }

        private Script(string script_path, Script parent_script, int parent_line_number)
            : this(script_path) {
            FormatScript(parent_script, parent_line_number);
        }

        private Script(string script_path) {
            this.Path = script_path;
            this.Directory = System.IO.Path.GetDirectoryName(this.Path);
            this.TextOriginal = (System.IO.File.ReadAllText(this.Path)).ToString();
            this.LineCount = TextOriginal.CountLines();
        }

        public override string ToString() {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(System.IO.Path.GetFileName(this.Path));
            if (this.Param != null) {
                buffer.Append('(');
                buffer.Append(this.Param);
                buffer.Append(')');
            }
            return buffer.ToString();
        }

        /// <summary>Recursive methode to read and format a script and the refered scripts defined in the #Include statement.</summary>
        /// <param name="par_script">Parent script used in recurtion call</param>
        /// <param name="par_line_number">Parent line number used in recurtion call</param>
        /// <param name="par_lien_of_code">Parent line of code used in recurtion call</param>
        /// <returns>A list of scripts</returns>
        private void FormatScript(Script par_script, int par_line_number) {

            this.TextFormated = this.TextOriginal;
            this.ParentScript = par_script;
            this.ParentLineNumber = par_line_number;

            //Handle includes
            Match match_inc = Regex.Match(this.TextFormated, @"^#Include ""([^""]+)""", RegexOptions.Multiline);
            while (match_inc.Success) {
                this.ChildenScripts = new List<Script>();
                var inc_path = match_inc.Groups[1].Value;
                var inc_line_number = this.TextFormated.CountLines(0, match_inc.Index);
                foreach (var child_script_path in Utils.ExpandFilePaths(new string[] { inc_path }, @"\.js$|\.vbs$", this.Directory)) {
                    this.ChildenScripts.Add(new Script(child_script_path, this, inc_line_number));
                }
                match_inc = match_inc.NextMatch();
            }
            this.TextFormated = this.TextFormated.RemoveAll(@"^#Include[^\r\n]*");

            //Handle Console prints
            this.TextFormated = Regex.Replace(this.TextFormated, @"Debug\.Print", @"Wscript.Echo", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (par_script == null) {
                //Replace param
                if (this.Param != null)
                    this.TextFormated = Regex.Replace(this.TextFormated, @"\@\bparam\b", this.Param, RegexOptions.IgnoreCase);
                //Replace the wrapper instantiation if it's called using "New"
                //this.TextFormated = Regex.Replace(this.TextFormated, @"Set ([\w_-]+) = New (Selenium\.\w+)", @"Set $1 = CreateObject(""$2"")", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                //this.TextFormated = Regex.Replace(this.TextFormated, @"Dim ([\w_-]+) As New (Selenium\.\w+)", @"Set $1 = CreateObject(""$2"")", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                //Handles With
                var matches_with = Regex.Matches(this.TextFormated, (@"^\[With\((.*)\)\]\s+(Private |Public )?Sub ([\w_]+)"), RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.ScriptWithParams = new Dictionary<string, WithParams>(matches_with.Count);
                foreach (Match match in matches_with) {
                    string procName = match.Groups[3].Value;
                    string procParams = match.Groups[1].Value.Trim('\r', '\n', ' ');
                    int procLine = this.TextFormated.CountLines(0, match.Index);
                    ScriptWithParams.Add(procName, new WithParams {
                        Params = procParams,
                        Line = procLine
                    });
                }
                this.TextFormated = this.TextFormated.ReplaceAll(@"^\[With\((.*)\)\]", "");
            }
        }

        public string GetCode() {
            var buffer = new StringBuilder();
            this.WriteTextFormated(buffer);
            return buffer.ToString();
        }

        public TraceLine GetTraceLineAt(int line_number) {
            return GetTraceLineAt(new []{line_number});
        }


        private void WriteTextFormated(StringBuilder buffer) {
            if (this.ChildenScripts != null) {
                foreach (var child_script in this.ChildenScripts) {
                    child_script.WriteTextFormated(buffer);
                }
            }
            buffer.AppendLine(this.TextFormated);
        }

        private TraceLine GetTraceLineAt(int[] lineNumber) {
            if (this.ChildenScripts != null) {
                foreach (var child_script in this.ChildenScripts) {
                    TraceLine traceline = child_script.GetTraceLineAt(lineNumber);
                    if (traceline != null)
                        return traceline;
                }
            }
            if (lineNumber[0] <= this.LineCount)
                return new TraceLine(this, lineNumber[0]);
            lineNumber[0] -= this.LineCount + 1;
            return null;
        }

    }

}
