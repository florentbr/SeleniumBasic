using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Selenium.Internal {

    static class ProcessExt {

        public struct ProcessInfo {
            public int Id;
            public int ParentId;
        }

        /// <summary>
        /// Terminates a process tree starting with the children.
        /// </summary>
        /// <param name="pid">Process id</param>
        public static void TerminateProcessTree(int pid) {
            ProcessInfo[] processes = GetRunningProcesses();
            TerminateProcessTreeRecursive(pid, processes, 20);
        }

        private static void TerminateProcessTreeRecursive(int pid, ProcessInfo[] processesIds, int depth) {
            if (depth-- < 0)  //To prevent overflow
                return;

            //terminates childs
            foreach (ProcessInfo process in processesIds) {
                if (process.ParentId == pid)
                    TerminateProcessTreeRecursive(process.Id, processesIds, depth);
            }

            //terminate the process
            IntPtr procHandle = NativeMethods.OpenProcess(NativeMethods.PROCESS_ALL_ACCESS, false, pid);
            if (procHandle != IntPtr.Zero) {
                try {
                    NativeMethods.TerminateProcess(procHandle, -1);
                } catch {
                } finally {
                    NativeMethods.CloseHandle(procHandle);
                }
            }
        }

        /// <summary>
        /// Returns a list of process ids.
        /// </summary>
        /// <returns></returns>
        public static unsafe ProcessInfo[] GetRunningProcesses() {
            List<ProcessInfo> items = new List<ProcessInfo>(100);

            IntPtr snapshot = NativeMethods.CreateToolhelp32Snapshot(NativeMethods.TH32CS_SNAPPROCESS, 0);

            int bufferSize = sizeof(NativeMethods.WinProcessEntry);
            IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
            Marshal.WriteInt32(buffer, bufferSize);
            var entryPtr = (NativeMethods.WinProcessEntry*)buffer;
            try {
                if (!NativeMethods.Process32First(snapshot, buffer))
                    throw new Win32Exception();
                do {
                    NativeMethods.WinProcessEntry entry = *entryPtr;
                    items.Add(new ProcessInfo {
                        Id = entry.th32ProcessID,
                        ParentId = entry.th32ParentProcessID
                    });
                } while (NativeMethods.Process32Next(snapshot, buffer));
            } finally {
                Marshal.FreeHGlobal(buffer);
                NativeMethods.CloseHandle(snapshot);
            }
            return items.ToArray();
        }

        static class NativeMethods {

            const string KERNEL32 = "kernel32.dll";

            public const int PROCESS_ALL_ACCESS = 0x001f0fff;
            public const int TH32CS_SNAPPROCESS = 0x00000002;

            [DllImport(KERNEL32, SetLastError = true)]
            public static extern IntPtr CreateToolhelp32Snapshot(int flags, int processId);

            [DllImport(KERNEL32, SetLastError = true)]
            public static extern bool Process32First(IntPtr handle, IntPtr entry);

            [DllImport(KERNEL32)]
            public static extern bool Process32Next(IntPtr handle, IntPtr entry);

            [DllImport(KERNEL32, ExactSpelling = true)]
            public static extern bool CloseHandle(IntPtr handle);

            [DllImport(KERNEL32)]
            public static extern bool TerminateProcess(IntPtr processHandle, int exitCode);

            [DllImport(KERNEL32)]
            public static extern IntPtr OpenProcess(int access, bool inherit, int processId);


            [StructLayout(LayoutKind.Sequential)]
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

        }

    }

}
