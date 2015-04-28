using System;
using System.Windows.Forms;

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
            this.Text = string.Format("xRAT 2.0 - Visit Website [Selected: {0}]", _selectedClients);
            txtURL.Text = Core.Misc.VisitWebsite.URL;
            chkVisitHidden.Checked = Core.Misc.VisitWebsite.Hidden;
        }

        private void btnVisitWebsite_Click(object sender, EventArgs e)
        {
            Core.Misc.VisitWebsite.URL = txtURL.Text;
            Core.Misc.VisitWebsite.Hidden = chkVisitHidden.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}