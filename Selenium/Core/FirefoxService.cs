using Selenium.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Selenium.Core {

    class FirefoxService : IDriverService {

        const string APP_FILENAME = @"firefox.exe";
        const string APP_PROFILES_FOLDER = @"Mozilla\Firefox";
        const string PROFILE_CACHE_FILENAME = @"ff-profile.bin";
        const string PROFILE_PERSISTANT_FOLDER = @"ff-persistant";
        const string PROFILE_PREFIX_TEMP_FOLDER = @"ff-";
        const string EXT_WEBDRIVER_FILENAME = @"firefoxdriver.xpi";
        const string EXT_WEBDRIVER_RID = @"fxdriver@googlecode.com";
        const string PREFS_RESOURCE_FILENAME = @"Selenium.Core.FirefoxPrefs.js";

        public string Profile;
        public bool Persistant;
        public IEnumerable Arguments;
        public IEnumerable Extensions;
        public Selenium.Dictionary Preferences;
        public Selenium.Dictionary Capabilities;


        private readonly string _this_assembly_dir;
        private readonly string _working_dir;
        private readonly string _profil_cache_path;

        private string _firefox_path;
        private ProcessExt _firefox_process;
        private string _profile_dir;
        private Exception _exception;
        private EndPointExt _endpoint;

        public FirefoxService() {
            _this_assembly_dir = IOExt.GetAssemblyDirectory();
            _working_dir = DriverService.GetTempFolder();
            _profil_cache_path = Path.Combine(_working_dir, PROFILE_CACHE_FILENAME);
        }

        public void Dispose() {
            if (_endpoint != null) {
                _endpoint.Dispose();
                _endpoint = null;
            }

            if (_firefox_process != null) {
                if (!_firefox_process.HasExited) {
                    _firefox_process.Kill();
                    _firefox_process.WaitForExit();
                }
                _firefox_process.Dispose();
                _firefox_process = null;
            }

            if (_profile_dir != null && !this.Persistant && Directory.Exists(_profile_dir)) {
                IOExt.DeleteDirectoryByShell(_profile_dir);
                _profile_dir = null;
            }
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Quit() {
            this.Dispose();
        }

        public IPEndPoint IPEndPoint {
            get {
                return _endpoint.IPEndPoint;
            }
        }

        public string Uri {
            get {
                return "http://" + _endpoint.ToString() + "/hub";
            }
        }

        public void Start(IEnumerable arguments, Dictionary preferences, IEnumerable extensions, Dictionary capabilities, string profile, bool persistant) {
            this.Arguments = arguments;
            this.Preferences = preferences;
            this.Extensions = extensions;
            this.Capabilities = capabilities;
            this.Profile = profile;
            this.Persistant = persistant;

            string debugAddress;
            if (capabilities.TryGetValue("debuggerAddress", out debugAddress)) {
                _endpoint = EndPointExt.Parse(debugAddress);
                return;
            }

            _endpoint = EndPointExt.Create(IPAddress.Loopback, false);

            Thread thread = new Thread(RunStart);
            thread.Start();
            thread.Join();
            if (_exception != null)
                throw _exception;
        }

        private void RunStart() {
            try {
                //Gets the binary location of firefox
                _firefox_path = GetBinaryLocation(this.Capabilities);
                if (!File.Exists(_firefox_path))
                    throw new Errors.FileNotFoundError(_firefox_path);

                SetupProfile();

                //starts firefox with the given profile
                StartApplication(_firefox_path, this.Arguments, _profile_dir);

            } catch (SeleniumException ex) {
                _exception = ex;
            } catch (Exception ex) {
                _exception = new SeleniumException(ex);
            }
        }

        private void SetupProfile() {

            if (string.IsNullOrEmpty(this.Profile)) {
                //Gets a profile
                if (this.Persistant) {
                    _profile_dir = Path.Combine(_working_dir, PROFILE_PERSISTANT_FOLDER); ;
                } else {
                    //create temp profile
                    _profile_dir = Path.Combine(_working_dir, PROFILE_PREFIX_TEMP_FOLDER + IOExt.GetRandomName());
                }
            } else {
                //Use an existing profile or create one
                if (IOExt.IsPath(this.Profile)) {
                    //if profile by path
                    _profile_dir = IOExt.ExpandPath(this.Profile);
                } else {
                    //if profile by name
                    string appdata_dir;
                    if (!this.Capabilities.TryGetValue("firefox_appdata", out appdata_dir))
                        appdata_dir = Path.Combine(IOExt.AppDataFolder, APP_PROFILES_FOLDER);

                    if (!Directory.Exists(appdata_dir))
                        throw new SeleniumError("Firefox profile folder is missing: {0}", appdata_dir);

                    Dictionary profiles = ParseExistingProfiles(appdata_dir);
                    if (!profiles.TryGetValue(this.Profile, out _profile_dir))
                        CreateProfile(_firefox_path, appdata_dir, this.Profile, out _profile_dir);
                }
                if (!this.Persistant) {
                    //create the temporary profile
                    _profile_dir = Path.Combine(_working_dir, PROFILE_PREFIX_TEMP_FOLDER + IOExt.GetRandomName());
                    IOExt.Copy(new DirectoryInfo(_profile_dir), new DirectoryInfo(_profile_dir), "parent.lock");
                }
            }

            Directory.CreateDirectory(_profile_dir);

            //Read existing prefs if any
            Dictionary prefs = new Dictionary();
            string prefs_path = Path.Combine(_profile_dir, "user.js");
            if (File.Exists(prefs_path)) {
                using (var stream = File.Open(prefs_path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    CopyProfilePrefs(stream, ref prefs);
            }

            //adds webdriver prefs
            prefs["webdriver_firefox_port"] = this.IPEndPoint.Port;
            using (var stream = typeof(FirefoxService).Assembly.GetManifestResourceStream(PREFS_RESOURCE_FILENAME))
                CopyProfilePrefs(stream, ref prefs);

            //adds proxy to prefs if any
            Dictionary proxy;
            if (this.Capabilities.TryGetValue("proxy", out proxy) && proxy.Count > 0)
                CopyProxyPrefs(proxy, ref prefs);

            //adds custom prefs if any
            foreach (DictionaryItem item in this.Preferences)
                prefs[item.Key] = item.Value;

            //writes the new prefs
            SaveProfilePrefs(prefs, prefs_path);

            //Install the WebDriver extention
            string ext_wd_path = Path.Combine(_this_assembly_dir, EXT_WEBDRIVER_FILENAME);
            InstallExtension(ext_wd_path, _profile_dir, EXT_WEBDRIVER_RID);
            
            //Install the provided extensions
            if (this.Extensions != null && !this.Persistant) {
                foreach (string ext_path in this.Extensions)
                    InstallExtension(ext_path, _profile_dir);
            }

            //delete logs
            foreach (string file in Directory.GetFiles(_profile_dir, "*.txt"))
                File.Delete(file);
        }

        public void StartApplication(string firefoxPath, IEnumerable arguments, string profilePath) {
            Hashtable env = ProcessExt.GetStdEnvironmentVariables();
            env["TEMP"] = _working_dir;
            env["TMP"] = _working_dir;
            env["XRE_PROFILE_PATH"] = profilePath;
            env["MOZ_NO_REMOTE"] = "1";
            env["MOZ_CRASHREPORTER_DISABLE"] = "1";
            env["NO_EM_RESTART"] = "1";

            //start the process
            _firefox_process = ProcessExt.Start(firefoxPath, arguments, null, env, false, true);
            Thread.Sleep(500);

            //Waits for the port to be listening
            SysWaiter.Wait(100);
            if (!_endpoint.WaitForListening(15000, 100))
                throw new Errors.TimeoutError("Firefox failed to open the listening port {0} within 15s", _endpoint);
        }



        static string GetBinaryLocation(Dictionary capabilities) {
            string path;
            if (!capabilities.TryGetValue("firefox_binary", out path))
                path = IOExt.GetApplicationPath(APP_FILENAME);

            if (!File.Exists(path))
                throw new Errors.FileNotFoundError(path);

            return path;
        }

        static void CreateProfile(string binary_path, string appdata_dir, string profile_name, out string profile_dir) {
            Hashtable env = ProcessExt.GetStdEnvironmentVariables();
            env["MOZ_NO_REMOTE"] = "1";

            string[] args = { "-CreateProfile", profile_name };

            using (var p = ProcessExt.Start(binary_path, args, null, env, false, false)) {
                if (!p.WaitForExit(10000))
                    throw new Errors.TimeoutError("Failed to create the profile. The process didn't exit within 10s.");
            }

            var profiles = ParseExistingProfiles(appdata_dir);
            if (!profiles.TryGetValue(profile_name, out profile_dir))
                throw new SeleniumError("Failed to create the profile: {0}", profile_name);
        }

        static void CopyProxyPrefs(Dictionary proxy, ref Dictionary prefs) {
            var TKEY = "network.proxy.type";
            switch ((string)proxy.Get("proxyType", null)) {
                case "direct": prefs[TKEY] = 0; break;
                case "manual": prefs[TKEY] = 1; break;
                case "pac": prefs[TKEY] = 2; break;
                case "autodetect": prefs[TKEY] = 4; break;
                case "system": prefs[TKEY] = 5; break;
            }
            if (proxy.ContainsKey("proxyAutoconfigUrl"))
                prefs["network.proxy.autoconfig_url"] = proxy["proxyAutoconfigUrl"];
            SetProxyCapability(proxy, "ftpProxy", prefs, "network.proxy.ftp", "network.proxy.ftp_port");
            SetProxyCapability(proxy, "httpProxy", prefs, "network.proxy.http", "network.proxy.http_port");
            SetProxyCapability(proxy, "sslProxy", prefs, "network.proxy.ssl", "network.proxy.ssl_port");
            SetProxyCapability(proxy, "socksProxy", prefs, "network.proxy.socks", "network.proxy.socks_port");
        }

        static void SetProxyCapability(Dictionary proxy, string proxyKey, Dictionary prefs, string prefKey0, string prefKey1) {
            object value;
            if (proxy.TryGetValue(proxyKey, out value) && value != null) {
                string[] items = value.ToString().Split(':');
                if (items.Length > 0) {
                    prefs[prefKey0] = items[0];
                    if (items.Length > 1)
                        prefs[prefKey1] = items[1];
                }
            }
        }

        /// <summary>
        /// Extract the name and path for each profile define in the provided INI file
        /// </summary>
        /// <param name="profiles_dir">Full path of the INI file (ApplicationData\Mozilla\Firefox\profiles.ini)</param>
        /// <returns>Dictionary Key:name Value:path</returns>
        static Dictionary ParseExistingProfiles(string profiles_dir) {
            Dictionary profiles = new Dictionary();
            var inifile = IniFile.Load(profiles_dir + @"\profiles.ini");
            foreach (var section in inifile) {
                if (!section.Key.StartsWith(@"Profile", StringComparison.OrdinalIgnoreCase))
                    continue;

                var properties = section.Value;
                string path = properties["Path"].Replace('/', '\\');
                if (properties["IsRelative"] == "1")
                    path = Path.Combine(profiles_dir, path);
                profiles.Add(properties["Name"], path);
            }
            return profiles;
        }

        static void SaveProfilePrefs(Dictionary preferences, string file_path) {
            var buffer = new StringBuilder(5000);
            foreach (DictionaryItem item in preferences) {
                object value = item.Value;
                buffer.Append(@"user_pref(""").Append(item.Key);
                if (value is string) {
                    buffer.Append(@""", """);
                    buffer.Append(((string)value).Replace(@"\", @"\\"));
                    buffer.AppendLine(@""");");
                } else {
                    buffer.Append(@""", ");
                    if (value is bool) {
                        buffer.Append((bool)value ? "true" : "false");
                    } else {
                        buffer.Append(value);
                    }
                    buffer.AppendLine(@");");
                }
            }
            using (var file = new StreamWriter(file_path, false, Encoding.ASCII)) {
                file.Write(buffer);
            }
        }

        /// <summary>
        /// Extract all the user_pref from a firefox file to a dictionary
        /// </summary>
        /// <returns>A <see cref="Dictionary{K, V}"/>containing key-value pairs representing the preferences.</returns>
        /// <remarks>Assumes that we only really care about the preferences, not the comments</remarks>
        static void CopyProfilePrefs(Stream file_stream, ref Dictionary prefs) {
            var reader = new StreamReader(file_stream, Encoding.ASCII, false);
            while (!reader.EndOfStream) {
                string line = reader.ReadLine();
                if (line.StartsWith("user_pref", StringComparison.OrdinalIgnoreCase)) {
                    //extract key
                    int istartKey = line.IndexOf('"') + 1;
                    int iendKey = line.IndexOf('"', istartKey);
                    string prefKey = line.Substring(istartKey, iendKey - istartKey);

                    //extract value
                    int istartVal = line.IndexOf(',', iendKey) + 1;
                    int iendVal = line.LastIndexOf(')');
                    string valueStr = line.Substring(istartVal, iendVal - istartVal).Trim();

                    //parse value
                    object prefValue;
                    if ("true".Equals(valueStr, StringComparison.OrdinalIgnoreCase)) {
                        prefValue = true;
                    } else if ("false".Equals(valueStr, StringComparison.OrdinalIgnoreCase)) {
                        prefValue = false;
                    } else if (valueStr.Length == 0 || valueStr[0] == '"') {
                        prefValue = valueStr.Trim('"');
                    } else if ("null".Equals(valueStr, StringComparison.OrdinalIgnoreCase)) {
                        prefValue = null;
                    } else {
                        prefValue = int.Parse(valueStr, CultureInfo.InvariantCulture);
                    }

                    //Update dictonary
                    prefs.Set(prefKey, prefValue);
                }
            }
        }

        /// <summary>
        /// Install an extention in a profile.
        /// </summary>
        /// <param name="xpi_path">Full path of the extention (.xpi file)</param>
        /// <param name="profile_dir">Full path of the profile folder</param>
        /// <param name="xpi_id">Extension id. If not provided, the id is extracted from the install.rdf file</param>
        static void InstallExtension(string xpi_path, string profile_dir, string xpi_id = null) {
            var xpifile = new FileInfo(xpi_path);
            if (!xpifile.Exists)
                throw new Errors.ArgumentError("Extension not present at {0}", xpifile.FullName);

            string extensions_dir = profile_dir + @"\extensions\";
            if (string.IsNullOrEmpty(xpi_id)) {
                //Extract the xpi to a temporary folder
                string tmp_dir = extensions_dir + Path.GetFileNameWithoutExtension(xpi_path);
                Zip.ZipFile.ExtractAll(xpifile.FullName, tmp_dir);

                //Read the xpi extension id
                string txt = File.ReadAllText(tmp_dir + @"\install.rdf");
                xpi_id = GetXmlTagInnerText(txt, @"<em:id>", @"</em:id>").Trim();

                //Rename the temporary folder with the xpi id
                Directory.Move(tmp_dir, extensions_dir + xpi_id);
            } else {
                var extdir = new DirectoryInfo(extensions_dir + xpi_id);
                if (extdir.Exists) {
                    //Skip installation if the extension is already present and up-to-date
                    if (extdir.CreationTimeUtc > xpifile.LastWriteTimeUtc)
                        return;
                    extdir.Delete(true);
                }
                Zip.ZipFile.ExtractAll(xpifile.FullName, extdir.FullName);
            }
        }

        static string GetXmlTagInnerText(string text, string tagLeft, string tagRight) {
            int istart = text.IndexOf(tagLeft, StringComparison.OrdinalIgnoreCase) + tagLeft.Length;
            if (istart == -1)
                throw new SeleniumException("Tag not found in the provided text: " + tagLeft);

            int iend = text.IndexOf(tagRight, istart, StringComparison.OrdinalIgnoreCase);
            if (iend == -1)
                throw new SeleniumException("Closing tag not found in the provided text: " + tagRight);

            return text.Substring(istart, iend - istart);
        }

    }

}
