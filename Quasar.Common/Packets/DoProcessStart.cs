using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoProcessStart : IPacket
    {
        [ProtoMember(1)]
        public string ApplicationName { get; set; }
    }
}
