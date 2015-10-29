using System.IO;

namespace Selenium.Serializer {

    /// <summary>
    /// Interface to serialize to a base64 string.
    /// </summary>
    internal interface IJsonBinary {
        void Save(Stream stream);
    }

}
