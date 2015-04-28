namespace xServer.Forms
{
    partial class FrmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAbout));
            this.picXRAT = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnOkay = new System.Windows.Forms.Button();
            this.lblCoder = new System.Windows.Forms.Label();
            this.lblCredits = new System.Windows.Forms.Label();
            this.rtxtContent = new System.Windows.Forms.RichTextBox();
            this.lblLicense = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picXRAT)).BeginInit();
            this.SuspendLayout();
            // 
            // picXRAT
            // 
            this.picXRAT.Image = global::xServer.Properties.Resources.xRAT_64x64;
            this.picXRAT.Location = new System.Drawing.Point(12, 12);
            this.picXRAT.Name = "picXRAT";
            this.picXRAT.Size = new System.Drawing.Size(64, 64);
            this.picXRAT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picXRAT.TabIndex = 0;
            this.picXRAT.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(82, 26);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(277, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "xRAT 2.0 - complete re-code";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(322, 55);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(67, 13);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "%VERSION%";
            // 
            // btnOkay
            // 
            this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOkay.Location = new System.Drawing.Point(341, 406);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(75, 23);
            this.btnOkay.TabIndex = 6;
            this.btnOkay.Text = "&Okay";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // lblCoder
            // 
            this.lblCoder.AutoSize = true;
            this.lblCoder.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoder.Location = new System.Drawing.Point(84, 59);
            this.lblCoder.Name = "lblCoder";
            this.lblCoder.Size = new System.Drawing.Size(138, 17);
            this.lblCoder.TabIndex = 2;
            this.lblCoder.Text = "Developed by MaxX0r";
            // 
            // lblCredits
            // 
            this.lblCredits.AutoSize = true;
            this.lblCredits.Location = new System.Drawing.Point(12, 90);
            this.lblCredits.Name = "lblCredits";
            this.lblCredits.Size = new System.Drawing.Size(67, 13);
            this.lblCredits.TabIndex = 3;
            this.lblCredits.Text = "%CREDITS%";
            // 
            // rtxtContent
            // 
            this.rtxtContent.Location = new System.Drawing.Point(15, 199);
            this.rtxtContent.Name = "rtxtContent";
            this.rtxtContent.ReadOnly = true;
            this.rtxtContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtContent.Size = new System.Drawing.Size(401, 201);
            this.rtxtContent.TabIndex = 5;
            this.rtxtContent.Text = "";
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.lblLicense.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicense.Location = new System.Drawing.Point(12, 181);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(46, 15);
            this.lblLicense.TabIndex = 4;
            this.lblLicense.Text = "License";
            // 
            // frmAbout
            // 
            this.AcceptButton = this.btnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOkay;
            this.ClientSize = new System.Drawing.Size(428, 441);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.rtxtContent);
            this.Controls.Add(this.lblCredits);
            this.Controls.Add(this.lblCoder);
            this.Controls.Add(this.btnOkay);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.picXRAT);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - About";
            ((System.ComponentModel.ISupportInitialize)(this.picXRAT)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picXRAT;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Label lblCoder;
        private System.Windows.Forms.Label lblCredits;
        private System.Windows.Forms.RichTextBox rtxtContent;
        private System.Windows.Forms.Label lblLicense;
    }
}