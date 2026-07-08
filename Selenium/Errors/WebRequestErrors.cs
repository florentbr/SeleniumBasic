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

        internal WebRequestError(string message, int code = 1)
            : base(message, code) { }


        internal static SeleniumError Select(int statusCode, string message) {
            switch (statusCode) {
                case  6: return new Errors.NoSuchDriverError(message);
                case  7: return new Errors.NoSuchElementError(message);
                case  8: return new Errors.NoSuchFrameError(message);
                case  9: return new Errors.UnknownCommandError(message);
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
                case 64: return new Errors.ElementNotClickableError(message);
                default: return new SeleniumError(message);
            }
        }

        // see https://w3c.github.io/webdriver/#errors
        internal static SeleniumError Select(string error, string message) {
#if DEBUG
            Console.WriteLine( "! " + error + ": " + message );
#endif                
            switch (error) {
                case "element click intercepted": return new Errors.ElementNotClickableError(message);
                case "element not interactable":  return new Errors.ElementNotInteractableError(message);
                case "invalid element state":     return new Errors.InvalidElementStateError(message);
                case "invalid cookie domain":     return new Errors.InvalidCookieDomainError(message);
                case "invalid selector":          return new Errors.InvalidSelectorError(message);
                case "move target out of bounds": return new Errors.MoveTargetOutOfBoundsError(message);
                case "no such alert":             return new Errors.NoAlertPresentError(message);
                case "no such element":           return new Errors.NoSuchElementError(message);
                case "unexpected alert open":     return new Errors.UnexpectedAlertOpenError(message);
                case "no such frame":             return new Errors.NoSuchFrameError(message);
                case "no such window":            return new Errors.NoSuchWindowError(message);
                case "script timeout":            return new Errors.ScriptTimeoutError(message);
                case "session not created":       return new Errors.SessionNotCreatedError(message);
                case "stale element reference":   return new Errors.StaleElementReferenceError(message);
                case "timeout":                   return new Errors.TimeoutError(message);
                case "unknown error":             return new Errors.UnknownError(message);
                case "unknown method":            return new Errors.UnknownCommandError(message);
                default: return new SeleniumError(message);
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class NoSuchDriverError : WebRequestError {
        internal NoSuchDriverError(string message = null)
            : base(6, message ?? "A session is either terminated or not started") { }
    }

    /// <summary>
    /// 7 An element could not be located
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
    /// 8 The frame could not be found
    /// </summary>
    public class NoSuchFrameError : WebRequestError {
        internal NoSuchFrameError()
            : base(8, "A request to switch to a frame could not be satisfied because the frame could not be found.") { }

        internal NoSuchFrameError(object identifier)
            : base(8, "Frame not be found.\nIdentifier:{0}", identifier) { }
    }

    /// <summary>
    /// 9 Unknown Command
    /// </summary>
    public class UnknownCommandError : WebRequestError {
        internal UnknownCommandError(string message = null)
            : base(9, message ?? "The requested resource could not be found, or a request was received using an HTTP method that is not supported by the mapped resource.") { }
    }

    /// <summary>
    /// 10 Referenced element is no longer attached
    /// </summary>
    public class StaleElementReferenceError : WebRequestError {
        internal StaleElementReferenceError(string message = null)
            : base(10, message ?? "An element command failed because the referenced element is no longer attached to the DOM.") { }
    }

    /// <summary>
    /// 11 An element command could not be completed because the element is not visible on the page.
    /// </summary>
    public class ElementNotVisibleError : WebRequestError {
        internal ElementNotVisibleError(string message = null)
            : base(11, message ?? "An element command could not be completed because the element is not visible on the page.") { }

        internal ElementNotVisibleError(By by)
            : base(11, "Element not visible for {0}", by.ToString()) { }
    }

    /// <summary>
    /// 12
    /// </summary>
    public class InvalidElementStateError : WebRequestError {
        internal InvalidElementStateError(string message = null)
            : base(12, message ?? "An element command could not be completed because the element is in an invalid state (e.g. attempting to click a disabled element).") { }
    }

    /// <summary>
    /// 13          An unknown server-side error
    /// </summary>
    public class UnknownError : WebRequestError {
        internal UnknownError(string message = null)
            : base(13, message ?? "An unknown server-side error occurred while processing the command.") { }
    }

    /// <summary>
    /// 15 Element that cannot be selected.
    /// </summary>
    public class ElementIsNotSelectableError : WebRequestError {
        internal ElementIsNotSelectableError(string message = null)
            : base(15, message ?? "An attempt was made to select an element that cannot be selected.") { }
    }

    /// <summary>
    /// 17 JavaScript error
    /// </summary>
    public class JavaScriptError : WebRequestError {
        internal JavaScriptError(string message = null)
            : base(17, message ?? "An error occurred while executing user supplied JavaScript.") { }
    }

    /// <summary>
    /// 19 XPath Lookup Error
    /// </summary>
    public class XPathLookupError : WebRequestError {
        internal XPathLookupError(string message = null)
            : base(19, message ?? "An error occurred while searching for an element by XPath.") { }
    }

    /// <summary>
    /// 21 
    /// </summary>
    public class TimeoutError : WebRequestError {
        internal TimeoutError(int timeout)
            : base(21, "Operation timed out after {0}ms.", timeout) { }
        internal TimeoutError(string message, params object[] args)
            : base(21, message, args) { }
    }

    /// <summary>
    /// 21 No response from the server
    /// </summary>
    public class WebRequestTimeout : WebRequestError {
        internal WebRequestTimeout(WebRequest request)
            : base(21, "No response from the server within {0} seconds ({1}).", request.Timeout / 1000, request.RequestUri) { }
    }

    /// <summary>
    /// 23 Window not found.
    /// </summary>
    public class NoSuchWindowError : WebRequestError {
        internal NoSuchWindowError()
            : base(23, "Window not found.") { }

        internal NoSuchWindowError(object info)
            : base(23, "Window not found: {0}", info) { }
    }

    /// <summary>
    /// 24 Invalid Cookie Domain
    /// </summary>
    public class InvalidCookieDomainError : WebRequestError {
        internal InvalidCookieDomainError(string message = null)
            : base(24, message ?? "An illegal attempt was made to set a cookie under a different domain than the current page.") { }
    }

    /// <summary>
    /// 25 Failed request to set a cookie
    /// </summary>
    public class UnableToSetCookieError : WebRequestError {
        internal UnableToSetCookieError(string message = null)
            : base(25, message ?? "A request to set a cookie's value could not be satisfied.") { }
    }

    /// <summary>
    /// 26
    /// </summary>
    public class UnexpectedAlertOpenError : WebRequestError {
        internal UnexpectedAlertOpenError(string message = null)
            : base(26, message ?? "A modal dialog was open, blocking this operation") { }
    }

    /// <summary>
    /// 27
    /// </summary>
    public class NoAlertPresentError : WebRequestError {
        internal NoAlertPresentError(string message = null)
            : base(27, message ?? "An attempt was made to operate on a modal dialog when one was not opened.") { }
    }

    /// <summary>
    /// 28
    /// </summary>
    public class ScriptTimeoutError : WebRequestError {
        internal ScriptTimeoutError(string message = null)
            : base(28, message ?? "A script did not complete before its timeout expired.") { }
    }

    /// <summary>
    /// 29
    /// </summary>
    public class InvalidElementCoordinatesError : WebRequestError {
        internal InvalidElementCoordinatesError(string message = null)
            : base(29, message ?? "The coordinates provided to an interactions operation are invalid.") { }
    }

    /// <summary>
    /// 30
    /// </summary>
    public class IMENotAvailableError : WebRequestError {
        internal IMENotAvailableError(string message = null)
            : base(30, message ?? "IME was not available.") { }
    }

    /// <summary>
    /// 31
    /// </summary>
    public class IMEEngineActivationFailedError : WebRequestError {
        internal IMEEngineActivationFailedError(string message = null)
            : base(31, message ?? "An IME engine could not be started.") { }
    }

    /// <summary>
    /// 32
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
    /// 34
    /// </summary>
    public class MoveTargetOutOfBoundsError : WebRequestError {
        internal MoveTargetOutOfBoundsError(string message = null)
            : base(34, message ?? "Target provided for a move action is out of bounds.") { }
    }



    /// <summary>
    /// 57
    /// </summary>
    public class BrowserNotStartedError : WebRequestError {
        internal BrowserNotStartedError()
            : base(57, "Browser not started. Call Get, Start or StartRemotely first.") { }
    }

    /// <summary>
    /// 59
    /// </summary>
    public class UnexpectedTagNameError : WebRequestError {
        internal UnexpectedTagNameError(string expected, string got)
            : base(59, "Unexpeted tag name.\nExpected={0}\nGot={1}", expected, got) { }
    }

    /// <summary>
    /// 60
    /// </summary>
    public class NoSuchCookieError : WebRequestError {
        internal NoSuchCookieError(string message = null)
            : base(60, message ?? "Cookie not found") { }
    }

    /// <summary>
    /// 77
    /// </summary>
    public class ElementPresentError : WebRequestError {
        internal ElementPresentError(By by)
            : base(77, "Element not expected for {0}", by.ToString()) { }
        internal ElementPresentError(Strategy strategy, string value)
            : base(77, "Element still present for {0}={1}", strategy, value) { }
    }

    // New ones

    /// <summary>
    /// 64 The Element Click command could not be completed because the element is obscured. 
    /// </summary>
    public class ElementNotClickableError : WebRequestError {
        internal ElementNotClickableError(string message = null)
            : base(64, message ?? "The Element Click command could not be completed because the element is obscured by an other element.") { }
    }

    /// <summary>
    /// 102 The element is not pointer- or keyboard interactable. 
    /// </summary>
    public class ElementNotInteractableError : WebRequestError {
        internal ElementNotInteractableError(string message = null)
            : base(102, message ?? "A command could not be completed because the element is not pointer- or keyboard interactable. ") { }
    }

    /// <summary>
    /// 9 A command cannot be supported. 
    /// </summary>
    public class UnsupportedOperationError : WebRequestError {
        internal UnsupportedOperationError(string message = null)
            : base(9, message ?? "A command that should have executed properly cannot be supported for some reason.") { }
    }

}
