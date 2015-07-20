using System;
using System.IO;
using System.Runtime.InteropServices;

namespace vbsc {

    [ComVisible(true)]
    [Guid("047DEC5A-95C1-4C86-827F-7B8C92EBA67B")]
    public interface IWscript {

        [DispId(0)]
        string Name { get; }

        [DispId(100)]
        IWscript Application { get; }

        [DispId(1002)]
        object Arguments(int index = -1);

        [DispId(2001)]
        void Echo(object message);

        [DispId(101)]
        string FullName { get; }

        [DispId(102)]
        string Path { get; }

        [DispId(1001)]
        string ScriptFullName { get; }

        [DispId(2000)]
        object CreateObject(string strProgID, string strPrefix = null);

        [DispId(2002)]
        object GetObject(string strPathname = null, string strProgID = null, string strPrefix = null);

        [DispId(1000)]
        string ScriptName { get; }

        [DispId(2004)]
        void Sleep(int timems);

        [DispId(1008)]
        TextWriter StdErr { get; }

        [DispId(1006)]
        TextReader StdIn { get; }

        [DispId(1007)]
        TextWriter StdOut { get; }

        [DispId(200)]
        void Quit(int exitcode = 0);
    }
}
