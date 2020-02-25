using System;
using System.IO;
using System.Windows.Forms;
using Quasar.Server.Helper;

namespace Quasar.Server.Forms
{
    public partial class FrmUploadAndExecute : Form
    {
        public string LocalFilePath { get; set; }
        public bool Hidden { get; set; }

        private readonly int _selectedClients;

        public FrmUploadAndExecute(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void FrmUploadAndExecute_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Upload & Execute", _selectedClients);
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
            LocalFilePath = File.Exists(txtPath.Text) ? txtPath.Text : string.Empty;
            Hidden = chkRunHidden.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}