using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoDownloadFile : IPacket
    {
        [ProtoMember(1)]
        public string RemotePath { get; set; }

        [ProtoMember(2)]
        public int Id { get; set; }
    }
}
