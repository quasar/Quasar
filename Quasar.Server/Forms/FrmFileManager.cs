using Quasar.Common.Enums;
using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Server.Controls;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Models;
using Quasar.Server.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace Quasar.Server.Forms
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
        /// <param name="client">The client used for the file manager form.</param>
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

        /// <summary>
        /// Called whenever drives changed.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="drives">The currently available drives.</param>
        private void DrivesChanged(object sender, Drive[] drives)
        {
            cmbDrives.Items.Clear();
            cmbDrives.DisplayMember = "DisplayName";
            cmbDrives.ValueMember = "RootDirectory";
            cmbDrives.DataSource = new BindingSource(drives, null);

            SetStatusMessage(this, "Ready");
        }

        /// <summary>
        /// Called whenever a directory changed.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="remotePath">The remote path of the directory.</param>
        /// <param name="items">The directory content.</param>
        private void DirectoryChanged(object sender, string remotePath, FileSystemEntry[] items)
        {
            txtPath.Text = remotePath;
            _currentDir = remotePath;

            lstDirectory.Items.Clear();

            AddItemToFileBrowser("..", 0, FileType.Back, 0);
            foreach (var item in items)
            {
                switch (item.EntryType)
                {
                    case FileType.Directory:
                        AddItemToFileBrowser(item.Name, 0, item.EntryType, 1);
                        break;
                    case FileType.File:
                        int imageIndex = item.ContentType == null ? 2 : (int)item.ContentType;
                        AddItemToFileBrowser(item.Name, item.Size, item.EntryType, imageIndex);
                        break;
                }
            }

            SetStatusMessage(this, "Ready");
        }

        /// <summary>
        /// Gets the image index of completed or canceled file transfers.
        /// </summary>
        /// <param name="status">File transfer status used to determine the image index.</param>
        /// <returns>The image index of the file transfer, default -1.</returns>
        private int GetTransferImageIndex(string status)
        {
            int imageIndex = -1;
            switch (status)
            {
                case "Completed":
                    imageIndex = 1;
                    break;
                case "Canceled":
                    imageIndex = 0;
                    break;
            }

            return imageIndex;
        }

        /// <summary>
        /// Called whenever a file transfer gets updated.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="transfer">The updated file transfer.</param>
        private void FileTransferUpdated(object sender, FileTransfer transfer)
        {
            for (var i = 0; i < lstTransfers.Items.Count; i++)
            {
                if (lstTransfers.Items[i].SubItems[(int)TransferColumn.Id].Text == transfer.Id.ToString())
                {
                    lstTransfers.Items[i].SubItems[(int)TransferColumn.Status].Text = transfer.Status;
                    lstTransfers.Items[i].ImageIndex = GetTransferImageIndex(transfer.Status);
                    return;
                }
            }

            var lvi = new ListViewItem(new[]
                    {transfer.Id.ToString(), transfer.Type.ToString(), transfer.Status, transfer.RemotePath})
                {Tag = transfer, ImageIndex = GetTransferImageIndex(transfer.Status)};

            lstTransfers.Items.Add(lvi);
        }

        /// <summary>
        /// Combines the current path with the new path.
        /// </summary>
        /// <param name="path">The path to combine with.</param>
        /// <returns>The absolute combined path.</returns>
        private string GetAbsolutePath(string path)
        {
            if (!string.IsNullOrEmpty(_currentDir) && _currentDir[0] == '/') // support forward slashes
            {
                if (_currentDir.Length == 1)
                    return Path.Combine(_currentDir, path);
                else
                    return Path.Combine(_currentDir + '/', path);
            }

            return Path.GetFullPath(Path.Combine(_currentDir, path));
        }

        /// <summary>
        /// Navigates one directory up in the hierarchical directory tree.
        /// </summary>
        /// <returns>The new directory path.</returns>
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

            _fileManagerHandler.RefreshDrives();
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

                    using (var frm = new FrmStartupAdd(path))
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

            FrmRemoteShell frmRs = FrmRemoteShell.CreateNewOrGetExisting(_connectClient);
            frmRs.Show();
            frmRs.Focus();
            frmRs.RemoteShellHandler.SendCommand($"cd \"{path}\"");
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

        /// <summary>
        /// Adds an item to the file browser.
        /// </summary>
        /// <param name="name">File or directory name.</param>
        /// <param name="size">File size, for directories use 0.</param>
        /// <param name="type">File type.</param>
        /// <param name="imageIndex">The image to display for this item.</param>
        private void AddItemToFileBrowser(string name, long size, FileType type, int imageIndex)
        {
            ListViewItem lvi = new ListViewItem(new string[]
            {
                name,
                (type == FileType.File) ? StringHelper.GetHumanReadableFileSize(size) : string.Empty,
                (type != FileType.Back) ? type.ToString() : string.Empty
            })
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

        /// <summary>
        /// Fetches the directory contents of the current directory.
        /// </summary>
        private void RefreshDirectory()
        {
            SwitchDirectory(_currentDir);
        }

        /// <summary>
        /// Switches to a new directory and fetches the contents of it.
        /// </summary>
        /// <param name="remotePath">Path of new directory.</param>
        private void SwitchDirectory(string remotePath)
        {
            _fileManagerHandler.GetDirectoryContents(remotePath);
            SetStatusMessage(this, "Loading directory content...");
        }
    }
}