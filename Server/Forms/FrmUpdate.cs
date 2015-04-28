using System;
using System.Windows.Forms;

namespace xServer.Forms
{
    public partial class FrmUpdate : Form
    {
        private readonly int _selectedClients;

        public FrmUpdate(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Update [Selected: {0}]", _selectedClients);
            txtURL.Text = Core.Misc.Update.DownloadURL;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Core.Misc.Update.DownloadURL = txtURL.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}