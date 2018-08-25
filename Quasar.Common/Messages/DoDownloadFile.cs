using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoDownloadFile : IMessage
    {
        [ProtoMember(1)]
        public string RemotePath { get; set; }

        [ProtoMember(2)]
        public int Id { get; set; }
    }
}
