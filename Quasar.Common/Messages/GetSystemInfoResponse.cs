using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetSystemInfoResponse : IMessage
    {
        [ProtoMember(1)]
        public string[] SystemInfos { get; set; }
    }
}
