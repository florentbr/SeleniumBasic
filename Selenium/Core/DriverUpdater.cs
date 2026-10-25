using Selenium.Internal;
using Selenium.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Selenium.Core {

    /// <summary>
    /// Downloads the Chrome or Edge driver matching the major version of the installed browser.
    /// Disabled when the environment variable SELENIUMBASIC_DRIVER_UPDATE is set to 0.
    /// </summary>
    static class DriverUpdater {

        const string ENV_DISABLE = "SELENIUMBASIC_DRIVER_UPDATE";
        const string MUTEX_NAME = @"Local\SeleniumBasic.DriverUpdate";
        const int REQUEST_TIMEOUT = 30000;
        const int MUTEX_TIMEOUT = 120000;

        //Tls12 is not defined in the .Net 3.5 enumeration
        const SecurityProtocolType TLS12 = (SecurityProtocolType)3072;

        class Source {
            public string Driver;
            public string Browser;
            public string[] BrowserPaths;
            public string VersionUrl;
            public string ZipUrl;
            public Encoding VersionEncoding;
        }

        static readonly Source[] SOURCES = {
            new Source {
                Driver = "chromedriver.exe",
                Browser = "chrome.exe",
                BrowserPaths = new[] { @"Google\Chrome\Application\chrome.exe" },
                VersionUrl = "https://googlechromelabs.github.io/chrome-for-testing/LATEST_RELEASE_{0}",
                ZipUrl = "https://storage.googleapis.com/chrome-for-testing-public/{0}/{1}/chromedriver-{1}.zip",
                VersionEncoding = Encoding.ASCII
            },
            new Source {
                Driver = "msedgedriver.exe",
                Browser = "msedge.exe",
                BrowserPaths = new[] { @"Microsoft\Edge\Application\msedge.exe" },
                VersionUrl = "https://msedgedriver.microsoft.com/LATEST_RELEASE_{0}_WINDOWS",
                ZipUrl = "https://msedgedriver.microsoft.com/{0}/edgedriver_{1}.zip",
                VersionEncoding = Encoding.Unicode
            }
        };

        /// <summary>
        /// Returns the path of the driver to start, after updating it if its major version
        /// differs from the browser one. Never throws: the existing driver is kept on failure.
        /// </summary>
        /// <param name="directory">Directory of the driver</param>
        /// <param name="filename">Driver file name</param>
        /// <param name="browserBinary">Browser executable set by the user or null</param>
        /// <returns>Driver path</returns>
        public static string Resolve(string directory, string filename, string browserBinary) {
            string driverPath = Path.Combine(directory, filename);
            try {
                Source source = GetSource(filename);
                if (source == null || Environment.GetEnvironmentVariable(ENV_DISABLE) == "0")
                    return driverPath;

                string browserPath = browserBinary != null ?
                    IOExt.ExpandPath(browserBinary) : FindBrowser(source);
                int browserMajor = GetMajorVersion(browserPath);
                if (browserMajor == 0)
                    return driverPath;

                //the driver is updated in the temp folder when the directory is read-only
                string tempPath = Path.Combine(DriverService.GetTempFolder(), filename);

                using (var mutex = new Mutex(false, MUTEX_NAME)) {
                    bool owned = false;
                    try {
                        try {
                            owned = mutex.WaitOne(MUTEX_TIMEOUT, false);
                        } catch (AbandonedMutexException) {
                            owned = true;
                        }

                        //checked after acquiring the mutex since another process may have updated it
                        if (GetMajorVersion(driverPath) == browserMajor)
                            return driverPath;
                        if (GetMajorVersion(tempPath) == browserMajor)
                            return tempPath;
                        if (!owned)
                            return driverPath;

                        string extractDir;
                        string newDriver = Download(source, browserMajor, out extractDir);
                        try {
                            if (TryReplace(newDriver, driverPath))
                                return driverPath;
                            if (TryReplace(newDriver, tempPath))
                                return tempPath;
                        } finally {
                            DeleteDirectory(extractDir);
                        }
                    } finally {
                        if (owned)
                            mutex.ReleaseMutex();
                    }
                }
            } catch (Exception ex) {
                Trace.TraceWarning("Failed to update {0}: {1}", filename, ex.Message);
            }
            return driverPath;
        }

        private static Source GetSource(string filename) {
            foreach (Source source in SOURCES) {
                if (string.Equals(source.Driver, filename, StringComparison.OrdinalIgnoreCase))
                    return source;
            }
            return null;
        }

        private static string FindBrowser(Source source) {
            try {
                return IOExt.GetApplicationPath(source.Browser);
            } catch (SeleniumError) { }

            //a 32bits process reads the redirected registry, so the usual locations are also tried
            string[] roots = {
                Environment.GetEnvironmentVariable("ProgramW6432"),
                Environment.GetEnvironmentVariable("ProgramFiles"),
                Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
                Environment.GetEnvironmentVariable("LOCALAPPDATA")
            };
            foreach (string root in roots) {
                if (string.IsNullOrEmpty(root))
                    continue;
                foreach (string relative in source.BrowserPaths) {
                    string path = Path.Combine(root, relative);
                    if (File.Exists(path))
                        return path;
                }
            }
            return null;
        }

        private static int GetMajorVersion(string path) {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return 0;
            return FileVersionInfo.GetVersionInfo(path).FileMajorPart;
        }

        /// <summary>
        /// Downloads and extracts the driver in a new temporary directory.
        /// </summary>
        /// <param name="source">Driver source</param>
        /// <param name="major">Major version of the browser</param>
        /// <param name="directory">Extraction directory to delete once the driver is copied</param>
        /// <returns>Path of the extracted driver</returns>
        private static string Download(Source source, int major, out string directory) {
            directory = null;
            ServicePointManager.SecurityProtocol |= TLS12;

            string text = source.VersionEncoding.GetString(
                DownloadData(string.Format(source.VersionUrl, major)));
            var version = new StringBuilder();
            foreach (char c in text) {
                if (char.IsDigit(c) || c == '.')
                    version.Append(c);
            }
            if (version.Length == 0)
                throw new SeleniumError("No driver version available for {0} {1}.", source.Browser, major);

            string platform = IntPtr.Size == 8
                || Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432") != null ? "win64" : "win32";
            byte[] zip = DownloadData(string.Format(source.ZipUrl, version, platform));

            directory = Path.Combine(DriverService.GetTempFolder(), "update-" + IOExt.GetRandomName());
            try {
                using (var stream = new MemoryStream(zip))
                    ZipFile.ExtractAll(stream, directory);

                string[] files = Directory.GetFiles(directory, source.Driver, SearchOption.AllDirectories);
                if (files.Length == 0)
                    throw new SeleniumError("{0} not found in the downloaded archive.", source.Driver);
                return files[0];
            } catch {
                DeleteDirectory(directory);
                throw;
            }
        }

        private static byte[] DownloadData(string url) {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = REQUEST_TIMEOUT;
            request.ReadWriteTimeout = REQUEST_TIMEOUT;
            //the default proxy is removed for the local drivers, so the system one is used here
            request.Proxy = WebRequest.GetSystemWebProxy();
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var buffer = new MemoryStream()) {
                byte[] bytes = new byte[81920];
                int count;
                while ((count = stream.Read(bytes, 0, bytes.Length)) > 0)
                    buffer.Write(bytes, 0, count);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Replaces the driver. A running driver can't be overwritten but can be renamed,
        /// so the current one is moved to a .old file first.
        /// </summary>
        private static bool TryReplace(string source, string target) {
            string newPath = target + ".new";
            string oldPath = target + ".old";
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(source, newPath, true);
                TryDelete(oldPath);
                if (File.Exists(target)) {
                    if (File.Exists(oldPath))
                        return false;
                    File.Move(target, oldPath);
                }
                File.Move(newPath, target);
                TryDelete(oldPath);
                return true;
            } catch (UnauthorizedAccessException) {
            } catch (IOException) { }
            TryDelete(newPath);
            return false;
        }

        private static void TryDelete(string path) {
            try {
                if (File.Exists(path))
                    File.Delete(path);
            } catch (UnauthorizedAccessException) {
            } catch (IOException) { }
        }

        private static void DeleteDirectory(string directory) {
            try {
                Directory.Delete(directory, true);
            } catch (UnauthorizedAccessException) {
            } catch (IOException) { }
        }

    }

}
