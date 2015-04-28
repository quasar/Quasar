using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Misc;

namespace xServer.Forms
{
    public partial class FrmAddToAutostart : Form
    {
        public FrmAddToAutostart()
        {
            InitializeComponent();
            AddTypes();
        }

        public FrmAddToAutostart(string StartupPath)
        {
            InitializeComponent();
            AddTypes();

            txtName.Text = Path.GetFileNameWithoutExtension(StartupPath);
            txtPath.Text = StartupPath;
        }

        private void AddTypes()
        {
            cmbType.Items.Add("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            cmbType.Items.Add("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
            cmbType.Items.Add("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            cmbType.Items.Add("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
            cmbType.Items.Add("COMMONAPPDATA\\Microsoft\\Windows\\Start Menu\\Programs\\Startup");
            cmbType.Items.Add("APPDATA\\Microsoft\\Windows\\Start Menu\\Programs\\Startup");
            cmbType.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AutostartItem.Name = txtName.Text;
            AutostartItem.Path = txtPath.Text;
            AutostartItem.Type = cmbType.SelectedIndex;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            string illegal = new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars());
            if ((e.KeyChar == '\\' || illegal.Contains(e.KeyChar.ToString())) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtPath_KeyPress(object sender, KeyPressEventArgs e)
        {
            string illegal = new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars());
            if ((e.KeyChar == '\\' || illegal.Contains(e.KeyChar.ToString())) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }
    }
}