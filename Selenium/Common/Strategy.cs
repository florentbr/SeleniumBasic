using System;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Mechanism by which to find elements within a document.
    /// </summary>
    /// <remarks>
    /// See W3's <see href="https://w3c.github.io/webdriver/#dfn-table-of-location-strategies">Locator strategies</see>.
    /// Those in the list below without string constant are not supported by the modern webdriver API
    /// </remarks>
    [Guid("0277FC34-FD1B-4616-BB19-300DAA508541")]
    [ComVisible(true)]
    public enum Strategy : short {
        /// <summary></summary>
        None = 0,
        /// <summary></summary>
        Class = 1,
        /// <summary>"css selector"</summary>
        Css = 2,
        /// <summary></summary>
        Id = 3,
        /// <summary></summary>
        Name = 4,
        /// <summary>"link text" </summary>
        LinkText = 5,
        /// <summary>"partial link text" </summary>
        PartialLinkText = 6,
        /// <summary>"tag name"</summary>
        Tag = 7,
        /// <summary>"xpath"</summary>
        XPath = 8,
        /// <summary></summary>
        Any = 9
    }

}
