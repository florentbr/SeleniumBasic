using Selenium.Core;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Point structure
    /// </summary>
    [Guid("0277FC34-FD1B-4616-BB19-E1305CCF61EC")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [DebuggerDisplay("X={X} Y={Y}")]
    public class Point : ComInterfaces._Point {

        private int _x, _y;

        internal Point(Dictionary dict) {
            try {
                _x = Convert.ToInt32(dict["x"]);
                _y = Convert.ToInt32(dict["y"]);
            } catch (Errors.KeyNotFoundError ex) {
                throw new DeserializeException(typeof(Point), ex);
            } catch (InvalidCastException ex) {
                throw new DeserializeException(typeof(Point), ex);
            }
        }

        /// <summary>
        /// Point construcor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y) {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// X
        /// </summary>
        public int X {
            get { return _x; }
        }

        /// <summary>
        /// Y
        /// </summary>
        public int Y {
            get { return _y; }
        }

        /// <summary>
        /// Returns the text representaton of this instance
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format(@"{{x={0} y={1}}}", _x, _y);
        }

    }

}
