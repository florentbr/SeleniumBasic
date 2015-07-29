using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Selenium.Internal {

    static class ObjExt {

        /// <summary>
        /// Compares 2 objets
        /// </summary>
        internal static bool AreEqual(object a, object b) {
            if (a == null){
                if(b == null)
                    return true;
                return false;
            }
            if(b == null){
                if(a == null)
                    return true;
                return false;
            }
            if (Type.GetTypeCode(a.GetType()) != TypeCode.Object
                && Type.GetTypeCode(b.GetType()) != TypeCode.Object) {
                return a.ToString() == b.ToString();
            }
            IEnumerable aEnum, bEnum;
            if ((aEnum = a as IEnumerable) != null && (bEnum = b as IEnumerable) != null) {
                aEnum = SplitEnumerableString(aEnum);
                bEnum = SplitEnumerableString(bEnum);
                var bIter = bEnum.GetEnumerator();
                foreach (var va in aEnum) {
                    if (!bIter.MoveNext() || !AreEqual(va, bIter.Current))
                        return false;
                }
                if (bIter.MoveNext())
                    return false;
                return true;
            }
            Image aImg;
            if ((aImg = a as Image) != null)
                return aImg.Equals(b as Image);
            return a.Equals(b);
        }

        /// <summary>
        /// Returns true if the object is a number, false otherwise
        /// </summary>
        internal static bool IsNumber(object value) {
            if (value == null)
                return false;
            int typecode = (int)Type.GetTypeCode(value.GetType());
            return typecode > 6 && typecode < 16;
        }

        /// <summary>
        /// Splits a string if it contains one of these separator characters : ";" "," "\n"
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>String or array of strings</returns>
        internal static IEnumerable SplitEnumerableString(IEnumerable value) {
            string str = value as string;
            if (str != null) {
                for(int i = 0; i < str.Length; i++){
                    switch (str[i]) {
                        case ';': return Regex.Split(str, @"(?<!\\);");
                        case '\n': return Regex.Split(str, @"\r?\n");
                        case ',': return str.Split(',');
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Converts an object to string.
        /// if null, returns "Empty"
        /// if is string, returns "..."
        /// if is enumerable, returns "[0, 0, ...]"
        /// else returns the default string representation
        /// </summary>
        internal static string ToStrings(object value) {
            if (value == null)
                return "Empty";

            if (value is string)
                return "\"" + ((string)value).Truncate(100) + "\"";

            IEnumerable enumereble = value as IEnumerable;
            if (enumereble == null) 
                return value.ToString();
            
            var sb = new StringBuilder("[");
            var iter = enumereble.GetEnumerator();
            if (iter.MoveNext()) {
                sb.Append(iter.Current);
                while (iter.MoveNext())
                    sb.Append(',').Append(iter.Current);
            }
            sb.Append(']');
            return sb.ToString();
        }

    }

}
