using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Misc;

namespace xServer.Forms
{
    public partial class FrmKeylogger : Form
    {
        private readonly Client _connectClient;
        private readonly ListViewColumnSorter _lvwColumnSorter;
        private string path;

        public FrmKeylogger(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmKl = this;
            path = Path.Combine(Application.StartupPath, "Clients\\" + _connectClient.EndPoint.Address.ToString() + "\\Logs\\");
            InitializeComponent();

            _lvwColumnSorter = new ListViewColumnSorter();
            lstLogs.ListViewItemSorter = _lvwColumnSorter;
        }

        private void FrmKeylogger_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = string.Format("xRAT 2.0 - Keylogger [{0}:{1}]", _connectClient.EndPoint.Address.ToString(), _connectClient.EndPoint.Port.ToString());

                DirectoryInfo dicInfo = new DirectoryInfo(path);

                FileInfo[] iFiles = dicInfo.GetFiles();

                foreach (FileInfo file in iFiles)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = file.Name;
                    lstLogs.Items.Add(lvi);
                }
            }
        }

        private void lstLogs_ColumnClick(object sender, ColumnClickEventArgs e)
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
            lstLogs.Sort();
        }

        private void btnGetLogs_Click(object sender, EventArgs e)
        {
            btnGetLogs.Enabled = false;

            lstLogs.Items.Clear();

            new Core.Packets.ServerPackets.GetLogs().Execute(_connectClient);
        }

        private void FrmKeylogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmKl = null;
        }

        private void lstLogs_ItemActivate(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;

            using (FileStream fileStream = new FileStream(Path.Combine(path, lv.Items[0].SubItems[0].Text), FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    try
                    {
                        rtbLogView.Text = sr.ReadToEnd();
                    }
                    catch
                    { }
                }
            }
        }
    }
}
