using System;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Mechanism by which to find elements within a document.
    /// </summary>
    [Guid("0277FC34-FD1B-4616-BB19-300DAA508541")]
    [ComVisible(true)]
    public enum Strategy : short {
        /// <summary></summary>
        None = 0,
        /// <summary></summary>
        ClassName = 1,
        /// <summary></summary>
        CssSelector = 2,
        /// <summary></summary>
        Id = 3,
        /// <summary></summary>
        Name = 4,
        /// <summary></summary>
        LinkText = 5,
        /// <summary></summary>
        PartialLinkText = 6,
        /// <summary></summary>
        TagName = 7,
        /// <summary></summary>
        XPath = 8,
        /// <summary></summary>
        Any = 9
    }

}
