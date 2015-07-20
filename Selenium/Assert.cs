using Selenium.Core;
using Selenium.Internal;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Selenium {

    /// <summary>
    /// Testing functions. Throws an exception if the condition is not met
    /// </summary>
    /// <example>
    /// 
    /// The following example asserts the page title.
    /// <code lang="vbs">	
    /// Set Assert = CreateObject("Selenium.Assert")
    /// Set driver = CreateObject("Selenium.FirefoxDriver")
    /// driver.get "http://www.google.com"
    /// Assert.Equals "Google", driver.Title
    /// driver.stop
    /// </code>
    /// 
    /// <code lang="vbs">	
    /// Public Sub TestCase()
    ///   Dim driver As New Selenium.FirefoxDriver, Assert as New Selenium.Assert
    ///   driver.get "http://www.google.com"
    ///   Assert.Equals "Google", driver.Title
    ///   driver.stop
    /// End Sub
    /// </code>
    /// 
    /// </example>
    [ProgId("Selenium.Assert")]
    [Description("Testing functions. Throws an exception if the condition is not met")]
    [Guid("0277FC34-FD1B-4616-BB19-6AAF7EDD33D6")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Assert : ComInterfaces._Assert {

        /// <summary>
        /// Create an assert object.
        /// </summary>
        public Assert() {
            UnhandledException.Initialize();
        }

        /// <summary>
        /// Raise an error if the value is true.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        public void True(bool input, string failmessage = null) {
            if (input != true)
                throw new AssertFailure(failmessage, "expected={0}\nwas={1}", true, input);
        }

        /// <summary>
        /// Raise an error if the value is false.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        public void False(bool input, string failmessage = null) {
            if (input != false)
                throw new AssertFailure(failmessage, "expected={0}\nwas={1}", false, input);
        }

        /// <summary>
        /// Test that two objects are equal and raise an exception if the result is false
        /// </summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="input">current object. Can be a string, number, array...</param>
        /// <param name="failmessage"></param>
        public void Equals(Object expected, Object input, string failmessage = null) {
            if (!ObjExt.AreEqual(expected, input))
                throw new AssertFailure(failmessage, "expected={0}\nwas={1}", expected, input);
        }

        /// <summary>
        /// Test that two objects are not equal and raise an exception if the result is false
        /// </summary>
        /// <param name="notexpected">expected object. Can be a string, number, array...</param>
        /// <param name="input">current object. Can be a string, number, array...</param>
        /// <param name="failmessage"></param>
        public void NotEquals(Object notexpected, Object input, string failmessage = null) {
            if (ObjExt.AreEqual(notexpected, input))
                throw new AssertFailure(failmessage, "expected!={0}\nwas={1}", notexpected, input);
        }

        /// <summary>
        /// Raise an error if the text matches the pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        public void Matches(string pattern, string input, string failmessage = null) {
            if (!Regex.IsMatch(input, pattern))
                throw new AssertFailure(failmessage, "pattern={0}\nwas={1}", pattern, input);
        }

        /// <summary>
        /// Raise an error if the text does not match the pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        public void NotMatches(string pattern, string input, string failmessage = null) {
            if (Regex.IsMatch(input, pattern))
                throw new AssertFailure(failmessage, "pattern!={0}\nwas={1}", pattern, input);
        }

        /// <summary>
        /// Raise an error if the text does not contain the value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        public void Contains(string value, string input, string failmessage = null) {
            if (!input.Contains(value))
                throw new AssertFailure(failmessage, "contains={0}\nwas={1}", value, input);
        }

        /// <summary>
        /// Raise an error.
        /// </summary>
        /// <param name="message"></param>
        public void Fail(string message = null) {
            throw new AssertFailure(message);
        }



        class AssertFailure : SeleniumException {

            const int HRESULT = -2146828288;
            string _message;

            public AssertFailure(string message, string template, object arg1, object arg2) {
                base.HResult = HRESULT;
                _message = message ?? string.Format(template
                    , ObjExt.ToStrings(arg1), ObjExt.ToStrings(arg2));
            }

            public AssertFailure(string message) {
                base.HResult = HRESULT;
                _message = message ?? string.Empty;
            }

            public override string Message {
                get {
                    var frame = new StackTrace(this, true).GetFrame(0);
                    var method = frame.GetMethod().Name;
                    return string.Format("Assert.{0} failed!\n{1}", method, _message).TrimEnd();
                }
            }

        }

    }

}
