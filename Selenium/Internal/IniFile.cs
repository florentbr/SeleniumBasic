using System.Collections.Generic;
using System.IO;

namespace Selenium.Internal {

    class IniFile {

        /// <summary>
        /// Parse a Windows ini file without marshaling the values.
        /// Multiline values are not supported
        /// Incorrect sections or properties are silently skipped
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>IniFile</returns>
        public static Dictionary<string, Dictionary<string, string>> Load(string filePath) {
            var sections = new Dictionary<string, Dictionary<string, string>>();
            using (var reader = new StreamReader(filePath)) {
                Dictionary<string, string> section = null;
                string section_key = null;
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine().Trim();
                    if (line.Length == 0 || line[0] == ';')
                        continue;

                    if (line[0] == '[' && line[line.Length - 1] == ']') {
                        if (section != null)
                            sections.Add(section_key, section);
                        section = new Dictionary<string, string>();
                        section_key = line.Substring(1, line.Length - 2);
                    } else if (section != null) {
                        int i = line.IndexOf('=');
                        if (i > 0) {
                            string prop_key = line.Substring(0, i).Trim();
                            if (++i < line.Length) {
                                section[prop_key] = line.Substring(i).Trim();
                            } else {
                                section[prop_key] = string.Empty;
                            }
                        }
                    }
                }
                if (section != null)
                    sections.Add(section_key, section);
            }
            return sections;
        }

    }

}
