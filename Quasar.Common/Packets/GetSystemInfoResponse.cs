using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetSystemInfoResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] SystemInfos { get; set; }
    }
}
