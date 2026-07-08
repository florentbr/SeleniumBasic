using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    /// <summary>
    /// Testing functions. Return the résult of the verification
    /// </summary>
    /// <seealso cref="Verify"/>
    [Guid("0277FC34-FD1B-4616-BB19-495CC9DBFB96")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Verify {

        [DispId(564), Description("Returns <OK> if the value is true, <NOK ...> otherwise.")]
        string True(bool value, string failmessage = null);

        [DispId(567), Description("Returns <OK> if the value is false, <NOK ...> otherwise.")]
        string False(bool value, string failmessage = null);

        [DispId(574), Description("Returns <OK> if the objects are equal, <NOK ...> otherwise.")]
        string Equals(object expected, object input, string failmessage = null);

        [DispId(579), Description("Returns <OK> if the objects are not equal, <NOK ...> otherwise.")]
        string NotEquals(object expected, object input, string failmessage = null);

        [DispId(583), Description("Returns <OK> if the text matches the pattern, <NOK ...> otherwise.")]
        string Matches(string pattern, string input, string failmessage = null);

        [DispId(587), Description("Returns <OK> if the text does not match the pattern, <NOK ...> otherwise.")]
        string NotMatches(string pattern, string input, string failmessage = null);

        [DispId(589), Description("Returns <OK> if the text contains the value, <NOK ...> otherwise.")]
        string Contains(string value, string input, string failmessage = null);

    }

}
