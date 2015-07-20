using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Selenium.Tests.Internals {

    static class TypeExt {

        internal static string GetGuid(this Type type) {
            object[] attributes = type.GetCustomAttributes(typeof(GuidAttribute), false);
            if (attributes.Length == 0)
                throw new Exception("Guid is missing on " + type.FullName);
            return "{" + ((GuidAttribute)attributes[0]).Value + "}";
        }

        internal static string GetProgId(this Type type) {
            object[] attributes = type.GetCustomAttributes(typeof(ProgIdAttribute), false);
            if (attributes.Length == 0)
                throw new Exception("Guid is missing on " + type.FullName);
            return ((ProgIdAttribute)attributes[0]).Value;
        }

        /// <summary>
        /// Returns the first attribute on the type if the attribute is found.
        /// If the attribute is missing:
        ///   If throwException is true then
        ///      throws AttributeMissingException
        ///   Else
        ///      returns null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="throwException">If true returns null. If false throws an AttributeMissingException.</param>
        /// <returns></returns>
        internal static T GetFirstAttribute<T>(this Type type, bool throwException = true) {
            object[] attributes = type.GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0) {
                if (throwException)
                    throw new AttributeMissingException(type, typeof(T));
                return (T)(object)null;
            }
            return (T)attributes[0];
        }

        /// <summary>
        /// Returns the first attribute on the assembly.
        /// If the attribute is missing:
        ///   If throwException is true then
        ///      throws AttributeMissingException
        ///   Else
        ///      returns null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="throwException">If true returns null. If false throws an AttributeMissingException.</param>
        /// <returns></returns>
        internal static T GetFirstAttribute<T>(this Assembly assembly, bool throwException = true) {
            object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0) {
                if (throwException)
                    throw new AttributeMissingException(assembly.GetType(), typeof(T));
                return (T)(object)null;
            }
            return (T)attributes[0];
        }

        class AttributeMissingException : Exception {
            public AttributeMissingException(Type typeObject, Type typeAttribute)
                : base(string.Format("Missing attribute {0} on type {1}", typeAttribute, typeObject)) { }
        }

    }

}
