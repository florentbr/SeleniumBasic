using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.MSScript {

    [Guid("0E59F1D5-1FBE-11D0-8FF2-00A0D10038BC")]
    [ComImport, ComSourceInterfaces(typeof(IMSScriptControl))]
    [ClassInterface((short)0), TypeLibType((short)34)]
    class MSScriptControl {

    }

    [Guid("0E59F1D3-1FBE-11D0-8FF2-00A0D10038BC")]
    [ComImport(), InterfaceType(2), TypeLibType(4304), CoClass(typeof(MSScriptControl))]
    public interface IMSScriptControl : IMSScriptControl_Events {

        [DispId(1500)]
        string Language { [DispId(1500), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1500), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }

        [DispId(1501)]
        ScriptControlStates State { [TypeLibFunc((short)1024), DispId(1501), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1501), TypeLibFunc((short)1024), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }

        [DispId(1502)]
        int SitehWnd { [TypeLibFunc((short)1024), DispId(1502), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1502), TypeLibFunc((short)1024), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }

        [DispId(1503)]
        int Timeout { [DispId(1503), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1503), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }

        [DispId(1504)]
        bool AllowUI { [DispId(1504), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1504), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }

        [DispId(1505)]
        bool UseSafeSubset { [DispId(1505), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1505), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] set; }

        [DispId(1506)]
        IMSScriptModules Modules { [DispId(1506), TypeLibFunc((short)1024), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(1507)]
        IMSScriptError Error { [DispId(1507), TypeLibFunc((short)1024), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(1000)]
        object CodeObject { [DispId(1000), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(1001)]
        IMSScriptProcedures Procedures { [DispId(1001), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(-552)]
        [TypeLibFunc((short)64)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void _AboutBox();

        [DispId(2500)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddObject([MarshalAs(UnmanagedType.BStr), In] string Name, [MarshalAs(UnmanagedType.IDispatch), In] object Object, [In] bool AddMembers = false);

        [DispId(2501)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Reset();

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
