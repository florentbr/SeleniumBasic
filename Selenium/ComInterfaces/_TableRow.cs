using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-0B61E370369D")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface _TableRow {

        [DispId(0), IndexerName("Column"), Description("Gets or sets the value associated with the column identifier")]
        object this[[MarshalAs(UnmanagedType.Struct), In] object identifier] { get; set; }

        [DispId(1), Description("Gets thevalue associated with the column identifier")]
        object Get(object identifier);

        [DispId(2), Description("Sets the value associated with the column identifier")]
        void Set(object identifier, object value);

        [DispId(346), Description("Gets the cell asscociated with the column identifier")]
        object Cell(object identifier);

        [DispId(378), Description("Gets an array containing all the values of the row")]
        object Values { get; }

    }

}
