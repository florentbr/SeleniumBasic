using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Selenium {

    /// <summary>
    /// Selenium base exception class.
    /// </summary>
    public class SeleniumException : Exception {

        internal SeleniumException()
            : base() { }

        internal SeleniumException(Exception exception)
            : base(exception.Message, exception) { }

        internal SeleniumException(string message, params object[] args)
            : base(args.Length > 0 ? string.Format(message, args) : message) { }

        /// <summary>
        /// Exception message
        /// </summary>
        public override string Message {
            get {
                StackTrace stack = new StackTrace(base.InnerException ?? this, true);

                string filename = null;
                int errline = 0;
                for (int i = 0; i < stack.FrameCount; i++) {
                    StackFrame frame = stack.GetFrame(i);
                    string filepath = frame.GetFileName();
                    if (filepath != null) {
                        filename = Path.GetFileName(filepath);
                        errline = frame.GetFileLineNumber();
                        break;
                    }
                }

                string typename = (base.InnerException ?? this).GetType().FullName;
                string message = base.InnerException == null ?
                    base.Message : base.InnerException.Message;

                return string.Format("{2}\nLine {0} in {1}\n{3}"
                    , errline, filename, typename, message);
            }
        }

        /// <summary>
        /// Exception source
        /// </summary>
        public override string Source {
            get { return "Selenium"; }
        }

    }

}
