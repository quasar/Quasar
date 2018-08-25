using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoCloseConnection : IMessage
    {
        [ProtoMember(1)]
        public int LocalPort { get; set; }

        [ProtoMember(2)]
        public int RemotePort { get; set; }
    }
}
