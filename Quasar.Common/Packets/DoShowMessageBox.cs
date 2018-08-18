using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoShowMessageBox : IPacket
    {
        [ProtoMember(1)]
        public string Caption { get; set; }

        [ProtoMember(2)]
        public string Text { get; set; }

        [ProtoMember(3)]
        public string Button { get; set; }

        [ProtoMember(4)]
        public string Icon { get; set; }
    }
}
