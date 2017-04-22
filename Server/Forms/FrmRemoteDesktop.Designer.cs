namespace xServer.Forms
{
    partial class FrmRemoteDesktop
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRemoteDesktop));
            this.cbMonitors = new System.Windows.Forms.ComboBox();
            this.menuRemoteDesktop = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenCaptureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyboardMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.monitorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MonitorTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.resolutionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qualityTextbox = new System.Windows.Forms.ToolStripTextBox();
            this.qualityShowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barQuality = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.picDesktop = new xServer.Controls.RapidPictureBox();
            this.menuRemoteDesktop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barQuality)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDesktop)).BeginInit();
            this.SuspendLayout();
            // 
            // cbMonitors
            // 
            this.cbMonitors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMonitors.FormattingEnabled = true;
            this.cbMonitors.Location = new System.Drawing.Point(246, 3);
            this.cbMonitors.Name = "cbMonitors";
            this.cbMonitors.Size = new System.Drawing.Size(100, 21);
            this.cbMonitors.TabIndex = 8;
            this.cbMonitors.TabStop = false;
            // 
            // menuRemoteDesktop
            // 
            this.menuRemoteDesktop.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.menuRemoteDesktop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.startMenuItem,
            this.stopMenuItem,
            this.mouseToolStripMenuItem,
            this.separatorTextBox1,
            this.monitorMenuItem,
            this.MonitorTextBox1,
            this.resolutionMenuItem,
            this.qualityTextbox,
            this.qualityShowMenuItem});
            this.menuRemoteDesktop.Location = new System.Drawing.Point(0, 0);
            this.menuRemoteDesktop.Name = "menuRemoteDesktop";
            this.menuRemoteDesktop.Size = new System.Drawing.Size(751, 27);
            this.menuRemoteDesktop.TabIndex = 12;
            this.menuRemoteDesktop.Text = "menuRemoteDesktop";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screenCaptureMenuItem,
            this.closeMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 23);
            this.fileMenuItem.Text = "&File";
            // 
            // screenCaptureMenuItem
            // 
            this.screenCaptureMenuItem.Name = "screenCaptureMenuItem";
            this.screenCaptureMenuItem.Size = new System.Drawing.Size(154, 22);
            this.screenCaptureMenuItem.Text = "&Screen Capture";
            this.screenCaptureMenuItem.Click += new System.EventHandler(this.screenCaptureMenuItem_Click);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.Size = new System.Drawing.Size(154, 22);
            this.closeMenuItem.Text = "&Close";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            // 
            // startMenuItem
            // 
            this.startMenuItem.Name = "startMenuItem";
            this.startMenuItem.Size = new System.Drawing.Size(43, 23);
            this.startMenuItem.Text = "&Start";
            this.startMenuItem.Click += new System.EventHandler(this.startMenuItem_Click);
            // 
            // stopMenuItem
            // 
            this.stopMenuItem.Name = "stopMenuItem";
            this.stopMenuItem.Size = new System.Drawing.Size(43, 23);
            this.stopMenuItem.Text = "S&top";
            this.stopMenuItem.Click += new System.EventHandler(this.stopMenuItem_Click);
            // 
            // mouseToolStripMenuItem
            // 
            this.mouseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mouseMenuItem,
            this.keyboardMenuItem});
            this.mouseToolStripMenuItem.Name = "mouseToolStripMenuItem";
            this.mouseToolStripMenuItem.Size = new System.Drawing.Size(47, 23);
            this.mouseToolStripMenuItem.Text = "&Input";
            // 
            // mouseMenuItem
            // 
            this.mouseMenuItem.Name = "mouseMenuItem";
            this.mouseMenuItem.Size = new System.Drawing.Size(124, 22);
            this.mouseMenuItem.Text = "&Mouse";
            this.mouseMenuItem.Click += new System.EventHandler(this.mouseMenuItem_Click);
            // 
            // keyboardMenuItem
            // 
            this.keyboardMenuItem.Name = "keyboardMenuItem";
            this.keyboardMenuItem.Size = new System.Drawing.Size(124, 22);
            this.keyboardMenuItem.Text = "&Keyboard";
            this.keyboardMenuItem.Click += new System.EventHandler(this.keyboardMenuItem_Click);
            // 
            // separatorTextBox1
            // 
            this.separatorTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.separatorTextBox1.Enabled = false;
            this.separatorTextBox1.Name = "separatorTextBox1";
            this.separatorTextBox1.Size = new System.Drawing.Size(4, 23);
            // 
            // monitorMenuItem
            // 
            this.monitorMenuItem.Name = "monitorMenuItem";
            this.monitorMenuItem.Size = new System.Drawing.Size(62, 23);
            this.monitorMenuItem.Text = "&Monitor";
            this.monitorMenuItem.Click += new System.EventHandler(this.monitorMenuItem_Click);
            // 
            // MonitorTextBox1
            // 
            this.MonitorTextBox1.Enabled = false;
            this.MonitorTextBox1.Name = "MonitorTextBox1";
            this.MonitorTextBox1.Size = new System.Drawing.Size(101, 23);
            // 
            // resolutionMenuItem
            // 
            this.resolutionMenuItem.Name = "resolutionMenuItem";
            this.resolutionMenuItem.Size = new System.Drawing.Size(57, 23);
            this.resolutionMenuItem.Text = "&Quality";
            // 
            // qualityTextbox
            // 
            this.qualityTextbox.Enabled = false;
            this.qualityTextbox.Name = "qualityTextbox";
            this.qualityTextbox.Size = new System.Drawing.Size(80, 23);
            // 
            // qualityShowMenuItem
            // 
            this.qualityShowMenuItem.Name = "qualityShowMenuItem";
            this.qualityShowMenuItem.Size = new System.Drawing.Size(63, 23);
            this.qualityShowMenuItem.Tag = "+3-4-*-";
            this.qualityShowMenuItem.Text = "75  High";
            // 
            // barQuality
            // 
            this.barQuality.Location = new System.Drawing.Point(405, 1);
            this.barQuality.Maximum = 100;
            this.barQuality.Minimum = 1;
            this.barQuality.Name = "barQuality";
            this.barQuality.Size = new System.Drawing.Size(80, 45);
            this.barQuality.TabIndex = 14;
            this.barQuality.TabStop = false;
            this.barQuality.Value = 75;
            this.barQuality.Scroll += new System.EventHandler(this.barQuality_Scroll);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.picDesktop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(751, 535);
            this.panel1.TabIndex = 15;
            // 
            // picDesktop
            // 
            this.picDesktop.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.picDesktop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDesktop.Cursor = System.Windows.Forms.Cursors.Default;
            this.picDesktop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picDesktop.GetImageSafe = null;
            this.picDesktop.Location = new System.Drawing.Point(0, 0);
            this.picDesktop.Name = "picDesktop";
            this.picDesktop.Running = false;
            this.picDesktop.Size = new System.Drawing.Size(751, 535);
            this.picDesktop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDesktop.TabIndex = 1;
            this.picDesktop.TabStop = false;
            // 
            // FrmRemoteDesktop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(751, 562);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.barQuality);
            this.Controls.Add(this.cbMonitors);
            this.Controls.Add(this.menuRemoteDesktop);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "FrmRemoteDesktop";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Desktop []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRemoteDesktop_FormClosing);
            this.Load += new System.EventHandler(this.FrmRemoteDesktop_Load);
            this.menuRemoteDesktop.ResumeLayout(false);
            this.menuRemoteDesktop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barQuality)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picDesktop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbMonitors;
        private System.Windows.Forms.MenuStrip menuRemoteDesktop;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopMenuItem;
        private System.Windows.Forms.ToolStripTextBox separatorTextBox1;
        private System.Windows.Forms.ToolStripMenuItem monitorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resolutionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mouseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mouseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyboardMenuItem;
        private System.Windows.Forms.ToolStripTextBox MonitorTextBox1;
        private System.Windows.Forms.ToolStripTextBox qualityTextbox;
        private System.Windows.Forms.ToolStripMenuItem qualityShowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenCaptureMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.TrackBar barQuality;
        private System.Windows.Forms.Panel panel1;
        private Controls.RapidPictureBox picDesktop;
    }
}