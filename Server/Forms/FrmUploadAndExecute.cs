using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Misc;

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
            Text = string.Format("xRAT 2.0 - Upload & Execute [Selected: {0}]", _selectedClients);
            chkRunHidden.Checked = UploadAndExecute.RunHidden;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.Filter = "Executable (*.exe)|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var filePath = Path.Combine(ofd.InitialDirectory, ofd.FileName);
                    txtPath.Text = filePath;

                    UploadAndExecute.File = (File.Exists(filePath) ? File.ReadAllBytes(filePath) : new byte[0]);
                    UploadAndExecute.FileName = ofd.SafeFileName;
                    UploadAndExecute.RunHidden = chkRunHidden.Checked;
                }
            }
        }

        private void btnUploadAndExecute_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}