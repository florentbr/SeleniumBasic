using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-0EA52ACB97D1")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Assert {

        [DispId(564), Description("Raise an error if the value is true.")]
        void True(bool value, string failmessage = null);

        [DispId(567), Description("Raise an error if the value is false.")]
        void False(bool value, string failmessage = null);

        [DispId(574), Description("Raise an error if the objects are not equal.")]
        void Equals(object expected, object input, string failmessage = null);

        [DispId(579), Description("Raise an error if the objects are equal.")]
        void NotEquals(object expected, object input, string failmessage = null);

        [DispId(583), Description("Raise an error if the text matches the pattern.")]
        void Matches(string pattern, string input, string failmessage = null);

        [DispId(587), Description("Raise an error if the text does not match the pattern.")]
        void NotMatches(string pattern, string input, string failmessage = null);

        [DispId(589), Description("Raise an error if the text does not contain the value.")]
        void Contains(string value, string input, string failmessage = null);

        [DispId(608), Description("Raise an error.")]
        void Fail(string message = null);

    }

}
