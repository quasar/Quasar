using ProtoBuf;
using Quasar.Common.Models;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetCreateRegistryValueResponse : IMessage
    {
        [ProtoMember(1)]
        public string KeyPath { get; set; }

        [ProtoMember(2)]
        public RegValueData Value { get; set; }

        [ProtoMember(3)]
        public bool IsError { get; set; }

        [ProtoMember(4)]
        public string ErrorMsg { get; set; }
    }
}
