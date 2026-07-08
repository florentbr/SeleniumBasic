using Selenium.Core;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Keyboard object
    /// </summary>
    [ProgId("Selenium.Keyboard")]
    [Guid("0277FC34-FD1B-4616-BB19-988CE6BB6E1F")]
    [Description("Keyboard interface.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Keyboard : ComInterfaces._Keyboard {

        const char KEY_SHIFT = '\xE008';
        const char KEY_ALT = '\xE009';
        const char KEY_CTRL = '\xE00A';

        private RemoteSession _session;
        private Keys _keys;

        internal Keyboard(RemoteSession session) {
            _session = session;
            _keys = new Keys();
        }

        /// <summary>
        /// Returns a list of pressable keys.
        /// </summary>
        public Keys Keys {
            get {
                return _keys;
            }
        }

        /// <summary>
        /// Sends a sequence of key strokes to the active element.
        /// </summary>
        /// <param name="keysOrModifiers">Modifier keys (Ctrl, Shit or Alt) or keys</param>
        /// <param name="keys">Keys</param>
        /// <returns>Self</returns>
        public Keyboard SendKeys(string keysOrModifiers, string keys = null) {
            if( WebDriver.LEGACY ) {
                if (keys != null)
                    keysOrModifiers = string.Concat(keysOrModifiers, keys, keysOrModifiers);
                _session.Send(RequestMethod.POST, "/keys", "value", new string[] { keysOrModifiers });
                //_session.Send(RequestMethod.POST, "/keys", "value", keysOrModifiers.ToCharArray() );
            } else {
                bool shift_down = false, ctrl_down = false, alt_down = false;
                Actions.ActionSequence k_s = new Actions.ActionSequence( Actions.SequenceType.Keyboard );
                Actions.AddKeys(k_s, keysOrModifiers, ref shift_down, ref ctrl_down, ref alt_down);
                if( keys != null ) {
                    bool shift_down_ = false, ctrl_down_ = false, alt_down_ = false;
                    Actions.AddKeys(k_s, keys, ref shift_down_, ref ctrl_down_, ref alt_down_);
                }
/*
                if (shift_down)
                    k_s.Add( new Actions.ActionKeyDownUp( false, KEY_SHIFT ) );
                if (ctrl_down)
                    k_s.Add( new Actions.ActionKeyDownUp( false, KEY_CTRL ) );
                if (alt_down)
                    k_s.Add( new Actions.ActionKeyDownUp( false, KEY_ALT ) );
*/
                Actions.SendActions( _session, k_s );
                if( shift_down || ctrl_down || alt_down )
                    Actions.ReleaseActions( _session );
            }
            return this;
        }

        /// <summary>
        /// Presses and holds modifier keys (Control, Alt and Shift).
        /// </summary>
        /// <param name="modifierKeys">The modifier key to Send. Values are defined in Keys class.</param>
        /// <returns>Self</returns>
        public Keyboard KeyDown(string modifierKeys) {
            if( WebDriver.LEGACY ) {
                check_keys_are_modifiers(modifierKeys);
                _session.Send(RequestMethod.POST, "/keys", "value", new string[] { modifierKeys });
            } else {
                if( modifierKeys == null || modifierKeys.Length == 0 )
                    throw new Errors.InvalideModifierKeyError();
                Actions.ActionSequence k_s = new Actions.ActionSequence( Actions.SequenceType.Keyboard );
                bool shift_down_ = false, ctrl_down_ = false, alt_down_ = false;
                Actions.AddKeys(k_s, modifierKeys, ref shift_down_, ref ctrl_down_, ref alt_down_);
                Actions.SendActions( _session, k_s );
            }
            return this;
        }

        /// <summary>
        /// Release modifier keys (Control, Alt and Shift).
        /// </summary>
        /// <param name="modifierKeys">The modifier key to Send. Values are defined in Keys class.</param>
        /// <returns>Self</returns>
        public Keyboard KeyUp(string modifierKeys) {
            if( WebDriver.LEGACY ) {
                check_keys_are_modifiers(modifierKeys);
                _session.Send(RequestMethod.POST, "/keys", "value", new string[] { modifierKeys });
            } else {
                if( modifierKeys == null || modifierKeys.Length == 0 )
                    throw new Errors.InvalideModifierKeyError();
                Actions.ActionSequence k_s = new Actions.ActionSequence( Actions.SequenceType.Keyboard );
                bool shift_down_ = true, ctrl_down_ = true, alt_down_ = true;
                Actions.AddKeys(k_s, modifierKeys, ref shift_down_, ref ctrl_down_, ref alt_down_);
                Actions.SendActions( _session, k_s );
            }
            return this;
        }


        #region private support methods

        private void check_keys_are_modifiers(string keys) {
            foreach (char key in keys) {
                if (key != KEY_SHIFT && key != KEY_ALT && key != KEY_CTRL)
                    throw new Errors.InvalideModifierKeyError();
            }
        }

        #endregion

    }

}
