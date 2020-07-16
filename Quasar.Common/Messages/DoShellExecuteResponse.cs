using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoShellExecuteResponse : IMessage
    {
        [ProtoMember(1)]
        public string Output { get; set; }

        [ProtoMember(2)]
        public bool IsError { get; set; }
    }
}
