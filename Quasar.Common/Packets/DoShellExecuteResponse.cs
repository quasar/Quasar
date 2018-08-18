using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoShellExecuteResponse : IPacket
    {
        [ProtoMember(1)]
        public string Output { get; set; }

        [ProtoMember(2)]
        public bool IsError { get; set; }
    }
}
