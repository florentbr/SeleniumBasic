
namespace Selenium.Errors {

    /// <summary>
    /// NullImageError
    /// </summary>
    public class ImageError : SeleniumError {
        internal ImageError(string message)
            : base(message) { }
    }

    /// <summary>
    /// NullImageError
    /// </summary>
    public class NullImageError : ImageError {
        internal NullImageError()
            : base("Image has been disposed.") { }
    }

}
