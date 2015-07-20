using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.MSScript {

    [Guid("70841C73-067D-11D0-95D8-00A02463AB28")]
    [ComImport(), InterfaceType(2), TypeLibType(4304)]
    public interface IMSScriptProcedure {

        [DispId(0)]
        string Name{ [DispId(0), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(100)]
        int NumArgs { [DispId(100), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(101)]
        bool HasReturnValue { [DispId(101), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    }

}