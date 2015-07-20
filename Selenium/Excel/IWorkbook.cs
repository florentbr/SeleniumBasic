using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Selenium.Excel {
#pragma warning disable 1591

    /// <summary>
    /// Workbook interface
    /// </summary>
    [Guid("000208DA-0000-0000-C000-000000000046"), ComImport, InterfaceType(2), TypeLibType(4160)]
    public interface IWorkbook {

        [DispId(181), LCIDConversion(1)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IWorkbook Add([MarshalAs(UnmanagedType.Struct), In, Optional] object Template);

        [DispId(494)]
        IWorksheets Worksheets { 
            [DispId(494), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

    }

}
