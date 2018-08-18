using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetDrivesResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] DriveDisplayName { get; set; }

        [ProtoMember(2)]
        public string[] RootDirectory { get; set; }
    }
}
