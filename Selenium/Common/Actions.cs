using Selenium.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium {

    /// <summary>
    /// The user-facing API for emulating complex user gestures. Use this class rather than using the Keyboard or Mouse directly. Implements the builder pattern: Builds a CompositeAction containing all actions specified by the method calls.
    /// </summary>
    /// <example>
    /// <code lang="vbs">	
    /// Set ele = driver.FindElementById("id")
    /// driver.Actions.ClickDouble(ele).SendKeys("abcd").Perform
    /// </code>
    /// </example>
    [ProgId("Selenium.Actions")]
    [Guid("0277FC34-FD1B-4616-BB19-FB8601D6B166")]
    [Description("User-facing API for emulating complex user gestures.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Actions : ComInterfaces._Actions {

        const char KEY_SHIFT = '\xE008';
        const char KEY_CTRL = '\xE009';
        const char KEY_ALT = '\xE00A';

        delegate void Action();

        private RemoteSession _session;
        private Mouse _mouse;
        private Keyboard _keyboard;
        private List<Action> _actions;
        private bool _isMouseDown = false;
        private bool _isKeyShiftDown = false;
        private bool _isKeyCtrlDown = false;
        private bool _isKeyAltDown = false;

        internal Actions(RemoteSession session) {
            _session = session;
            _mouse = session.mouse;
            _keyboard = session.keyboard;
            _actions = new List<Action>();
        }

        /// <summary>
        /// Performs all stored Actions.
        /// </summary>
        public void Perform() {
            //perform actions
            foreach (var action in _actions)
                action();

            //release the mouse if the state is down
            if (_isMouseDown)
                _mouse.Release();
            _isMouseDown = false;

            //release the keyboard if the modifiers keys are pressed
            var modifiers = new StringBuilder(10);
            if (_isKeyShiftDown)
                modifiers.Append(KEY_SHIFT);
            if (_isKeyCtrlDown)
                modifiers.Append(KEY_CTRL);
            if (_isKeyAltDown)
                modifiers.Append(KEY_ALT);
            if (modifiers.Length != 0) {
                _keyboard.SendKeys(modifiers.ToString());
                _isKeyShiftDown = false;
                _isKeyCtrlDown = false;
                _isKeyAltDown = false;
            }
        }

        /// <summary>
        /// Waits the given time in millisecond.
        /// </summary>
        /// <param name="timems">Time to wait in millisecond.</param>
        /// <returns>Self</returns>
        public Actions Wait(int timems) {
            _actions.Add(() => SysWaiter.Wait(timems));
            return this;
        }

        /// <summary>
        /// Clicks an element.
        /// </summary>
        /// <param name="element">The element to click. If None, clicks on current mouse position.</param>
        /// <returns>Self</returns>
        public Actions Click(WebElement element = null) {
            _actions.Add(() => act_click(element));
            return this;
        }

        /// <summary>
        /// Holds down the left mouse button on an element.
        /// </summary>
        /// <param name="element">The element to mouse down. If None, clicks on current mouse position.</param>
        /// <returns>Self</returns>
        public Actions ClickAndHold(WebElement element = null) {
            _actions.Add(() => act_mouse_press(element));
            return this;
        }

        /// <summary>
        /// Performs a context-click (right click) on an element.
        /// </summary>
        /// <param name="element"> The element to context-click. If None, clicks on current mouse position.</param>
        /// <returns>Self</returns>
        public Actions ClickContext(WebElement element = null) {
            _actions.Add(() => act_click_context(element));
            return this;
        }

        /// <summary>
        /// Double-clicks an element.
        /// </summary>
        /// <param name="element">The element to double-click. If None, clicks on current mouse position.</param>
        /// <returns>Self</returns>
        public Actions ClickDouble(WebElement element = null) {
            _actions.Add(() => act_click_double(element));
            return this;
        }

        /// <summary>
        /// Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.
        /// </summary>
        /// <param name="elementSource">The element to mouse down.</param>
        /// <param name="elementTarget">The element to mouse up.</param>
        /// <returns>Self</returns>
        public Actions DragAndDrop(WebElement elementSource, WebElement elementTarget) {
            _actions.Add(() => act_drag_drop(elementSource, elementTarget));
            return this;
        }

        /// <summary>
        /// Holds down the left mouse button on the source element, then moves to the target element and releases the mouse button.
        /// </summary>
        /// <param name="element">The element to mouse down.</param>
        /// <param name="offset_x">X offset to move to.</param>
        /// <param name="offset_y">Y offset to move to.</param>
        /// <returns>Self</returns>
        public Actions DragAndDropByOffset(WebElement element, int offset_x, int offset_y) {
            _actions.Add(() => act_drag_drop_offset(element, offset_x, offset_y));
            return this;
        }

        /// <summary>
        /// Sends a key press only, without releasing it. Should only be used with modifier keys (Control, Alt and Shift).
        /// </summary>
        /// <param name="modifierKey">The modifier key to Send. Values are defined in Keys class.</param>
        /// <param name="element">The element to Send keys. If None, sends a key to current focused element.</param>
        /// <returns>Self</returns>
        public Actions KeyDown(string modifierKey, WebElement element = null) {
            _actions.Add(() => act_key_down(modifierKey, element));
            return this;
        }

        /// <summary>
        /// Releases a modifier key.
        /// </summary>
        /// <param name="modifierKey">The modifier key to Send. Values are defined in Keys class.</param>
        /// <returns>Self</returns>
        public Actions KeyUp(string modifierKey) {
            _actions.Add(() => act_send_modifier_key(modifierKey));
            return this;
        }

        /// <summary>
        /// Moving the mouse to an offset from current mouse position.
        /// </summary>
        /// <param name="offset_x">X offset to move to.</param>
        /// <param name="offset_y">Y offset to move to.</param>
        /// <returns>Self</returns>
        public Actions MoveByOffset(int offset_x, int offset_y) {
            _actions.Add(() => act_mouse_mouve(offset_x, offset_y));
            return this;
        }

        /// <summary>
        /// Moving the mouse to the middle of an element.
        /// </summary>
        /// <param name="element">The element to move to.</param>
        /// <returns>Self</returns>
        public Actions MoveToElement(WebElement element) {
            _actions.Add(() => act_mouse_mouve(element));
            return this;
        }

        /// <summary>
        /// Releasing a held mouse button.
        /// </summary>
        /// <returns>Self</returns>
        public Actions Release([MarshalAs(UnmanagedType.Struct)]WebElement element = null) {
            _actions.Add(() => act_mouse_release(element));
            return this;
        }

        /// <summary>
        /// Sends keys to an element.
        /// </summary>
        /// <param name="keys">Keys to send</param>
        /// <param name="element">Element to Send keys. If None, Send keys to the current mouse position.</param>
        /// <returns>Self</returns>
        public Actions SendKeys(string keys, WebElement element = null) {
            _actions.Add(() => act_send_keys(keys, element));
            return this;
        }


        #region private support methods

        private void act_click(WebElement element) {
            act_mouse_mouve(element);
            act_mouse_click();
        }

        private void act_click_context(WebElement element) {
            act_mouse_mouve(element);
            act_mouse_click_context();
        }

        private void act_click_double(WebElement element) {
            act_mouse_mouve(element);
            act_mouse_click_double();
        }

        private void act_drag_drop(WebElement elementSource, WebElement elementTarget) {
            act_mouse_press(elementSource);
            act_mouse_release(elementTarget);
        }
        private void act_drag_drop_offset(WebElement element, int offset_x, int offset_y) {
            act_mouse_press(element);
            act_mouse_mouve(offset_x, offset_y);
            act_mouse_release(null);
        }

        private void act_key_down(string modifierKey, WebElement element) {
            if (element != null) {
                act_mouse_mouve(element);
                act_mouse_click();
            }
            act_send_modifier_key(modifierKey);
        }

        private void act_send_keys(string keys, WebElement element) {
            if (element != null) {
                act_mouse_mouve(element);
                act_mouse_click();
            }
            act_send_keys(keys);
        }

        private void act_send_modifier_key(string modifierKey) {
            if (modifierKey.Length == 0)
                throw new Errors.InvalideModifierKeyError();
            char c = modifierKey[0];
            if (c != KEY_ALT && c != KEY_CTRL && c != KEY_SHIFT)
                throw new Errors.InvalideModifierKeyError();
            act_send_keys(modifierKey);
        }

        private void act_send_keys(string keys) {
            foreach (char c in keys) {
                if (c == KEY_SHIFT)
                    _isKeyShiftDown ^= true;
                if (c == KEY_CTRL)
                    _isKeyCtrlDown ^= true;
                if (c == KEY_ALT)
                    _isKeyAltDown ^= true;
            }
            _keyboard.SendKeys(keys);
        }

        private void act_mouse_mouve(WebElement element) {
            if (element != null)
                _mouse.moveTo(element);
        }

        private void act_mouse_mouve(int offsetX, int offsetY) {
            _mouse.MoveTo(null, offsetX, offsetY);
        }

        private void act_mouse_click() {
            _mouse.Click();
        }

        private void act_mouse_press(WebElement element) {
            if (element != null)
                _mouse.moveTo(element);
            _mouse.ClickAndHold();
            _isMouseDown = true;
        }

        private void act_mouse_release(WebElement element) {
            if (element != null)
                _mouse.moveTo(element);
            _mouse.Release();
            _isMouseDown = false;
        }

        private void act_mouse_click_double() {
            _mouse.ClickDouble();
        }

        private void act_mouse_click_context() {
            _mouse.Click(MouseButton.Right);
        }

        #endregion

    }

}
