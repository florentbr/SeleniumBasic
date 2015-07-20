using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-B825A6BF9610")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface _Table : IEnumerable {

        [DispId(0), Description("Gets the row at the the given index (One based)")]
        object this[int index] { get; }

        [DispId(128), Description("Returns the number of rows")]
        int Count { get; }

        [DispId(8253), Description("Loads the data from an Excel source (address, range or worksheet)")]
        Table From(object sourceExcel, bool hasHeaders = true);

        [DispId(8346), Description("Filter the rows using the given expression")]
        Table Where(string expression, string sortBy = null);

        [DispId(9076), Description("Returns the values in a 2dim array")]
        object[,] Values();

        [DispId(550), Description("Copy the data to the target (Address, Range or Worksheet).")]
        void ToExcel(object target = null, bool clear = false);

        [DispId(9999)]
        void Dispose();

        [DispId(-4)]
        System.Collections.IEnumerator _NewEnum();  //Has to be declared last

    }

}