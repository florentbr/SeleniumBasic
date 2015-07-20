using Selenium.Core;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Point structure
    /// </summary>
    [Guid("0277FC34-FD1B-4616-BB19-ACE280CD7780")]
    [ComVisible(true)]
    [DebuggerDisplay("x = {X} y = {Y}")]
    public struct Point {

        /// <summary>
        /// X value
        /// </summary>
        public int X;

        /// <summary>
        /// Y value
        /// </summary>
        public int Y;

        internal Point(Dictionary dict) {
            try {
                this.X = Convert.ToInt32(dict["x"]);
                this.Y = Convert.ToInt32(dict["y"]);
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
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Returns the text representaton of this instance
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format(@"{{x={0} y={1}}}", this.X, this.Y);
        }

    }
}
