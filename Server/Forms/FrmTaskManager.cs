using System;
using System.Windows.Forms;
using xServer.Controls;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Utilities;

namespace xServer.Forms
{
    public partial class FrmTaskManager : Form
    {
        private readonly Client _connectClient;
        private readonly ListViewColumnSorter _lvwColumnSorter;

        public FrmTaskManager(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmTm = this;

            InitializeComponent();

            _lvwColumnSorter = new ListViewColumnSorter();
            lstTasks.ListViewItemSorter = _lvwColumnSorter;
        }

        private void FrmTaskManager_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = WindowHelper.GetWindowTitle("Task Manager", _connectClient);
                new Core.Packets.ServerPackets.GetProcesses().Execute(_connectClient);
            }
        }

        private void FrmTaskManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmTm = null;
        }

        private void ctxtKillProcess_Click(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                foreach (ListViewItem lvi in lstTasks.SelectedItems)
                {
                    new Core.Packets.ServerPackets.DoProcessKill(int.Parse(lvi.SubItems[1].Text)).Execute(_connectClient);
                }
            }
        }

        private void ctxtStartProcess_Click(object sender, EventArgs e)
        {
            string processname = string.Empty;
            if (InputBox.Show("Processname", "Enter Processname:", ref processname) == DialogResult.OK)
            {
                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DoProcessStart(processname).Execute(_connectClient);
            }
        }

        private void ctxtRefresh_Click(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                new Core.Packets.ServerPackets.GetProcesses().Execute(_connectClient);
            }
        }

        public void ClearListview()
        {
            try
            {
                lstTasks.Invoke((MethodInvoker)delegate
                {
                    lstTasks.Items.Clear();
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void AddProcessToListview(ListViewItem lvi)
        {
            try
            {
                lstTasks.Invoke((MethodInvoker)delegate
                {
                    lstTasks.Items.Add(lvi);
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void lstTasks_ColumnClick(object sender, ColumnClickEventArgs e)
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
            lstTasks.Sort();
        }
    }
}