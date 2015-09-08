using Selenium.ComInterfaces;
using Selenium.Core;
using Selenium.Internal;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Selenium {

    /// <summary>
    /// Testing functions. Return the résult of the verification
    /// </summary>
    /// <example>
    /// 
    /// The following example asserts the page title.
    /// <code lang="vbs">	
    /// Set driver = CreateObject("Selenium.WebDriver")
    /// Set Verify = CreateObject("Selenium.Verify")
    /// driver.start "firefox", "http://www.google.com"
    /// driver.get "/"
    /// wscript.echo Verify.Equals("Google", driver.Title)
    /// driver.stop
    /// </code>
    /// 
    /// <code lang="vbs">	
    /// Public Sub TestCase()
    ///   Dim driver As New FirefoxDriver, Verify as New Verify
    ///   driver.Get "http://www.google.com"
    ///   Range("A1") = Verify.Equals("Google", driver.Title)
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// 
    /// </example>
    ///

    [ProgId("Selenium.Verify")]
    [Guid("0277FC34-FD1B-4616-BB19-B0C8C528C673")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [Description("Testing functions. Return the résult of the verification")]
    public class Verify : _Verify {

        const string MSG_OK = "OK";
        const string MSG_KO = "KO";

        private static string format(string message, string template, object arg1, object arg2) {
            if (message == null)
                message = string.Format(template, ObjExt.ToStrings(arg1), ObjExt.ToStrings(arg2));
            return (MSG_KO + ' ' + message).TrimEnd();
        }


        #region Static API

        /// <summary>
        /// Tests that an input is true.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public static string True(bool input, string failmessage = null) {
            return input ? MSG_OK : format(failmessage, "expected={0} was={1}", true, input);
        }

        /// <summary>
        /// Tests that an input is false.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public static string False(bool input, string failmessage = null) {
            return !input ? MSG_OK : format(failmessage, "expected={0} was={1}", false, input);
        }

        /// <summary>
        /// Tests that two objects are equal.
        /// </summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="input">current object. Can be a string, number, array...</param>
        /// <param name="failmessage">Message to return if the verification fails...</param>
        public static string Equals(object expected, object input, string failmessage = null) {
            return ObjExt.AreEqual(expected, input) ? MSG_OK
                : format(failmessage, "expected={0} was={1}", expected, input);
        }

        /// <summary>
        /// Tests that two objects are not equal.
        /// </summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="input">current object. Can be a string, number, array...</param>
        /// <param name="failmessage">Message to return if the verification fails...</param>
        public static string NotEquals(object expected, object input, string failmessage = null) {
            return !ObjExt.AreEqual(expected, input) ? MSG_OK
                : format(failmessage, "expected!={0} was={1}", expected, input);
        }

        /// <summary>
        /// Tests that an input matches a regular expression pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public static string Matches(string pattern, string input, string failmessage = null) {
            return Regex.IsMatch(input, pattern) ? MSG_OK
                : format(failmessage, "pattern={0} was={1}", input, pattern);
        }

        /// <summary>
        /// Tests that an input does not matche a regular expression pattern.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public static string NotMatches(string pattern, string input, string failmessage = null) {
            return !Regex.IsMatch(input, pattern) ? MSG_OK
                : format(failmessage, "pattern!={0} was={1}", pattern, input);
        }

        /// <summary>
        /// Tests that an input does not contain a value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        public static string Contains(string value, string input, string failmessage = null) {
            return input.Contains(value) ? MSG_OK
                : format(failmessage, "contains={0} was={1}", value, input);
        }

        #endregion


        #region COM Interface

        /// <summary>
        /// Create a Verify object.
        /// </summary>
        public Verify() {
            UnhandledException.Initialize();
        }

        /// <summary>
        /// Tests that an input is true.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        string _Verify.True(bool input, string failmessage) {
            return True(input, failmessage);
        }

        /// <summary>
        /// Tests that an input is false.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        string _Verify.False(bool input, string failmessage) {
            return False(input, failmessage);
        }

        /// <summary>
        /// Tests that two objects are equal.
        /// </summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="input">current object. Can be a string, number, array...</param>
        /// <param name="failmessage">Message to return if the verification fails...</param>
        string _Verify.Equals(object expected, object input, string failmessage) {
            return Equals(expected, input, failmessage);
        }

        /// <summary>
        /// Tests that two objects are not equal.
        /// </summary>
        /// <param name="expected">expected object. Can be a string, number, array...</param>
        /// <param name="input">current object. Can be a string, number, array...</param>
        /// <param name="failmessage">Message to return if the verification fails...</param>
        string _Verify.NotEquals(object expected, object input, string failmessage) {
            return NotEquals(expected, input, failmessage);
        }

        /// <summary>
        /// Tests that an input matches a regular expression pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input"></param>
        /// <param name="failmessage"></param>
        /// <returns></returns>
        string _Verify.Matches(string pattern, string input, string failmessage) {
            return Matches(pattern, input, failmessage);
        }

        /// <summary>
        /// Tests that an input does not matche a regular expression pattern.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="failmessage">Optional</param>
        /// <returns></returns>
        string _Verify.NotMatches(string pattern, string input, string failmessage) {
            return NotMatches(pattern, input, failmessage);
        }

        /// <summary>
        /// Tests that an input does not contain a value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <param name="failmessage">Optional</param>
        /// <returns></returns>
        string _Verify.Contains(string value, string input, string failmessage) {
            return Contains(value, input, failmessage);
        }

        #endregion

    }

}
