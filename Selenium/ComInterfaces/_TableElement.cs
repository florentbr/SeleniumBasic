using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-BE15C121F199")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _TableElement {

        [DispId(810), Description("Return an array filled with data from a table element")]
        object[,] Data(int firstRowsToSkip = 0, int lastRowsToSkip = 0, string map = null);

        [DispId(812), Description("Copy the data to the target (Address, Range or Worksheet).")]
        void ToExcel(object target = null, bool clear = false, int firstRowsToSkip = 0, int lastRowsToSkip = 0, string map = null);

    }

}
