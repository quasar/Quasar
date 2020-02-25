using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoProcessStart : IMessage
    {
        [ProtoMember(1)]
        public string ApplicationName { get; set; }
    }
}
