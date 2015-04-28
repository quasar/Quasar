using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DrivesResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] Drives { get; set; }

        public DrivesResponse()
        {
        }

        public DrivesResponse(string[] drives)
        {
            this.Drives = drives;
        }

        public void Execute(Client client)
        {
            client.Send<DrivesResponse>(this);
        }
    }
}