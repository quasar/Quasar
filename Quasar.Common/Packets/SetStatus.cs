using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class SetStatus : IPacket
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }
}
