namespace xServer.Forms
{
    partial class FrmBuilder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBuilder));
            this.groupConnection = new System.Windows.Forms.GroupBox();
            this.lblMS = new System.Windows.Forms.Label();
            this.txtDelay = new System.Windows.Forms.TextBox();
            this.lblDelay = new System.Windows.Forms.Label();
            this.chkShowPass = new System.Windows.Forms.CheckBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.groupInstall = new System.Windows.Forms.GroupBox();
            this.chkIconChange = new System.Windows.Forms.CheckBox();
            this.chkElevation = new System.Windows.Forms.CheckBox();
            this.picUAC2 = new System.Windows.Forms.PictureBox();
            this.picUAC1 = new System.Windows.Forms.PictureBox();
            this.rbSystem = new System.Windows.Forms.RadioButton();
            this.rbProgramFiles = new System.Windows.Forms.RadioButton();
            this.txtRegistryKeyName = new System.Windows.Forms.TextBox();
            this.lblRegistryKeyName = new System.Windows.Forms.Label();
            this.chkStartup = new System.Windows.Forms.CheckBox();
            this.chkHide = new System.Windows.Forms.CheckBox();
            this.btnMutex = new System.Windows.Forms.Button();
            this.lblExamplePath = new System.Windows.Forms.Label();
            this.txtExamplePath = new System.Windows.Forms.TextBox();
            this.txtInstallsub = new System.Windows.Forms.TextBox();
            this.lblInstallsub = new System.Windows.Forms.Label();
            this.lblInstallpath = new System.Windows.Forms.Label();
            this.rbAppdata = new System.Windows.Forms.RadioButton();
            this.txtMutex = new System.Windows.Forms.TextBox();
            this.lblMutex = new System.Windows.Forms.Label();
            this.lblExtension = new System.Windows.Forms.Label();
            this.txtInstallname = new System.Windows.Forms.TextBox();
            this.lblInstallname = new System.Windows.Forms.Label();
            this.chkInstall = new System.Windows.Forms.CheckBox();
            this.btnBuild = new System.Windows.Forms.Button();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.groupAsmInfo = new System.Windows.Forms.GroupBox();
            this.chkChangeAsmInfo = new System.Windows.Forms.CheckBox();
            this.txtFileVersion = new System.Windows.Forms.TextBox();
            this.lblFileVersion = new System.Windows.Forms.Label();
            this.txtProductVersion = new System.Windows.Forms.TextBox();
            this.lblProductVersion = new System.Windows.Forms.Label();
            this.txtOriginalFilename = new System.Windows.Forms.TextBox();
            this.lblOriginalFilename = new System.Windows.Forms.Label();
            this.txtTrademarks = new System.Windows.Forms.TextBox();
            this.lblTrademarks = new System.Windows.Forms.Label();
            this.txtCopyright = new System.Windows.Forms.TextBox();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.txtCompanyName = new System.Windows.Forms.TextBox();
            this.lblCompanyName = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.lblProductName = new System.Windows.Forms.Label();
            this.groupConnection.SuspendLayout();
            this.groupInstall.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUAC2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUAC1)).BeginInit();
            this.groupAsmInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupConnection
            // 
            this.groupConnection.Controls.Add(this.lblMS);
            this.groupConnection.Controls.Add(this.txtDelay);
            this.groupConnection.Controls.Add(this.lblDelay);
            this.groupConnection.Controls.Add(this.chkShowPass);
            this.groupConnection.Controls.Add(this.txtPassword);
            this.groupConnection.Controls.Add(this.lblPassword);
            this.groupConnection.Controls.Add(this.txtPort);
            this.groupConnection.Controls.Add(this.lblPort);
            this.groupConnection.Controls.Add(this.txtHost);
            this.groupConnection.Controls.Add(this.lblHost);
            this.groupConnection.Location = new System.Drawing.Point(12, 12);
            this.groupConnection.Name = "groupConnection";
            this.groupConnection.Size = new System.Drawing.Size(325, 153);
            this.groupConnection.TabIndex = 0;
            this.groupConnection.TabStop = false;
            this.groupConnection.Text = "Connection";
            // 
            // lblMS
            // 
            this.lblMS.AutoSize = true;
            this.lblMS.Location = new System.Drawing.Point(186, 126);
            this.lblMS.Name = "lblMS";
            this.lblMS.Size = new System.Drawing.Size(21, 13);
            this.lblMS.TabIndex = 9;
            this.lblMS.Text = "ms";
            // 
            // txtDelay
            // 
            this.txtDelay.Location = new System.Drawing.Point(118, 120);
            this.txtDelay.MaxLength = 6;
            this.txtDelay.Name = "txtDelay";
            this.txtDelay.Size = new System.Drawing.Size(66, 22);
            this.txtDelay.TabIndex = 8;
            this.txtDelay.Text = "5000";
            this.txtDelay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDelay_KeyPress);
            // 
            // lblDelay
            // 
            this.lblDelay.AutoSize = true;
            this.lblDelay.Location = new System.Drawing.Point(17, 123);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(95, 13);
            this.lblDelay.TabIndex = 7;
            this.lblDelay.Text = "Reconnect Delay:";
            // 
            // chkShowPass
            // 
            this.chkShowPass.AutoSize = true;
            this.chkShowPass.Location = new System.Drawing.Point(118, 99);
            this.chkShowPass.Name = "chkShowPass";
            this.chkShowPass.Size = new System.Drawing.Size(107, 17);
            this.chkShowPass.TabIndex = 6;
            this.chkShowPass.Text = "Show Password";
            this.chkShowPass.UseVisualStyleBackColor = true;
            this.chkShowPass.CheckedChanged += new System.EventHandler(this.chkShowPass_CheckedChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(118, 71);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.Size = new System.Drawing.Size(201, 22);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(53, 74);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(59, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(118, 43);
            this.txtPort.MaxLength = 5;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(66, 22);
            this.txtPort.TabIndex = 3;
            this.txtPort.TextChanged += new System.EventHandler(this.txtPort_TextChanged);
            this.txtPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort_KeyPress);
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(81, 46);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(31, 13);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(118, 15);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(201, 22);
            this.txtHost.TabIndex = 1;
            this.txtHost.TextChanged += new System.EventHandler(this.txtHost_TextChanged);
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(37, 18);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(75, 13);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "IP/Hostname:";
            // 
            // groupInstall
            // 
            this.groupInstall.Controls.Add(this.chkIconChange);
            this.groupInstall.Controls.Add(this.chkElevation);
            this.groupInstall.Controls.Add(this.picUAC2);
            this.groupInstall.Controls.Add(this.picUAC1);
            this.groupInstall.Controls.Add(this.rbSystem);
            this.groupInstall.Controls.Add(this.rbProgramFiles);
            this.groupInstall.Controls.Add(this.txtRegistryKeyName);
            this.groupInstall.Controls.Add(this.lblRegistryKeyName);
            this.groupInstall.Controls.Add(this.chkStartup);
            this.groupInstall.Controls.Add(this.chkHide);
            this.groupInstall.Controls.Add(this.btnMutex);
            this.groupInstall.Controls.Add(this.lblExamplePath);
            this.groupInstall.Controls.Add(this.txtExamplePath);
            this.groupInstall.Controls.Add(this.txtInstallsub);
            this.groupInstall.Controls.Add(this.lblInstallsub);
            this.groupInstall.Controls.Add(this.lblInstallpath);
            this.groupInstall.Controls.Add(this.rbAppdata);
            this.groupInstall.Controls.Add(this.txtMutex);
            this.groupInstall.Controls.Add(this.lblMutex);
            this.groupInstall.Controls.Add(this.lblExtension);
            this.groupInstall.Controls.Add(this.txtInstallname);
            this.groupInstall.Controls.Add(this.lblInstallname);
            this.groupInstall.Controls.Add(this.chkInstall);
            this.groupInstall.Location = new System.Drawing.Point(12, 171);
            this.groupInstall.Name = "groupInstall";
            this.groupInstall.Size = new System.Drawing.Size(325, 348);
            this.groupInstall.TabIndex = 1;
            this.groupInstall.TabStop = false;
            this.groupInstall.Text = "Install";
            // 
            // chkIconChange
            // 
            this.chkIconChange.AutoSize = true;
            this.chkIconChange.Location = new System.Drawing.Point(118, 326);
            this.chkIconChange.Name = "chkIconChange";
            this.chkIconChange.Size = new System.Drawing.Size(91, 17);
            this.chkIconChange.TabIndex = 20;
            this.chkIconChange.Text = "Change Icon";
            this.tooltip.SetToolTip(this.chkIconChange, "Custom social engineering tactic to elevate Admin privileges.");
            this.chkIconChange.UseVisualStyleBackColor = true;
            this.chkIconChange.CheckedChanged += new System.EventHandler(this.chkIconChange_CheckedChanged);
            // 
            // chkElevation
            // 
            this.chkElevation.AutoSize = true;
            this.chkElevation.Location = new System.Drawing.Point(118, 303);
            this.chkElevation.Name = "chkElevation";
            this.chkElevation.Size = new System.Drawing.Size(147, 17);
            this.chkElevation.TabIndex = 19;
            this.chkElevation.Text = "Enable Admin Elevation";
            this.tooltip.SetToolTip(this.chkElevation, "Custom social engineering tactic to elevate Admin privileges.");
            this.chkElevation.UseVisualStyleBackColor = true;
            this.chkElevation.CheckedChanged += new System.EventHandler(this.chkElevation_CheckedChanged);
            // 
            // picUAC2
            // 
            this.picUAC2.Image = global::xServer.Properties.Resources.uac_shield;
            this.picUAC2.Location = new System.Drawing.Point(240, 138);
            this.picUAC2.Name = "picUAC2";
            this.picUAC2.Size = new System.Drawing.Size(16, 20);
            this.picUAC2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picUAC2.TabIndex = 32;
            this.picUAC2.TabStop = false;
            this.tooltip.SetToolTip(this.picUAC2, "Administrator Privileges are required to install the client in System.");
            // 
            // picUAC1
            // 
            this.picUAC1.Image = global::xServer.Properties.Resources.uac_shield;
            this.picUAC1.Location = new System.Drawing.Point(240, 118);
            this.picUAC1.Name = "picUAC1";
            this.picUAC1.Size = new System.Drawing.Size(16, 20);
            this.picUAC1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picUAC1.TabIndex = 31;
            this.picUAC1.TabStop = false;
            this.tooltip.SetToolTip(this.picUAC1, "Administrator Privileges are required to install the client in Program Files.");
            // 
            // rbSystem
            // 
            this.rbSystem.AutoSize = true;
            this.rbSystem.Location = new System.Drawing.Point(118, 141);
            this.rbSystem.Name = "rbSystem";
            this.rbSystem.Size = new System.Drawing.Size(60, 17);
            this.rbSystem.TabIndex = 10;
            this.rbSystem.TabStop = true;
            this.rbSystem.Text = "System";
            this.tooltip.SetToolTip(this.rbSystem, "Administrator Privileges are required to install the client in System.");
            this.rbSystem.UseVisualStyleBackColor = true;
            this.rbSystem.CheckedChanged += new System.EventHandler(this.rbSystem_CheckedChanged);
            // 
            // rbProgramFiles
            // 
            this.rbProgramFiles.AutoSize = true;
            this.rbProgramFiles.Location = new System.Drawing.Point(118, 118);
            this.rbProgramFiles.Name = "rbProgramFiles";
            this.rbProgramFiles.Size = new System.Drawing.Size(94, 17);
            this.rbProgramFiles.TabIndex = 9;
            this.rbProgramFiles.TabStop = true;
            this.rbProgramFiles.Text = "Program Files";
            this.tooltip.SetToolTip(this.rbProgramFiles, "Administrator Privileges are required to install the client in Program Files.");
            this.rbProgramFiles.UseVisualStyleBackColor = true;
            this.rbProgramFiles.CheckedChanged += new System.EventHandler(this.rbProgramFiles_CheckedChanged);
            // 
            // txtRegistryKeyName
            // 
            this.txtRegistryKeyName.Location = new System.Drawing.Point(118, 275);
            this.txtRegistryKeyName.Name = "txtRegistryKeyName";
            this.txtRegistryKeyName.Size = new System.Drawing.Size(201, 22);
            this.txtRegistryKeyName.TabIndex = 18;
            this.txtRegistryKeyName.TextChanged += new System.EventHandler(this.txtRegistryKeyName_TextChanged);
            // 
            // lblRegistryKeyName
            // 
            this.lblRegistryKeyName.AutoSize = true;
            this.lblRegistryKeyName.Location = new System.Drawing.Point(9, 278);
            this.lblRegistryKeyName.Name = "lblRegistryKeyName";
            this.lblRegistryKeyName.Size = new System.Drawing.Size(103, 13);
            this.lblRegistryKeyName.TabIndex = 17;
            this.lblRegistryKeyName.Text = "Registry Key Name:";
            // 
            // chkStartup
            // 
            this.chkStartup.AutoSize = true;
            this.chkStartup.Location = new System.Drawing.Point(118, 252);
            this.chkStartup.Name = "chkStartup";
            this.chkStartup.Size = new System.Drawing.Size(102, 17);
            this.chkStartup.TabIndex = 16;
            this.chkStartup.Text = "Add to Startup";
            this.chkStartup.UseVisualStyleBackColor = true;
            this.chkStartup.CheckedChanged += new System.EventHandler(this.chkStartup_CheckedChanged);
            // 
            // chkHide
            // 
            this.chkHide.AutoSize = true;
            this.chkHide.Location = new System.Drawing.Point(118, 229);
            this.chkHide.Name = "chkHide";
            this.chkHide.Size = new System.Drawing.Size(71, 17);
            this.chkHide.TabIndex = 15;
            this.chkHide.Text = "Hide File";
            this.chkHide.UseVisualStyleBackColor = true;
            this.chkHide.CheckedChanged += new System.EventHandler(this.chkHide_CheckedChanged);
            // 
            // btnMutex
            // 
            this.btnMutex.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMutex.Location = new System.Drawing.Point(244, 41);
            this.btnMutex.Name = "btnMutex";
            this.btnMutex.Size = new System.Drawing.Size(75, 18);
            this.btnMutex.TabIndex = 3;
            this.btnMutex.Text = "New Mutex";
            this.btnMutex.UseVisualStyleBackColor = true;
            this.btnMutex.Click += new System.EventHandler(this.btnMutex_Click);
            // 
            // lblExamplePath
            // 
            this.lblExamplePath.AutoSize = true;
            this.lblExamplePath.Location = new System.Drawing.Point(34, 204);
            this.lblExamplePath.Name = "lblExamplePath";
            this.lblExamplePath.Size = new System.Drawing.Size(78, 13);
            this.lblExamplePath.TabIndex = 13;
            this.lblExamplePath.Text = "Example Path:";
            // 
            // txtExamplePath
            // 
            this.txtExamplePath.Location = new System.Drawing.Point(118, 201);
            this.txtExamplePath.Name = "txtExamplePath";
            this.txtExamplePath.ReadOnly = true;
            this.txtExamplePath.Size = new System.Drawing.Size(201, 22);
            this.txtExamplePath.TabIndex = 14;
            // 
            // txtInstallsub
            // 
            this.txtInstallsub.Location = new System.Drawing.Point(118, 173);
            this.txtInstallsub.Name = "txtInstallsub";
            this.txtInstallsub.Size = new System.Drawing.Size(201, 22);
            this.txtInstallsub.TabIndex = 12;
            this.txtInstallsub.TextChanged += new System.EventHandler(this.txtInstallsub_TextChanged);
            this.txtInstallsub.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInstallsub_KeyPress);
            // 
            // lblInstallsub
            // 
            this.lblInstallsub.AutoSize = true;
            this.lblInstallsub.Location = new System.Drawing.Point(17, 176);
            this.lblInstallsub.Name = "lblInstallsub";
            this.lblInstallsub.Size = new System.Drawing.Size(95, 13);
            this.lblInstallsub.TabIndex = 11;
            this.lblInstallsub.Text = "Install Subfolder:";
            // 
            // lblInstallpath
            // 
            this.lblInstallpath.AutoSize = true;
            this.lblInstallpath.Location = new System.Drawing.Point(45, 97);
            this.lblInstallpath.Name = "lblInstallpath";
            this.lblInstallpath.Size = new System.Drawing.Size(67, 13);
            this.lblInstallpath.TabIndex = 7;
            this.lblInstallpath.Text = "Install Path:";
            // 
            // rbAppdata
            // 
            this.rbAppdata.AutoSize = true;
            this.rbAppdata.Checked = true;
            this.rbAppdata.Location = new System.Drawing.Point(118, 95);
            this.rbAppdata.Name = "rbAppdata";
            this.rbAppdata.Size = new System.Drawing.Size(111, 17);
            this.rbAppdata.TabIndex = 8;
            this.rbAppdata.TabStop = true;
            this.rbAppdata.Text = "Application Data";
            this.rbAppdata.UseVisualStyleBackColor = true;
            this.rbAppdata.CheckedChanged += new System.EventHandler(this.rbAppdata_CheckedChanged);
            // 
            // txtMutex
            // 
            this.txtMutex.Location = new System.Drawing.Point(118, 15);
            this.txtMutex.MaxLength = 64;
            this.txtMutex.Name = "txtMutex";
            this.txtMutex.Size = new System.Drawing.Size(201, 22);
            this.txtMutex.TabIndex = 1;
            this.txtMutex.TextChanged += new System.EventHandler(this.txtMutex_TextChanged);
            // 
            // lblMutex
            // 
            this.lblMutex.AutoSize = true;
            this.lblMutex.Location = new System.Drawing.Point(74, 18);
            this.lblMutex.Name = "lblMutex";
            this.lblMutex.Size = new System.Drawing.Size(42, 13);
            this.lblMutex.TabIndex = 0;
            this.lblMutex.Text = "Mutex:";
            // 
            // lblExtension
            // 
            this.lblExtension.AutoSize = true;
            this.lblExtension.Location = new System.Drawing.Point(288, 71);
            this.lblExtension.Name = "lblExtension";
            this.lblExtension.Size = new System.Drawing.Size(27, 13);
            this.lblExtension.TabIndex = 6;
            this.lblExtension.Text = ".exe";
            // 
            // txtInstallname
            // 
            this.txtInstallname.Location = new System.Drawing.Point(118, 65);
            this.txtInstallname.Name = "txtInstallname";
            this.txtInstallname.Size = new System.Drawing.Size(168, 22);
            this.txtInstallname.TabIndex = 5;
            this.txtInstallname.TextChanged += new System.EventHandler(this.txtInstallname_TextChanged);
            this.txtInstallname.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInstallname_KeyPress);
            // 
            // lblInstallname
            // 
            this.lblInstallname.AutoSize = true;
            this.lblInstallname.Location = new System.Drawing.Point(40, 68);
            this.lblInstallname.Name = "lblInstallname";
            this.lblInstallname.Size = new System.Drawing.Size(73, 13);
            this.lblInstallname.TabIndex = 4;
            this.lblInstallname.Text = "Install Name:";
            // 
            // chkInstall
            // 
            this.chkInstall.AutoSize = true;
            this.chkInstall.Location = new System.Drawing.Point(118, 42);
            this.chkInstall.Name = "chkInstall";
            this.chkInstall.Size = new System.Drawing.Size(90, 17);
            this.chkInstall.TabIndex = 2;
            this.chkInstall.Text = "Install Client";
            this.chkInstall.UseVisualStyleBackColor = true;
            this.chkInstall.CheckedChanged += new System.EventHandler(this.chkInstall_CheckedChanged);
            // 
            // btnBuild
            // 
            this.btnBuild.Location = new System.Drawing.Point(217, 525);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(121, 23);
            this.btnBuild.TabIndex = 3;
            this.btnBuild.Text = "Build client!";
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // groupAsmInfo
            // 
            this.groupAsmInfo.Controls.Add(this.chkChangeAsmInfo);
            this.groupAsmInfo.Controls.Add(this.txtFileVersion);
            this.groupAsmInfo.Controls.Add(this.lblFileVersion);
            this.groupAsmInfo.Controls.Add(this.txtProductVersion);
            this.groupAsmInfo.Controls.Add(this.lblProductVersion);
            this.groupAsmInfo.Controls.Add(this.txtOriginalFilename);
            this.groupAsmInfo.Controls.Add(this.lblOriginalFilename);
            this.groupAsmInfo.Controls.Add(this.txtTrademarks);
            this.groupAsmInfo.Controls.Add(this.lblTrademarks);
            this.groupAsmInfo.Controls.Add(this.txtCopyright);
            this.groupAsmInfo.Controls.Add(this.lblCopyright);
            this.groupAsmInfo.Controls.Add(this.txtCompanyName);
            this.groupAsmInfo.Controls.Add(this.lblCompanyName);
            this.groupAsmInfo.Controls.Add(this.txtDescription);
            this.groupAsmInfo.Controls.Add(this.lblDescription);
            this.groupAsmInfo.Controls.Add(this.txtProductName);
            this.groupAsmInfo.Controls.Add(this.lblProductName);
            this.groupAsmInfo.Location = new System.Drawing.Point(343, 12);
            this.groupAsmInfo.Name = "groupAsmInfo";
            this.groupAsmInfo.Size = new System.Drawing.Size(317, 269);
            this.groupAsmInfo.TabIndex = 2;
            this.groupAsmInfo.TabStop = false;
            this.groupAsmInfo.Text = "Assembly Information";
            // 
            // chkChangeAsmInfo
            // 
            this.chkChangeAsmInfo.AutoSize = true;
            this.chkChangeAsmInfo.Location = new System.Drawing.Point(110, 17);
            this.chkChangeAsmInfo.Name = "chkChangeAsmInfo";
            this.chkChangeAsmInfo.Size = new System.Drawing.Size(180, 17);
            this.chkChangeAsmInfo.TabIndex = 0;
            this.chkChangeAsmInfo.Text = "Change Assembly Information";
            this.chkChangeAsmInfo.UseVisualStyleBackColor = true;
            this.chkChangeAsmInfo.CheckedChanged += new System.EventHandler(this.chkChangeAsmInfo_CheckedChanged);
            // 
            // txtFileVersion
            // 
            this.txtFileVersion.Location = new System.Drawing.Point(110, 236);
            this.txtFileVersion.Name = "txtFileVersion";
            this.txtFileVersion.Size = new System.Drawing.Size(201, 22);
            this.txtFileVersion.TabIndex = 16;
            this.txtFileVersion.TextChanged += new System.EventHandler(this.txtFileVersion_TextChanged);
            // 
            // lblFileVersion
            // 
            this.lblFileVersion.AutoSize = true;
            this.lblFileVersion.Location = new System.Drawing.Point(34, 239);
            this.lblFileVersion.Name = "lblFileVersion";
            this.lblFileVersion.Size = new System.Drawing.Size(70, 13);
            this.lblFileVersion.TabIndex = 15;
            this.lblFileVersion.Text = "File Version:";
            // 
            // txtProductVersion
            // 
            this.txtProductVersion.Location = new System.Drawing.Point(110, 208);
            this.txtProductVersion.Name = "txtProductVersion";
            this.txtProductVersion.Size = new System.Drawing.Size(201, 22);
            this.txtProductVersion.TabIndex = 14;
            this.txtProductVersion.TextChanged += new System.EventHandler(this.txtProductVersion_TextChanged);
            // 
            // lblProductVersion
            // 
            this.lblProductVersion.AutoSize = true;
            this.lblProductVersion.Location = new System.Drawing.Point(12, 211);
            this.lblProductVersion.Name = "lblProductVersion";
            this.lblProductVersion.Size = new System.Drawing.Size(92, 13);
            this.lblProductVersion.TabIndex = 13;
            this.lblProductVersion.Text = "Product Version:";
            // 
            // txtOriginalFilename
            // 
            this.txtOriginalFilename.Location = new System.Drawing.Point(110, 180);
            this.txtOriginalFilename.Name = "txtOriginalFilename";
            this.txtOriginalFilename.Size = new System.Drawing.Size(201, 22);
            this.txtOriginalFilename.TabIndex = 12;
            this.txtOriginalFilename.TextChanged += new System.EventHandler(this.txtOriginalFilename_TextChanged);
            // 
            // lblOriginalFilename
            // 
            this.lblOriginalFilename.AutoSize = true;
            this.lblOriginalFilename.Location = new System.Drawing.Point(3, 183);
            this.lblOriginalFilename.Name = "lblOriginalFilename";
            this.lblOriginalFilename.Size = new System.Drawing.Size(101, 13);
            this.lblOriginalFilename.TabIndex = 11;
            this.lblOriginalFilename.Text = "Original Filename:";
            // 
            // txtTrademarks
            // 
            this.txtTrademarks.Location = new System.Drawing.Point(110, 152);
            this.txtTrademarks.Name = "txtTrademarks";
            this.txtTrademarks.Size = new System.Drawing.Size(201, 22);
            this.txtTrademarks.TabIndex = 10;
            this.txtTrademarks.TextChanged += new System.EventHandler(this.txtTrademarks_TextChanged);
            // 
            // lblTrademarks
            // 
            this.lblTrademarks.AutoSize = true;
            this.lblTrademarks.Location = new System.Drawing.Point(36, 155);
            this.lblTrademarks.Name = "lblTrademarks";
            this.lblTrademarks.Size = new System.Drawing.Size(68, 13);
            this.lblTrademarks.TabIndex = 9;
            this.lblTrademarks.Text = "Trademarks:";
            // 
            // txtCopyright
            // 
            this.txtCopyright.Location = new System.Drawing.Point(110, 124);
            this.txtCopyright.Name = "txtCopyright";
            this.txtCopyright.Size = new System.Drawing.Size(201, 22);
            this.txtCopyright.TabIndex = 8;
            this.txtCopyright.TextChanged += new System.EventHandler(this.txtCopyright_TextChanged);
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Location = new System.Drawing.Point(43, 127);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(61, 13);
            this.lblCopyright.TabIndex = 7;
            this.lblCopyright.Text = "Copyright:";
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Location = new System.Drawing.Point(110, 96);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(201, 22);
            this.txtCompanyName.TabIndex = 6;
            this.txtCompanyName.TextChanged += new System.EventHandler(this.txtCompanyName_TextChanged);
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.AutoSize = true;
            this.lblCompanyName.Location = new System.Drawing.Point(14, 99);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(90, 13);
            this.lblCompanyName.TabIndex = 5;
            this.lblCompanyName.Text = "Company Name:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(110, 68);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(201, 22);
            this.txtDescription.TabIndex = 4;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(35, 71);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(69, 13);
            this.lblDescription.TabIndex = 3;
            this.lblDescription.Text = "Description:";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(110, 40);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(201, 22);
            this.txtProductName.TabIndex = 2;
            this.txtProductName.TextChanged += new System.EventHandler(this.txtProductName_TextChanged);
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.Location = new System.Drawing.Point(22, 43);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(82, 13);
            this.lblProductName.TabIndex = 1;
            this.lblProductName.Text = "Product Name:";
            // 
            // FrmBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(673, 560);
            this.Controls.Add(this.groupAsmInfo);
            this.Controls.Add(this.btnBuild);
            this.Controls.Add(this.groupInstall);
            this.Controls.Add(this.groupConnection);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmBuilder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Builder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmBuilder_FormClosing);
            this.Load += new System.EventHandler(this.FrmBuilder_Load);
            this.groupConnection.ResumeLayout(false);
            this.groupConnection.PerformLayout();
            this.groupInstall.ResumeLayout(false);
            this.groupInstall.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUAC2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUAC1)).EndInit();
            this.groupAsmInfo.ResumeLayout(false);
            this.groupAsmInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupConnection;
        private System.Windows.Forms.CheckBox chkShowPass;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtDelay;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.GroupBox groupInstall;
        private System.Windows.Forms.CheckBox chkInstall;
        private System.Windows.Forms.TextBox txtInstallname;
        private System.Windows.Forms.Label lblInstallname;
        private System.Windows.Forms.TextBox txtMutex;
        private System.Windows.Forms.Label lblMutex;
        private System.Windows.Forms.Label lblExtension;
        private System.Windows.Forms.Label lblInstallpath;
        private System.Windows.Forms.RadioButton rbAppdata;
        private System.Windows.Forms.TextBox txtInstallsub;
        private System.Windows.Forms.Label lblInstallsub;
        private System.Windows.Forms.Label lblExamplePath;
        private System.Windows.Forms.TextBox txtExamplePath;
        private System.Windows.Forms.Button btnMutex;
        private System.Windows.Forms.CheckBox chkHide;
        private System.Windows.Forms.TextBox txtRegistryKeyName;
        private System.Windows.Forms.Label lblRegistryKeyName;
        private System.Windows.Forms.CheckBox chkStartup;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.Label lblMS;
        private System.Windows.Forms.RadioButton rbSystem;
        private System.Windows.Forms.RadioButton rbProgramFiles;
        private System.Windows.Forms.PictureBox picUAC1;
        private System.Windows.Forms.PictureBox picUAC2;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.CheckBox chkElevation;
        private System.Windows.Forms.CheckBox chkIconChange;
        private System.Windows.Forms.GroupBox groupAsmInfo;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.TextBox txtOriginalFilename;
        private System.Windows.Forms.Label lblOriginalFilename;
        private System.Windows.Forms.TextBox txtTrademarks;
        private System.Windows.Forms.Label lblTrademarks;
        private System.Windows.Forms.TextBox txtCopyright;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.TextBox txtCompanyName;
        private System.Windows.Forms.Label lblCompanyName;
        private System.Windows.Forms.TextBox txtFileVersion;
        private System.Windows.Forms.Label lblFileVersion;
        private System.Windows.Forms.TextBox txtProductVersion;
        private System.Windows.Forms.Label lblProductVersion;
        private System.Windows.Forms.CheckBox chkChangeAsmInfo;
    }
}