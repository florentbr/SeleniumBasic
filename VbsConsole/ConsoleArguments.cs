using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace vbsc {

    /// <summary>
    /// Class to parse the concole arguments
    /// </summary>
    class ConsoleArguments {

        private const char CHAR_OPTION = '-';

        private readonly Dictionary<string, ConsoleArgument> _options;
        private readonly List<string> _files;
        private readonly StringBuilder _examples;
        private readonly StringBuilder _usage;

        public ConsoleArguments() {
            _usage = new StringBuilder();
            _options = new Dictionary<string, ConsoleArgument>(20);
            _files = new List<string>(10);
            _examples = new StringBuilder();
        }

        public object this[string id] {
            get {
                return _options[id].Value;
            }
        }

        public List<string> Files {
            get {
                return _files;
            }
        }

        public void AddOption(string id, string pattern, object default_value, string help, string info = null) {
            _options.Add(id, new ConsoleArgument {
                Id = id,
                Pattern = pattern,
                Value = default_value,
                Help = help,
                Info = info
            });
        }

        public void Update(string[] arguments, char option_char) {
            bool[] updatedArgs = new bool[arguments.Length];
            foreach (var option in _options.Values) {
                UpdateOptionValue(option, arguments, ref updatedArgs);
            }
            for (int i = 0; i < arguments.Length; i++) {
                if (updatedArgs[i])
                    continue;
                var arg = arguments[i];
                if (arg[0] == option_char)
                    throw new ConsoleArgumentException(arg);
                _files.Add(arguments[i]);
            }
        }

        public void AddExample(string example) {
            _examples.Append(' ').AppendLine(example);
        }

        private void UpdateOptionValue(ConsoleArgument option, string[] arguments, ref bool[] updatedArgs) {
            object defaultValue = option.Value;
            var re = new Regex(option.Pattern);
            string arg;
            for (int i = 0; i < arguments.Length; i++) {
                if (updatedArgs[i] || !re.IsMatch(arg = arguments[i]))
                    continue;
                updatedArgs[i] = true;
                if (defaultValue is bool) {
                    option.Value = true;
                } else {
                    var value = arg.Substring(arg.IndexOf('=') + 1).Trim('"');
                    if (defaultValue is string[]) {
                        option.Value = value.Split(',');
                    } else if (defaultValue is int) {
                        option.Value = int.Parse(value);
                    }
                }
                return;
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(@"Usage:");
            sb.Append(_usage);
            sb.AppendLine();
            sb.AppendLine(@"Options:");
            sb.AppendLine();
            foreach (ConsoleArgument option in _options.Values) {
                sb.Append(option).AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine(@"Exemples:");
            sb.AppendLine();
            sb.Append(_examples);
            return sb.ToString();
        }

        internal void AddUsage(string text) {
            _usage.AppendLine(text);
        }
    }

    class ConsoleArgumentException : Exception {
        public ConsoleArgumentException(string argument) :
            base("Invalide commande line argument: " + argument) {
        }
    }

}
