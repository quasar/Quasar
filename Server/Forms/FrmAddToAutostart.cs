using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Utilities;

namespace xServer.Forms
{
    public partial class FrmAddToAutostart : Form
    {
        public FrmAddToAutostart()
        {
            InitializeComponent();
            AddTypes();
        }

        public FrmAddToAutostart(string startupPath)
        {
            InitializeComponent();
            AddTypes();

            txtName.Text = Path.GetFileNameWithoutExtension(startupPath);
            txtPath.Text = startupPath;
        }

        private void AddTypes()
        {
            cmbType.Items.Add("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            cmbType.Items.Add("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
            cmbType.Items.Add("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            cmbType.Items.Add("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
            cmbType.Items.Add("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run");
            cmbType.Items.Add("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
            cmbType.Items.Add("%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Startup");
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
            e.Handled = ((e.KeyChar == '\\' || FileHelper.CheckPathForIllegalChars(e.KeyChar.ToString())) &&
                         !char.IsControl(e.KeyChar));
        }

        private void txtPath_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = ((e.KeyChar == '\\' || FileHelper.CheckPathForIllegalChars(e.KeyChar.ToString())) &&
                         !char.IsControl(e.KeyChar));
        }
    }
}