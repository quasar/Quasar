using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoDownloadFileCancel : IPacket
    {
        [ProtoMember(1)]
        public int Id { get; set; }
    }
}
