using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoProcessEnd : IMessage
    {
        [ProtoMember(1)]
        public int Pid { get; set; }
    }
}
