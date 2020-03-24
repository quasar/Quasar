using ProtoBuf;

namespace Quasar.Common.Messages.ReverseProxy
{
    [ProtoContract]
    public class ReverseProxyData : IMessage
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
        public byte[] Data { get; set; }
    }
}
