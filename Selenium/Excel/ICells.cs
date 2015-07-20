using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Selenium.Excel {
#pragma warning disable 1591

    /// <summary>
    /// Range interface with the Resize property as an indexer
    /// </summary>
    [Guid("00020846-0000-0000-C000-000000000046"), ComImport, InterfaceType(2), TypeLibType(4096)]
    interface ICells {

        [DispId(0), IndexerName("_Default")]
        object this[[MarshalAs(UnmanagedType.Struct), In, Optional] object RowIndex, [MarshalAs(UnmanagedType.Struct), In, Optional] object ColumnIndex] {
            [TypeLibFunc((short)1024), DispId(0), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
            [TypeLibFunc((short)1024), DispId(0), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            set;
        }

    }

}
