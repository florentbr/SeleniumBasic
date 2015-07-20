using Selenium;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.ComInterfaces {
#pragma warning disable 1591

    [Guid("0277FC34-FD1B-4616-BB19-6E0522EA435E")]
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Application {

        [DispId(10), Description("")]
        Assert Assert { get; }

        [DispId(18), Description("")]
        Verify Verify { get; }

        [DispId(23), Description("")]
        Waiter Waiter { get; }

        [DispId(27), Description("")]
        Utils Utils { get; }



        [DispId(42), Description("")]
        By By { get; }

        [DispId(46), Description("")]
        Keys Keys { get; }


        [DispId(63), Description("")]
        WebDriver WebDriver();

        [DispId(65), Description("")]
        WebDriver ChromeDriver();

        [DispId(68), Description("")]
        WebDriver FirefoxDriver();

        [DispId(72), Description("")]
        WebDriver IEDriver();

        [DispId(75), Description("")]
        WebDriver OperaDriver();

        [DispId(78), Description("")]
        WebDriver PhantomJSDriver();



        [DispId(92), Description("")]
        List Dictionary();

        [DispId(95), Description("")]
        List List();

        [DispId(97), Description("")]
        Point Point(int x, int y);

        [DispId(102), Description("")]
        Size Size(int width, int height);

        [DispId(105), Description("")]
        Table Table();



        [DispId(120), Description("")]
        PdfFile PdfFile();


    }

}
