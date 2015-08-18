using Selenium;

namespace Selenium {

    /// <summary>
    /// Selenium base error class.
    /// </summary>
    public class SeleniumError : SeleniumException {

        /// <summary></summary>
        public Dictionary ResponseData { get; internal set; }
        private string _message;

        internal SeleniumError(string message, params object[] args)
            : this(args.Length > 0 ? string.Format(message, args) : message, 0) {
        }

        internal SeleniumError(int code, string message, params object[] args)
            : this(args.Length > 0 ? string.Format(message, args) : message, code) {
        }

        internal SeleniumError(string message, int code = 0) {
            _message = message;
            base.HResult = -2146828288 + code;
        }

        /// <summary>
        /// Exception message
        /// </summary>
        public override string Message {
            get {
                var typename = this.GetType().Name;
                return string.Format("{0}\n{1}", typename, _message);
            }
        }

    }

}