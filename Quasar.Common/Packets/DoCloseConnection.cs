using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoCloseConnection : IPacket
    {
        [ProtoMember(1)]
        public int LocalPort { get; set; }

        [ProtoMember(2)]
        public int RemotePort { get; set; }
    }
}
