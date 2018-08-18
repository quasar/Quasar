using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoPathRename : IPacket
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public string NewPath { get; set; }

        [ProtoMember(3)]
        public PathType PathType { get; set; }
    }
}
