using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.CustomMarshalers;

namespace Interop.MSScript {

    [Guid("70841C71-067D-11D0-95D8-00A02463AB28")]
    [ComImport(), InterfaceType(2), TypeLibType(4304)]
    public interface IMSScriptProcedures : IEnumerable {

        [DispId(0)]
        IMSScriptProcedure this[[MarshalAs(UnmanagedType.Struct), In] object Index] { [DispId(0), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(1)]
        int Count { [DispId(1), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [TypeLibFunc((short)64)]
        [DispId(-4)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(EnumeratorToEnumVariantMarshaler))]
        new IEnumerator GetEnumerator();

    }

}