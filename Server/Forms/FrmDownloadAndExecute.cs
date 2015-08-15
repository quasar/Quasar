using System;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Utilities;

namespace xServer.Forms
{
    public partial class FrmDownloadAndExecute : Form
    {
        private readonly int _selectedClients;

        public FrmDownloadAndExecute(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void btnDownloadAndExecute_Click(object sender, EventArgs e)
        {
            DownloadAndExecute.URL = txtURL.Text;
            DownloadAndExecute.RunHidden = chkRunHidden.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmDownloadAndExecute_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Download & Execute", _selectedClients);
            txtURL.Text = DownloadAndExecute.URL;
            chkRunHidden.Checked = DownloadAndExecute.RunHidden;
        }
    }
}