using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoDeleteRegistryValue : IPacket
    {
        [ProtoMember(1)]
        public string KeyPath { get; set; }

        [ProtoMember(2)]
        public string ValueName { get; set; }
    }
}
