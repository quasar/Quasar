using ProtoBuf;

namespace Quasar.Common.Messages.ReverseProxy
{
    [ProtoContract]
    public class ReverseProxyDisconnect : IMessage
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }
    }
}
