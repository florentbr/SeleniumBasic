
namespace Selenium.Serializer {

    /// <summary>
    /// Json serializer exception
    /// </summary>
    class JsonException : SeleniumException {

        internal JsonException(string message)
            : base(message) { }

        internal JsonException(string message, byte[] data, int index)
            : base(string.Format("{0} (position: {1})", message, index)) { }
    }

}
