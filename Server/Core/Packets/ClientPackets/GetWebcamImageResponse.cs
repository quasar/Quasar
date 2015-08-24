using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetWebcamImageResponse : IPacket
    {
        public byte[] Image { get; set; }

        public int Quality { get; set; }

        public int Webcam { get; set; }

        public string Resolution { get; set; }

        public GetWebcamImageResponse()
        {
        }

        public GetWebcamImageResponse(byte[] image, int quality, int webcam, string resolution)
        {
            this.Image = image;
            this.Quality = quality;
            this.Webcam = webcam;
            this.Resolution = resolution;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}