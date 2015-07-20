using System;

namespace Selenium.Internal {

    [AttributeUsage(AttributeTargets.Assembly)]
    class AssemblyURLAttribute : Attribute {

        public string URL;

        public AssemblyURLAttribute(string url) {
            this.URL = url;
        }

    }

}
