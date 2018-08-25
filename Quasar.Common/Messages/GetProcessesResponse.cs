using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetProcessesResponse : IMessage
    {
        [ProtoMember(1)]
        public string[] Processes { get; set; }

        [ProtoMember(2)]
        public int[] Ids { get; set; }

        [ProtoMember(3)]
        public string[] Titles { get; set; }
    }
}
