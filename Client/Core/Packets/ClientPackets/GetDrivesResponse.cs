using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetDrivesResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] DriveDisplayName { get; set; }

        [ProtoMember(2)]
        public string[] RootDirectory { get; set; }

        public GetDrivesResponse()
        {
        }

        public GetDrivesResponse(string[] driveDisplayName, string[] rootDirectory)
        {
            this.DriveDisplayName = driveDisplayName;
            this.RootDirectory = rootDirectory;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}