namespace xServer.Forms
{
    partial class FrmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettings));
            this.btnSave = new System.Windows.Forms.Button();
            this.lblPort = new System.Windows.Forms.Label();
            this.ncPort = new System.Windows.Forms.NumericUpDown();
            this.chkAutoListen = new System.Windows.Forms.CheckBox();
            this.chkPopup = new System.Windows.Forms.CheckBox();
            this.btnListen = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkUseUpnp = new System.Windows.Forms.CheckBox();
            this.chkShowTooltip = new System.Windows.Forms.CheckBox();
            this.chkNoIPIntegration = new System.Windows.Forms.CheckBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.lblPass = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtNoIPPass = new System.Windows.Forms.TextBox();
            this.txtNoIPUser = new System.Windows.Forms.TextBox();
            this.txtNoIPHost = new System.Windows.Forms.TextBox();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.chkIPv6Support = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.ncPort)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(227, 296);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(12, 11);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(93, 13);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Port to listen on:";
            // 
            // ncPort
            // 
            this.ncPort.Location = new System.Drawing.Point(111, 7);
            this.ncPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ncPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ncPort.Name = "ncPort";
            this.ncPort.Size = new System.Drawing.Size(75, 22);
            this.ncPort.TabIndex = 1;
            this.ncPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkAutoListen
            // 
            this.chkAutoListen.AutoSize = true;
            this.chkAutoListen.Location = new System.Drawing.Point(15, 86);
            this.chkAutoListen.Name = "chkAutoListen";
            this.chkAutoListen.Size = new System.Drawing.Size(222, 17);
            this.chkAutoListen.TabIndex = 6;
            this.chkAutoListen.Text = "Listen for new connections on startup";
            this.chkAutoListen.UseVisualStyleBackColor = true;
            // 
            // chkPopup
            // 
            this.chkPopup.AutoSize = true;
            this.chkPopup.Location = new System.Drawing.Point(15, 109);
            this.chkPopup.Name = "chkPopup";
            this.chkPopup.Size = new System.Drawing.Size(259, 17);
            this.chkPopup.TabIndex = 7;
            this.chkPopup.Text = "Show popup notification on new connection";
            this.chkPopup.UseVisualStyleBackColor = true;
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(192, 6);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(110, 23);
            this.btnListen.TabIndex = 2;
            this.btnListen.Text = "Start listening";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(146, 296);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 38);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(59, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(111, 35);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(158, 22);
            this.txtPassword.TabIndex = 4;
            // 
            // chkUseUpnp
            // 
            this.chkUseUpnp.AutoSize = true;
            this.chkUseUpnp.Location = new System.Drawing.Point(15, 132);
            this.chkUseUpnp.Name = "chkUseUpnp";
            this.chkUseUpnp.Size = new System.Drawing.Size(230, 17);
            this.chkUseUpnp.TabIndex = 8;
            this.chkUseUpnp.Text = "Try to automatically port forward (UPnP)";
            this.chkUseUpnp.UseVisualStyleBackColor = true;
            // 
            // chkShowTooltip
            // 
            this.chkShowTooltip.AutoSize = true;
            this.chkShowTooltip.Location = new System.Drawing.Point(15, 155);
            this.chkShowTooltip.Name = "chkShowTooltip";
            this.chkShowTooltip.Size = new System.Drawing.Size(268, 17);
            this.chkShowTooltip.TabIndex = 9;
            this.chkShowTooltip.Text = "Show tooltip on client with system information";
            this.chkShowTooltip.UseVisualStyleBackColor = true;
            // 
            // chkNoIPIntegration
            // 
            this.chkNoIPIntegration.AutoSize = true;
            this.chkNoIPIntegration.Location = new System.Drawing.Point(15, 178);
            this.chkNoIPIntegration.Name = "chkNoIPIntegration";
            this.chkNoIPIntegration.Size = new System.Drawing.Size(192, 17);
            this.chkNoIPIntegration.TabIndex = 10;
            this.chkNoIPIntegration.Text = "Activate No-Ip.com DNS Updater";
            this.chkNoIPIntegration.UseVisualStyleBackColor = true;
            this.chkNoIPIntegration.CheckedChanged += new System.EventHandler(this.chkNoIPIntegration_CheckedChanged);
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Enabled = false;
            this.lblHost.Location = new System.Drawing.Point(33, 204);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(34, 13);
            this.lblHost.TabIndex = 11;
            this.lblHost.Text = "Host:";
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Enabled = false;
            this.lblPass.Location = new System.Drawing.Point(170, 232);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(32, 13);
            this.lblPass.TabIndex = 15;
            this.lblPass.Text = "Pass:";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Enabled = false;
            this.lblUser.Location = new System.Drawing.Point(33, 232);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(32, 13);
            this.lblUser.TabIndex = 13;
            this.lblUser.Text = "Mail:";
            // 
            // txtNoIPPass
            // 
            this.txtNoIPPass.Enabled = false;
            this.txtNoIPPass.Location = new System.Drawing.Point(202, 229);
            this.txtNoIPPass.Name = "txtNoIPPass";
            this.txtNoIPPass.Size = new System.Drawing.Size(100, 22);
            this.txtNoIPPass.TabIndex = 16;
            // 
            // txtNoIPUser
            // 
            this.txtNoIPUser.Enabled = false;
            this.txtNoIPUser.Location = new System.Drawing.Point(73, 229);
            this.txtNoIPUser.Name = "txtNoIPUser";
            this.txtNoIPUser.Size = new System.Drawing.Size(91, 22);
            this.txtNoIPUser.TabIndex = 14;
            // 
            // txtNoIPHost
            // 
            this.txtNoIPHost.Enabled = false;
            this.txtNoIPHost.Location = new System.Drawing.Point(73, 201);
            this.txtNoIPHost.Name = "txtNoIPHost";
            this.txtNoIPHost.Size = new System.Drawing.Size(229, 22);
            this.txtNoIPHost.TabIndex = 12;
            // 
            // chkShowPassword
            // 
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Enabled = false;
            this.chkShowPassword.Location = new System.Drawing.Point(195, 257);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new System.Drawing.Size(107, 17);
            this.chkShowPassword.TabIndex = 17;
            this.chkShowPassword.Text = "Show Password";
            this.chkShowPassword.UseVisualStyleBackColor = true;
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);
            // 
            // chkIPv6Support
            // 
            this.chkIPv6Support.AutoSize = true;
            this.chkIPv6Support.Location = new System.Drawing.Point(15, 63);
            this.chkIPv6Support.Name = "chkIPv6Support";
            this.chkIPv6Support.Size = new System.Drawing.Size(128, 17);
            this.chkIPv6Support.TabIndex = 5;
            this.chkIPv6Support.Text = "Enable IPv6 support";
            this.chkIPv6Support.UseVisualStyleBackColor = true;
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(314, 333);
            this.Controls.Add(this.chkIPv6Support);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(this.txtNoIPHost);
            this.Controls.Add(this.txtNoIPUser);
            this.Controls.Add(this.txtNoIPPass);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lblPass);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.chkNoIPIntegration);
            this.Controls.Add(this.chkShowTooltip);
            this.Controls.Add(this.chkUseUpnp);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.chkPopup);
            this.Controls.Add(this.chkAutoListen);
            this.Controls.Add(this.ncPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.btnSave);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FrmSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ncPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown ncPort;
        private System.Windows.Forms.CheckBox chkAutoListen;
        private System.Windows.Forms.CheckBox chkPopup;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkUseUpnp;
        private System.Windows.Forms.CheckBox chkShowTooltip;
        private System.Windows.Forms.CheckBox chkNoIPIntegration;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtNoIPPass;
        private System.Windows.Forms.TextBox txtNoIPUser;
        private System.Windows.Forms.TextBox txtNoIPHost;
        private System.Windows.Forms.CheckBox chkShowPassword;
        private System.Windows.Forms.CheckBox chkIPv6Support;
    }
}