
namespace Selenium.Errors {

    /// <summary>
    /// File not found error
    /// </summary>
    class FileNotFoundError : SeleniumError {
        internal FileNotFoundError(string filepath)
            : base("File: " + filepath) { }
    }

}
