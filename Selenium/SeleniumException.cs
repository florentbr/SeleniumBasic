using System;
using System.Diagnostics;
using System.IO;

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
                StackFrame frame = new StackTrace(base.InnerException ?? this, true).GetFrame(0);
                string filename = Path.GetFileName(frame.GetFileName());
                int errline = frame.GetFileLineNumber();

                string typename = (base.InnerException ?? this).GetType().FullName;
                string message = base.InnerException == null ?
                    base.Message : base.InnerException.Message;

                return string.Format("{2}\nLine {0} in {1}\n{3}", errline, filename, typename, message);
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
