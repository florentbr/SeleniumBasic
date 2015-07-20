using System;
using System.Net;

namespace Selenium.Errors {

    /// <summary>
    /// 
    /// </summary>
    public class WebRequestError : SeleniumError {

        internal WebRequestError(string message, params object[] args)
            : base(message, args) { }

        internal WebRequestError(int code, string message, params object[] args)
            : base(code, message, args) { }

        internal WebRequestError(string message, int code = 0)
            : base(message, code) { }


        internal static SeleniumError Select(int statusCode, string message) {
            switch (statusCode) {
                case 6: return new Errors.NoSuchDriverError(message);
                case 7: return new Errors.NoSuchElementError(message);
                case 8: return new Errors.NoSuchFrameError(message);
                case 9: return new Errors.UnknownCommandError(message);
                case 10: return new Errors.StaleElementReferenceError(message);
                case 11: return new Errors.ElementNotVisibleError(message);
                case 12: return new Errors.InvalidElementStateError(message);
                case 13: return new Errors.UnknownError(message);
                case 15: return new Errors.ElementIsNotSelectableError(message);
                case 17: return new Errors.JavaScriptError(message);
                case 19: return new Errors.XPathLookupError(message);
                case 21: return new Errors.TimeoutError(message);
                case 23: return new Errors.NoSuchWindowError(message);
                case 24: return new Errors.InvalidCookieDomainError(message);
                case 25: return new Errors.UnableToSetCookieError(message);
                case 26: return new Errors.UnexpectedAlertOpenError(message);
                case 27: return new Errors.NoAlertPresentError(message);
                case 28: return new Errors.ScriptTimeoutError(message);
                case 29: return new Errors.InvalidElementCoordinatesError(message);
                case 30: return new Errors.IMENotAvailableError(message);
                case 31: return new Errors.IMEEngineActivationFailedError(message);
                case 32: return new Errors.InvalidSelectorError(message);
                case 33: return new Errors.SessionNotCreatedError(message);
                case 34: return new Errors.MoveTargetOutOfBoundsError(message);
                default: return new SeleniumError(message);
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class WebRequestTimeout : WebRequestError {
        internal WebRequestTimeout(WebRequest request)
            : base(101, "No response from the server within {0} seconds ({1}).", request.Timeout, request.RequestUri) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoSuchDriverError : WebRequestError {
        internal NoSuchDriverError(string message = null)
            : base(6, message ?? "A session is either terminated or not started") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoSuchElementError : WebRequestError {
        internal NoSuchElementError(string message)
            : base(7, message ?? "An element could not be located on the page using the given search parameters.") { }

        internal NoSuchElementError(Strategy strategy, string value)
            : base(7, "Element not found for {0}={1}", strategy, value) { }

        internal NoSuchElementError(By by)
            : base(7, "Element not found for {0}", by.ToString()) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoSuchFrameError : WebRequestError {
        internal NoSuchFrameError()
            : base(8, "A request to switch to a frame could not be satisfied because the frame could not be found.") { }

        internal NoSuchFrameError(object identifier)
            : base(8, "Frame not be found.\nIdentifier:{0}", identifier) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UnknownCommandError : WebRequestError {
        internal UnknownCommandError(string message = null)
            : base(9, message ?? "The requested resource could not be found, or a request was received using an HTTP method that is not supported by the mapped resource.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StaleElementReferenceError : WebRequestError {
        internal StaleElementReferenceError(string message = null)
            : base(10, message ?? "An element command failed because the referenced element is no longer attached to the DOM.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ElementNotVisibleError : WebRequestError {
        internal ElementNotVisibleError(string message = null)
            : base(11, message ?? "An element command could not be completed because the element is not visible on the page.") { }

        internal ElementNotVisibleError(By by)
            : base(7, "Element not visible for {0}", by.ToString()) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvalidElementStateError : WebRequestError {
        internal InvalidElementStateError(string message = null)
            : base(12, message ?? "An element command could not be completed because the element is in an invalid state (e.g. attempting to click a disabled element).") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UnknownError : WebRequestError {
        internal UnknownError(string message = null)
            : base(13, message ?? "An unknown server-side error occurred while processing the command.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ElementIsNotSelectableError : WebRequestError {
        internal ElementIsNotSelectableError(string message = null)
            : base(15, message ?? "An attempt was made to select an element that cannot be selected.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JavaScriptError : WebRequestError {
        internal JavaScriptError(string message = null)
            : base(17, message ?? "An error occurred while executing user supplied JavaScript.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class XPathLookupError : WebRequestError {
        internal XPathLookupError(string message = null)
            : base(19, message ?? "An error occurred while searching for an element by XPath.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TimeoutError : WebRequestError {
        internal TimeoutError(int timeout)
            : base(21, "Operation timed out after {0}ms.", timeout) { }
        internal TimeoutError(string message, params object[] args)
            : base(21, message, args) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoSuchWindowError : WebRequestError {
        internal NoSuchWindowError()
            : base(23, "Window not found.") { }

        internal NoSuchWindowError(object info)
            : base(23, "Window not found: {0}", info) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvalidCookieDomainError : WebRequestError {
        internal InvalidCookieDomainError(string message = null)
            : base(24, message ?? "An illegal attempt was made to set a cookie under a different domain than the current page.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UnableToSetCookieError : WebRequestError {
        internal UnableToSetCookieError(string message = null)
            : base(25, message ?? "A request to set a cookie's value could not be satisfied.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UnexpectedAlertOpenError : WebRequestError {
        internal UnexpectedAlertOpenError(string message = null)
            : base(26, message ?? "A modal dialog was open, blocking this operation") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoAlertPresentError : WebRequestError {
        internal NoAlertPresentError(string message = null)
            : base(27, message ?? "An attempt was made to operate on a modal dialog when one was not opened.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ScriptTimeoutError : WebRequestError {
        internal ScriptTimeoutError(string message = null)
            : base(28, message ?? "A script did not complete before its timeout expired.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvalidElementCoordinatesError : WebRequestError {
        internal InvalidElementCoordinatesError(string message = null)
            : base(29, message ?? "The coordinates provided to an interactions operation are invalid.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class IMENotAvailableError : WebRequestError {
        internal IMENotAvailableError(string message = null)
            : base(30, message ?? "IME was not available.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class IMEEngineActivationFailedError : WebRequestError {
        internal IMEEngineActivationFailedError(string message = null)
            : base(31, message ?? "An IME engine could not be started.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InvalidSelectorError : WebRequestError {
        internal InvalidSelectorError(string message = null)
            : base(32, message ?? "Argument was an invalid selector (e.g. XPath/CSS).") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SessionNotCreatedError : WebRequestError {
        internal SessionNotCreatedError(string message = null)
            : base(33, message ?? "A new session could not be created.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MoveTargetOutOfBoundsError : WebRequestError {
        internal MoveTargetOutOfBoundsError(string message = null)
            : base(34, message ?? "Target provided for a move action is out of bounds.") { }
    }



    /// <summary>
    /// 
    /// </summary>
    public class BrowserNotStartedError : WebRequestError {
        internal BrowserNotStartedError()
            : base(57, "Browser not started. Use Start or StartRemotely.") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UnexpectedTagNameError : WebRequestError {
        internal UnexpectedTagNameError(string expected, string got)
            : base(59, "Unexpeted tag name.\nExpected={0}\nGot={1}", expected, got) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoSuchCookieError : WebRequestError {
        internal NoSuchCookieError(string message = null)
            : base(60, message ?? "Cookie not found") { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ElementPresentError : WebRequestError {
        internal ElementPresentError(By by)
            : base(77, "Element not expected for {0}", by.ToString()) { }
        internal ElementPresentError(Strategy strategy, string value)
            : base(77, "Element still present for {0}={1}", strategy, value) { }
    }


}
