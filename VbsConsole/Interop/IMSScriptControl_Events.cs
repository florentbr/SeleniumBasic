using System.Runtime.InteropServices;

namespace Interop.MSScript {

    public delegate void DScriptControlSource_ErrorEventHandler();
    public delegate void DScriptControlSource_TimeoutEventHandler();

    [InterfaceType(2), TypeLibType((short)16)]
    [ComEventInterface(typeof(IMSScriptControl_EventSink), typeof(MSScriptControl_EventProvider))]
    public interface IMSScriptControl_Events {

        event DScriptControlSource_ErrorEventHandler ErrorEvent;
        event DScriptControlSource_TimeoutEventHandler TimeoutEvent;

    }

}
