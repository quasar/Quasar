using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetDirectoryResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] Files { get; set; }

        [ProtoMember(2)]
        public string[] Folders { get; set; }

        [ProtoMember(3)]
        public long[] FilesSize { get; set; }
    }
}
