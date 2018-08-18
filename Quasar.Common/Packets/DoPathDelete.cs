using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoPathDelete : IPacket
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public PathType PathType { get; set; }
    }
}
