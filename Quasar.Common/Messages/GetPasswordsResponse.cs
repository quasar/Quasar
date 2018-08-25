using System.Collections.Generic;
using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetPasswordsResponse : IMessage
    {
        [ProtoMember(1)]
        public List<string> Passwords { get; set; }
    }
}
