namespace xServer.Forms
{
    partial class FrmShowMessagebox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmShowMessagebox));
            this.groupMsgSettings = new System.Windows.Forms.GroupBox();
            this.lblCaption = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.txtCaption = new System.Windows.Forms.TextBox();
            this.txtText = new System.Windows.Forms.TextBox();
            this.lblMsgButtons = new System.Windows.Forms.Label();
            this.cmbMsgButtons = new System.Windows.Forms.ComboBox();
            this.lblMsgIcon = new System.Windows.Forms.Label();
            this.cmbMsgIcon = new System.Windows.Forms.ComboBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.groupMsgSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupMsgSettings
            // 
            this.groupMsgSettings.Controls.Add(this.cmbMsgIcon);
            this.groupMsgSettings.Controls.Add(this.lblMsgIcon);
            this.groupMsgSettings.Controls.Add(this.cmbMsgButtons);
            this.groupMsgSettings.Controls.Add(this.lblMsgButtons);
            this.groupMsgSettings.Controls.Add(this.txtText);
            this.groupMsgSettings.Controls.Add(this.txtCaption);
            this.groupMsgSettings.Controls.Add(this.lblText);
            this.groupMsgSettings.Controls.Add(this.lblCaption);
            this.groupMsgSettings.Location = new System.Drawing.Point(12, 12);
            this.groupMsgSettings.Name = "groupMsgSettings";
            this.groupMsgSettings.Size = new System.Drawing.Size(325, 146);
            this.groupMsgSettings.TabIndex = 0;
            this.groupMsgSettings.TabStop = false;
            this.groupMsgSettings.Text = "Messagebox Settings";
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(6, 24);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(51, 13);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "Caption:";
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(24, 52);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(30, 13);
            this.lblText.TabIndex = 3;
            this.lblText.Text = "Text:";
            // 
            // txtCaption
            // 
            this.txtCaption.Location = new System.Drawing.Point(60, 21);
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Size = new System.Drawing.Size(249, 22);
            this.txtCaption.TabIndex = 2;
            this.txtCaption.Text = "Error";
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(60, 49);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(249, 22);
            this.txtText.TabIndex = 4;
            this.txtText.Text = "The application could not be started.";
            // 
            // lblMsgButtons
            // 
            this.lblMsgButtons.AutoSize = true;
            this.lblMsgButtons.Location = new System.Drawing.Point(23, 83);
            this.lblMsgButtons.Name = "lblMsgButtons";
            this.lblMsgButtons.Size = new System.Drawing.Size(118, 13);
            this.lblMsgButtons.TabIndex = 5;
            this.lblMsgButtons.Text = "Messagebox Buttons:";
            // 
            // cmbMsgButtons
            // 
            this.cmbMsgButtons.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMsgButtons.FormattingEnabled = true;
            this.cmbMsgButtons.Location = new System.Drawing.Point(147, 80);
            this.cmbMsgButtons.Name = "cmbMsgButtons";
            this.cmbMsgButtons.Size = new System.Drawing.Size(162, 21);
            this.cmbMsgButtons.TabIndex = 6;
            // 
            // lblMsgIcon
            // 
            this.lblMsgIcon.AutoSize = true;
            this.lblMsgIcon.Location = new System.Drawing.Point(42, 110);
            this.lblMsgIcon.Name = "lblMsgIcon";
            this.lblMsgIcon.Size = new System.Drawing.Size(99, 13);
            this.lblMsgIcon.TabIndex = 7;
            this.lblMsgIcon.Text = "Messagebox Icon:";
            // 
            // cmbMsgIcon
            // 
            this.cmbMsgIcon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMsgIcon.FormattingEnabled = true;
            this.cmbMsgIcon.Location = new System.Drawing.Point(147, 107);
            this.cmbMsgIcon.Name = "cmbMsgIcon";
            this.cmbMsgIcon.Size = new System.Drawing.Size(162, 21);
            this.cmbMsgIcon.TabIndex = 8;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(181, 164);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(262, 164);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // frmShowMessagebox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 199);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.groupMsgSettings);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmShowMessagebox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Show Messagebox []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmShowMessagebox_FormClosing);
            this.Load += new System.EventHandler(this.FrmShowMessagebox_Load);
            this.groupMsgSettings.ResumeLayout(false);
            this.groupMsgSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupMsgSettings;
        private System.Windows.Forms.ComboBox cmbMsgIcon;
        private System.Windows.Forms.Label lblMsgIcon;
        private System.Windows.Forms.ComboBox cmbMsgButtons;
        private System.Windows.Forms.Label lblMsgButtons;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.TextBox txtCaption;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnSend;
    }
}