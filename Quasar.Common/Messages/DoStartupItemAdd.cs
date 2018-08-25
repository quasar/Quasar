using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoStartupItemAdd : IMessage
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string Path { get; set; }

        [ProtoMember(3)]
        public int Type { get; set; }
    }
}
