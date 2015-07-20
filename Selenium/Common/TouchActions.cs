using Selenium.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// TouchActions object
    /// </summary>
    /// <example>
    /// <code lang="vbs">	
    /// Set ele = driver.FindElementById("id")
    /// driver.Actions.PressHold(ele).Move(10, 10).PressRelease().Perform
    /// </code>
    /// </example>
    [ProgId("Selenium.TouchActions")]
    [Guid("0277FC34-FD1B-4616-BB19-0308BDD409AE")]
    [Description("Generate touch actions.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class TouchActions : ComInterfaces._TouchActions {

        private RemoteSession _session;
        private TouchScreen _screen;
        private List<Action> _actions;

        internal TouchActions(RemoteSession session, TouchScreen screen) {
            _session = session;
            _screen = screen;
            _actions = new List<Action>();
        }

        /// <summary>
        /// Performs all stored Actions.
        /// </summary>
        public void Perform() {
            //perform actions
            foreach (Action action in _actions)
                action();
        }

        /// <summary>
        /// Waits the given time in millisecond.
        /// </summary>
        /// <param name="timems"></param>
        /// <returns></returns>
        public TouchActions Wait(int timems) {
            _actions.Add(() => SysWaiter.Wait(timems));
            return this;
        }

        /// <summary>
        /// Allows the execution of single tap on the screen, analogous to click using a Mouse.
        /// </summary>
        /// <param name="element">The web element on the screen</param>
        public TouchActions Tap(WebElement element) {
            _actions.Add(() => _screen.Tap(element));
            return this;
        }

        /// <summary>
        /// Allows the execution of double tap on the screen, analogous to click using a Mouse.
        /// </summary>
        /// <param name="element">The web element on the screen where the scroll starts.</param>
        public TouchActions TapDouble(WebElement element) {
            _actions.Add(() => _screen.TapDouble(element));
            return this;
        }

        /// <summary>
        /// Allows the execution of the gesture 'down' on the screen. It is typically the first of a
        /// sequence of touch gestures.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        public TouchActions PressHold(int locationX, int locationY) {
            _actions.Add(() => _screen.PressHold(locationX, locationY));
            return this;
        }

        /// <summary>
        /// Allows the execution of the gesture 'up' on the screen. It is typically the last of a
        /// sequence of touch gestures.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        public TouchActions PressRelease(int locationX, int locationY) {
            _actions.Add(() => _screen.PressRelease(locationX, locationY));
            return this;
        }

        /// <summary>
        /// Allows the execution of the gesture 'move' on the screen.
        /// </summary>
        /// <param name="locationX">The x coordinate relative to the view port.</param>
        /// <param name="locationY">The y coordinate relative to the view port.</param>
        public TouchActions Move(int locationX, int locationY) {
            _actions.Add(() => _screen.Move(locationX, locationY));
            return this;
        }

        /// <summary>
        /// Creates a scroll gesture that starts on a particular screen location.
        /// </summary>
        /// <param name="offsetX">The x coordinate relative to the view port.</param>
        /// <param name="offsetY">The y coordinate relative to the view port.</param>
        public TouchActions Scroll(int offsetX, int offsetY) {
            _actions.Add(() => _screen.Scroll(offsetX, offsetY));
            return this;
        }

        /// <summary>
        /// Creates a scroll gesture that starts on a particular screen location.
        /// </summary>
        /// <param name="element">The web element on the screen where the scroll starts</param>
        /// <param name="offsetX">The x coordinate relative to the view port.</param>
        /// <param name="offsetY">The y coordinate relative to the view port.</param>
        public TouchActions ScrollFrom(WebElement element, int offsetX, int offsetY) {
            _actions.Add(() => _screen.ScrollFrom(element, offsetX, offsetY));
            return this;
        }

        /// <summary>
        /// Allows the execution of a long press gesture on the screen.
        /// </summary>
        /// <param name="element">The web element on the screen.</param>
        public TouchActions TapLong(WebElement element) {
            _actions.Add(() => _screen.TapLong(element));
            return this;
        }

        /// <summary>
        /// Creates a flick gesture for the current view.
        /// </summary>
        /// <param name="speedX">The horizontal speed in pixels per second.</param>
        /// <param name="speedY">The vertical speed in pixels per second.</param>
        public TouchActions Flick(int speedX, int speedY) {
            _actions.Add(() => _screen.Flick(speedX, speedY));
            return this;
        }

        /// <summary>
        /// Creates a flick gesture for the current view starting at a specific location.
        /// </summary>
        /// <param name="element">The web element on the screen where the scroll starts.</param>
        /// <param name="offsetX">The x offset relative to the viewport.</param>
        /// <param name="offsetY">The y offset relative to the viewport.</param>
        /// <param name="speed">The speed in pixels per second.</param>
        public TouchActions FlickFrom(WebElement element, int offsetX, int offsetY, int speed) {
            _actions.Add(() => _screen.FlickFrom(element, offsetX, offsetY, speed));
            return this;
        }

    }

}
