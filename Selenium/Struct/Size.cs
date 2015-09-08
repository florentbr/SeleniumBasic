using Selenium.Core;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Size structure
    /// </summary>
    [Guid("0277FC34-FD1B-4616-BB19-2108C1FE2EE9")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [DebuggerDisplay("Width={Width} Height={Height}")]
    public class Size : ComInterfaces._Size {

        private int _width, _height;

        /// <summary>
        /// Get the size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Size(int width, int height) {
            _width = width;
            _height = height;
        }

        internal Size(Dictionary dict) {
            try {
                _width = Convert.ToInt32(dict["width"]);
                _height = Convert.ToInt32(dict["height"]);
            } catch (Errors.KeyNotFoundError ex) {
                throw new DeserializeException(typeof(Size), ex);
            } catch (InvalidCastException ex) {
                throw new DeserializeException(typeof(Size), ex);
            }
        }

        /// <summary>
        /// Width
        /// </summary>
        public int Width {
            get { return _width; }
        }

        /// <summary>
        /// Height
        /// </summary>
        public int Height {
            get { return _height; }
        }

        /// <summary>
        /// Returns the text representaton of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format(@"{{width={0} height={1}}}", _width, _height);
        }

    }

}
