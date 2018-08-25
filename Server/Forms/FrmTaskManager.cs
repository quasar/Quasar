using System;
using System.Windows.Forms;
using Quasar.Common.Messages;
using xServer.Controls;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmTaskManager : Form
    {
        private readonly Client _connectClient;

        public FrmTaskManager(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmTm = this;

            InitializeComponent();
        }

        private void FrmTaskManager_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = WindowHelper.GetWindowTitle("Task Manager", _connectClient);
                _connectClient.Send(new GetProcesses());
            }
        }

        private void FrmTaskManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmTm = null;
        }

        #region "ContextMenuStrip"

        private void killProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                foreach (ListViewItem lvi in lstTasks.SelectedItems)
                {
                    _connectClient.Send(new DoProcessKill {Pid = int.Parse(lvi.SubItems[1].Text)});
                }
            }
        }

        private void startProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string processname = string.Empty;
            if (InputBox.Show("Processname", "Enter Processname:", ref processname) == DialogResult.OK)
            {
                _connectClient?.Send(new DoProcessStart {ApplicationName = processname});
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connectClient?.Send(new GetProcesses());
        }

        #endregion

        public void ClearListviewItems()
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

        public void AddProcessToListview(string processName, int pid, string windowTitle)
        {
            try
            {
                ListViewItem lvi = new ListViewItem(new string[]
                {
                    processName, pid.ToString(), windowTitle
                });

                lstTasks.Invoke((MethodInvoker)delegate
                {
                    lstTasks.Items.Add(lvi);
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void SetProcessesCount(int processesCount)
        {
            try
            {
                statusStrip.Invoke((MethodInvoker) delegate
                {
                    processesToolStripStatusLabel.Text = "Processes: " + processesCount.ToString();
                });
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}