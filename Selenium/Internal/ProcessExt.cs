using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium.Internal {

    public class ProcessExt : IDisposable {
        private static readonly NLog.Logger _l = NLog.LogManager.GetCurrentClassLogger();

        #region Standard Environemnt Variables

        static readonly string[] STD_ENV_VARS = {
            "ALLUSERSPROFILE","APPDATA","COMPUTERNAME","ComSpec","CommonProgramFiles",
            "CommonProgramFiles(x86)","CommonProgramW6432","HOMEDRIVE","HOMEPATH",
            "LOCALAPPDATA","LOGONSERVER","NUMBER_OF_PROCESSORS","OS","PATHEXT",
            "PROCESSOR_ARCHITECTURE","PROCESSOR_ARCHITEW6432","PROCESSOR_IDENTIFIER",
            "PROCESSOR_LEVEL","PROCESSOR_REVISION","PUBLIC","ProgramData","ProgramFiles",
            "ProgramFiles(x86)","ProgramW6432","SystemDrive","SystemRoot","TEMP","TMP",
            "USERDOMAIN","USERDOMAIN_ROAMINGPROFILE","USERNAME","USERPROFILE","WINDIR",
            "JAVA_HOME"
        };

        #endregion


        /// <summary>
        /// Starts a process
        /// </summary>
        /// <param name="filepath">File path</param>
        /// <param name="args">Arguments</param>
        /// <param name="dir">Working dir. Inherits the current directory if null</param>
        /// <param name="env">Environement variables. Inherits all of them if null</param>
        /// <param name="noWindow">Hides the window if true</param>
        /// <param name="createJob">Creates a Job if true</param>
        /// <returns></returns>
        public static ProcessExt Start(string filepath, IEnumerable args, string dir, Hashtable env, bool noWindow, bool createJob) {
            bool dl = _l.IsDebugEnabled;
            string cmd = ProcessExt.BuildCommandLine(filepath, args);
            StringBuilder envVars = BuildEnvironmentVars(env);
            
            if( dl ) _l.Debug( "Starting command: " + cmd );
            var si = new Native.STARTUPINFO();
            var pi = new Native.PROCESS_INFORMATION();

            int createFlags = Native.CREATE_UNICODE_ENVIRONMENT;
            if (noWindow)
                createFlags |= Native.CREATE_NO_WINDOW;

            IntPtr hJob = IntPtr.Zero;
            bool success = false;
            if (createJob) {
                IntPtr curProc = Native.GetCurrentProcess();

                bool isProcessInJob = false;
                success = Native.IsProcessInJob(curProc, IntPtr.Zero, out isProcessInJob);

                Native.CloseHandle(curProc);
                if (success) {
                    int createFlagsJob = createFlags | Native.CREATE_SUSPENDED;
                    if (isProcessInJob)
                        createFlagsJob |= Native.CREATE_BREAKAWAY_FROM_JOB;
                    if( dl ) _l.Debug( "Creating process" );
                    success = Native.CreateProcess(null
                        , cmd
                        , IntPtr.Zero
                        , IntPtr.Zero
                        , false
                        , createFlagsJob
                        , envVars
                        , dir, si, pi);

                    if (success) {
                        if( dl ) _l.Debug( "Assigning process to new job" );
                        success = AssignProcessToNewJob(pi.hProcess, null, out hJob);
                        if (success) {
                            if (-1 == Native.ResumeThread(pi.hThread))
                                throw new Win32Exception();
                        } else {
                            Native.TerminateProcess(pi.hProcess, -1);
                            Native.CloseHandle(pi.hProcess);
                            Native.CloseHandle(pi.hThread);
                            Native.CloseHandle(hJob);
                            hJob = IntPtr.Zero;
                        }
                    }
                }
            }

            if (!success) {
                if( dl ) _l.Debug( "Failed. Creating process again." );
                success = Native.CreateProcess(null
                    , cmd
                    , IntPtr.Zero
                    , IntPtr.Zero
                    , false
                    , createFlags
                    , envVars
                    , dir, si, pi);

                if (!success)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot start: " + cmd);
            }
            if( dl ) _l.Debug( CultureInfo.CurrentCulture, "Process {0} created", pi.dwProcessId );
            return new ProcessExt(pi.dwProcessId, pi.hProcess, hJob);
        }

        /// <summary>
        /// Execute a command line.
        /// </summary>
        /// <param name="cmd">Command line</param>
        /// <param name="dir">Working dir. Inherits the current directory if null.</param>
        /// <param name="env">Environement variables. Inherits all of them if null.</param>
        /// <param name="noWindow">Hides the window if true</param>
        public static void Execute(string cmd, string dir
            , Hashtable env = null, bool noWindow = false) {

            var si = new Native.STARTUPINFO();
            var pi = new Native.PROCESS_INFORMATION();

            int createFlags = Native.CREATE_UNICODE_ENVIRONMENT;
            if (noWindow)
                createFlags |= Native.CREATE_NO_WINDOW;

            StringBuilder envVars = BuildEnvironmentVars(env);

            try {
                bool success = Native.CreateProcess(null
                    , cmd
                    , IntPtr.Zero
                    , IntPtr.Zero
                    , false
                    , createFlags
                    , envVars
                    , dir, si, pi);

                if (!success)
                    throw new Win32Exception();
            } finally {
                Native.CloseHandle(pi.hThread);
                Native.CloseHandle(pi.hProcess);
            }
        }

        /// <summary>
        /// Returns all the standards environement variables.
        /// </summary>
        /// <returns></returns>
        public static Hashtable GetStdEnvironmentVariables() {
            IDictionary dict = Environment.GetEnvironmentVariables();
            Hashtable newDict = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            foreach (string key in STD_ENV_VARS) {
                object value = dict[key];
                if (value != null)
                    newDict.Add(key, (string)value);
            }
            return newDict;
        }


        [DebuggerDisplay("Pid:{Id} ParentPid:{ParentId}")]
        public class ProcessRelationship {
            public int Pid;
            public int ParentPid;
        }

        /// <summary>
        /// Returns all the process relationships.
        /// </summary>
        /// <returns></returns>
        public static unsafe ProcessRelationship[] GetProcessRelationships() {
            var relationchips = new List<ProcessRelationship>(100);

            IntPtr snapshot = Native.CreateToolhelp32Snapshot(Native.TH32CS_SNAPPROCESS, 0);

            int bufferSize = sizeof(Native.WinProcessEntry);
            IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
            Marshal.WriteInt32(buffer, bufferSize);

            var natEntryPtr = (Native.WinProcessEntry*)buffer;
            try {
                if (!Native.Process32First(snapshot, buffer))
                    throw new Win32Exception();

                do {
                    Native.WinProcessEntry natEntry = *natEntryPtr;

                    ProcessRelationship relationchip = new ProcessRelationship();
                    relationchip.Pid = natEntry.th32ProcessID;
                    relationchip.ParentPid = natEntry.th32ParentProcessID;

                    relationchips.Add(relationchip);
                } while (Native.Process32Next(snapshot, buffer));
            } finally {
                Marshal.FreeHGlobal(buffer);
                Native.CloseHandle(snapshot);
            }
            return relationchips.ToArray();
        }


        #region Instance

        private int _pid;
        private IntPtr _hJob;
        private IntPtr _hProcess;

        // Summary:
        //     Initializes a new instance of the System.Diagnostics.Process class.
        private ProcessExt(int pid, IntPtr hProcess, IntPtr hJob) {
            _pid = pid;
            _hJob = hJob;
            _hProcess = hProcess;
        }

        /// <summary>
        /// Release the resources
        /// </summary>
        public void Dispose() {
            if (_hJob != IntPtr.Zero) {
                Native.CloseHandle(_hJob);
                _hJob = IntPtr.Zero;
            }
            if (_hProcess != IntPtr.Zero) {
                Native.CloseHandle(_hProcess);
                _hProcess = IntPtr.Zero;
            }
            _pid = 0;
        }

        /// <summary>
        /// Process id
        /// </summary>
        public int Id {
            get {
                return _pid;
            }
        }

        /// <summary>
        /// Immediately stops the associated process and children processes.
        /// </summary>
        public void Kill() {
            if (_hProcess == IntPtr.Zero)
                return;

            if (_hJob == IntPtr.Zero || !Native.TerminateJobObject(_hJob, -1)) {
                ProcessRelationship[] relationships = GetProcessRelationships();
                TerminateProcessTree(_pid, relationships);
            }
        }

        /// <summary>
        /// Waits the specified number of milliseconds for the associated process to exit.
        /// </summary>
        /// <param name="milliseconds">Amount of time, in milliseconds</param>
        /// <returns>True if the associated process has exited, false otherwise</returns>
        public bool WaitForExit(int milliseconds = -1) {
            if (_hProcess == IntPtr.Zero)
                return true;

            int ret = Native.WaitForSingleObject(_hProcess, milliseconds);
            if (ret == unchecked((int)0xFFFFFFFF))  // == WAIT_FAILED
                throw new Win32Exception();

            if (ret == 0x00000102L) // == WAIT_TIMEOUT
                return false;

            return true;
        }

        public int GetExitCode() {
            if (_hProcess == IntPtr.Zero)
                return 0;

            int code;
            if (!Native.GetExitCodeProcess(_hProcess, out code))
                throw new Win32Exception();

            return code;
        }

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        public bool HasExited {
            get {
                return this.WaitForExit(0);
            }
        }

        #endregion


        #region Support


        private static string BuildCommandLine(string filepath, IEnumerable args) {
            var cmd = new StringBuilder();
            cmd.Append('"').Append(filepath.Trim('"')).Append('"');
            foreach (string arg in args)
                cmd.Append(' ').Append(arg);
            return cmd.ToString();
        }

        /// <summary>
        /// Assign a process to a new Job.
        /// </summary>
        /// <param name="hProcess">Process handle</param>
        /// <param name="name">Optional - Job name</param>
        /// <returns>Job handle or IntPtr.Zero if failed</returns>
        private static bool AssignProcessToNewJob(IntPtr hProcess, string name, out IntPtr hJob) {

            //Create a new Job
            hJob = Native.CreateJobObject(IntPtr.Zero, name);
            if (hJob == IntPtr.Zero)
                return false;

            var jeli = new Native.JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
            jeli.BasicLimitInformation.LimitFlags = Native.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;
            jeli.BasicLimitInformation.LimitFlags |= Native.JOB_OBJECT_LIMIT_BREAKAWAY_OK;

            bool success = Native.SetInformationJobObject(hJob
                , Native.JOB_EXTENDED_LIMIT_INFORMATION
                , ref jeli
                , Native.JOBOBJECT_EXTENDED_LIMIT_INFORMATION.SIZE);

            //Assign the process to the Job
            if (!success || !Native.AssignProcessToJobObject(hJob, hProcess)) {
                Native.CloseHandle(hJob);
                hJob = IntPtr.Zero;
                return false;
            }

            return true;
        }

        private static StringBuilder BuildEnvironmentVars(Hashtable dict) {
            if (dict == null)
                return null;

            string[] keys = new string[dict.Count];
            string[] values = new string[dict.Count];
            dict.Keys.CopyTo(keys, 0);
            dict.Values.CopyTo(values, 0);

            Array.Sort(keys, values, CaseInsensitiveComparer.Default);

            StringBuilder sb = new StringBuilder(800);
            for (int i = 0; i < dict.Count; ++i) {
                sb.Append(keys[i]);
                sb.Append('=');
                sb.Append(values[i]);
                sb.Append('\0');
            }

            return sb;
        }

        private static void TerminateProcessTree(int pid, ProcessRelationship[] relationchips) {
            if (pid == 0)
                return;

            //terminate childs recursively
            for (int i = relationchips.Length; i-- > 0; ) {
                ProcessRelationship relationchip = relationchips[i];
                if (relationchip.ParentPid == pid) {
                    relationchip.ParentPid = 0;
                    TerminateProcessTree(relationchip.Pid, relationchips);
                }
            }

            //terminate the process
            IntPtr procHandle = Native.OpenProcess(Native.PROCESS_ALL_ACCESS, false, pid);
            if (procHandle != IntPtr.Zero) {
                try {
                    Native.TerminateProcess(procHandle, -1);
                } catch {

                } finally {
                    Native.CloseHandle(procHandle);
                }
            }
        }


        class Native {

            const string KERNEL32 = "kernel32.dll";
            const string USER32 = "user32.dll";

            public const int JOB_OBJECT_LIMIT_BREAKAWAY_OK = 0x00000800;
            public const int JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000;
            public const int JOB_EXTENDED_LIMIT_INFORMATION = 9;

            public const int CREATE_SUSPENDED = 0x00000004;
            public const int CREATE_NO_WINDOW = 0x08000000;
            public const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
            public const int CREATE_BREAKAWAY_FROM_JOB = 0x01000000;

            public const int PROCESS_ALL_ACCESS = 0x001f0fff;
            public const int TH32CS_SNAPPROCESS = 0x00000002;

            public const int STARTF_USESHOWWINDOW = 0x00000001;
            public const short SW_SHOWNOACTIVATE = 0x00000004;


            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public unsafe struct WinProcessEntry {
                public int dwSize;
                public int cntUsage;
                public int th32ProcessID;
                public IntPtr th32DefaultHeapID;
                public int th32ModuleID;
                public int cntThreads;
                public int th32ParentProcessID;
                public int pcPriClassBase;
                public int dwFlags;
                public fixed byte fileName[260];
            }

            [StructLayout(LayoutKind.Sequential)]
            public class STARTUPINFO {
                public int cb = Marshal.SizeOf(typeof(STARTUPINFO));
                public IntPtr lpReserved = IntPtr.Zero;
                public IntPtr lpDesktop = IntPtr.Zero;
                public IntPtr lpTitle = IntPtr.Zero;
                public int dwX = 0;
                public int dwY = 0;
                public int dwXSize = 0;
                public int dwYSize = 0;
                public int dwXCountChars = 0;
                public int dwYCountChars = 0;
                public int dwFillAttribute = 0;
                public int dwFlags = 0;
                public short wShowWindow = 0;
                public short cbReserved2 = 0;
                public IntPtr lpReserved2 = IntPtr.Zero;
                public IntPtr hStdInput = IntPtr.Zero;
                public IntPtr hStdOutput = IntPtr.Zero;
                public IntPtr hStdError = IntPtr.Zero;
            }

            [StructLayout(LayoutKind.Sequential)]
            public class PROCESS_INFORMATION {
                public IntPtr hProcess = IntPtr.Zero;
                public IntPtr hThread = IntPtr.Zero;
                public int dwProcessId = 0;
                public int dwThreadId = 0;
            }

            [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
            public static extern bool CreateProcess(
                [MarshalAs(UnmanagedType.LPTStr)]
                string lpApplicationName,
                string lpCommandLine,
                IntPtr lpProcessAttributes,
                IntPtr lpThreadAttributes,
                bool bInheritHandles,
                int dwCreationFlags,
                [MarshalAs(UnmanagedType.LPWStr)]   
                StringBuilder lpEnvironment,
                [MarshalAs(UnmanagedType.LPTStr)]           
                string lpCurrentDirectory,
                STARTUPINFO lpStartupInfo,
                PROCESS_INFORMATION lpProcessInformation
            );

            [StructLayout(LayoutKind.Sequential)]
            public struct IO_COUNTERS {
                public UInt64 ReadOperationCount;
                public UInt64 WriteOperationCount;
                public UInt64 OtherOperationCount;
                public UInt64 ReadTransferCount;
                public UInt64 WriteTransferCount;
                public UInt64 OtherTransferCount;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION {
                public static int SIZE = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));

                public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
                public IO_COUNTERS IoInfo;
                public UIntPtr ProcessMemoryLimit;
                public UIntPtr JobMemoryLimit;
                public UIntPtr PeakProcessMemoryUsed;
                public UIntPtr PeakJobMemoryUsed;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct JOBOBJECT_BASIC_LIMIT_INFORMATION {
                public Int64 PerProcessUserTimeLimit;
                public Int64 PerJobUserTimeLimit;
                public UInt32 LimitFlags;
                public UIntPtr MinimumWorkingSetSize;
                public UIntPtr MaximumWorkingSetSize;
                public UInt32 ActiveProcessLimit;
                public UIntPtr Affinity;
                public UInt32 PriorityClass;
                public UInt32 SchedulingClass;
            }

            [DllImport(KERNEL32, SetLastError = false)]
            public static extern IntPtr GetCurrentProcess();

            [DllImport(KERNEL32, SetLastError = false)]
            public static extern bool IsProcessInJob(IntPtr Process, IntPtr Job, out bool Result);

            [DllImport(KERNEL32, SetLastError = false, CharSet = CharSet.Ansi)]
            public static extern IntPtr CreateJobObject(IntPtr a, string lpName);

            [DllImport(KERNEL32, SetLastError = false)]
            public static extern bool SetInformationJobObject(IntPtr hJob, int infoType
                , ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo, int cbJobObjectInfoLength);

            [DllImport(KERNEL32, SetLastError = false)]
            public static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

            [DllImport(KERNEL32, SetLastError = false)]
            public static extern bool TerminateJobObject(IntPtr processHandle, int exitCode);


            [DllImport(KERNEL32)]
            public static extern IntPtr OpenProcess(int access, bool inherit, int processId);

            [DllImport(KERNEL32)]
            public static extern bool TerminateProcess(IntPtr processHandle, int exitCode);


            [DllImport(KERNEL32, SetLastError = true)]
            public static extern int ResumeThread(IntPtr hObject);

            [DllImport(KERNEL32, SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hObject);

            [DllImport(KERNEL32, SetLastError = true, ExactSpelling = true)]
            public static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

            [DllImport(KERNEL32, SetLastError = true)]
            public static extern bool GetExitCodeProcess(IntPtr hProcess, out int lpExitCode);


            [DllImport(KERNEL32, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr CreateToolhelp32Snapshot(int flags, int processId);

            [DllImport(KERNEL32, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern bool Process32First(IntPtr handle, IntPtr entry);

            [DllImport(KERNEL32, CharSet = CharSet.Ansi)]
            public static extern bool Process32Next(IntPtr handle, IntPtr entry);

        }

        #endregion

    }

}
