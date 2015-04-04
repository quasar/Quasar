using System;
using System.Windows.Forms;
using xServer.Core.Misc;

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
            Text = string.Format("xRAT 2.0 - Visit Website [Selected: {0}]", _selectedClients);
            txtURL.Text = VisitWebsite.URL;
            chkVisitHidden.Checked = VisitWebsite.Hidden;
        }

        private void btnVisitWebsite_Click(object sender, EventArgs e)
        {
            VisitWebsite.URL = txtURL.Text;
            VisitWebsite.Hidden = chkVisitHidden.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}