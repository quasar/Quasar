using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoKeyboardEvent : IMessage
    {
        [ProtoMember(1)]
        public byte Key { get; set; }

        [ProtoMember(2)]
        public bool KeyDown { get; set; }
    }
}
