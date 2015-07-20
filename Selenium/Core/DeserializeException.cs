using Selenium;
using System;

namespace Selenium.Core {

    /// <summary>
    /// Exception occuring when a Json object is not recognized.
    /// </summary>
    class DeserializeException : SeleniumException {

        internal DeserializeException(Type type, Errors.KeyNotFoundError ex)
            : base("Failed to deserialize the {0} object. Missing key: {1}", type.Name, ex.Key) { }

        internal DeserializeException(Type type, InvalidCastException ex)
            : base("Failed to deserialize the {0} object. Invalid type of value.", type.Name) { }
    }

}
