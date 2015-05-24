using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace xServer.Core.Extensions
{
    /// <summary>
    /// Socket Extension for KeepAlive
    /// </summary>
    /// <Author>Abdullah Saleem</Author>
    /// <Email>a.saleem2993@gmail.com</Email>
    public static class SocketExtensions
    {
        /// <summary>
        ///     A structure used by SetKeepAliveEx Method
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct TcpKeepAlive
        {
            internal uint onoff;
            internal uint keepalivetime;
            internal uint keepaliveinterval;
        };

        /// <summary>
        ///     Sets the Keep-Alive values for the current tcp connection
        /// </summary>
        /// <param name="socket">Current socket instance</param>
        /// <param name="keepAliveInterval">Specifies how often TCP repeats keep-alive transmissions when no response is received. TCP sends keep-alive transmissions to verify that idle connections are still active. This prevents TCP from inadvertently disconnecting active lines.</param>
        /// <param name="keepAliveTime">Specifies how often TCP sends keep-alive transmissions. TCP sends keep-alive transmissions to verify that an idle connection is still active. This entry is used when the remote system is responding to TCP. Otherwise, the interval between transmissions is determined by the value of the keepAliveInterval entry.</param>
        public static void SetKeepAliveEx(this Socket socket, uint keepAliveInterval, uint keepAliveTime)
        {
            var keepAlive = new TcpKeepAlive
            {
                onoff = 1,
                keepaliveinterval = keepAliveInterval,
                keepalivetime = keepAliveTime
            };
            int size = Marshal.SizeOf(keepAlive);
            IntPtr keepAlivePtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(keepAlive, keepAlivePtr, true);
            var buffer = new byte[size];
            Marshal.Copy(keepAlivePtr, buffer, 0, size);
            Marshal.FreeHGlobal(keepAlivePtr);
            socket.IOControl(IOControlCode.KeepAliveValues, buffer, null);
        }
    }
}