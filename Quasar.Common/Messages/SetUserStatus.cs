using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class SetUserStatus : IMessage
    {
        [ProtoMember(1)]
        public UserStatus Message { get; set; }
    }
}
