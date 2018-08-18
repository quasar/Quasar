using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class SetStatusFileManager : IPacket
    {
        [ProtoMember(1)]
        public string Message { get; set; }

        [ProtoMember(2)]
        public bool SetLastDirectorySeen { get; set; }
    }
}
