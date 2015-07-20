using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace vbsc {

    static class Utils {

        /// <summary>
        /// Join an array with the provided separator.
        /// String items are truncated to the given length.
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator character</param>
        /// <param name="truncateLength">Length to truncate the string to</param>
        /// <returns></returns>
        public static StringBuilder Join(this object[] array, char separator, int truncateLength) {
            if (array == null)
                return new StringBuilder(0);
            var buffer = new StringBuilder();
            for (int i = 0, len = array.Length; i < len; i++) {
                if (i != 0)
                    buffer.Append(separator);
                object ele = array[i];
                if (ele is string) {
                    string eleStr = (string)ele;
                    buffer.Append('"');
                    if (eleStr.Length > truncateLength) {
                        buffer.Append(eleStr, 0, truncateLength);
                        buffer.Append("...");
                    } else {
                        buffer.Append(eleStr);
                    }
                    buffer.Append('"');
                } else {
                    buffer.Append(ele);
                }
            }
            return buffer;
        }

        /// <summary>
        /// Returns the last item of a generic list
        /// </summary>
        /// <typeparam name="T">Type of the list</typeparam>
        /// <param name="list">Generic list</param>
        /// <returns>Item of type T</returns>
        public static T LastItem<T>(this List<T> list) {
            if (list == null || list.Count == 0)
                return default(T);
            return list[list.Count - 1];
        }

        /// <summary>
        /// Removes all the trailling space, tab and new lines
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CleanEnd(this string text) {
            if (text == null)
                return string.Empty;
            return text.TrimEnd(' ', '\t', '\r', '\n');
        }

        /// <summary>
        /// Returns the text for a line number. (One base number)
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="line">Line number starts at 1</param>
        /// <returns></returns>
        public static string GetLineAt(this string text, int line) {
            int iline = 1, startIdx = 0, endIdx = 0, len = text.Length;
            while ((endIdx = text.IndexOf('\n', startIdx + 1)) != -1 && iline++ < line)
                startIdx = endIdx;
            if (endIdx == -1)
                endIdx = len;
            return text.Substring(startIdx, endIdx - startIdx).Trim(' ', '\t', '\r', '\n');
        }

        /// <summary>
        /// Returns the number of lines in a text
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="startIndex">Char index to start counting</param>
        /// <param name="endIndex">Char index to stop counting</param>
        /// <returns></returns>
        public static int CountLines(this string text, int startIndex = 0, int endIndex = -1) {
            int count = 1;
            int i = startIndex;
            int ibreak = endIndex == -1 ? text.Length : endIndex + 1;
            while ((i = text.IndexOf('\n', i + 1)) != -1 && i < ibreak)
                count++;
            return count;
        }

        /// <summary>
        /// Returns the number of the line matching the given pattern.
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="options">Regex options</param>
        /// <returns>Line number starting at 1</returns>
        public static int GetLineNumber(this string text, string pattern, RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase) {
            var matches = Regex.Match(text, pattern, options);
            if (!matches.Success)
                return 0;
            return text.CountLines(0, matches.Index);
        }

        /// <summary>
        /// Returns all the strings matching the given pattern.
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="group">Regex group which returns the text</param>
        /// <param name="options">Regex options</param>
        /// <returns>Array of strings</returns>
        public static string[] FindAll(this string text, string pattern, int group = 0, RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase) {
            var matches = Regex.Matches(text, pattern, options);
            var result = new string[matches.Count];
            for (int i = 0, count = matches.Count; i < count; i++)
                result[i] = matches[i].Groups[group].Value;
            return result;
        }

        /// <summary>
        /// Replace all the occurences matching the given regex pattern
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="replacement">Replacement string</param>
        /// <param name="options">Regex options</param>
        /// <returns>string</returns>
        public static string ReplaceAll(this string text, string pattern, string replacement, RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase) {
            return Regex.Replace(text, pattern, replacement, options);
        }

        /// <summary>
        /// Removes all the occurences matching the given regex pattern
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string RemoveAll(this string text, string pattern, RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase) {
            return Regex.Replace(text, pattern, string.Empty, options);
        }

        /// <summary>
        /// Expands all the given relative file paths.
        /// </summary>
        /// <param name="paths">Relative paths</param>
        /// <param name="extention">Extension filter(pattern)</param>
        /// <param name="workingdir">Relative directory</param>
        /// <returns>Array of absolutes paths</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string[] ExpandFilePaths(IEnumerable<string> paths, string extention, string workingdir = null) {
            var fileList = new List<string>();

            string working_dir_bak = null;
            if (workingdir != null) {
                working_dir_bak = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(workingdir);
            }
            var re = new Regex(extention);
            foreach (var arg in paths) {
                var file_path = arg.Trim('"');
                if (file_path.IndexOf('%') != -1)
                    file_path = System.Environment.ExpandEnvironmentVariables(file_path);
                var directory = Path.GetDirectoryName(file_path);
                if (directory.Length == 0)
                    directory = ".";
                var filename = Path.GetFileName(file_path);
                bool hasone = false;
                foreach (var filepath in Directory.GetFiles(directory, filename)) {
                    if (re.IsMatch(filepath)) {
                        fileList.Add(Path.GetFullPath(filepath));
                        hasone = true;
                    }
                }
                if (!hasone)
                    throw new FileNotFoundException("File not found", arg);
            }
            if (working_dir_bak != null) {
                Directory.SetCurrentDirectory(working_dir_bak);
            }
            return fileList.ToArray();
        }

    }

}
