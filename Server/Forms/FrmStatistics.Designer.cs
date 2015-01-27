namespace xServer.Forms
{
    partial class FrmStatistics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmStatistics));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabTraffic = new System.Windows.Forms.TabPage();
            this.tabClients = new System.Windows.Forms.TabPage();
            this.lblTrafficStats = new System.Windows.Forms.Label();
            this.lblClientStats = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabTraffic.SuspendLayout();
            this.tabClients.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabTraffic);
            this.tabControl.Controls.Add(this.tabClients);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(478, 279);
            this.tabControl.TabIndex = 0;
            // 
            // tabTraffic
            // 
            this.tabTraffic.Controls.Add(this.lblTrafficStats);
            this.tabTraffic.Location = new System.Drawing.Point(4, 22);
            this.tabTraffic.Name = "tabTraffic";
            this.tabTraffic.Padding = new System.Windows.Forms.Padding(3);
            this.tabTraffic.Size = new System.Drawing.Size(376, 253);
            this.tabTraffic.TabIndex = 0;
            this.tabTraffic.Text = "Traffic Stats";
            this.tabTraffic.UseVisualStyleBackColor = true;
            this.tabTraffic.Paint += new System.Windows.Forms.PaintEventHandler(this.tabTraffic_Paint);
            // 
            // tabClients
            // 
            this.tabClients.Controls.Add(this.lblClientStats);
            this.tabClients.Location = new System.Drawing.Point(4, 22);
            this.tabClients.Name = "tabClients";
            this.tabClients.Padding = new System.Windows.Forms.Padding(3);
            this.tabClients.Size = new System.Drawing.Size(470, 253);
            this.tabClients.TabIndex = 1;
            this.tabClients.Text = "Client Stats";
            this.tabClients.UseVisualStyleBackColor = true;
            this.tabClients.Paint += new System.Windows.Forms.PaintEventHandler(this.tabClients_Paint);
            // 
            // lblTrafficStats
            // 
            this.lblTrafficStats.AutoSize = true;
            this.lblTrafficStats.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrafficStats.Location = new System.Drawing.Point(40, 13);
            this.lblTrafficStats.Name = "lblTrafficStats";
            this.lblTrafficStats.Size = new System.Drawing.Size(118, 21);
            this.lblTrafficStats.TabIndex = 0;
            this.lblTrafficStats.Text = "Traffic Statistics";
            // 
            // lblClientStats
            // 
            this.lblClientStats.AutoSize = true;
            this.lblClientStats.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientStats.Location = new System.Drawing.Point(40, 13);
            this.lblClientStats.Name = "lblClientStats";
            this.lblClientStats.Size = new System.Drawing.Size(115, 21);
            this.lblClientStats.TabIndex = 1;
            this.lblClientStats.Text = "Client Statistics";
            // 
            // frmStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 279);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStatistics";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Statistics";
            this.Load += new System.EventHandler(this.FrmStatistics_Load);
            this.tabControl.ResumeLayout(false);
            this.tabTraffic.ResumeLayout(false);
            this.tabTraffic.PerformLayout();
            this.tabClients.ResumeLayout(false);
            this.tabClients.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabTraffic;
        private System.Windows.Forms.TabPage tabClients;
        private System.Windows.Forms.Label lblTrafficStats;
        private System.Windows.Forms.Label lblClientStats;
    }
}