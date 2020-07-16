using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class FileTransferComplete : IMessage
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string FilePath { get; set; }
    }
}
