using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoShutdownAction : IPacket
    {
        [ProtoMember(1)]
        public ShutdownAction Action { get; set; }
    }
}
