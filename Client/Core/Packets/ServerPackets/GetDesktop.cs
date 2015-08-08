using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetDesktop : IPacket
    {
        public int Quality { get; set; }

        public int Monitor { get; set; }

        public GetDesktop()
        {
        }

        public GetDesktop(int quality, int monitor)
        {
            this.Quality = quality;
            this.Monitor = monitor;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}