using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Interop.MSScript {

    internal sealed class MSScriptControl_EventProvider : IMSScriptControl_Events, IDisposable {


        event DScriptControlSource_ErrorEventHandler IMSScriptControl_Events.ErrorEvent {
            add {
                AddEvent(s => s._ErrorDelegate = value);
            }
            remove {
                RemoveEvent(s => value.Equals(s._ErrorDelegate));
            }
        }

        event DScriptControlSource_TimeoutEventHandler IMSScriptControl_Events.TimeoutEvent {
            add {
                AddEvent(s => s._TimeoutDelegate = value);
            }
            remove {
                RemoveEvent(s => value.Equals(s._TimeoutDelegate));
            }
        }


        class SinkItem {
            public SinkItem Next;
            public int Cookie;
            public MSScriptControl_EventSink UnkSink;
        }


        private delegate void EventSinkCreateDelegate(MSScriptControl_EventSink eventSink);
        private delegate bool EventSinkMatchDelegate(MSScriptControl_EventSink eventSink);

        private IConnectionPointContainer _connectionPointContainer;
        private IConnectionPoint _connectionPoint;
        private SinkItem _sinkHead;
        private Guid _riid;

        protected MSScriptControl_EventProvider(object connectionPoint) {
            _connectionPointContainer = (IConnectionPointContainer)connectionPoint;
            _riid = typeof(IMSScriptControl_EventSink).GUID;
        }

        ~MSScriptControl_EventProvider() {
            this.Dispose();
        }
        

        public new void Dispose() {
            lock (this) {
                if (_connectionPoint == null)
                    return;

                for (SinkItem node = _sinkHead; node != null; node = node.Next)
                    _connectionPoint.Unadvise(node.Cookie);

                Marshal.ReleaseComObject(_connectionPoint);
                _connectionPoint = null;
                _sinkHead = null;
            }
            GC.SuppressFinalize(this);
        }

        private void AddEvent(EventSinkCreateDelegate assign) {
            lock (this) {
                if (this._connectionPoint == null) {
                    _connectionPointContainer.FindConnectionPoint(ref _riid, out _connectionPoint);
                }

                var eventsSinkHelper = new MSScriptControl_EventSink();
                assign(eventsSinkHelper);

                SinkItem node = new SinkItem();
                node.UnkSink = eventsSinkHelper;
                node.Next = _sinkHead;

                _connectionPoint.Advise(eventsSinkHelper, out node.Cookie);
                _sinkHead = node;
            }
        }

        private void RemoveEvent(EventSinkMatchDelegate match) {
            lock (this) {
                SinkItem prevNode = null;
                SinkItem node = _sinkHead;
                while (node != null) {
                    if (match(node.UnkSink)) {
                        _connectionPoint.Unadvise(node.Cookie);
                        if (prevNode == null) {
                            _sinkHead = node.Next;
                            if (_sinkHead == null) {
                                Marshal.ReleaseComObject(_connectionPoint);
                                _connectionPoint = null;
                            }
                        } else {
                            prevNode.Next = node.Next;
                        }
                        break;
                    }
                    prevNode = node;
                    node = node.Next;
                }
            }
        }

    }

}