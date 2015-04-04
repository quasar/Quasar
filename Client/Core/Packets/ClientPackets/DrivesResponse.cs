using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DrivesResponse : IPacket
    {
        public DrivesResponse()
        {
        }

        public DrivesResponse(string[] drives)
        {
            Drives = drives;
        }

        [ProtoMember(1)]
        public string[] Drives { get; set; }

        public void Execute(Client client)
        {
            client.Send<DrivesResponse>(this);
        }
    }
}