using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class SetStatusFileManager : IMessage
    {
        [ProtoMember(1)]
        public string Message { get; set; }

        [ProtoMember(2)]
        public bool SetLastDirectorySeen { get; set; }
    }
}
