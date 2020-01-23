namespace Quasar.Server.Forms
{
    partial class FrmDownloadAndExecute
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDownloadAndExecute));
            this.btnDownloadAndExecute = new System.Windows.Forms.Button();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.chkRunHidden = new System.Windows.Forms.CheckBox();
            this.lblArgs = new System.Windows.Forms.Label();
            this.txtArgs = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnDownloadAndExecute
            // 
            this.btnDownloadAndExecute.Location = new System.Drawing.Point(367, 87);
            this.btnDownloadAndExecute.Margin = new System.Windows.Forms.Padding(4);
            this.btnDownloadAndExecute.Name = "btnDownloadAndExecute";
            this.btnDownloadAndExecute.Size = new System.Drawing.Size(207, 34);
            this.btnDownloadAndExecute.TabIndex = 3;
            this.btnDownloadAndExecute.Text = "Download && Execute";
            this.btnDownloadAndExecute.UseVisualStyleBackColor = true;
            this.btnDownloadAndExecute.Click += new System.EventHandler(this.btnDownloadAndExecute_Click);
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(72, 9);
            this.txtURL.Margin = new System.Windows.Forms.Padding(4);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(502, 29);
            this.txtURL.TabIndex = 1;
            this.txtURL.TextChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(18, 14);
            this.lblURL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(44, 23);
            this.lblURL.TabIndex = 0;
            this.lblURL.Text = "URL:";
            // 
            // chkRunHidden
            // 
            this.chkRunHidden.AutoSize = true;
            this.chkRunHidden.Location = new System.Drawing.Point(72, 90);
            this.chkRunHidden.Margin = new System.Windows.Forms.Padding(4);
            this.chkRunHidden.Name = "chkRunHidden";
            this.chkRunHidden.Size = new System.Drawing.Size(151, 27);
            this.chkRunHidden.TabIndex = 2;
            this.chkRunHidden.Text = "Run file hidden";
            this.chkRunHidden.UseVisualStyleBackColor = true;
            // 
            // lblArgs
            // 
            this.lblArgs.AutoSize = true;
            this.lblArgs.Location = new System.Drawing.Point(18, 51);
            this.lblArgs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblArgs.Name = "lblArgs";
            this.lblArgs.Size = new System.Drawing.Size(48, 23);
            this.lblArgs.TabIndex = 4;
            this.lblArgs.Text = "Args:";
            // 
            // txtArgs
            // 
            this.txtArgs.Location = new System.Drawing.Point(72, 46);
            this.txtArgs.Margin = new System.Windows.Forms.Padding(4);
            this.txtArgs.Name = "txtArgs";
            this.txtArgs.ReadOnly = true;
            this.txtArgs.Size = new System.Drawing.Size(502, 29);
            this.txtArgs.TabIndex = 5;
            // 
            // FrmDownloadAndExecute
            // 
            this.AcceptButton = this.btnDownloadAndExecute;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(594, 139);
            this.Controls.Add(this.lblArgs);
            this.Controls.Add(this.txtArgs);
            this.Controls.Add(this.chkRunHidden);
            this.Controls.Add(this.lblURL);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.btnDownloadAndExecute);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmDownloadAndExecute";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download & Execute []";
            this.Load += new System.EventHandler(this.FrmDownloadAndExecute_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDownloadAndExecute;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.CheckBox chkRunHidden;
        private System.Windows.Forms.Label lblArgs;
        private System.Windows.Forms.TextBox txtArgs;
    }
}