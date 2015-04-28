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
                    var filePath = Path.Combine(ofd.InitialDirectory, ofd.FileName);
                    txtPath.Text = filePath;

                    Core.Misc.UploadAndExecute.FilePath = File.Exists(filePath) ? filePath : "";
                    Core.Misc.UploadAndExecute.RunHidden = chkRunHidden.Checked;
                }
            }
        }

        private void btnUploadAndExecute_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}