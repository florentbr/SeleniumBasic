using Selenium.Core;
using Selenium.Serializer;
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

        internal enum SequenceType {
            Mouse = 0, Keyboard = 1
        }
        
        internal class ActionSequence : IJsonObject {
            private SequenceType _stype; 
            private List<ActionBase> _actions;
            private bool _has_real_actions; 
            private ActionSequence _other;

            internal ActionSequence( SequenceType type )
            {
                _stype = type;
                _actions = new List<ActionBase>();
            }

            public void SetOther( ActionSequence oas ) {
                _other = oas;
            }

            public bool IsMouse()
            {
                return _stype == SequenceType.Mouse;
            }
            public bool IsKey()
            {
                return _stype == SequenceType.Keyboard;
            }
            public bool HasActions()
            {
                return _has_real_actions;
            }

            public void Add( ActionBase a ) {
                _actions.Add( a );
                if( !(a is ActionPause) ) {
                    _has_real_actions = true;
                    if( _other != null )
                        _other.Add( new ActionPause() );
                }
            }

            public virtual Dictionary SerializeJson() {
                Dictionary dict = new Dictionary();
                if( IsMouse() ) {
                    dict.Add( "type", "pointer" );
                    dict.Add( "id", "default mouse" );
                    Dictionary parameters = new Dictionary();
                    parameters.Add( "pointerType", "mouse" );
                    dict.Add( "parameters", parameters );
                }  else if( IsKey() ) {
                    dict.Add( "type", "key" );
                    dict.Add( "id", "default keyboard" );
                }
                dict.Add( "actions", _actions );
                return dict;
            }
        }

        internal class ActionBase : IJsonObject {
            private string _type;

            internal ActionBase( string type )
            {
                _type = type;
            }

            public virtual Dictionary SerializeJson() {
                Dictionary dict = new Dictionary();
                dict.Add( "type", _type );
                return dict;
            }
        }

        class ActionPause : ActionBase {
            private int    _duration;
            public ActionPause() : this( 0 ) { }
            public ActionPause( int duration ) : base( "pause" ) {
                _duration = duration;
            }
            public override Dictionary SerializeJson() {
                Dictionary dict = base.SerializeJson();
                dict.Add( "duration", _duration );
                return dict;
            }
        }

        internal class ActionMove : ActionBase {
            private int    _x, _y;
            private string _elementId;
            public ActionMove() : this( 0, 0 ) { }
            public ActionMove( int x, int y ) : base( "pointerMove" ) {
                _x = x;
                _y = y;
            }
            public ActionMove( string id, int x = 0, int y = 0 ) : this( x, y ) {
                _elementId = id;
            }
            public override Dictionary SerializeJson() {
                Dictionary dict = base.SerializeJson();
                dict.Add( "x", _x );
                dict.Add( "y", _y );
                dict.Add( "duration", 250 );
                if( _elementId != null ) {
                    Dictionary origin_dict = new Dictionary();
                    origin_dict.Add( WebElement.IDENTIFIER, _elementId );
                    dict.Add( "origin", origin_dict );
                } else
                    dict.Add( "origin", "pointer" );
                return dict;
            }
        }

        internal class ActionPtrDownUp : ActionBase {
            private MouseButton _button;
            public ActionPtrDownUp( bool down ) : this( down, 0 ) { }
            public ActionPtrDownUp( bool down, MouseButton button ) : base( down ? "pointerDown" : "pointerUp" ) {
                _button = button;
            }
            public override Dictionary SerializeJson() {
                Dictionary dict = base.SerializeJson();
                dict.Add( "button", (int)_button );
                return dict;
            }
        }

        internal class ActionKeyDownUp : ActionBase {
            private char _key;
            public ActionKeyDownUp( bool down, char key ) : base( down ? "keyDown" : "keyUp" ) {
                _key = key;
            }
            public override Dictionary SerializeJson() {
                Dictionary dict = base.SerializeJson();
                dict.Add( "value", _key );
                return dict;
            }
        }

        private List<ActionSequence> _action_sequences;

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
            if( !WebDriver.LEGACY ) {
                _action_sequences = new List<ActionSequence>( 2 );
                ActionSequence m_s = new ActionSequence(SequenceType.Mouse);
                ActionSequence k_s = new ActionSequence(SequenceType.Keyboard);
                m_s.SetOther( k_s );
                k_s.SetOther( m_s );
                _action_sequences.Add( m_s );
                _action_sequences.Add( k_s );
            }
        }

        /// <summary>
        /// Performs all stored Actions.
        /// </summary>
        public void Perform() {
            if( WebDriver.LEGACY ) {
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
            } else {
                if (_isKeyShiftDown)
                    GetKeySeq().Add( new ActionKeyDownUp( false, KEY_SHIFT ) );
                if (_isKeyCtrlDown)
                    GetKeySeq().Add( new ActionKeyDownUp( false, KEY_CTRL ) );
                if (_isKeyAltDown)
                    GetKeySeq().Add( new ActionKeyDownUp( false, KEY_ALT ) );
                _isKeyShiftDown = false;
                _isKeyCtrlDown = false;
                _isKeyAltDown = false;


                ActionSequence m_s = GetMouseSeq();
                ActionSequence k_s = GetKeySeq();
                if( !m_s.HasActions() )
                    _action_sequences.Remove( m_s );
                if( !k_s.HasActions() )
                    _action_sequences.Remove( k_s );

                SendActions( _session, _action_sequences );
            }
        }

        internal static void SendActions( RemoteSession session, ActionSequence a_s1 ) {
            List<Actions.ActionSequence> a_s = new List<Actions.ActionSequence>( 1 );
            a_s.Add( a_s1 );
            Actions.SendActions( session, a_s );
        }

        internal static void SendActions( RemoteSession session, List<ActionSequence> a_s ) {
            Dictionary actions_dict = new Dictionary();
            actions_dict.Add( "actions", a_s ); 
#if DEBUG
            JSON json = JSON.Serialize( actions_dict );
            Console.WriteLine( json.ToString() );
#endif
            session.Send(RequestMethod.POST, "/actions", actions_dict );
        }

        internal static void ReleaseActions( RemoteSession session ) {
            session.Send(RequestMethod.DELETE, "/actions" );
        }

        private ActionSequence GetMouseSeq() {
            return _action_sequences[ (int)SequenceType.Mouse ];
        }

        private ActionSequence GetKeySeq() {
            return _action_sequences[ (int)SequenceType.Keyboard ];
        }

        private const int _DOWN = 1, _UP = 2;

        private void AddClickAction( WebElement element, MouseButton button, int what = _DOWN | _UP, int rep = 1 ) {
            AddClickAction( GetMouseSeq(), element, button, what, rep );
        }

        internal static void AddClickAction( ActionSequence m_s, WebElement element, MouseButton button, int what = _DOWN | _UP, int rep = 1 ) {
            if( element != null )
                m_s.Add( new ActionMove( element.Id ) );
            for( int i = 1; i <= rep; i++ ) {
                if( (what & _DOWN) != 0)
                    m_s.Add( new ActionPtrDownUp( true,  button ) );
                if( (what & _UP) != 0)
                    m_s.Add( new ActionPtrDownUp( false, button ) );
            }
        }

        internal static void AddKeys(ActionSequence k_s, string keys, ref bool shift_down, ref bool ctrl_down, ref bool alt_down) {
            foreach( char c in keys ) {
                if (c == KEY_SHIFT) {
                    k_s.Add( new ActionKeyDownUp( ( shift_down ^= true ),  c ) );
                } else if (c == KEY_CTRL) {
                    k_s.Add( new ActionKeyDownUp( ( ctrl_down ^= true ),  c ) );
                } else if (c == KEY_ALT) {
                    k_s.Add( new ActionKeyDownUp( ( alt_down ^= true ),  c ) );
                } else {
                    k_s.Add( new ActionKeyDownUp( true,  c ) );
                    k_s.Add( new ActionKeyDownUp( false, c ) );
                }
            }
        }

        /// <summary>
        /// Waits the given time in millisecond.
        /// </summary>
        /// <param name="timems">Time to wait in millisecond.</param>
        /// <returns>Self</returns>
        public Actions Wait(int timems) {
            if( _action_sequences != null ) {
                GetMouseSeq().Add( new ActionPause( timems ) );
                GetKeySeq().Add( new ActionPause() );
            }  else
                _actions.Add(() => SysWaiter.Wait(timems));
            return this;
        }

        /// <summary>
        /// Clicks an element.
        /// </summary>
        /// <param name="element">The element to click. If None, clicks on current mouse position.</param>
        /// <returns>Self</returns>
        public Actions Click(WebElement element = null) {
            if( _action_sequences != null )
                AddClickAction( element, MouseButton.Left, _DOWN | _UP );
            else
                _actions.Add(() => act_click(element));
            return this;
        }

        /// <summary>
        /// Holds down the left mouse button on an element.
        /// </summary>
        /// <param name="element">The element to mouse down. If None, clicks on current mouse position.</param>
        /// <returns>Self</returns>
        public Actions ClickAndHold(WebElement element = null) {
            if( _action_sequences != null )
                AddClickAction( element, MouseButton.Left, _DOWN );
            else
                _actions.Add(() => act_mouse_press(element));
            return this;
        }

        /// <summary>
        /// Performs a context-click (right click) on an element.
        /// </summary>
        /// <param name="element"> The element to context-click. If None, clicks on current mouse position.</param>
        /// <returns>Self</returns>
        public Actions ClickContext(WebElement element = null) {
            if( _action_sequences != null )
                AddClickAction( element, MouseButton.Right, _DOWN | _UP );
            else            
                _actions.Add(() => act_click_context(element));
            return this;
        }

        /// <summary>
        /// Double-clicks an element.
        /// </summary>
        /// <param name="element">The element to double-click. If None, clicks on current mouse position.</param>
        /// <returns>Self</returns>
        public Actions ClickDouble(WebElement element = null) {
            if( _action_sequences != null )
                AddClickAction( element, MouseButton.Left, _DOWN | _UP, 2 );
            else            
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
            if( _action_sequences != null ) {
                AddClickAction( elementSource, MouseButton.Left, _DOWN );
                AddClickAction( elementTarget, MouseButton.Left, 0 );
                AddClickAction( null, MouseButton.Left, _UP );
            }  else            
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
            if( _action_sequences != null ) {
                AddClickAction( element, MouseButton.Left, _DOWN );
                ActionSequence m_s = GetMouseSeq();
                m_s.Add( new ActionMove( offset_x, offset_y ) );
                m_s.Add( new ActionPtrDownUp( false, MouseButton.Left ) );
            }  else            
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
            if( _action_sequences != null )
                GetKeySeq().Add( new ActionKeyDownUp( true, modifierKey[0] ) );
            else
                _actions.Add(() => act_key_down(modifierKey, element));
            return this;
        }

        /// <summary>
        /// Releases a modifier key.
        /// </summary>
        /// <param name="modifierKey">The modifier key to Send. Values are defined in Keys class.</param>
        /// <returns>Self</returns>
        public Actions KeyUp(string modifierKey) {
            if( _action_sequences != null )
                GetKeySeq().Add( new ActionKeyDownUp( false, modifierKey[0] ) );
            else
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
            if( _action_sequences != null )
                GetMouseSeq().Add( new ActionMove( offset_x, offset_y ) );
            else
                _actions.Add(() => act_mouse_mouve(offset_x, offset_y));
            return this;
        }

        /// <summary>
        /// Moving the mouse to the middle of an element.
        /// </summary>
        /// <param name="element">The element to move to.</param>
        /// <returns>Self</returns>
        public Actions MoveToElement(WebElement element) {
            if( _action_sequences != null )
                GetMouseSeq().Add( new ActionMove( element.Id ) );
            else
                _actions.Add(() => act_mouse_mouve(element));
            return this;
        }

        /// <summary>
        /// Releasing a held mouse button.
        /// </summary>
        /// <returns>Self</returns>
        public Actions Release([MarshalAs(UnmanagedType.Struct)]WebElement element = null) {
            if( _action_sequences != null )
                AddClickAction( element, 0, _UP );
            else
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
            if( _action_sequences != null ) {
                if( element != null )
                    AddClickAction( element, MouseButton.Left, _DOWN | _UP );
                AddKeys(GetKeySeq(), keys, ref _isKeyShiftDown, ref _isKeyCtrlDown, ref _isKeyAltDown);
            } else
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
