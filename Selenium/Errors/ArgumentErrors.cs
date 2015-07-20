using System;

namespace Selenium.Errors {

    /// <summary>
    /// 
    /// </summary>
    public class ArgumentError : SeleniumError {

        const int CODE = 200;

        internal ArgumentError(string message, params object[] args)
            : this(CODE, message, args) { }

        internal ArgumentError(int code, string message, params object[] args)
            : base(CODE + code, message, args) { }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ArgumentTypeError : ArgumentError {
        internal ArgumentTypeError(Type extected, Type got)
            : base(1, "Invalid type. Was expecting a {0}, got a {1} intead", extected.Name, got.Name) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ArgumentRangeError : ArgumentError {
        internal ArgumentRangeError(string arg)
            : base(2, "Argument out of range: {0}", arg) {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvalideCommandError : ArgumentError {
        internal InvalideCommandError(string message)
            : base(3, message) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvalideModifierKeyError : ArgumentError {
        internal InvalideModifierKeyError()
            : base(56, "Invalid modifier key.\nOnly shift, ctrl or alt are allowed.") { }
    }

}
