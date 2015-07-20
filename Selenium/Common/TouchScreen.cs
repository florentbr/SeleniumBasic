using Selenium.Core;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// TouchScreen object
    /// </summary>
    [ProgId("Selenium.TouchScreen")]
    [Guid("0277FC34-FD1B-4616-BB19-15D881CFBB7E")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [Description("TouchScreen object")]
    public class TouchScreen : ComInterfaces._TouchScreen {

        private const string _baseUri = "/touch";
        private RemoteSession _session;

        internal TouchScreen(RemoteSession session) {
            _session = session;
        }

        /// <summary>
        /// Returns true if the screen is portrait, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool IsPortrait() {
            var value = (string)_session.Send(RequestMethod.GET, "/orientation");
            return "PORTRAIT".Equals(value);
        }

        /// <summary>
        /// Orientates the screen to portrait.
        /// </summary>
        public void ToPortrait() {
            _session.Send(RequestMethod.POST, "/orientation", "orientation", "PORTRAIT");
        }

        /// <summary>
        /// Orientates the screen to landscape.
        /// </summary>
        public void ToLandscape() {
            _session.Send(RequestMethod.POST, "/orientation", "orientation", "LANDSCAPE");
        }

        /// <summary>
        /// Single tap on the touch enabled device.
        /// </summary>
        /// <param name="element">ID of the element to single tap on.</param>
        public TouchScreen Tap(WebElement element) {
            _session.Send(RequestMethod.POST, _baseUri + "/click", "element", element.Id);
            return this;
        }

        /// <summary>
        /// Finger down on the screen.
        /// </summary>
        /// <param name="x">{number} X coordinate on the screen.</param>
        /// <param name="y">{number} Y coordinate on the screen.</param>
        public TouchScreen PressHold(int x, int y) {
            _session.Send(RequestMethod.POST, _baseUri + "/down", "x", x, "y", y);
            return this;
        }

        /// <summary>
        /// Finger up on the screen.
        /// </summary>
        /// <param name="x">{number} X coordinate on the screen.</param>
        /// <param name="y">{number} Y coordinate on the screen.</param>
        public TouchScreen PressRelease(int x, int y) {
            _session.Send(RequestMethod.POST, _baseUri + "/up", "x", x, "y", y);
            return this;
        }

        /// <summary>
        /// Finger move on the screen.
        /// </summary>
        /// <param name="x">{number} X coordinate on the screen.</param>
        /// <param name="y">{number} Y coordinate on the screen.</param>
        public TouchScreen Move(int x, int y) {
            _session.Send(RequestMethod.POST, _baseUri + "/move", "x", x, "y", y);
            return this;
        }

        /// <summary>
        /// Scroll on the touch screen using finger based motion events.
        /// </summary>
        /// <param name="element">ID of the element where the scroll starts.</param>
        /// <param name="xoffset">{number} The x offset in pixels to scrollby.</param>
        /// <param name="yoffset">{number} The y offset in pixels to scrollby</param>
        public TouchScreen ScrollFrom(WebElement element, int xoffset, int yoffset) {
            _session.Send(RequestMethod.POST, _baseUri + "/scroll", "element", element.Id, "xoffset", xoffset, "yoffset", yoffset);
            return this;
        }

        /// <summary>
        /// Scroll on the touch screen using finger based motion events.
        /// </summary>
        /// <param name="xoffset">{number} The x offset in pixels to scrollby.</param>
        /// <param name="yoffset">{number} The y offset in pixels to scrollby</param>
        public TouchScreen Scroll(int xoffset, int yoffset) {
            _session.Send(RequestMethod.POST, _baseUri + "/scroll", "xoffset", xoffset, "yoffset", yoffset);
            return this;
        }

        /// <summary>
        /// Double tap on the touch screen using finger motion events.
        /// </summary>
        /// <param name="element">ID of the element to double tap on.</param>
        public TouchScreen TapDouble(WebElement element) {
            _session.Send(RequestMethod.POST, _baseUri + "/doubleclick", "element", element.Id);
            return this;
        }

        /// <summary>
        /// Long press on the touch screen using finger motion events.
        /// </summary>
        /// <param name="element"> ID of the element to long press on.</param>
        public TouchScreen TapLong(WebElement element) {
            _session.Send(RequestMethod.POST, _baseUri + "/longclick", "element", element.Id);
            return this;
        }

        /// <summary>
        /// Flick on the touch screen using finger motion events.
        /// </summary>
        /// <param name="element">ID of the element where the flick starts.</param>
        /// <param name="xoffset">{number} The x offset in pixels to flick by.</param>
        /// <param name="yoffset">{number} The y offset in pixels to flick by.</param>
        /// <param name="speed">{number} The speed in pixels per seconds.</param>
        public TouchScreen FlickFrom(WebElement element, int xoffset, int yoffset, int speed) {
            _session.Send(RequestMethod.POST, _baseUri + "/flick", "element", element.Id, "xoffset", xoffset, "yoffset", yoffset, "speed", speed);
            return this;
        }

        /// <summary>
        /// Flick on the touch screen using finger motion events.
        /// </summary>
        /// <param name="xspeed">{number} The x speed in pixels per second.</param>
        /// <param name="yspeed">{number} The y speed in pixels per second.</param>
        public TouchScreen Flick(int xspeed, int yspeed) {
            _session.Send(RequestMethod.POST, _baseUri + "/flick", "xspeed", xspeed, "yspeed", yspeed);
            return this;
        }

    }

}
