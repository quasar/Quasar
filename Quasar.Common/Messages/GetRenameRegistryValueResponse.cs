using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetRenameRegistryValueResponse : IMessage
    {
        [ProtoMember(1)]
        public string KeyPath { get; set; }

        [ProtoMember(2)]
        public string OldValueName { get; set; }

        [ProtoMember(3)]
        public string NewValueName { get; set; }

        [ProtoMember(4)]
        public bool IsError { get; set; }

        [ProtoMember(5)]
        public string ErrorMsg { get; set; }
    }
}
