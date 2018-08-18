using ProtoBuf;
using Quasar.Common.Registry;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetChangeRegistryValueResponse : IPacket
    {
        [ProtoMember(1)]
        public string KeyPath { get; set; }

        [ProtoMember(2)]
        public RegValueData Value { get; set; }

        [ProtoMember(3)]
        public bool IsError { get; set; }

        [ProtoMember(4)]
        public string ErrorMsg { get; set; }
    }
}
