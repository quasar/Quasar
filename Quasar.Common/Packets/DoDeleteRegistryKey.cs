using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoDeleteRegistryKey : IPacket
    {
        [ProtoMember(1)]
        public string ParentPath { get; set; }

        [ProtoMember(2)]
        public string KeyName { get; set; }
    }
}
