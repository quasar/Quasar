using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoCreateRegistryKey : IPacket
    {
        [ProtoMember(1)]
        public string ParentPath { get; set; }
    }
}
