using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class ClientIdentification : IMessage
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public string OperatingSystem { get; set; }

        [ProtoMember(3)]
        public string AccountType { get; set; }

        [ProtoMember(4)]
        public string Country { get; set; }

        [ProtoMember(5)]
        public string CountryCode { get; set; }

        [ProtoMember(6)]
        public string Region { get; set; }

        [ProtoMember(7)]
        public string City { get; set; }

        [ProtoMember(8)]
        public int ImageIndex { get; set; }

        [ProtoMember(9)]
        public string Id { get; set; }

        [ProtoMember(10)]
        public string Username { get; set; }

        [ProtoMember(11)]
        public string PcName { get; set; }

        [ProtoMember(12)]
        public string Tag { get; set; }

        [ProtoMember(13)]
        public string EncryptionKey { get; set; }

        [ProtoMember(14)]
        public byte[] Signature { get; set; }
    }
}
