using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetDesktop : IMessage
    {
        [ProtoMember(1)]
        public int Quality { get; set; }

        [ProtoMember(2)]
        public int Monitor { get; set; }
    }
}
