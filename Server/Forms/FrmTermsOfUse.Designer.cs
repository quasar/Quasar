namespace xServer.Forms
{
    partial class FrmTermsOfUse
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTermsOfUse));
            this.rtxtContent = new System.Windows.Forms.RichTextBox();
            this.lblToU = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnDecline = new System.Windows.Forms.Button();
            this.chkDontShowAgain = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // rtxtContent
            // 
            this.rtxtContent.Location = new System.Drawing.Point(12, 42);
            this.rtxtContent.Name = "rtxtContent";
            this.rtxtContent.ReadOnly = true;
            this.rtxtContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtContent.Size = new System.Drawing.Size(398, 242);
            this.rtxtContent.TabIndex = 0;
            this.rtxtContent.Text = "";
            // 
            // lblToU
            // 
            this.lblToU.AutoSize = true;
            this.lblToU.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToU.Location = new System.Drawing.Point(12, 9);
            this.lblToU.Name = "lblToU";
            this.lblToU.Size = new System.Drawing.Size(140, 30);
            this.lblToU.TabIndex = 1;
            this.lblToU.Text = "Terms of Use";
            // 
            // btnAccept
            // 
            this.btnAccept.Enabled = false;
            this.btnAccept.Location = new System.Drawing.Point(335, 291);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnDecline
            // 
            this.btnDecline.Location = new System.Drawing.Point(254, 291);
            this.btnDecline.Name = "btnDecline";
            this.btnDecline.Size = new System.Drawing.Size(75, 23);
            this.btnDecline.TabIndex = 3;
            this.btnDecline.Text = "Decline";
            this.btnDecline.UseVisualStyleBackColor = true;
            this.btnDecline.Click += new System.EventHandler(this.btnDecline_Click);
            // 
            // chkDontShowAgain
            // 
            this.chkDontShowAgain.AutoSize = true;
            this.chkDontShowAgain.Location = new System.Drawing.Point(12, 295);
            this.chkDontShowAgain.Name = "chkDontShowAgain";
            this.chkDontShowAgain.Size = new System.Drawing.Size(120, 17);
            this.chkDontShowAgain.TabIndex = 4;
            this.chkDontShowAgain.Text = "Don\'t Show Again";
            this.chkDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // frmTermsOfUse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 326);
            this.Controls.Add(this.chkDontShowAgain);
            this.Controls.Add(this.btnDecline);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.lblToU);
            this.Controls.Add(this.rtxtContent);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTermsOfUse";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Terms of Use";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTermsOfUse_FormClosing);
            this.Load += new System.EventHandler(this.FrmTermsOfUse_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxtContent;
        private System.Windows.Forms.Label lblToU;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnDecline;
        private System.Windows.Forms.CheckBox chkDontShowAgain;
    }
}