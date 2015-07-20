using System;

namespace Selenium.Core {

    /// <summary>
    /// Context manager
    /// </summary>
    class FrameContext {

        private RemoteSession _session;

        /// <summary>
        /// Contructor over a session
        /// </summary>
        /// <param name="session"></param>
        internal FrameContext(RemoteSession session) {
            _session = session;
        }

        /// <summary>
        /// Switches focus to the specified frame, by index, name or WebElement.
        /// </summary>
        /// <param name="identifier">The name, id, or WebElement of the frame to switch.</param>
        /// <param name="timeout">Optional timeout in milliseconds</param>
        public void SwitchToFrame(object identifier, int timeout) {
            if (identifier == null)
                throw new Errors.ArgumentError("Invalid type for argument identifier");

            var element = identifier as WebElement;
            if (element != null) {
                identifier = element.SerializeJson();
            }

            try {
                _session.Send(RequestMethod.POST, "/frame", "id", identifier);
            } catch (Errors.NoSuchFrameError) {
                if (timeout == 0)
                    throw;
                var endTime = _session.GetEndTime(timeout);
                while (true) {
                    SysWaiter.Wait();
                    try {
                        _session.SendAgain();
                        break;
                    } catch (Errors.NoSuchFrameError) {
                        if (DateTime.UtcNow > endTime)
                            throw;
                    }
                }
            }
        }

        /// <summary>
        /// Select the parent frame of the currently selected frame.
        /// </summary>
        public void SwitchToParentFrame() {
            _session.Send(RequestMethod.POST, "/frame/parent");
        }

        /// <summary>
        /// Selects either the first frame on the page or the main document when a page contains iFrames.
        /// </summary>
        public void SwitchToDefaultContent() {
            _session.Send(RequestMethod.POST, "/frame", "id", null);
        }

    }

}
