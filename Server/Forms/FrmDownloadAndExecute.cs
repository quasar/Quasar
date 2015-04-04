using System;
using System.Windows.Forms;
using xServer.Core.Misc;

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

            DialogResult = DialogResult.OK;
            Close();
        }

        private void FrmDownloadAndExecute_Load(object sender, EventArgs e)
        {
            Text = string.Format("xRAT 2.0 - Download & Execute [Selected: {0}]", _selectedClients);
            txtURL.Text = DownloadAndExecute.URL;
            chkRunHidden.Checked = DownloadAndExecute.RunHidden;
        }
    }
}