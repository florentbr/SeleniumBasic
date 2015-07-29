using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.Excel {
#pragma warning disable 1591

    /// <summary>
    /// Worksheets interface
    /// </summary>
    [Guid("000208D7-0000-0000-C000-000000000046"), ComImport, InterfaceType(2), TypeLibType(4288)]
    public interface IWorksheets {

        [DispId(0), IndexerName("_Default")]
        IWorksheet this[[MarshalAs(UnmanagedType.Struct), In] object Index] {
            [DispId(0), TypeLibFunc((short)1024), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(118)]
        int Count { 
            [DispId(118), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(181), LCIDConversion(4)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.IDispatch)]
        IWorksheet Add([MarshalAs(UnmanagedType.Struct), In, Optional] object Before
            , [MarshalAs(UnmanagedType.Struct), In, Optional] object After
            , [MarshalAs(UnmanagedType.Struct), In, Optional] object Count
            , [MarshalAs(UnmanagedType.Struct), In, Optional] object Type);

    }

}
