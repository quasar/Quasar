using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class ReverseProxyData : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
        public byte[] Data { get; set; }
    }
}
