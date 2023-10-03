using Selenium.ComInterfaces;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium {

    /// <summary>
    /// Selenium base class to access other useful objects
    /// </summary>
    /// <seealso cref="_Application"/>
    /// <remarks>
    /// The object accessed via a property (Waiter,Verify,Assert,Utils,Keys, and By) are singletons.
    /// For those are accessed via a method a new instance will be created each time the method is called.
    /// </remarks>
    /// <example>
    /// <code lang="vbs">	
    /// set sa = CreateObject("Selenium.Application")
    /// ad = sa.Keys.ArrowDown
    /// </code>
    /// </example>
    [ProgId("Selenium.Application")]
    [Guid("0277FC34-FD1B-4616-BB19-E9AAFA695FFB")]
    [Description("Base object class")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Application : _Application {

        static Waiter _waiter_;
        static Verify _verify_;
        static Assert _assert_;
        static Utils _utils_;
        static Keys _keys_;
        static By _by_;

        /// <summary>
        /// Returns the Waiter object
        /// </summary>
        public Waiter Waiter {
            get{
                return _waiter_ ?? (_waiter_ = new Waiter());
            }
        }

        /// <summary>
        /// Returns the Verify object
        /// </summary>
        public Verify Verify {
            get {
                return _verify_ ?? (_verify_ = new Verify());
            }
        }

        /// <summary>
        /// Returns the Assert object
        /// </summary>
        public Assert Assert {
            get {
                return _assert_ ?? (_assert_ = new Assert());
            }
        }

        /// <summary>
        /// Returns the Utils object
        /// </summary>
        public Utils Utils {
            get {
                return _utils_ ?? (_utils_ = new Utils());
            }
        }

        /// <summary>
        /// Returns the Keys object
        /// </summary>
        public Keys Keys {
            get {
                return _keys_ ?? (_keys_ = new Keys());
            }
        }

        /// <summary>
        /// Returns the By object
        /// </summary>
        public By By {
            get {
                return _by_ ?? (_by_ = new By());
            }
        }

        /// <summary>
        /// Creates a new instance of WebDriver
        /// </summary>
        /// <returns></returns>
        public WebDriver WebDriver() {
            return new WebDriver();
        }

        /// <summary>
        /// Creates a new instance of FirefoxDriver
        /// </summary>
        /// <returns></returns>
        public WebDriver FirefoxDriver() {
            return new FirefoxDriver();
        }

        /// <summary>
        /// Creates a new instance of ChromeDriver
        /// </summary>
        /// <returns></returns>
        public WebDriver ChromeDriver() {
            return new ChromeDriver();
        }

        /// <summary>
        /// Creates a new instance of OperaDriver
        /// </summary>
        /// <returns></returns>
        public WebDriver OperaDriver() {
            return new ChromeDriver();
        }

        /// <summary>
        /// Creates a new instance of IEDriver
        /// </summary>
        /// <returns></returns>
        public WebDriver IEDriver() {
            return new ChromeDriver();
        }

        /// <summary>
        /// Creates a new instance of PhantomJSDriver
        /// </summary>
        /// <returns></returns>
        public WebDriver PhantomJSDriver() {
            return new ChromeDriver();
        }


        /// <summary>
        /// Creates a new instance of List
        /// </summary>
        /// <returns></returns>
        public List List() {
            return new List();
        }

        /// <summary>
        /// Creates a new instance of Table
        /// </summary>
        /// <returns></returns>
        public Table Table() {
            return new Table();
        }

        /// <summary>
        /// Creates a new instance of Dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary Dictionary() {
            return new Dictionary();
        }

        /// <summary>
        /// Creates a new instance of Point
        /// </summary>
        /// <returns></returns>
        public Point Point(int x, int y) {
            return new Point(x, y);
        }

        /// <summary>
        /// Creates a new instance of Size
        /// </summary>
        /// <returns></returns>
        public Size Size(int width, int height) {
            return new Size(width, height);
        }

        /// <summary>
        /// Creates a new instance of PdfFile
        /// </summary>
        /// <returns></returns>
        public PdfFile PdfFile() {
            return new PdfFile();
        }

    }

}
