using Selenium.Core;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Selenium.Internal {

    class EndPointExt : IDisposable {

        /// <summary>
        /// Lock a new TCP end point that can be unlocked later.
        /// </summary>
        public static EndPointExt Create(IPAddress address, bool disableInheritence) {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.ReceiveBufferSize = 0;
            socket.SendBufferSize = 0;
            socket.Bind(new IPEndPoint(address, 0));    // any available port

            //Disable inheritance to the child processes so the main process can close the
            //socket once a child process is launched.
            if (disableInheritence)
                Native.SetHandleInformation(socket.Handle, 1, 0);

            return new EndPointExt(socket, (IPEndPoint)socket.LocalEndPoint);
        }

        internal static EndPointExt Parse(string address) {
            string[] parts = address.Split(':');
            IPAddress ipAddress = IPAddress.Parse(parts[0]);
            int ipPort = int.Parse(parts[1]);
            IPEndPoint endPoint = new IPEndPoint(ipAddress, ipPort);
            return new EndPointExt(null, endPoint);
        }

        Socket _socket;
        IPEndPoint _ipEndPoint;

        public EndPointExt(Socket socket, IPEndPoint endpoint) {
            _socket = socket;
            _ipEndPoint = endpoint;
        }

        public void Dispose() {
            if (_socket == null)
                return;
            _socket.Close();
            _socket = null;
        }

        public IPEndPoint IPEndPoint {
            get {
                return _ipEndPoint;
            }
        }
        public override string ToString() {
            return _ipEndPoint.ToString();
        }

        /// <summary>
        /// Returns true if a given host:port is connectable, false otherwise
        /// </summary>
        /// <param name="timeout">Timeout in millisecond</param>
        /// <param name="delay">Delay to retry in millisecond</param>
        /// <returns>True if succeed, false otherwise</returns>
        public bool WaitForConnectable(int timeout, int delay) {
            _socket.Close();

            var endtime = DateTime.UtcNow.AddMilliseconds(timeout);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveTimeout = socket.SendTimeout = 1;
            socket.NoDelay = true;
            try {
                while (true) {
                    try {
                        socket.Connect(_ipEndPoint);
                        return true;
                    } catch (SocketException) {
                    } catch (Exception ex) {
                        throw new SeleniumException(ex);
                    }

                    if (DateTime.UtcNow > endtime)
                        return false;
                    SysWaiter.Wait(delay);
                }
            } finally {
                socket.Disconnect(false);
                socket.Close(0);
            }
        }

        /// <summary>
        /// Waits for a local port to be listening on the Loopback or Any address.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <param name="delay">Time to wait in milliseconds to wait before checking again</param>
        /// <returns>True on detecting the port is being listening</returns>
        public bool WaitForListening(int timeout, int delay) {
            _socket.Close();

            DateTime endtime = DateTime.UtcNow.AddMilliseconds(timeout);
            int portLE = _ipEndPoint.Port;
            int portBE = (portLE & 0xFF) << 8 | (portLE & 0xFF00) >> 8;  //Convert port to big endian

            int bufferSize = 1024;
            IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
            int[] tcpTable = new int[bufferSize / sizeof(int)];
            try {
                while (true) {
                    int result = 0;
                    while (122 == (result = Native.GetTcpTable(buffer, ref bufferSize, false))) {
                        Marshal.FreeHGlobal(buffer);
                        buffer = Marshal.AllocHGlobal(bufferSize);
                        tcpTable = new int[bufferSize / sizeof(int)];
                    }
                    if (result != 0)
                        throw new NetworkInformationException(result);

                    // Buffer MIB_TCPTABLE :
                    // int dwNumEntries 
                    // int dwState      //MIB_TCPROW entry 1
                    // int dwLocalAddr
                    // int dwLocalPort
                    // int dwRemoteAddr
                    // int dwRemotePort
                    // int dwState      //MIB_TCPROW entry 2
                    // ...

                    Marshal.Copy(buffer, tcpTable, 0, bufferSize / sizeof(int));
                    int dwNumEntries = tcpTable[0];
                    int tcpTableLength = 1 + dwNumEntries * 5;

                    //loop over each entry to find the port (1 entry is 5 integers)
                    for (int i = 3; i < tcpTableLength; i += 5) {
                        int dwLocalPort = tcpTable[i];
                        if (dwLocalPort == portBE) {
                            int dwLocalAddr = tcpTable[i - 1];
                            if (dwLocalAddr == 0 || dwLocalAddr == 0x0100007f) { //Any or Loopback
                                int dwState = tcpTable[i - 2];
                                if (dwState == 2) // 2=listening
                                    return true;
                                break;
                            }
                        }
                    }

                    if (DateTime.UtcNow > endtime)
                        return false;

                    SysWaiter.Wait(delay);
                }
            } finally {
                Marshal.FreeHGlobal(buffer);
            }
        }

        static class Native {

            const string IPHLPAPI = "Iphlpapi.dll";
            const string KERNEL32 = "kernel32.dll";

            [DllImport(IPHLPAPI)]
            internal extern static int GetTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool order);

            [DllImport(KERNEL32)]
            internal static extern bool SetHandleInformation(IntPtr hObject, int dwMask, int dwFlags);

        }

    }

}
