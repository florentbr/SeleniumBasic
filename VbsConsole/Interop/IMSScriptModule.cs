using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.MSScript {

    [Guid("0E59F1DC-1FBE-11D0-8FF2-00A0D10038BC")]
    [ComImport, ClassInterface((short)0)]
    public class MSScriptModule {

    }

    [Guid("70841C70-067D-11D0-95D8-00A02463AB28")]
    [ComImport(), InterfaceType(2), TypeLibType(4304), CoClass(typeof(MSScriptModule))]
    public interface IMSScriptModule {

        [DispId(0)]
        string Name { [DispId(0), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(1000)]
        object CodeObject { [DispId(1000), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(1001)]
        IMSScriptProcedures Procedures { [DispId(1001), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(2000)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddCode([MarshalAs(UnmanagedType.BStr), In] string Code);

        [DispId(2001)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object Eval([MarshalAs(UnmanagedType.BStr), In] string Expression);

        [DispId(2002)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ExecuteStatement([MarshalAs(UnmanagedType.BStr), In] string Statement);

        [DispId(2003)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object Run([MarshalAs(UnmanagedType.BStr), In] string ProcedureName, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT), In] params object[] Parameters);

    }

}