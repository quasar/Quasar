using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoClientUpdate : IMessage
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string DownloadUrl { get; set; }

        [ProtoMember(3)]
        public string FileName { get; set; }

        [ProtoMember(4)]
        public byte[] Block { get; set; }

        [ProtoMember(5)]
        public int MaxBlocks { get; set; }

        [ProtoMember(6)]
        public int CurrentBlock { get; set; }
    }
}
