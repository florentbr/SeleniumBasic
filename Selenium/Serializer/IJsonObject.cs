using JsonObject = Selenium.Dictionary;

namespace Selenium.Serializer {

    /// <summary>
    /// Interface to serialize to a Json object.
    /// </summary>
    internal interface IJsonObject {
        JsonObject SerializeJson();
    }

}
