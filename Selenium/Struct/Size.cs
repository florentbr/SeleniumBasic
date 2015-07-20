using Selenium.Core;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Size structure
    /// </summary>
    [Guid("0277FC34-FD1B-4616-BB19-7E2EBB6C82E9")]
    [ComVisible(true)]
    [DebuggerDisplay("width = {Width} height = {Height}")]
    public struct Size {

        /// <summary>
        /// 
        /// </summary>
        public int Width;

        /// <summary>
        /// 
        /// </summary>
        public int Height;

        /// <summary>
        /// Get the size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Size(int width, int height) {
            this.Width = width;
            this.Height = height;
        }

        internal Size(Dictionary dict) {
            try {
                this.Width = Convert.ToInt32(dict["width"]);
                this.Height = Convert.ToInt32(dict["height"]);
            } catch (Errors.KeyNotFoundError ex) {
                throw new DeserializeException(typeof(Size), ex);
            } catch (InvalidCastException ex) {
                throw new DeserializeException(typeof(Size), ex);
            }
        }

        /// <summary>
        /// Returns the text representaton of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format(@"{{width={0} height={1}}}", this.Width, this.Height);
        }

    }
}
