using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.Excel {
#pragma warning disable 1591

    /// <summary>
    /// Workbooks interface
    /// </summary>
    [Guid("000208DB-0000-0000-C000-000000000046"), ComImport, InterfaceType(2), TypeLibType(4288)]
    public interface IWorkbooks {

        [DispId(181)]
        [LCIDConversion(1)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IWorkbook Add([MarshalAs(UnmanagedType.Struct), In, Optional] object Template);

    }

}
