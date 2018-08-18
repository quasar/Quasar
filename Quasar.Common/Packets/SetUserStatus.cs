using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class SetUserStatus : IPacket
    {
        [ProtoMember(1)]
        public UserStatus Message { get; set; }
    }
}
