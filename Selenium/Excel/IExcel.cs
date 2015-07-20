using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Selenium.Excel {
#pragma warning disable 1591

    [Guid("00024500-0000-0000-C000-000000000046"), ComImport, TypeLibType(2), ClassInterface((short)0)]
    class Excel { }


    /// <summary>
    /// Range interface with the Range property as an indexer
    /// </summary>
    [Guid("000208D5-0000-0000-C000-000000000046"), ComImport, InterfaceType(2), TypeLibType(4160)]
    public interface IExcel {

        [DispId(197), IndexerName("Range")]
        IRange this[[MarshalAs(UnmanagedType.Struct), In] object Cell1, [MarshalAs(UnmanagedType.Struct), In, Optional] object Cell2] {
            [TypeLibFunc((short)1024), DispId(0), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(283), TypeLibFunc((short)64), LCIDConversion(1)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Save([MarshalAs(UnmanagedType.Struct), In, Optional] object Filename);

        [DispId(494)]
        IWorksheets Worksheets {
            [DispId(494), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(572)]
        IWorkbooks Workbooks {
            [DispId(572), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(558)]
        bool Visible {
            [DispId(558), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
            [DispId(558), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            set;
        }

    }

}
