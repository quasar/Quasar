using System;
using System.Windows.Forms;

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
            Core.Misc.DownloadAndExecute.URL = txtURL.Text;
            Core.Misc.DownloadAndExecute.RunHidden = chkRunHidden.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmDownloadAndExecute_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Download & Execute [Selected: {0}]", _selectedClients);
            txtURL.Text = Core.Misc.DownloadAndExecute.URL;
            chkRunHidden.Checked = Core.Misc.DownloadAndExecute.RunHidden;
        }
    }
}