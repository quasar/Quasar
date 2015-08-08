using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoProcessKill : IPacket
    {
        public int PID { get; set; }

        public DoProcessKill()
        {
        }

        public DoProcessKill(int pid)
        {
            this.PID = pid;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}