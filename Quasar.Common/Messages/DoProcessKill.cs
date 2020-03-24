using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoProcessKill : IMessage
    {
        [ProtoMember(1)]
        public int Pid { get; set; }
    }
}
