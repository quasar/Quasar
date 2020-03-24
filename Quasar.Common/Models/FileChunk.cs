using ProtoBuf;

namespace Quasar.Common.Models
{
    [ProtoContract]
    public class FileChunk
    {
        [ProtoMember(1)]
        public long Offset { get; set; }

        [ProtoMember(2)]
        public byte[] Data { get; set; }
    }
}
