using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-4CE442A16502")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _SelectElement {

        [DispId(401), Description("")]
        bool IsMultiple { get; }

        [DispId(404), Description("Returns a list of all options belonging to this select tag")]
        WebElements Options { get; }

        [DispId(408), Description("The first selected option in this select tag (or the currently selected option in a normal select)")]
        WebElement SelectedOption { get; }

        [DispId(411), Description("Gets all of the selected options within the select element.")]
        WebElements AllSelectedOptions { get; }

        [DispId(413), Description("Select the option at the given index. This is done by examing the “index” attribute of an element, and not merely by counting.")]
        void SelectByIndex(int index);

        [DispId(416), Description("Select all options that display text matching the argument.")]
        void SelectByText(string text);

        [DispId(418), Description("Select all options that have a value matching the argument.")]
        void SelectByValue(string value);

        [DispId(421), Description("Clear all selected entries. This is only valid when the SELECT supports multiple selections. throws NotImplementedError If the SELECT does not support multiple selections")]
        void DeselectAll();

        [DispId(423), Description("Deselect the option at the given index. This is done by examing the “index” attribute of an element, and not merely by counting.")]
        void DeselectByIndex(int index);

        [DispId(425), Description("Deselect all options that display text matching the argument.")]
        void DeselectByText(string text);

        [DispId(427), Description("Deselect all options that have a value matching the argument. That is, when given “foo” this would deselect an option like:")]
        void DeselectByValue(string value);

    }
}
