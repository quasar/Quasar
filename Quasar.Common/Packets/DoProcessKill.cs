using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoProcessKill : IPacket
    {
        [ProtoMember(1)]
        public int Pid { get; set; }
    }
}
