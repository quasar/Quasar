using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoRenameRegistryKey : IPacket
    {
        [ProtoMember(1)]
        public string ParentPath { get; set; }

        [ProtoMember(2)]
        public string OldKeyName { get; set; }

        [ProtoMember(3)]
        public string NewKeyName { get; set; }
    }
}
