using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetConnectionsResponse : IMessage
    {
        [ProtoMember(1)]
        public string[] Processes { get; set; }

        [ProtoMember(2)]
        public string[] LocalAddresses { get; set; }

        [ProtoMember(3)]
        public string[] LocalPorts { get; set; }

        [ProtoMember(4)]
        public string[] RemoteAddresses { get; set; }

        [ProtoMember(5)]
        public string[] RemotePorts { get; set; }

        [ProtoMember(6)]
        public byte[] States { get; set; }
    }
}
