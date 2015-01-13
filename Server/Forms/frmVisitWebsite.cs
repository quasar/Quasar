using System;
using System.Windows.Forms;

namespace xServer.Forms
{
    public partial class frmVisitWebsite : Form
    {
        private int selectedClients;

        public frmVisitWebsite(int selected)
        {
            selectedClients = selected;
            InitializeComponent();
        }

        private void btnVisitWebsite_Click(object sender, EventArgs e)
        {
            VisitWebsite.URL = txtURL.Text;
            VisitWebsite.Hidden = chkVisitHidden.Checked;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmVisitWebsite_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Visit Website [Selected: {0}]", selectedClients);
            txtURL.Text = VisitWebsite.URL;
            chkVisitHidden.Checked = VisitWebsite.Hidden;
        }
    }

    public class VisitWebsite
    {
        public static string URL { get; set; }
        public static bool Hidden { get; set; }
    }
}
