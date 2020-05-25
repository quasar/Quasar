using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoRemoteExecutionResponse : IMessage
    {
        [ProtoMember(1)]
        public bool Success { get; set; }
    }
}
