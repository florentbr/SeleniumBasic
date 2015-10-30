using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.Excel {
#pragma warning disable 1591

    /// <summary>
    /// Workbook interface
    /// </summary>
    [Guid("00024471-0000-0000-C000-000000000046"), ComImport, InterfaceType(2), TypeLibType(4160)]
    public interface IListObject {

        [DispId(2313)]
        IRange HeaderRowRange {
            [DispId(2313), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

        [DispId(705)]
        IRange DataBodyRange {
            [DispId(705), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
        }

    }

}
