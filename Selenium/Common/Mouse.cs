using Selenium.Core;
using Selenium;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Mouse interraction.
    /// </summary>
    [ProgId("Selenium.Mouse")]
    [Guid("0277FC34-FD1B-4616-BB19-94EBFCC1BAFC")]
    [Description("Mouse interface.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Mouse : ComInterfaces._Mouse {

        private RemoteSession _session;

        internal Mouse(RemoteSession session) {
            _session = session;
        }

        /// <summary>
        /// Move the mouse to the specificed element.
        /// </summary>
        /// <param name="element">Opaque ID assigned to the element to move to, as described in the WebElement JSON Object. If not specified or is null, the offset is relative to current position of the mouse.</param>
        public Mouse moveTo(WebElement element) {
            _session.Send(RequestMethod.POST, "/moveto", "element", element.Id);
            return this;
        }

        /// <summary>
        /// Move the mouse by an offset of the specificed element.
        /// </summary>
        /// <param name="element">Opaque ID assigned to the element to move to, as described in the WebElement JSON Object. If not specified or is null, the offset is relative to current position of the mouse.</param>
        /// <param name="xoffset">{number} X offset to move to, relative to the top-left corner of the element. If not specified, the mouse will move to the middle of the element.</param>
        /// <param name="yoffset"> {number} Y offset to move to, relative to the top-left corner of the element. If not specified, the mouse will move to the middle of the element.</param>
        public Mouse MoveTo(WebElement element, int xoffset = 0, int yoffset = 0) {
            var data = new Dictionary();
            if (element != null)
                data.Add("element", element.Id);
            if (xoffset != 0)
                data.Add("xoffset", xoffset);
            if (yoffset != 0)
                data.Add("yoffset", yoffset);
            _session.Send(RequestMethod.POST, "/moveto", data);
            return this;
        }

        /// <summary>
        /// Click any mouse button (at the coordinates set by the last moveto command).
        /// </summary>
        /// <param name="button">{number} Which button, enum: {LEFT = 0, MIDDLE = 1 , RIGHT = 2}. Defaults to the left mouse button if not specified.</param>
        public Mouse Click(MouseButton button = MouseButton.Left) {
            if (button == MouseButton.Left) {
                _session.Send(RequestMethod.POST, "/click");
            } else {
                _session.Send(RequestMethod.POST, "/click", "button", (int)button);
            }
            return this;
        }

        /// <summary>
        /// Click and hold the left mouse button (at the coordinates set by the last moveto command).
        /// </summary>
        /// <param name="button">{number} Which button, enum: {LEFT = 0, MIDDLE = 1 , RIGHT = 2}. Defaults to the left mouse button if not specified.</param>
        public Mouse ClickAndHold(MouseButton button = MouseButton.Left) {
            if (button == MouseButton.Left) {
                _session.Send(RequestMethod.POST, "/buttondown");
            } else {
                _session.Send(RequestMethod.POST, "/buttondown", "button", (int)button);
            }
            return this;
        }

        /// <summary>
        /// Releases the mouse button previously held (where the mouse is currently at).
        /// </summary>
        /// <param name="button">{number} Which button, enum: {LEFT = 0, MIDDLE = 1 , RIGHT = 2}. Defaults to the left mouse button if not specified.</param>
        public Mouse Release(MouseButton button = MouseButton.Left) {
            if (button == MouseButton.Left){
                _session.Send(RequestMethod.POST, "/buttonup");
            } else {
                _session.Send(RequestMethod.POST, "/buttonup", "button", (int)button);
            }
            return this;
        }

        /// <summary>
        /// Double-clicks at the current mouse coordinates (set by moveto).
        /// </summary>
        public Mouse ClickDouble() {
            _session.Send(RequestMethod.POST, "/doubleclick");
            return this;
        }

    }

}
