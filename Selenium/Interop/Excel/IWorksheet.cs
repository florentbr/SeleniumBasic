using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.Excel {
#pragma warning disable 1591

    /// <summary>
    /// Worksheet interface
    /// </summary>
    [Guid("000208D8-0000-0000-C000-000000000046"), ComImport, InterfaceType(2), TypeLibType(4288)]
    public interface IWorksheet {

        [DispId(412)]
        IRange UsedRange {
            [LCIDConversion(0), DispId(412), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(211), LCIDConversion(2)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Paste([MarshalAs(UnmanagedType.Struct), In, Optional] object Destination
            , [MarshalAs(UnmanagedType.Struct), In, Optional] object Link);

    }

}
