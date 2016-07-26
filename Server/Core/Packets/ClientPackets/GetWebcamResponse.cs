using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetWebcamResponse : IPacket
    {
        public byte[] Image { get; set; }
        public int Webcam { get; set; }
        public int Resolution { get; set; }

        public GetWebcamResponse()
        {
        }

        public GetWebcamResponse(byte[] image, int webcam, int resolution)
        {
            this.Image = image;
            this.Webcam = webcam;
            this.Resolution = resolution;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
