
namespace Selenium.Errors {

    /// <summary>
    /// Exception occurring when a key is missing in the dictionary
    /// </summary>
    public class KeyNotFoundError : SeleniumError {

        /// <summary>
        /// Missing key
        /// </summary>
        public readonly string Key;

        internal KeyNotFoundError(string key)
            : base("Dictionary key not found: " + key) {
            Key = key;
        }

    }

}
