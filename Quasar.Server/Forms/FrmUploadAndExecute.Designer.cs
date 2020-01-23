namespace Quasar.Server.Forms
{
    partial class FrmUploadAndExecute
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUploadAndExecute));
            this.btnUploadAndExecute = new System.Windows.Forms.Button();
            this.chkRunHidden = new System.Windows.Forms.CheckBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtArgs = new System.Windows.Forms.TextBox();
            this.lblArgs = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnUploadAndExecute
            // 
            this.btnUploadAndExecute.Location = new System.Drawing.Point(408, 83);
            this.btnUploadAndExecute.Margin = new System.Windows.Forms.Padding(4);
            this.btnUploadAndExecute.Name = "btnUploadAndExecute";
            this.btnUploadAndExecute.Size = new System.Drawing.Size(166, 34);
            this.btnUploadAndExecute.TabIndex = 4;
            this.btnUploadAndExecute.Text = "Upload && Execute";
            this.btnUploadAndExecute.UseVisualStyleBackColor = true;
            this.btnUploadAndExecute.Click += new System.EventHandler(this.btnUploadAndExecute_Click);
            // 
            // chkRunHidden
            // 
            this.chkRunHidden.AutoSize = true;
            this.chkRunHidden.Location = new System.Drawing.Point(70, 89);
            this.chkRunHidden.Margin = new System.Windows.Forms.Padding(4);
            this.chkRunHidden.Name = "chkRunHidden";
            this.chkRunHidden.Size = new System.Drawing.Size(151, 27);
            this.chkRunHidden.TabIndex = 2;
            this.chkRunHidden.Text = "Run file hidden";
            this.chkRunHidden.UseVisualStyleBackColor = true;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(18, 14);
            this.lblPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(48, 23);
            this.lblPath.TabIndex = 0;
            this.lblPath.Text = "Path:";
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(76, 9);
            this.txtPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtPath.MaxLength = 300;
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(498, 29);
            this.txtPath.TabIndex = 1;
            this.txtPath.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(274, 83);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(124, 34);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtArgs
            // 
            this.txtArgs.Location = new System.Drawing.Point(76, 46);
            this.txtArgs.Margin = new System.Windows.Forms.Padding(4);
            this.txtArgs.MaxLength = 300;
            this.txtArgs.Name = "txtArgs";
            this.txtArgs.ReadOnly = true;
            this.txtArgs.Size = new System.Drawing.Size(498, 29);
            this.txtArgs.TabIndex = 5;
            // 
            // lblArgs
            // 
            this.lblArgs.AutoSize = true;
            this.lblArgs.Location = new System.Drawing.Point(18, 49);
            this.lblArgs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblArgs.Name = "lblArgs";
            this.lblArgs.Size = new System.Drawing.Size(48, 23);
            this.lblArgs.TabIndex = 6;
            this.lblArgs.Text = "Args:";
            // 
            // FrmUploadAndExecute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(594, 139);
            this.Controls.Add(this.lblArgs);
            this.Controls.Add(this.txtArgs);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.chkRunHidden);
            this.Controls.Add(this.btnUploadAndExecute);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUploadAndExecute";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Upload & Execute []";
            this.Load += new System.EventHandler(this.FrmUploadAndExecute_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUploadAndExecute;
        private System.Windows.Forms.CheckBox chkRunHidden;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtArgs;
        private System.Windows.Forms.Label lblArgs;
    }
}