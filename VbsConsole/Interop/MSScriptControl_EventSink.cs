using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Interop.MSScript {
    
    [Guid("8B167D60-8605-11D0-ABCB-00A0C90FFFC0")]
    [ComImport, InterfaceType((short)2), TypeLibType((short)4112)]
    public interface IMSScriptControl_EventSink {

        [DispId(3000)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Error();

        [DispId(3001)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Timeout();

    }


    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    class MSScriptControl_EventSink : IMSScriptControl_EventSink {

        public DScriptControlSource_ErrorEventHandler _ErrorDelegate;
        public DScriptControlSource_TimeoutEventHandler _TimeoutDelegate;

        internal MSScriptControl_EventSink() {
            _ErrorDelegate = null;
            _TimeoutDelegate = null;
        }

        public void Error() {
            if (_ErrorDelegate != null)
                _ErrorDelegate();
        }

        public void Timeout() {
            if (_TimeoutDelegate != null)
                _TimeoutDelegate();
        }
    }

}