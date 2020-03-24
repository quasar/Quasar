using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoCloseConnection : IMessage
    {
        [ProtoMember(1)]
        public string LocalAddress { get; set; }

        [ProtoMember(2)]
        public ushort LocalPort { get; set; }

        [ProtoMember(3)]
        public string RemoteAddress { get; set; }

        [ProtoMember(4)]
        public ushort RemotePort { get; set; }
    }
}
