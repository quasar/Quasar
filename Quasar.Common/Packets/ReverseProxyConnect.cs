using ProtoBuf;

namespace Quasar.Common.Packets
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
    }
}
