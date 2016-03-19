using System.Threading;
using xServer.Core.Networking;
using xServer.Core.Packets.ClientPackets;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE TCP Connections COMMANDS. */
    public static partial class CommandHandler
    {
        public static void HandleGetConnectionsResponse(Client client, GetConnectionsResponse packet)
        {

            if (client.Value == null || client.Value.FrmCon == null)
                return;

            client.Value.FrmCon.ClearListviewItems();

            // None of the arrays containing the process' information can be null.
            // The must also be the exact same length because each entry in the five
            // different arrays represents one process.
            if (packet.Processes == null || packet.LocalAddresses == null || packet.LocalPorts == null ||
                packet.RemoteAddresses == null || packet.RemotePorts == null || packet.States == null ||
                packet.Processes.Length != packet.LocalAddresses.Length || packet.Processes.Length != packet.LocalPorts.Length ||
                packet.Processes.Length != packet.RemoteAddresses.Length || packet.Processes.Length != packet.RemotePorts.Length ||
                packet.Processes.Length != packet.States.Length)

                return;

            new Thread(() =>
            {
                /*if (client.Value != null && client.Value.FrmTm != null)
                    client.Value.FrmTm.SetProcessesCount(packet.Process.Length);*/

                for (int i = 0; i < packet.Processes.Length; i++)
                {
                    /*if (packet.IDs[i] == 0 || packet.Processes[i] == "System.exe")
                        continue;*/

                    if (client.Value == null || client.Value.FrmCon == null)
                        break;

                    client.Value.FrmCon.AddConnectionToListview(packet.Processes[i], packet.LocalAddresses[i], packet.LocalPorts[i],
                        packet.RemoteAddresses[i], packet.RemotePorts[i], ((ConnectionStates)packet.States[i]).ToString());
                }
            }).Start();
        }

        enum ConnectionStates : byte
        {
            Closed = 1,
            Listening = 2,
            SYN_Sent = 3,
            Syn_Recieved = 4,
            Established = 5,
            Finish_Wait_1 = 6,
            Finish_Wait_2 = 7,
            Closed_Wait = 8,
            Closing = 9,
            Last_ACK = 10,
            Time_Wait = 11,
            Delete_TCB = 12
        }
    }
}
