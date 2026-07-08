using Selenium;

namespace Selenium {
    /// <summary>
    /// Selenium base error class.
    /// </summary>
    /// <example>
    /// <code lang="vbs">
    /// Err.Clear : on error resume next
    /// driver.FindElementByCss("invalid selector").Click
    /// WScript.Echo "Err: " &amp; Err.Number &amp; " " &amp; Err.Description
    /// </code>
    /// </example>
    public class SeleniumError : SeleniumException {
        private sealed class ErrorCounter {
            private static int _lastMessageHash, _lastMessageCount;

            void LogCounter() {
                if( _lastMessageCount == 0 ) return;
                _l.Error( string.Format( "Previous error happened {0} times", _lastMessageCount ) );
                _lastMessageCount = 0;
            }
            public bool OnError( string message ) {
                int this_message_hash = message.GetHashCode();
                if( _lastMessageHash != this_message_hash ) {
                    LogCounter();
                    _lastMessageHash = this_message_hash;
                    return true;
                }
                _lastMessageCount++;
                return false;
            }
        }
        private static readonly NLog.Logger _l = NLog.LogManager.GetCurrentClassLogger();
        private static ErrorCounter _ec = _l.IsErrorEnabled ? new ErrorCounter() : null;

        protected const int FACILITY_CONTROL_ERROR = unchecked((int)0xA00A0000); // Err.Number will contain the lower 16 bit.

        /// <summary></summary>
        public Dictionary ResponseData { get; internal set; }
        private string _message;

        internal SeleniumError(string message, params object[] args)
            : this(args.Length > 0 ? string.Format(message, args) : message, 0) {
        }

        internal SeleniumError(int code, string message, params object[] args)
            : this(args.Length > 0 ? string.Format(message, args) : message, code) {
        }

        internal SeleniumError(string message, int code = 1) {
            if( _ec != null && _ec.OnError( message ) )
                _l.Error( this, message );
            _message = message;
            base.HResult = FACILITY_CONTROL_ERROR | code;
        }

        /// <summary>
        /// Exception message
        /// </summary>
        public override string Message {
            get {
                var typename = this.GetType().Name;
                return string.Format("{0}: {1}", typename, _message);
            }
        }

    }

}