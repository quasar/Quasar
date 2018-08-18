using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class ReverseProxyDisconnect : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }
    }
}
