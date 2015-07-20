using System;
using System.IO;

namespace vbsc {

    class Program {

        [MTAThread]
        static void Main(string[] arguments) {

            Environment.ExitCode = 1;

            if (arguments.Length == 0) {
                ScriptDebugger.Run();
                return;
            }

            var args = new ConsoleArguments();
            args.AddUsage(@"vbsc.exe [OPTIONS] ""file1"" ""file2"" ...");
            args.AddOption("help", @"^[-/]?(help|\?)$", false, "-help :  Provide help");
            args.AddOption("debug", @"^-debug$", false, "-debug : Breaks on errors");
            args.AddOption("noexit", @"^-i$|^-noexit$", false, "-i,noexit : The console remains open at the end");
            args.AddOption("noinfo", @"^-noinfo$", false, "-noinfo : Do not display the text from WScript.Echo");
            args.AddOption("args", @"^-args=|^a=", new string[0], "-args,-a=value1,value2,... :", "Lists of arguments to send to the script (Comma to separate values).");
            args.AddOption("out", @"^-out=|^-o=", default(string), "-out,-o=filepath : ", "Log file. To add the current datetime : {yyyyMMdd-HHmmss}");
            args.AddOption("filter", @"^-filter=|^-f=", ".*", "-filter,-f=value : ", "Pattern to filter procedures");
            args.AddOption("params", @"^-params=|^-p=", new string[0], "-params,-p=value1,value2,... : ", "Lists of params to run each script with (Tag = \"@param\").");
            args.AddOption("threads", @"^-threads=|^-t=", 1, "-threads,-t=n : ", "Number of script to execute in parallel.");
            args.AddExample(@"vbsc noexit args=firefox,chrome ""c:\scripts\*.vbs\"" ""c:\scripts\*.vbs\""");
            args.AddExample(@"vbsc o=""c:\scripts\result-{t}.log"" ""c:\scripts\*.vbs""  //log with datetime");
            args.AddExample(@"vbsc o=""c:\scripts\result-{i}.log"" ""c:\scripts\*.vbs""  //log with number");
            args.AddExample(@"vbsc t=4 ""c:\scripts\*.vbs""  //parallele execution");

            try {
                args.Update(arguments, '-');
            } catch (ConsoleArgumentException ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(args.ToString());
                return;
            }

            if ((bool)args["help"]) {
                //Display help
                Console.WriteLine(args);
            } else {
                if (args.Files.Count > 0) {
                    var dir = Path.GetDirectoryName(args.Files[0]);
                    if (!string.IsNullOrEmpty(dir))
                        Directory.SetCurrentDirectory(dir);
                }

                //Run scripts
                try {
                    if (RunScripts(args))
                        Environment.ExitCode = 0;
                } catch (Exception ex) {
                    Logger.LogException(ex, arguments);
                }
            }

            //Wait for a key pressed if noexit is present
            if ((bool)args["noexit"])
                Console.ReadKey();
        }

        static bool RunScripts(ConsoleArguments options) {

            var starttime = DateTime.Now;
            Logger.LogStart(starttime);
            Logger.HideInfo = (bool)options["noinfo"];

            //Check log path
            var logpath = (string)options["out"];
            if (logpath != null) {
                if (logpath.IndexOf("{t}") != -1) {
                    logpath = logpath.Replace("{t}", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                }
                if (logpath.IndexOf("{n}") != -1) {
                    var id = 0;
                    string new_logpath;
                    do {
                        new_logpath = logpath.Replace("{n}", (++id).ToString());
                    } while (File.Exists(new_logpath));
                    logpath = new_logpath;
                }
                if (logpath.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
                    Logger.LogError("Invalide log file path.", string.Concat("Argument: ", options["out"]));
                    return false;
                }
            }

            string[] list_scripts_path;
            try {
                list_scripts_path = Utils.ExpandFilePaths(options.Files, @"\.vbs$|\.js$");
            } catch (FileNotFoundException ex) {
                Logger.LogError(ex.Message, "Argument: " + ex.FileName);
                return false;
            }

            var runner = new MultiScriptRunner((int)options["threads"]);
            var results = runner.Run(list_scripts_path, (string[])options["args"], (string[])options["params"], (string)options["filter"], (bool)options["debug"]);

            //Print final result
            Logger.LogResults(results, starttime, DateTime.Now);

            //Save log file                
            if (!string.IsNullOrEmpty(logpath)) {
                Logger.SaveTo(logpath);
            }
            return results.Exists((r) => !r.Succeed) == false;
        }

    }

}
