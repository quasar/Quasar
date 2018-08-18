using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class ReverseProxyConnectResponse : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
        public bool IsConnected { get; set; }

        [ProtoMember(3)]
        public byte[] LocalAddress { get; set; }

        [ProtoMember(4)]
        public int LocalPort { get; set; }

        [ProtoMember(5)]
        public string HostName { get; set; }
    }
}
