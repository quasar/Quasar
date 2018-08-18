using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoKeyboardEvent : IPacket
    {
        [ProtoMember(1)]
        public byte Key { get; set; }

        [ProtoMember(2)]
        public bool KeyDown { get; set; }
    }
}
