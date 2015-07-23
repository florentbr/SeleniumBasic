using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-B51CB7C5A694")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Alert {

        [DispId(10), Description("Dismisses the alert.")]
        void Dismiss();

        [DispId(14), Description("Accepts the alert.")]
        void Accept();

        [DispId(24), Description("Sends keys to the alert.")]
        void SendKeys(string keysToSend);

        [DispId(43), Description("Gets the text of the alert.")]
        string Text { get; }

        [DispId(54), Description("Sets the user name and password in an alert prompting for credentials.")]
        void SetAuthenticationCredentials(string user, string password);

    }

}
