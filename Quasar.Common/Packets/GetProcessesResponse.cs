using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetProcessesResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] Processes { get; set; }

        [ProtoMember(2)]
        public int[] Ids { get; set; }

        [ProtoMember(3)]
        public string[] Titles { get; set; }
    }
}
