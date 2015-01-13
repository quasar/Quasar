using System;
using System.Windows.Forms;

namespace xServer.Forms
{
    public partial class frmDownloadAndExecute : Form
    {
        private int selectedClients;

        public frmDownloadAndExecute(int selected)
        {
            selectedClients = selected;
            InitializeComponent();
        }

        private void btnDownloadAndExecute_Click(object sender, EventArgs e)
        {
            DownloadAndExecute.URL = txtURL.Text;
            DownloadAndExecute.RunHidden = chkRunHidden.Checked;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmDownloadAndExecute_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Download & Execute [Selected: {0}]", selectedClients);
            txtURL.Text = DownloadAndExecute.URL;
            chkRunHidden.Checked = DownloadAndExecute.RunHidden;
        }
    }

    public class DownloadAndExecute
    {
        public static string URL { get; set; }
        public static bool RunHidden { get; set; }
    }
}
