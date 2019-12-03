namespace Quasar.Server.Forms
{
    partial class FrmCertPass
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.CertPassSubmit = new System.Windows.Forms.Button();
            this.CertPass = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.CertPass);
            this.flowLayoutPanel1.Controls.Add(this.CertPassSubmit);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(299, 31);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // CertPassSubmit
            // 
            this.CertPassSubmit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CertPassSubmit.Location = new System.Drawing.Point(215, 3);
            this.CertPassSubmit.Name = "CertPassSubmit";
            this.CertPassSubmit.Size = new System.Drawing.Size(75, 23);
            this.CertPassSubmit.TabIndex = 3;
            this.CertPassSubmit.Text = "Submit";
            this.CertPassSubmit.UseVisualStyleBackColor = true;
            // 
            // CertPass
            // 
            this.CertPass.Location = new System.Drawing.Point(3, 3);
            this.CertPass.Name = "CertPass";
            this.CertPass.Size = new System.Drawing.Size(206, 20);
            this.CertPass.TabIndex = 2;
            this.CertPass.TextChanged += new System.EventHandler(this.CertPass_TextChanged);
            // 
            // FrmCertPass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 31);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "FrmCertPass";
            this.Text = "FrmCertPass";
            this.Load += new System.EventHandler(this.FrmCertPass_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button CertPassSubmit;
        public System.Windows.Forms.TextBox CertPass;
    }
}