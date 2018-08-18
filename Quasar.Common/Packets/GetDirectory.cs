using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetDirectory : IPacket
    {
        [ProtoMember(1)]
        public string RemotePath { get; set; }
    }
}
