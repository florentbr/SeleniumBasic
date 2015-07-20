
namespace Selenium.Serializer {

    /// <summary>
    /// Json serializer exception
    /// </summary>
    class JsonException : SeleniumException {

        internal JsonException(string message)
            : base(message) { }

        internal JsonException(string message, int index)
            : base(string.Format("{0} (index: {1})", message, index)) { }
    }

}
