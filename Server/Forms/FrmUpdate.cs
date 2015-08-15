using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Helper;

namespace xServer.Forms
{
    public partial class FrmUpdate : Form
    {
        private readonly int _selectedClients;

        public FrmUpdate(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Update Clients", _selectedClients);
            if (Core.Data.Update.UseDownload)
                radioURL.Checked = true;
            txtPath.Text = File.Exists(Core.Data.Update.UploadPath) ? Core.Data.Update.UploadPath : string.Empty;
            txtURL.Text = Core.Data.Update.DownloadURL;
            btnUpdate.Text = "Update Client" + ((_selectedClients > 1) ? "s" : string.Empty);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Core.Data.Update.UseDownload = radioURL.Checked;
            Core.Data.Update.UploadPath = File.Exists(txtPath.Text) ? txtPath.Text : string.Empty;
            Core.Data.Update.DownloadURL = txtURL.Text;

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
                    txtPath.Text = Path.Combine(ofd.InitialDirectory, ofd.FileName);
                }
            }
        }

        private void radioLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            groupLocalFile.Enabled = radioLocalFile.Checked;
            groupURL.Enabled = !radioLocalFile.Checked;
        }

        private void radioURL_CheckedChanged(object sender, EventArgs e)
        {
            groupLocalFile.Enabled = !radioURL.Checked;
            groupURL.Enabled = radioURL.Checked;
        }

    }
}