using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetMonitorsResponse : IPacket
    {
        [ProtoMember(1)]
        public int Number { get; set; }
    }
}
