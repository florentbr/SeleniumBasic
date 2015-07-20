using System;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Cache status
    /// </summary>
    [Guid("0277FC34-FD1B-4616-BB19-C724C5135B6E")]
    [ComVisible(true)]
    public enum CacheState {
        /// <summary></summary>
        UNCACHED = 0,
        /// <summary></summary>
        IDLE = 1,
        /// <summary></summary>
        CHECKING = 2,
        /// <summary></summary>
        DOWNLOADING = 3,
        /// <summary></summary>
        UPDATE_READY = 4,
        /// <summary></summary>
        OBSOLETE = 5
    }

}
