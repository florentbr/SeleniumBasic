using Selenium.Core;
using Selenium.Internal;
using Selenium.Serializer;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Selenium {

    /// <summary>
    /// Represents an element's ShadowRoot. See <see cref="WebElement.Shadow"/>
    /// </summary>
    [ProgId("Selenium.Shadow")]
    [Guid("0277FC34-FD1B-4616-BB19-98333C96F89B")]
    [Description("Interface to a shadow root.")]
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public class Shadow : SearchContext, ComInterfaces._Shadow, IJsonObject {
        internal const string SHADOW     = "SHADOW";
        internal const string IDENTIFIER = "shadow-6066-11e4-a52e-4f735466cecf";


        internal static bool TryParse(Dictionary dict, out string id) {
            return dict.TryGetValue(IDENTIFIER, out id);
        }

        internal readonly RemoteSession _session;
        internal readonly string Id;

        internal Shadow(RemoteSession session, string id) {
            _session = session;
            this.Id = id;
        }

        internal Shadow(RemoteSession session, Dictionary dict) {
            _session = session;
            if (!Shadow.TryParse(dict, out this.Id))
                throw new SeleniumException("Failed to extact the Shadow from the dictionary. Missing key.");
        }

        internal override RemoteSession session {
            get {
                return _session;
            }
        }

        internal override string uri {
            get {
                return "/shadow/" + this.Id;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Dictionary SerializeJson() {
            var dict = new Dictionary();
            if( !WebDriver.LEGACY )
                dict.Add(IDENTIFIER, this.Id); // Selenium.NET sends both. Let's do the same.
            dict.Add( SHADOW, this.Id);
            return dict;
        }

        private object Send(RequestMethod method, string relativeUri) {
            return _session.Send(method, this.uri + relativeUri);
        }

        private object Send(RequestMethod method, string relativeUri, string key, object value) {
            return _session.Send(method, this.uri + relativeUri, key, value);
        }

        private object Send(RequestMethod method, string relativeUri, string key1, object value1, string key2, object value2) {
            return _session.Send(method, this.uri + relativeUri, key1, value1, key2, value2);
        }

        /// <summary>
        /// Compares if two web elements are equal
        /// </summary>
        /// <param name="other">WebElement to compare against</param>
        /// <returns>A boolean if it is equal or not</returns>
        public bool Equals(WebElement other) {
            if (Id == other.Id)
                return true;
            return (bool)Send(RequestMethod.GET, "/equals/" + other.Id);
        }

        #region Protected support methods

        /// <summary>
        /// Determines whether the specified instances are considered equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            Shadow shd = obj as Shadow;
            if (shd == null)
                return false;
            return Id == shd.Id;
        }

        /// <summary>
        /// Returns the hash code for this element
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        #endregion

    }
}
