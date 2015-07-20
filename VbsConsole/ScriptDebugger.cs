using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace vbsc {

    class ScriptDebugger {

        public enum Cmd { Continue, Exit, Retry, Next }

        static ScriptDebugger() {
            EnableConsoleQuickMode();
        }

        public static void Run() {
            using (var scriptcontrol = new ScriptControl()) {
                scriptcontrol.Build();
                ScriptDebugger.Debug(scriptcontrol, false);
            }
        }

        public static Cmd Debug(ScriptControl scriptcontrol, bool stepMode = true) {
            if (stepMode){
                Logger.ConsoleOut.WriteLine("Debug mode: type n for the next execution.");
            } else {
                Logger.ConsoleOut.WriteLine("VisualBasic Script Host Console\nPreface with '?' for an evaluation.");
            }
            try {
                scriptcontrol.WScript.OnEcho += Logger.ConsoleOut.WriteLine;
                StringBuilder buffer = new StringBuilder();
                while (true) {
                    Logger.ConsoleOut.Write('>');
                    buffer.Length = 0;
                    while (true) {
                        string line = Logger.ConsoleIn.ReadLine();
                        if (line == null)
                            break;
                        if (line.Equals("n"))
                            return Cmd.Next;
                        if (string.IsNullOrEmpty(line))
                            break;
                        buffer.AppendLine(line);
                        if ((Control.ModifierKeys & Keys.Shift) == 0)
                            break;
                        Logger.ConsoleOut.Write(' ');
                    }
                    if (buffer.Length != 0) {
                        string command = buffer.ToString().Trim();
                        if (command.StartsWith("?")) {
                            object result;
                            if (scriptcontrol.Eval(command.Substring(1), out result)) {
                                if(result is string){
                                    string txt = "\"" + ((string)result).Replace("\"", "\"\"") + "\"";
                                    Logger.ConsoleOut.WriteLine(txt);
                                } else {
                                    Logger.ConsoleOut.WriteLine(string.Format("{0}", result));
                                }
                            } else {
                                Logger.ConsoleOut.WriteLine(scriptcontrol.Error.Message);
                            }
                        } else {
                            if (!scriptcontrol.Execute(command))
                                Logger.ConsoleOut.WriteLine(scriptcontrol.Error.Message);
                        }
                    }
                }
            } finally {
                scriptcontrol.WScript.OnEcho -= Logger.ConsoleOut.WriteLine;
            }
        }

        #region Support

        private static void EnableConsoleQuickMode() {
            IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(handle, mode);
        }

        #endregion


        #region Imports

        const string KERNEL32 = "kernel32.dll";
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x40 | 0x80;

        [DllImport(KERNEL32)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport(KERNEL32)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport(KERNEL32)]
        static extern IntPtr GetStdHandle(int handle);

        #endregion
    }

}
