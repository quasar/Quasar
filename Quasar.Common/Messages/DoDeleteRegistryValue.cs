using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoDeleteRegistryValue : IMessage
    {
        [ProtoMember(1)]
        public string KeyPath { get; set; }

        [ProtoMember(2)]
        public string ValueName { get; set; }
    }
}
