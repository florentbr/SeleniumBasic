using Microsoft.Win32;
using Selenium.Core;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Selenium.Internal {

    class IOExt {

        /// <summary>
        /// Generate a random name with base16 characters
        /// Range: "0" to "ffffffff"
        /// </summary>
        /// <returns></returns>
        public static string GetRandomName() {
            byte[] data = new byte[8];
            new RNGCryptoServiceProvider().GetBytes(data);
            uint value = BitConverter.ToUInt32(data, 0);
            return Convert.ToString(value, 16);
        }

        /// <summary>
        /// Returns the directory that serves as a common repository for application-specific
        //  data for the current roaming user.
        /// </summary>
        public static string AppDataFolder {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        public static bool IsPath(string txt) {
            return txt.IndexOf('\\') != -1 || txt.IndexOf('/') != -1;
        }

        public static string ExpandPath(string path) {
            path = path.Trim(' ');
            path = path.Trim('"');
            path = path.TrimEnd('\\', '/');
            if (path.IndexOf('%') != -1)
                path = Environment.ExpandEnvironmentVariables(path);
            return path;
        }

        /// <summary>
        /// Copies a source directory to a destination one.
        /// </summary>
        /// <param name="sourceDir">Source direcory</param>
        /// <param name="destDir">Destination directory</param>
        /// <param name="excludes">File names to exclude</param>
        public static void Copy(DirectoryInfo sourceDir, DirectoryInfo destDir, params string[] excludes) {
            destDir.Create();
            foreach (FileInfo file in sourceDir.GetFiles()) {
                var index = Array.IndexOf(excludes, file.Name);
                if (index == -1)
                    file.CopyTo(Path.Combine(destDir.FullName, file.Name), false);
            }
            foreach (DirectoryInfo subdir in sourceDir.GetDirectories())
                Copy(subdir, new DirectoryInfo(Path.Combine(destDir.FullName, subdir.Name)), excludes);
        }

        /// <summary>
        /// Returns the location of an application from the registry.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns>Application path</returns>
        public static string GetApplicationPath(string appName) {
            string subkey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + appName;
            using (var key = Registry.LocalMachine.OpenSubKey(subkey)) {
                if (key != null) {
                    string path = (string)key.GetValue(null);
                    if (!string.IsNullOrEmpty(path)) {
                        path = path.Trim('"', ' ');
                        if (File.Exists(path))
                            return path;
                    }
                }
            }
            throw new SeleniumError("Failed to locate {0} in the registry.", appName);
        }

        /// <summary>
        /// Returns the directory of this assembly.
        /// </summary>
        /// <returns>Directory path</returns>
        public static string GetAssemblyDirectory() {
            var assembly = typeof(IOExt).Assembly;
            string path = AppDomain.CurrentDomain.ShadowCopyFiles ?
                new Uri(assembly.CodeBase).LocalPath : assembly.Location;
            string folder = Path.GetDirectoryName(path);
            return folder;
        }

        /// <summary>
        /// Deletes a directory recursively by using a shell command.
        /// </summary>
        /// <param name="directory">Directory path</param>
        public static void DeleteDirectoryByShell(string directory) {
            if (!Directory.Exists(directory))
                return;

            Hashtable env = new Hashtable();
            env["PATH"] = Environment.GetFolderPath(Environment.SpecialFolder.System);

            var cmd = "cmd.exe /C RD /S /Q \"" + directory + "\"";
            ProcessExt.Execute(cmd, null, env, true);
        }

        /// <summary>
        /// Deletes a directory recusively. Retries 5 times every 200ms when failing.
        /// </summary>
        /// <param name="directory">Directory path</param>
        public static void DeleteDirectory(string directory) {
            if (!Directory.Exists(directory))
                return;
            for (int count = 5; count-- > 0; ) {
                try {
                    Directory.Delete(directory, true);
                    return;
                } catch (IOException) {
                } catch (UnauthorizedAccessException) { }
                SysWaiter.Wait(200);
            }
        }

    }

}
