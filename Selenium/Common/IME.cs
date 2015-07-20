using Selenium.Core;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    //TODO : Include IME

    /// <summary>
    /// Input method engine object
    /// </summary>
    [ProgId("Selenium.IME")]
    [Guid("0277FC34-FD1B-4616-BB19-C8F963D978AA")]
    [Description("Input method engine.")]
    [ComVisible(false), ClassInterface(ClassInterfaceType.None)]
    public class IME : ComInterfaces._IME {

        private Session _session;

        internal IME(Session session) {
            _session = session;
        }

        /// <summary>
        /// List all available engines on the machine.
        /// </summary>
        /// <returns>A list of available engines</returns>
        public List AvailableEngines {
            get {
                return (List)_session.Send(RequestMethod.GET, "/ime/available_engines");
            }
        }

        /// <summary>
        /// Get the name of the active IME engine.
        /// </summary>
        /// <returns>The name of the active IME engine.</returns>
        public string ActiveEngine {
            get {
                return (string)_session.Send(RequestMethod.GET, "/ime/active_engine");
            }
        }

        /// <summary>
        /// Indicates whether IME input is active at the moment.
        /// </summary>
        /// <returns>true if IME input is available and currently active, false otherwise</returns>
        public bool IsActivated {
            get {
                return (bool)_session.Send(RequestMethod.GET, "/ime/activated");
            }
        }

        /// <summary>
        /// De-activates the currently-active IME engine.
        /// </summary>
        public void Deactivate() {
            _session.Send(RequestMethod.POST, "/ime/deactivate");
        }

        /// <summary>
        /// Make an engines that is available (appears on the listreturned by getAvailableEngines) active.
        /// </summary>
        /// <param name="engine">Name of the engine to activate.</param>
        public void Activate(string engine) {
            _session.Send(RequestMethod.POST, "/ime/activate", "engine", engine);
        }
    }

}
