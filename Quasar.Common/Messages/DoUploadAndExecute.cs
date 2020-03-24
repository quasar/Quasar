using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoUploadAndExecute : IMessage
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string FileName { get; set; }

        [ProtoMember(3)]
        public byte[] Block { get; set; }

        [ProtoMember(4)]
        public int MaxBlocks { get; set; }

        [ProtoMember(5)]
        public int CurrentBlock { get; set; }

        [ProtoMember(6)]
        public bool RunHidden { get; set; }
    }
}
