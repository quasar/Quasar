namespace xClient.Core.Elevation
{
    partial class FrmElevation
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
            this.lblHead = new System.Windows.Forms.Label();
            this.picError = new System.Windows.Forms.PictureBox();
            this.panelBot = new System.Windows.Forms.Panel();
            this.linkError = new System.Windows.Forms.LinkLabel();
            this.picInfo = new System.Windows.Forms.PictureBox();
            this.lblText = new System.Windows.Forms.Label();
            this.btnRestoreAndCheck = new Core.Elevation.CommandButton();
            this.btnRestore = new Core.Elevation.CommandButton();
            ((System.ComponentModel.ISupportInitialize)(this.picError)).BeginInit();
            this.panelBot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHead
            // 
            this.lblHead.AutoSize = true;
            this.lblHead.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHead.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblHead.Location = new System.Drawing.Point(60, 12);
            this.lblHead.Name = "lblHead";
            this.lblHead.Size = new System.Drawing.Size(79, 20);
            this.lblHead.TabIndex = 2;
            this.lblHead.Text = "%ERROR%";
            // 
            // picError
            // 
            this.picError.Location = new System.Drawing.Point(12, 12);
            this.picError.Name = "picError";
            this.picError.Size = new System.Drawing.Size(42, 42);
            this.picError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picError.TabIndex = 3;
            this.picError.TabStop = false;
            // 
            // panelBot
            // 
            this.panelBot.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBot.Controls.Add(this.linkError);
            this.panelBot.Controls.Add(this.picInfo);
            this.panelBot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBot.Location = new System.Drawing.Point(0, 245);
            this.panelBot.Name = "panelBot";
            this.panelBot.Size = new System.Drawing.Size(542, 38);
            this.panelBot.TabIndex = 4;
            // 
            // linkError
            // 
            this.linkError.AutoSize = true;
            this.linkError.Location = new System.Drawing.Point(37, 11);
            this.linkError.Name = "linkError";
            this.linkError.Size = new System.Drawing.Size(88, 13);
            this.linkError.TabIndex = 1;
            this.linkError.TabStop = true;
            this.linkError.Text = "%MOREDETAILS";
            this.linkError.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkError_LinkClicked);
            // 
            // picInfo
            // 
            this.picInfo.Image = global::xClient.Properties.Resources.information;
            this.picInfo.Location = new System.Drawing.Point(12, 10);
            this.picInfo.Name = "picInfo";
            this.picInfo.Size = new System.Drawing.Size(16, 16);
            this.picInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picInfo.TabIndex = 0;
            this.picInfo.TabStop = false;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(61, 48);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(47, 13);
            this.lblText.TabIndex = 5;
            this.lblText.Text = "%TEXT%";
            // 
            // btnRestoreAndCheck
            // 
            this.btnRestoreAndCheck.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRestoreAndCheck.Location = new System.Drawing.Point(12, 190);
            this.btnRestoreAndCheck.Name = "btnRestoreAndCheck";
            this.btnRestoreAndCheck.Size = new System.Drawing.Size(518, 42);
            this.btnRestoreAndCheck.TabIndex = 1;
            this.btnRestoreAndCheck.Text = "%RESTOREANDCHECK%";
            this.btnRestoreAndCheck.UseVisualStyleBackColor = true;
            this.btnRestoreAndCheck.Click += new System.EventHandler(this.btnRestoreAndCheck_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRestore.Location = new System.Drawing.Point(12, 140);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(518, 42);
            this.btnRestore.TabIndex = 0;
            this.btnRestore.Text = "%RESTORE%";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // frmElevation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(542, 283);
            this.ControlBox = false;
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.picError);
            this.Controls.Add(this.lblHead);
            this.Controls.Add(this.btnRestoreAndCheck);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.panelBot);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmElevation";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "%TITLE%";
            this.TopMost = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FrmElevation_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.picError)).EndInit();
            this.panelBot.ResumeLayout(false);
            this.panelBot.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CommandButton btnRestore;
        private CommandButton btnRestoreAndCheck;
        private System.Windows.Forms.Label lblHead;
        private System.Windows.Forms.PictureBox picError;
        private System.Windows.Forms.Panel panelBot;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.PictureBox picInfo;
        private System.Windows.Forms.LinkLabel linkError;
    }
}