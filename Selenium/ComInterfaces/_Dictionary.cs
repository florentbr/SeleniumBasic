using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-1456C48D8E5C")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface _Dictionary {

        [DispId(0), Description("Gets or sets the value associated with the specified key")]
        object this[string key] { get; set; }

        [DispId(128), Description("Gets the number of elements")]
        int Count { get; }

        [DispId(345), Description("Returns the value associated with the specified key or the default value if the key doesn't exist.")]
        object Get(string key, object defaultValue);

        [DispId(346), Description("Set the value for the given key")]
        void Set(string key, object value);

        [DispId(356), Description("Adds the specified key and value to the dictionary")]
        void Add(string key, object value);

        [DispId(367), Description("Removes all keys and values")]
        void Clear();

        [DispId(478), Description("Returms an array containing the keys")]
        string[] Keys();

        [DispId(489), Description("Returms an array containing the values")]
        object[] Values();

        [DispId(497), Description("Returns true if the dictionary has the key, false otherwise")]
        bool ContainsKey(string key);

        [DispId(550), Description("Copies the keys and values to the target range in Excel")]
        void ToExcel(object target = null, bool clear = false);

        [DispId(-4)]
        System.Collections.IEnumerator _NewEnum();  //Has to be declared last

    }

}
