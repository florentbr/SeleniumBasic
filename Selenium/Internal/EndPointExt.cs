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
            socket.Bind(new IPEndPoint(address, 0));

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
        /// <param name="endPoint">Endpoint holding the host ip and port</param>
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
        /// <param name="port">Port number</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <param name="delay">Time to wait in milliseconds to wait before checking again</param>
        /// <returns></returns>
        public bool WaitForListening(int timeout, int delay) {
            _socket.Close();

            //A connection attempt is used since the states returned by GetTcpTable are
            //not reliable on recent Windows builds (the listening state is not always 2).
            DateTime endtime = DateTime.UtcNow.AddMilliseconds(timeout);
            IPAddress address = _ipEndPoint.Address.Equals(IPAddress.Any) ?
                IPAddress.Loopback : _ipEndPoint.Address;
            var endpoint = new IPEndPoint(address, _ipEndPoint.Port);
            while (true) {
                using (var probe = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
                    try {
                        IAsyncResult ar = probe.BeginConnect(endpoint, null, null);
                        if (ar.AsyncWaitHandle.WaitOne(500, false)) {
                            probe.EndConnect(ar);
                            probe.Shutdown(SocketShutdown.Both);
                            return true;
                        }
                    } catch (SocketException) { }
                }

                if (DateTime.UtcNow > endtime)
                    return false;

                SysWaiter.Wait(delay);
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
