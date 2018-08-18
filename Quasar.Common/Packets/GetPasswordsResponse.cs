using System.Collections.Generic;
using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetPasswordsResponse : IPacket
    {
        [ProtoMember(1)]
        public List<string> Passwords { get; set; }
    }
}
