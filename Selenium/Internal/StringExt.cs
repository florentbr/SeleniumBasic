using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Selenium.Internal {

    static class StringExt {

        /// <summary>
        /// Get a substring of the first N characters.
        /// </summary>
        internal static string Truncate(this string source, int length) {
            if (source.Length > length)
                source = source.Substring(0, length) + "...";
            return source;
        }

        internal static string Remove(this string source, char exclude) {
            char[] sb = new char[source.Length];
            int j = 0;
            foreach (char c in source) {
                if (c != exclude)
                    sb[j++] = c;
            }
            return new string(sb, 0, j);
        }

        public static string Join(this IEnumerable values, string separator) {
            StringBuilder sb = new StringBuilder();
            IEnumerator iter = values.GetEnumerator();
            while (iter.MoveNext()) {
                object current = iter.Current;
                if (current != null)
                    sb.Append(current).Append(separator);
            }
            if (sb.Length == 0)
                return string.Empty;
            return sb.ToString(0, sb.Length - separator.Length);
        }

        public static List Matches(this string text, string pattern, int group = 0) {
            MatchCollection matches = Regex.Matches(text, pattern);
            int count = matches.Count;
            if (count == 0)
                return null;

            List values = new List(count);
            GroupCollection groups = matches[0].Groups;
            if (groups == null || group == 0 || groups.Count == 0) {
                for (var i = 0; i < count; i++)
                    values.Add(matches[i].Value);
            } else {
                for (var i = 0; i < count; i++)
                    values.Add(matches[i].Groups[group].Value);
            }
            return values;
        }

        public static string Match(this string text, string pattern, int group = 0) {
            Match match = Regex.Match(text, pattern);
            if (!match.Success)
                return null;

            if (match.Groups == null || group == 0 || match.Groups.Count == 0) {
                return match.Value;
            } else {
                return match.Groups[group].Value;
            }
        }

        public static bool TryExtractNumber(this string txt, out double number, char decimalCharacter = '.') {
            char[] chars = new char[txt.Length];
            int count = 0;
            foreach (char c in txt) {
                if ((c >= '0' && c <= '9') || c == '-') {
                    chars[count++] = c;
                } else if (c == decimalCharacter) {
                    chars[count++] = '.';
                }
            }
            if (count != 0) {
                string str = new string(chars, 0, count);
                return double.TryParse(str, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out number);
            }
            number = 0;
            return false;
        }

    }

}
