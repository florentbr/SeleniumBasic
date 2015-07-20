
namespace Selenium.Errors {

    /// <summary>
    /// 
    /// </summary>
    public class InvalidOperationError : SeleniumError {
        internal InvalidOperationError(string message, params object[] args)
            : base(50, message, args) { }
    }

}
