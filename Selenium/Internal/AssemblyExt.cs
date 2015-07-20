using System.Reflection;

namespace Selenium.Internal {

    static class AssemblyExt {

        internal static T GetFirstAttribute<T>(this Assembly assembly) {
            object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0)
                return default(T);
            return (T)attributes[0];
        }

    }

}
