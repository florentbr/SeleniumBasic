using Interop.Excel;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591
    /// <summary>
    /// Represents a collection of values.
    /// </summary>
    /// <seealso cref="List"/>
    [Guid("0277FC34-FD1B-4616-BB19-C539CB44B63F")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface _List {

        [DispId(0), Description("Gets or sets the value at index (One based)")]
        object this[int index] { get; set; }

        [DispId(1), Description("Set the value at index (One based)")]
        void Set(int index, object value);

        [DispId(2), Description("Get the value at index (One based)")]
        object Get(int index);

        [DispId(118), Description("Gets the number of elements")]
        int Count { get; }

        [DispId(345), Description("Returns the first element")]
        object First();

        [DispId(367), Description("Returns the last element")]
        object Last();

        [DispId(478), Description("Adds a value")]
        int Add(object value);

        [DispId(485), Description("Adds mutiple values at once")]
        void AddRange(object[] value);

        [DispId(487), Description("Remove item at index (One based)")]
        void RemoveAt(int index);

        [DispId(490), Description("Insert item at index (One based)")]
        void Insert(int index, object value);

        [DispId(492), Description("Checks if the list contains the vale")]
        bool Contains(object value);

        [DispId(496), Description("Returns an array with the values")]
        object[] Values();

        [DispId(550), Description("Copy the values to the target address, range or worksheet in Excel. Returns the range.")]
        IRange ToExcel(object target = null, string title = null, bool clear = false);

        [DispId(600), Description("Sorts the elements.")]
        List Sort();

        [DispId(608), Description("Remove duplicates.")]
        List Distinct(bool ignoreCase = false);

        [DispId(-4)]
        System.Collections.IEnumerator _NewEnum();  //Has to be declared last

    }

}
