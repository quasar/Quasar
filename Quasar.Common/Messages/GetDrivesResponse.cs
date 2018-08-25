using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetDrivesResponse : IMessage
    {
        [ProtoMember(1)]
        public string[] DriveDisplayName { get; set; }

        [ProtoMember(2)]
        public string[] RootDirectory { get; set; }
    }
}
