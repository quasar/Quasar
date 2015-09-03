using System;
using System.Linq;
using System.Windows.Forms;
using xServer.Core.Extensions;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmSystemInformation : Form
    {
        private readonly Client _connectClient;

        public FrmSystemInformation(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmSi = this;

            InitializeComponent();
        }

        private void FrmSystemInformation_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = WindowHelper.GetWindowTitle("System Information", _connectClient);
                new Core.Packets.ServerPackets.GetSystemInfo().Execute(_connectClient);

                if (_connectClient.Value != null)
                {
                    ListViewItem lvi =
                        new ListViewItem(new string[] {"Operating System", _connectClient.Value.OperatingSystem});
                    lstSystem.Items.Add(lvi);
                    lvi =
                        new ListViewItem(new string[]
                        {
                            "Architecture",
                            (_connectClient.Value.OperatingSystem.Contains("32 Bit")) ? "x86 (32 Bit)" : "x64 (64 Bit)"
                        });
                    lstSystem.Items.Add(lvi);
                    lvi = new ListViewItem(new string[] {"", "Getting more information..."});
                    lstSystem.Items.Add(lvi);
                }
            }
        }

        private void FrmSystemInformation_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmSi = null;
        }

        public void AddItems(ListViewItem[] lviCollection)
        {
            try
            {
                lstSystem.Invoke((MethodInvoker) delegate
                {
                    lstSystem.Items.RemoveAt(2); // Loading... Information

                    foreach (var lviItem in lviCollection)
                    {
                        if (lviItem != null)
                            lstSystem.Items.Add(lviItem);
                    }

                    lstSystem.AutosizeColumns();
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstSystem.Items.Count == 0) return;

            string output = string.Empty;

            foreach (ListViewItem lvi in lstSystem.Items)
            {
                output = lvi.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate(output, (current, lvs) => current + (lvs.Text + " : "));
                output = output.Remove(output.Length - 3);
                output = output + "\r\n";
            }

            ClipboardHelper.SetClipboardText(output);
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstSystem.SelectedItems.Count == 0) return;

            string output = string.Empty;

            foreach (ListViewItem lvi in lstSystem.SelectedItems)
            {
                output = lvi.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate(output, (current, lvs) => current + (lvs.Text + " : "));
                output = output.Remove(output.Length - 3);
                output = output + "\r\n";
            }

            ClipboardHelper.SetClipboardText(output);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstSystem.Items.Clear();

            if (_connectClient != null)
            {
                this.Text = WindowHelper.GetWindowTitle("System Information", _connectClient);
                new Core.Packets.ServerPackets.GetSystemInfo().Execute(_connectClient);

                if (_connectClient.Value != null)
                {
                    ListViewItem lvi =
                        new ListViewItem(new string[] { "Operating System", _connectClient.Value.OperatingSystem });
                    lstSystem.Items.Add(lvi);
                    lvi =
                        new ListViewItem(new string[]
                        {
                            "Architecture",
                            (_connectClient.Value.OperatingSystem.Contains("32 Bit")) ? "x86 (32 Bit)" : "x64 (64 Bit)"
                        });
                    lstSystem.Items.Add(lvi);
                    lvi = new ListViewItem(new string[] { "", "Getting more information..." });
                    lstSystem.Items.Add(lvi);
                }
            }
        }
    }
}