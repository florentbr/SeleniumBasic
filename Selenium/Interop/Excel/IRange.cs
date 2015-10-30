using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.Excel {
#pragma warning disable 1591

    /// <summary>
    /// Range interface with the Resize property as an indexer
    /// </summary>
    [Guid("00020846-0000-0000-C000-000000000046"), ComImport, InterfaceType(2), TypeLibType(4096)]
    public interface IRange {

        [DispId(138)]
        object Text {
            [DispId(138), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(256), IndexerName("Resize")]
        IRange this[[MarshalAs(UnmanagedType.Struct), In, Optional] object RowSize, [MarshalAs(UnmanagedType.Struct), In, Optional] object ColumnSize] {
            [TypeLibFunc((short)1024), DispId(0), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(148)]
        IExcel Application {
            [DispId(148), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(1388)]
        object Value2 {
            [DispId(1388), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
            [DispId(1388), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            set;
        }

        [DispId(243)]
        IRange CurrentRegion {
            [DispId(243), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(111)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object Clear();

        [DispId(118)]
        int Count {
            [DispId(118), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(240)]
        int Column {
            [DispId(240), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(257)]
        int Row {
            [DispId(257), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(2257)]
        IListObject ListObject {
            [DispId(2257), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(348)]
        IWorksheet Worksheet {
            [DispId(348), MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(410)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IRange SpecialCells([In] XlCellType Type, [MarshalAs(UnmanagedType.Struct), In, Optional] object Value);

    }

    public enum XlCellType {
        xlCellTypeSameValidation = -4175,
        xlCellTypeAllValidation = -4174,
        xlCellTypeSameFormatConditions = -4173,
        xlCellTypeAllFormatConditions = -4172,
        xlCellTypeComments = -4144,
        xlCellTypeFormulas = -4123,
        xlCellTypeConstants = 2,
        xlCellTypeBlanks = 4,
        xlCellTypeLastCell = 11,
        xlCellTypeVisible = 12
    }

}
