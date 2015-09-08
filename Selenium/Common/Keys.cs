using Selenium.ComInterfaces;
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
    public class Keys : _Keys {

        #region Static API

        /// <summary>
        /// Represents the NUL keystroke.
        /// </summary>
        public const string Null = "\xE000";

        /// <summary>
        /// Represents the Cancel keystroke.
        /// </summary>
        public const string Cancel = "\xE001";

        /// <summary>
        /// Represents the Help keystroke.
        /// </summary>
        public const string Help = "\xE002";

        /// <summary>
        /// Represents the Backspace key.
        /// </summary>
        public const string Backspace = "\xE003";

        /// <summary>
        /// Represents the Tab key.
        /// </summary>
        public const string Tab = "\xE004";

        /// <summary>
        /// Represents the Clear keystroke.
        /// </summary>
        public const string Clear = "\xE005";

        /// <summary>
        /// Represents the Return key.
        /// </summary>
        public const string Return = "\xE006";

        /// <summary>
        /// Represents the Enter key.
        /// </summary>
        public const string Enter = "\xE007";

        /// <summary>
        /// Represents the Shift key.
        /// </summary>
        public const string Shift = "\xE008";

        /// <summary>
        /// Represents the Shift key.
        /// </summary>
        public const string LeftShift = "\xE008";

        /// <summary>
        /// Represents the Control key.
        /// </summary>
        public const string Control = "\xE009";

        /// <summary>
        /// Represents the Control key.
        /// </summary>
        public const string LeftControl = "\xE009";

        /// <summary>
        /// Represents the Alt key.
        /// </summary>
        public const string Alt = "\xE00A";

        /// <summary>
        /// Represents the Alt key.
        /// </summary>
        public const string LeftAlt = "\xE00A";

        /// <summary>
        /// Represents the Pause key.
        /// </summary>
        public const string Pause = "\xE00B";

        /// <summary>
        /// Represents the Escape key.
        /// </summary>
        public const string Escape = "\xE00C";

        /// <summary>
        /// Represents the Spacebar key.
        /// </summary>
        public const string Space = "\xE00D";

        /// <summary>
        /// Represents the Page Up key.
        /// </summary>
        public const string PageUp = "\xE00E";

        /// <summary>
        /// Represents the Page Down key.
        /// </summary>
        public const string PageDown = "\xE00F";

        /// <summary>
        /// Represents the End key.
        /// </summary>
        public const string End = "\xE010";

        /// <summary>
        /// Represents the Home key.
        /// </summary>
        public const string Home = "\xE011";

        /// <summary>
        /// Represents the left arrow key.
        /// </summary>
        public const string Left = "\xE012";

        /// <summary>
        /// Represents the left arrow key.
        /// </summary>
        public const string ArrowLeft = "\xE012";

        /// <summary>
        /// Represents the up arrow key.
        /// </summary>
        public const string Up = "\xE013";

        /// <summary>
        /// Represents the up arrow key.
        /// </summary>
        public const string ArrowUp = "\xE013";

        /// <summary>
        /// Represents the right arrow key.
        /// </summary>
        public const string Right = "\xE014";

        /// <summary>
        /// Represents the right arrow key.
        /// </summary>
        public const string ArrowRight = "\xE014";

        /// <summary>
        /// Represents the Left arrow key.
        /// </summary>
        public const string Down = "\xE015";

        /// <summary>
        /// Represents the Left arrow key.
        /// </summary>
        public const string ArrowDown = "\xE015";

        /// <summary>
        /// Represents the Insert key.
        /// </summary>
        public const string Insert = "\xE016";

        /// <summary>
        /// Represents the Delete key.
        /// </summary>
        public const string Delete = "\xE017";

        /// <summary>
        /// Represents the semi-colon key.
        /// </summary>
        public const string Semicolon = "\xE018";

        /// <summary>
        /// Represents the equal sign key.
        /// </summary>
        public const string Equal = "\xE019";

        // Number pad keys

        /// <summary>
        /// Represents the number pad 0 key.
        /// </summary>
        public const string NumPad0 = "\xE01A";

        /// <summary>
        /// Represents the number pad 1 key.
        /// </summary>
        public const string NumPad1 = "\xE01B";

        /// <summary>
        /// Represents the number pad 2 key.
        /// </summary>
        public const string NumPad2 = "\xE01C";

        /// <summary>
        /// Represents the number pad 3 key.
        /// </summary>
        public const string NumPad3 = "\xE01D";

        /// <summary>
        /// Represents the number pad 4 key.
        /// </summary>
        public const string NumPad4 = "\xE01E";

        /// <summary>
        /// Represents the number pad 5 key.
        /// </summary>
        public const string NumPad5 = "\xE01F";

        /// <summary>
        /// Represents the number pad 6 key.
        /// </summary>
        public const string NumPad6 = "\xE020";

        /// <summary>
        /// Represents the number pad 7 key.
        /// </summary>
        public const string NumPad7 = "\xE021";

        /// <summary>
        /// Represents the number pad 8 key.
        /// </summary>
        public const string NumPad8 = "\xE022";

        /// <summary>
        /// Represents the number pad 9 key.
        /// </summary>
        public const string NumPad9 = "\xE023";

        /// <summary>
        /// Represents the number pad multiplication key.
        /// </summary>
        public const string Multiply = "\xE024";

        /// <summary>
        /// Represents the number pad addition key.
        /// </summary>
        public const string Add = "\xE025";

        /// <summary>
        /// Represents the number pad thousands separator key.
        /// </summary>
        public const string Separator = "\xE026";

        /// <summary>
        /// Represents the number pad subtraction key.
        /// </summary>
        public const string Subtract = "\xE027";

        /// <summary>
        /// Represents the number pad decimal separator key.
        /// </summary>
        public const string Decimal = "\xE028";

        /// <summary>
        /// Represents the number pad division key.
        /// </summary>
        public const string Divide = "\xE029";

        // Function keys

        /// <summary>
        /// Represents the function key F1.
        /// </summary>
        public const string F1 = "\xE031";

        /// <summary>
        /// Represents the function key F2.
        /// </summary>
        public const string F2 = "\xE032";

        /// <summary>
        /// Represents the function key F3.
        /// </summary>
        public const string F3 = "\xE033";

        /// <summary>
        /// Represents the function key F4.
        /// </summary>
        public const string F4 = "\xE034";

        /// <summary>
        /// Represents the function key F5.
        /// </summary>
        public const string F5 = "\xE035";

        /// <summary>
        /// Represents the function key F6.
        /// </summary>
        public const string F6 = "\xE036";

        /// <summary>
        /// Represents the function key F7.
        /// </summary>
        public const string F7 = "\xE037";

        /// <summary>
        /// Represents the function key F8.
        /// </summary>
        public const string F8 = "\xE038";

        /// <summary>
        /// Represents the function key F9.
        /// </summary>
        public const string F9 = "\xE039";

        /// <summary>
        /// Represents the function key F10.
        /// </summary>
        public const string F10 = "\xE03A";

        /// <summary>
        /// Represents the function key F11.
        /// </summary>
        public const string F11 = "\xE03B";

        /// <summary>
        /// Represents the function key F12.
        /// </summary>
        public const string F12 = "\xE03C";

        /// <summary>
        /// Represents the function key META.
        /// </summary>
        public const string Meta = "\xE03D";

        /// <summary>
        /// Represents the function key COMMAND.
        /// </summary>
        public const string Command = "\xE03D";

        #endregion


        #region COM interface

        /// <summary>
        /// Represents the NUL keystroke.
        /// </summary>
        string _Keys.Null { get { return Null; } }

        /// <summary>
        /// Represents the Cancel keystroke.
        /// </summary>
        string _Keys.Cancel { get { return Cancel; } }

        /// <summary>
        /// Represents the Help keystroke.
        /// </summary>
        string _Keys.Help { get { return Help; } }

        /// <summary>
        /// Represents the Backspace key.
        /// </summary>
        string _Keys.Backspace { get { return Backspace; } }

        /// <summary>
        /// Represents the Tab key.
        /// </summary>
        string _Keys.Tab { get { return Tab; } }

        /// <summary>
        /// Represents the Clear keystroke.
        /// </summary>
        string _Keys.Clear { get { return Clear; } }

        /// <summary>
        /// Represents the Return key.
        /// </summary>
        string _Keys.Return { get { return Return; } }

        /// <summary>
        /// Represents the Enter key.
        /// </summary>
        string _Keys.Enter { get { return Enter; } }

        /// <summary>
        /// Represents the Shift key.
        /// </summary>
        string _Keys.Shift { get { return Shift; } }

        /// <summary>
        /// Represents the Shift key.
        /// </summary>
        string _Keys.LeftShift { get { return LeftShift; } }

        /// <summary>
        /// Represents the Control key.
        /// </summary>
        string _Keys.Control { get { return Control; } }

        /// <summary>
        /// Represents the Control key.
        /// </summary>
        string _Keys.LeftControl { get { return LeftControl; } }

        /// <summary>
        /// Represents the Alt key.
        /// </summary>
        string _Keys.Alt { get { return Alt; } }

        /// <summary>
        /// Represents the Alt key.
        /// </summary>
        string _Keys.LeftAlt { get { return LeftAlt; } }

        /// <summary>
        /// Represents the Pause key.
        /// </summary>
        string _Keys.Pause { get { return Pause; } }

        /// <summary>
        /// Represents the Escape key.
        /// </summary>
        string _Keys.Escape { get { return Escape; } }

        /// <summary>
        /// Represents the Spacebar key.
        /// </summary>
        string _Keys.Space { get { return Space; } }

        /// <summary>
        /// Represents the Page Up key.
        /// </summary>
        string _Keys.PageUp { get { return PageUp; } }

        /// <summary>
        /// Represents the Page Down key.
        /// </summary>
        string _Keys.PageDown { get { return PageDown; } }

        /// <summary>
        /// Represents the End key.
        /// </summary>
        string _Keys.End { get { return End; } }

        /// <summary>
        /// Represents the Home key.
        /// </summary>
        string _Keys.Home { get { return Home; } }

        /// <summary>
        /// Represents the left arrow key.
        /// </summary>
        string _Keys.Left { get { return Left; } }

        /// <summary>
        /// Represents the left arrow key.
        /// </summary>
        string _Keys.ArrowLeft { get { return ArrowLeft; } }

        /// <summary>
        /// Represents the up arrow key.
        /// </summary>
        string _Keys.Up { get { return Up; } }

        /// <summary>
        /// Represents the up arrow key.
        /// </summary>
        string _Keys.ArrowUp { get { return ArrowUp; } }

        /// <summary>
        /// Represents the right arrow key.
        /// </summary>
        string _Keys.Right { get { return Right; } }

        /// <summary>
        /// Represents the right arrow key.
        /// </summary>
        string _Keys.ArrowRight { get { return ArrowRight; } }

        /// <summary>
        /// Represents the Left arrow key.
        /// </summary>
        string _Keys.Down { get { return Down; } }

        /// <summary>
        /// Represents the Left arrow key.
        /// </summary>
        string _Keys.ArrowDown { get { return ArrowDown; } }

        /// <summary>
        /// Represents the Insert key.
        /// </summary>
        string _Keys.Insert { get { return Insert; } }

        /// <summary>
        /// Represents the Delete key.
        /// </summary>
        string _Keys.Delete { get { return Delete; } }

        /// <summary>
        /// Represents the semi-colon key.
        /// </summary>
        string _Keys.Semicolon { get { return Semicolon; } }

        /// <summary>
        /// Represents the equal sign key.
        /// </summary>
        string _Keys.Equal { get { return Equal; } }

        // Number pad keys

        /// <summary>
        /// Represents the number pad 0 key.
        /// </summary>
        string _Keys.NumPad0 { get { return NumPad0; } }

        /// <summary>
        /// Represents the number pad 1 key.
        /// </summary>
        string _Keys.NumPad1 { get { return NumPad1; } }

        /// <summary>
        /// Represents the number pad 2 key.
        /// </summary>
        string _Keys.NumPad2 { get { return NumPad2; } }

        /// <summary>
        /// Represents the number pad 3 key.
        /// </summary>
        string _Keys.NumPad3 { get { return NumPad3; } }

        /// <summary>
        /// Represents the number pad 4 key.
        /// </summary>
        string _Keys.NumPad4 { get { return NumPad4; } }

        /// <summary>
        /// Represents the number pad 5 key.
        /// </summary>
        string _Keys.NumPad5 { get { return NumPad5; } }

        /// <summary>
        /// Represents the number pad 6 key.
        /// </summary>
        string _Keys.NumPad6 { get { return NumPad6; } }

        /// <summary>
        /// Represents the number pad 7 key.
        /// </summary>
        string _Keys.NumPad7 { get { return NumPad7; } }

        /// <summary>
        /// Represents the number pad 8 key.
        /// </summary>
        string _Keys.NumPad8 { get { return NumPad8; } }

        /// <summary>
        /// Represents the number pad 9 key.
        /// </summary>
        string _Keys.NumPad9 { get { return NumPad9; } }

        /// <summary>
        /// Represents the number pad multiplication key.
        /// </summary>
        string _Keys.Multiply { get { return Multiply; } }

        /// <summary>
        /// Represents the number pad addition key.
        /// </summary>
        string _Keys.Add { get { return Add; } }

        /// <summary>
        /// Represents the number pad thousands separator key.
        /// </summary>
        string _Keys.Separator { get { return Separator; } }

        /// <summary>
        /// Represents the number pad subtraction key.
        /// </summary>
        string _Keys.Subtract { get { return Subtract; } }

        /// <summary>
        /// Represents the number pad decimal separator key.
        /// </summary>
        string _Keys.Decimal { get { return Decimal; } }

        /// <summary>
        /// Represents the number pad division key.
        /// </summary>
        string _Keys.Divide { get { return Divide; } }

        // Function keys

        /// <summary>
        /// Represents the function key F1.
        /// </summary>
        string _Keys.F1 { get { return F1; } }

        /// <summary>
        /// Represents the function key F2.
        /// </summary>
        string _Keys.F2 { get { return F2; } }

        /// <summary>
        /// Represents the function key F3.
        /// </summary>
        string _Keys.F3 { get { return F3; } }

        /// <summary>
        /// Represents the function key F4.
        /// </summary>
        string _Keys.F4 { get { return F4; } }

        /// <summary>
        /// Represents the function key F5.
        /// </summary>
        string _Keys.F5 { get { return F5; } }

        /// <summary>
        /// Represents the function key F6.
        /// </summary>
        string _Keys.F6 { get { return F6; } }

        /// <summary>
        /// Represents the function key F7.
        /// </summary>
        string _Keys.F7 { get { return F7; } }

        /// <summary>
        /// Represents the function key F8.
        /// </summary>
        string _Keys.F8 { get { return F8; } }

        /// <summary>
        /// Represents the function key F9.
        /// </summary>
        string _Keys.F9 { get { return F9; } }

        /// <summary>
        /// Represents the function key F10.
        /// </summary>
        string _Keys.F10 { get { return F10; } }

        /// <summary>
        /// Represents the function key F11.
        /// </summary>
        string _Keys.F11 { get { return F11; } }

        /// <summary>
        /// Represents the function key F12.
        /// </summary>
        string _Keys.F12 { get { return F12; } }

        /// <summary>
        /// Represents the function key META.
        /// </summary>
        string _Keys.Meta { get { return Meta; } }

        /// <summary>
        /// Represents the function key COMMAND.
        /// </summary>
        string _Keys.Command { get { return Command; } }

        #endregion

    }

}
