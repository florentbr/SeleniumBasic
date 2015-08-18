using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Selenium.Internal {

    /// <summary>
    /// Class to save and restor a directory recursively to and from a backup file.
    /// The created backup is a binary file not compressed.
    /// </summary>
    static class FolderCache {

        const int BUFFER_LENGTH = 1024 * 512;

        /// <summary>
        /// Saves a directory recursively to a backup file
        /// </summary>
        /// <param name="folder">Full path of the directory source</param>
        /// <param name="path">Full path of the backup target file</param>
        /// <param name="excludes">Paths to exclude. ex: *.tmp</param>
        public static unsafe void Save(string folder, string path, params string[] excludes) {
            string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            int subPathIndex = folder.Length + 1;

            IntPtr buffer = Marshal.AllocHGlobal(BUFFER_LENGTH);
            try {
                IntPtr fileOut = CreateFile(path);
                try {
                    foreach (var filepath in files) {
                        var relativePath = filepath.Substring(subPathIndex);
                        if (WildcardMatchAny(relativePath.ToLowerInvariant(), excludes))
                            continue;

                        IntPtr fileIn;
                        try {
                            fileIn = OpenFile(filepath);
                        } catch {
                            continue;
                        }

                        try {
                            int filesize = NativeMethods.GetFileSize(fileIn, IntPtr.Zero);

                            //set the length of the file name in the header
                            Marshal.WriteInt16(buffer, 0, (short)relativePath.Length);
                            //set the length of the data in the header
                            Marshal.WriteInt32(buffer, 2, filesize);
                            WriteBlock(fileOut, buffer, 6);

                            //set the filename in the header
                            IntPtr bufName = Marshal.StringToHGlobalUni(relativePath);
                            WriteBlock(fileOut, bufName, relativePath.Length * sizeof(char));
                            Marshal.FreeHGlobal(bufName);

                            Copy(fileIn, fileOut, filesize, buffer, BUFFER_LENGTH);
                        } finally {
                            NativeMethods.CloseHandle(fileIn);
                        }
                    }

                } finally {
                    NativeMethods.CloseHandle(fileOut);
                }
            } finally {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Restores a directory from a backup file
        /// </summary>
        /// <param name="bakpath">Full path of the backup source file</param>
        /// <param name="targetdir">Full path of the target directory</param>
        public static void Restore(string bakpath, string targetdir) {
            IntPtr buffer = Marshal.AllocHGlobal(BUFFER_LENGTH);
            try {
                //loads the binary file in the memory
                IntPtr fileIn = OpenFile(bakpath);
                try {
                    string prevdir = null;
                    //iterates all files
                    while (ReadBlock(fileIn, buffer, 6) == 6) {
                        //read the length of the file path / 2 bytes
                        int namelen = Marshal.ReadInt16(buffer, 0);
                        //read the data length / 4 bytes
                        int datalen = Marshal.ReadInt32(buffer, 2);

                        //read name
                        ReadBlock(fileIn, buffer, namelen * sizeof(char));
                        string relativePath = Marshal.PtrToStringUni(buffer, namelen);

                        //create the folder if not present
                        string filepath = Path.Combine(targetdir, relativePath);
                        string dirname = Path.GetDirectoryName(filepath);
                        if (prevdir != dirname)
                            Directory.CreateDirectory(prevdir = dirname);

                        //copy the data
                        IntPtr fileOut = CreateFile(filepath);
                        try {
                            Copy(fileIn, fileOut, datalen, buffer, BUFFER_LENGTH);
                        } finally {
                            NativeMethods.CloseHandle(fileOut);
                        }
                    }
                } finally {
                    NativeMethods.CloseHandle(fileIn);
                }
            } finally {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static IntPtr OpenFile(string path) {
            IntPtr handle = NativeMethods.CreateFile(path, NativeMethods.GENERIC_READ
                , NativeMethods.FILE_SHARE_READWRITE, 0, NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero);
            if (handle == IntPtr.Zero)
                throw new Win32Exception();
            return handle;
        }

        private static IntPtr CreateFile(string path) {
            IntPtr handle = NativeMethods.CreateFile(path, NativeMethods.GENERIC_WRITE
                , 0, 0, NativeMethods.CREATE_ALWAYS, 0, IntPtr.Zero);
            if (handle == IntPtr.Zero)
                throw new Win32Exception();
            return handle;
        }

        private static int ReadBlock(IntPtr fileHandle, IntPtr buffer, int count) {
            int read;
            if (!NativeMethods.ReadFile(fileHandle, buffer, count, out read, IntPtr.Zero))
                throw new Win32Exception();
            return read;
        }

        private static int WriteBlock(IntPtr fileHandle, IntPtr buffer, int count) {
            int read;
            if (!NativeMethods.WriteFile(fileHandle, buffer, count, out read, IntPtr.Zero))
                throw new Win32Exception();
            return read;
        }

        private static void Copy(IntPtr fileSource, IntPtr fileTarget, int count, IntPtr buffer, int bufferSize) {
            int read = Math.Min(count, bufferSize);
            int wrote;
            while (count > 0) {
                if (!NativeMethods.ReadFile(fileSource, buffer, read, out read, IntPtr.Zero))
                    throw new Win32Exception();
                if (read == 0)
                    break;
                if (!NativeMethods.WriteFile(fileTarget, buffer, read, out wrote, IntPtr.Zero) || wrote == 0)
                    throw new Win32Exception();
                if ((count -= read) < read)
                    read = count;
            };
        }

        private unsafe static bool WildcardMatchAny(string text, string[] patterns) {
            fixed (char* txt = text) {
                foreach (string pattern in patterns) {
                    fixed (char* pat = pattern) {
                        if (WildcardMatch(txt, text.Length - 1, pat, pattern.Length - 1))
                            return true;
                    }
                }
            }
            return false;
        }

        private static unsafe bool WildcardMatch(char* txt, int itxt, char* pat, int ipat) {
            while (true) {
                if (ipat < 0)
                    return itxt < 0 || pat[0] == '*';
                if (itxt < 0)
                    return ipat == 0 && pat[ipat] == '*';
                if (pat[ipat] == '*') {
                    return WildcardMatch(txt, itxt, pat, ipat - 1)
                        || WildcardMatch(txt, itxt - 1, pat, ipat);
                }
                if (pat[ipat] != '?' && txt[itxt] != pat[ipat])
                    return false;
                itxt--;
                ipat--;
            }
        }


        class NativeMethods {

            const string KERNEL32 = "kernel32.dll";

            public const uint GENERIC_READ = 0x80000000;
            public const uint GENERIC_WRITE = 0x40000000;
            public const uint FILE_SHARE_READWRITE = 0x00000003;
            public const uint OPEN_EXISTING = 3;
            public const uint CREATE_ALWAYS = 2;

            [DllImport(KERNEL32, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CreateFile(string FileName, uint DesiredAccess
                , uint ShareMode, uint SecurityAttributes, uint CreationDisposition
                , uint FlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport(KERNEL32, SetLastError = true)]
            public static extern bool ReadFile(IntPtr handle, IntPtr bytes
                , int numBytesToRead, out int numBytesRead, IntPtr mustBeZero);

            [DllImport(KERNEL32, SetLastError = true)]
            public static extern bool WriteFile(IntPtr handle, IntPtr bytes
                , int numBytesToWrite, out int numBytesWritten, IntPtr mustBeZero);

            [DllImport(KERNEL32, SetLastError = true)]
            public static extern int GetFileSize(IntPtr hFile, IntPtr highSize);

            [DllImport(KERNEL32, SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hObject);

        }

    }

}
