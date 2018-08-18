using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetDesktop : IPacket
    {
        [ProtoMember(1)]
        public int Quality { get; set; }

        [ProtoMember(2)]
        public int Monitor { get; set; }
    }
}
