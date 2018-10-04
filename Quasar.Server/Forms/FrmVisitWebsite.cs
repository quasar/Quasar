using System;
using System.Windows.Forms;
using Quasar.Server.Helper;

namespace Quasar.Server.Forms
{
    public partial class FrmVisitWebsite : Form
    {
        public string Url { get; set; }
        public bool Hidden { get; set; }

        private readonly int _selectedClients;

        public FrmVisitWebsite(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void FrmVisitWebsite_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Visit Website", _selectedClients);
        }

        private void btnVisitWebsite_Click(object sender, EventArgs e)
        {
            Url = txtURL.Text;
            Hidden = chkVisitHidden.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}