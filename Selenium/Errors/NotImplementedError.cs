
namespace Selenium.Errors {

    /// <summary>
    /// 
    /// </summary>
    public class NotImplementedError : SeleniumError {
        internal NotImplementedError()
            : base("The command is not yet implemented") { }
    }

}
