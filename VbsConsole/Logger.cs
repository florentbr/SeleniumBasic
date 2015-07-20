using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace vbsc {

    static class Logger {

        const string SEPARATOR_LINE_DASH = "------------------------------------------------------------------";
        const string SEPARATOR_LINE_STAR = "************************************************************************";

        public static bool HideInfo { get; set; }

        private static readonly StringWriter _stdout;
        private static readonly TextWriter _console_stdout;
        private static readonly TextWriter _console_stderr;
        private static readonly TextReader _console_stdin;
        private static readonly StringBuilder _logfile_text;

        static Logger() {
            HideInfo = false;
            _stdout = new StringWriter();
            _console_stdout = System.Console.Out;
            _console_stderr = System.Console.Error;
            _console_stdin = System.Console.In;
            System.Console.SetError(_stdout);
            System.Console.SetOut(_stdout);
            _logfile_text = new StringBuilder();
        }

        public static TextWriter ConsoleOut {
            get {
                return _console_stdout;
            }
        }

        public static TextReader ConsoleIn {
            get {
                return _console_stdin;
            }
        }

        public static void Write(StringBuilder text, bool fileonly = false) {
            if (!fileonly) {
                lock (_console_stdout) {
                    _console_stdout.WriteLine(text);
                }
            }
            lock (_logfile_text) {
                _logfile_text.Append(text);
                _logfile_text.AppendLine();
            }
        }

        public static void LogStart(DateTime starttime) {
            var sb = new StringBuilder();
            sb.Append("START ");
            sb.AppendLine(starttime.ToString("yyyy/MM/dd HH:mm:ss"));
            Write(sb);
        }

        public static void LogException(Exception ex, string[] arguments) {
            var sb = new StringBuilder();
            sb.Append("[Exception] ");
            sb.AppendLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
            sb.AppendLine(SEPARATOR_LINE_DASH);
            sb.AppendLine(ex.Message.Trim('\r', '\n', ' '));
            sb.AppendLine("Arguments:");
            WriteArguments(sb, arguments);
            sb.AppendLine("StackTrace:");
            sb.AppendLine(ex.StackTrace);
            sb.AppendLine(SEPARATOR_LINE_STAR);
            sb.AppendLine("FAILED");
            Write(sb);
        }

        private static void WriteArguments(StringBuilder sb, string[] arguments) {
            if (arguments.Length != 0) {
                foreach (var arg in arguments) {
                    if (arg.Trim().Length != 0) {
                        sb.Append(' ').AppendLine(arg);
                    }
                }
            }
        }

        public static void LogError(string title, string info) {
            var sb = new StringBuilder();
            sb.Append("[Error] ").Append(title).AppendLine();
            sb.AppendLine(SEPARATOR_LINE_DASH);
            sb.AppendLine(info);
            sb.AppendLine(SEPARATOR_LINE_DASH);
            sb.AppendLine("FAILED");
            Write(sb);
        }

        internal static void LogScriptException(Script script, Exception ex) {
            StringBuilder sb = new StringBuilder();
            sb.Append("[Exception] ").Append(script).AppendLine();
            sb.AppendLine(SEPARATOR_LINE_DASH);
            sb.AppendLine(ex.Message.Trim('\r', '\n', ' '));
            sb.AppendLine();
            sb.AppendLine("StackTrace:");
            sb.AppendLine(ex.StackTrace);
            Write(sb);
        }

        internal static void LogScriptError(ScriptError error) {
            StringBuilder sb = new StringBuilder();
            sb.Append("[Fail] ").Append(error.Source).AppendLine();
            sb.AppendLine(SEPARATOR_LINE_DASH);
            sb.AppendLine(error.ToString().CleanEnd());
            Write(sb);
        }

        internal static void LogScriptInfo(string source, string text) {
            if (HideInfo)
                return;
            StringBuilder sb = new StringBuilder();
            sb.Append("[Info] ").Append(source).AppendLine();
            sb.AppendLine(SEPARATOR_LINE_DASH);
            sb.AppendLine(text.CleanEnd());
            Write(sb);
        }

        public static void SaveTo(string filepath) {
            using (var writer = new StreamWriter(new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.None))) {
                writer.Write(_logfile_text);
                writer.Write("\r\n\r\n\r\n\r\n\r\n\r\n");
            }
        }

        internal static void LogResults(List<IScriptResult> results, DateTime starttime, DateTime endtime) {
            List<int> succeed = new List<int>(results.Count);
            List<int> failed = new List<int>(results.Count);
            for (int i = 0, len = results.Count; i < len; i++) {
                (results[i].Succeed ? succeed : failed).Add(i);
            }
            var sb_results = new StringBuilder();
            sb_results.AppendLine();
            sb_results.AppendLine("[Results summary]");
            sb_results.AppendLine(SEPARATOR_LINE_DASH);
            if (failed.Count != 0) {
                sb_results.AppendLine("Failed:");
                foreach (int i in failed)
                    sb_results.Append(' ').AppendLine(results[i].Source);
            }
            if (succeed.Count != 0) {
                sb_results.AppendLine("Succeed:");
                foreach (int i in succeed)
                    sb_results.Append(' ').AppendLine(results[i].Source);
            }
            sb_results.AppendLine();
            sb_results.AppendLine(SEPARATOR_LINE_STAR);
            sb_results.AppendFormat("{0}: {1} failed / {2} run in {3}s",
                failed.Count == 0 ? "SUCCEED" : "FAILED",   //{0}
                failed.Count,                               //{1}
                failed.Count + succeed.Count,               //{2}
                Math.Round(endtime.Subtract(starttime).TotalSeconds, 2) //{3}
            );
            Write(sb_results);
        }

    }
}
