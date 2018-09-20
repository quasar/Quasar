using ProtoBuf;
using Quasar.Common.Models;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetRegistryKeysResponse : IMessage
    {
        [ProtoMember(1)]
        public RegSeekerMatch[] Matches { get; set; }

        [ProtoMember(2)]
        public string RootKey { get; set; }

        [ProtoMember(3)]
        public bool IsError { get; set; }

        [ProtoMember(4)]
        public string ErrorMsg { get; set; }
    }
}
