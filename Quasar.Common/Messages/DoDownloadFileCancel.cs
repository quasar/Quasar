using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoDownloadFileCancel : IMessage
    {
        [ProtoMember(1)]
        public int Id { get; set; }
    }
}
