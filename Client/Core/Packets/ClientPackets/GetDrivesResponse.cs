using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetDrivesResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] Drives { get; set; }

        public GetDrivesResponse()
        {
        }

        public GetDrivesResponse(string[] drives)
        {
            this.Drives = drives;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}