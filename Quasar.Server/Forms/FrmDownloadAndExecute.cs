using System;
using System.Windows.Forms;
using Quasar.Server.Helper;

namespace Quasar.Server.Forms
{
    public partial class FrmDownloadAndExecute : Form
    {
        public string Url { get; set; }
        public bool Hidden { get; set; }
        public string Args { get; set; }

        private readonly int _selectedClients;

        public FrmDownloadAndExecute(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void btnDownloadAndExecute_Click(object sender, EventArgs e)
        {
            Url = txtURL.Text;
            Hidden = chkRunHidden.Checked;
            Args = txtArgs.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmDownloadAndExecute_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Download & Execute", _selectedClients);
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            txtArgs.ReadOnly = string.IsNullOrEmpty(txtURL.Text);
        }
    }
}