using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Representations of pressable keys that are not text keys for sending to the browser.
    /// </summary>
    [ProgId("Selenium.Keys")]
    [Guid("0277FC34-FD1B-4616-BB19-BE75D14E7B41")]
    [Description("Representations of pressable keys that are not text keys for sending to the browser.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Keys : ComInterfaces._Keys {

        /// <summary>
        /// Represents the NUL keystroke.
        /// </summary>
        public string Null { get { return "\xE000"; } }

        /// <summary>
        /// Represents the Cancel keystroke.
        /// </summary>
        public string Cancel { get { return "\xE001"; } }

        /// <summary>
        /// Represents the Help keystroke.
        /// </summary>
        public string Help { get { return "\xE002"; } }

        /// <summary>
        /// Represents the Backspace key.
        /// </summary>
        public string Backspace { get { return "\xE003"; } }

        /// <summary>
        /// Represents the Tab key.
        /// </summary>
        public string Tab { get { return "\xE004"; } }

        /// <summary>
        /// Represents the Clear keystroke.
        /// </summary>
        public string Clear { get { return "\xE005"; } }

        /// <summary>
        /// Represents the Return key.
        /// </summary>
        public string Return { get { return "\xE006"; } }

        /// <summary>
        /// Represents the Enter key.
        /// </summary>
        public string Enter { get { return "\xE007"; } }

        /// <summary>
        /// Represents the Shift key.
        /// </summary>
        public string Shift { get { return "\xE008"; } }

        /// <summary>
        /// Represents the Shift key.
        /// </summary>
        public string LeftShift { get { return "\xE008"; } }

        /// <summary>
        /// Represents the Control key.
        /// </summary>
        public string Control { get { return "\xE009"; } }

        /// <summary>
        /// Represents the Control key.
        /// </summary>
        public string LeftControl { get { return "\xE009"; } }

        /// <summary>
        /// Represents the Alt key.
        /// </summary>
        public string Alt { get { return "\xE00A"; } }

        /// <summary>
        /// Represents the Alt key.
        /// </summary>
        public string LeftAlt { get { return "\xE00A"; } }

        /// <summary>
        /// Represents the Pause key.
        /// </summary>
        public string Pause { get { return "\xE00B"; } }

        /// <summary>
        /// Represents the Escape key.
        /// </summary>
        public string Escape { get { return "\xE00C"; } }

        /// <summary>
        /// Represents the Spacebar key.
        /// </summary>
        public string Space { get { return "\xE00D"; } }

        /// <summary>
        /// Represents the Page Up key.
        /// </summary>
        public string PageUp { get { return "\xE00E"; } }

        /// <summary>
        /// Represents the Page Down key.
        /// </summary>
        public string PageDown { get { return "\xE00F"; } }

        /// <summary>
        /// Represents the End key.
        /// </summary>
        public string End { get { return "\xE010"; } }

        /// <summary>
        /// Represents the Home key.
        /// </summary>
        public string Home { get { return "\xE011"; } }

        /// <summary>
        /// Represents the left arrow key.
        /// </summary>
        public string Left { get { return "\xE012"; } }

        /// <summary>
        /// Represents the left arrow key.
        /// </summary>
        public string ArrowLeft { get { return "\xE012"; } }

        /// <summary>
        /// Represents the up arrow key.
        /// </summary>
        public string Up { get { return "\xE013"; } }

        /// <summary>
        /// Represents the up arrow key.
        /// </summary>
        public string ArrowUp { get { return "\xE013"; } }

        /// <summary>
        /// Represents the right arrow key.
        /// </summary>
        public string Right { get { return "\xE014"; } }

        /// <summary>
        /// Represents the right arrow key.
        /// </summary>
        public string ArrowRight { get { return "\xE014"; } }

        /// <summary>
        /// Represents the Left arrow key.
        /// </summary>
        public string Down { get { return "\xE015"; } }

        /// <summary>
        /// Represents the Left arrow key.
        /// </summary>
        public string ArrowDown { get { return "\xE015"; } }

        /// <summary>
        /// Represents the Insert key.
        /// </summary>
        public string Insert { get { return "\xE016"; } }

        /// <summary>
        /// Represents the Delete key.
        /// </summary>
        public string Delete { get { return "\xE017"; } }

        /// <summary>
        /// Represents the semi-colon key.
        /// </summary>
        public string Semicolon { get { return "\xE018"; } }

        /// <summary>
        /// Represents the equal sign key.
        /// </summary>
        public string Equal { get { return "\xE019"; } }

        // Number pad keys

        /// <summary>
        /// Represents the number pad 0 key.
        /// </summary>
        public string NumPad0 { get { return "\xE01A"; } }

        /// <summary>
        /// Represents the number pad 1 key.
        /// </summary>
        public string NumPad1 { get { return "\xE01B"; } }

        /// <summary>
        /// Represents the number pad 2 key.
        /// </summary>
        public string NumPad2 { get { return "\xE01C"; } }

        /// <summary>
        /// Represents the number pad 3 key.
        /// </summary>
        public string NumPad3 { get { return "\xE01D"; } }

        /// <summary>
        /// Represents the number pad 4 key.
        /// </summary>
        public string NumPad4 { get { return "\xE01E"; } }

        /// <summary>
        /// Represents the number pad 5 key.
        /// </summary>
        public string NumPad5 { get { return "\xE01F"; } }

        /// <summary>
        /// Represents the number pad 6 key.
        /// </summary>
        public string NumPad6 { get { return "\xE020"; } }

        /// <summary>
        /// Represents the number pad 7 key.
        /// </summary>
        public string NumPad7 { get { return "\xE021"; } }

        /// <summary>
        /// Represents the number pad 8 key.
        /// </summary>
        public string NumPad8 { get { return "\xE022"; } }

        /// <summary>
        /// Represents the number pad 9 key.
        /// </summary>
        public string NumPad9 { get { return "\xE023"; } }

        /// <summary>
        /// Represents the number pad multiplication key.
        /// </summary>
        public string Multiply { get { return "\xE024"; } }

        /// <summary>
        /// Represents the number pad addition key.
        /// </summary>
        public string Add { get { return "\xE025"; } }

        /// <summary>
        /// Represents the number pad thousands separator key.
        /// </summary>
        public string Separator { get { return "\xE026"; } }

        /// <summary>
        /// Represents the number pad subtraction key.
        /// </summary>
        public string Subtract { get { return "\xE027"; } }

        /// <summary>
        /// Represents the number pad decimal separator key.
        /// </summary>
        public string Decimal { get { return "\xE028"; } }

        /// <summary>
        /// Represents the number pad division key.
        /// </summary>
        public string Divide { get { return "\xE029"; } }

        // Function keys

        /// <summary>
        /// Represents the function key F1.
        /// </summary>
        public string F1 { get { return "\xE031"; } }

        /// <summary>
        /// Represents the function key F2.
        /// </summary>
        public string F2 { get { return "\xE032"; } }

        /// <summary>
        /// Represents the function key F3.
        /// </summary>
        public string F3 { get { return "\xE033"; } }

        /// <summary>
        /// Represents the function key F4.
        /// </summary>
        public string F4 { get { return "\xE034"; } }

        /// <summary>
        /// Represents the function key F5.
        /// </summary>
        public string F5 { get { return "\xE035"; } }

        /// <summary>
        /// Represents the function key F6.
        /// </summary>
        public string F6 { get { return "\xE036"; } }

        /// <summary>
        /// Represents the function key F7.
        /// </summary>
        public string F7 { get { return "\xE037"; } }

        /// <summary>
        /// Represents the function key F8.
        /// </summary>
        public string F8 { get { return "\xE038"; } }

        /// <summary>
        /// Represents the function key F9.
        /// </summary>
        public string F9 { get { return "\xE039"; } }

        /// <summary>
        /// Represents the function key F10.
        /// </summary>
        public string F10 { get { return "\xE03A"; } }

        /// <summary>
        /// Represents the function key F11.
        /// </summary>
        public string F11 { get { return "\xE03B"; } }

        /// <summary>
        /// Represents the function key F12.
        /// </summary>
        public string F12 { get { return "\xE03C"; } }

        /// <summary>
        /// Represents the function key META.
        /// </summary>
        public string Meta { get { return "\xE03D"; } }

        /// <summary>
        /// Represents the function key COMMAND.
        /// </summary>
        public string Command { get { return "\xE03D"; } }

    }

}
