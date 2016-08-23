namespace xServer.Forms
{
    partial class FrmRemoteWebcam
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRemoteWebcam));
            this.cbResolutions = new System.Windows.Forms.ComboBox();
            this.cbWebcams = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenCaptureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSeparator = new System.Windows.Forms.ToolStripTextBox();
            this.webcamMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtwebcam = new System.Windows.Forms.ToolStripTextBox();
            this.resolutionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.picWebcam = new xServer.Controls.RapidPictureBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWebcam)).BeginInit();
            this.SuspendLayout();
            // 
            // cbResolutions
            // 
            this.cbResolutions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbResolutions.FormattingEnabled = true;
            this.cbResolutions.Location = new System.Drawing.Point(453, 3);
            this.cbResolutions.Name = "cbResolutions";
            this.cbResolutions.Size = new System.Drawing.Size(125, 21);
            this.cbResolutions.TabIndex = 9;
            this.cbResolutions.TabStop = false;
            // 
            // cbWebcams
            // 
            this.cbWebcams.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWebcams.FormattingEnabled = true;
            this.cbWebcams.Location = new System.Drawing.Point(202, 3);
            this.cbWebcams.Name = "cbWebcams";
            this.cbWebcams.Size = new System.Drawing.Size(175, 21);
            this.cbWebcams.TabIndex = 8;
            this.cbWebcams.TabStop = false;
            this.cbWebcams.SelectedIndexChanged += new System.EventHandler(this.cbWebcams_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.startMenuItem,
            this.stopMenuItem,
            this.txtSeparator,
            this.webcamMenuItem,
            this.txtwebcam,
            this.resolutionMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(795, 27);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuWebcam";
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
            // txtSeparator
            // 
            this.txtSeparator.Enabled = false;
            this.txtSeparator.Name = "txtSeparator";
            this.txtSeparator.Size = new System.Drawing.Size(4, 23);
            // 
            // webcamMenuItem
            // 
            this.webcamMenuItem.Name = "webcamMenuItem";
            this.webcamMenuItem.Size = new System.Drawing.Size(66, 23);
            this.webcamMenuItem.Text = "&Webcam";
            this.webcamMenuItem.Click += new System.EventHandler(this.webcamMenuItem_Click);
            // 
            // txtwebcam
            // 
            this.txtwebcam.Enabled = false;
            this.txtwebcam.Name = "txtwebcam";
            this.txtwebcam.Size = new System.Drawing.Size(175, 23);
            // 
            // resolutionMenuItem
            // 
            this.resolutionMenuItem.Name = "resolutionMenuItem";
            this.resolutionMenuItem.Size = new System.Drawing.Size(75, 23);
            this.resolutionMenuItem.Text = "&Resolution";
            this.resolutionMenuItem.Click += new System.EventHandler(this.resolutionMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.picWebcam);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(795, 535);
            this.panel1.TabIndex = 12;
            // 
            // picWebcam
            // 
            this.picWebcam.BackColor = System.Drawing.Color.Black;
            this.picWebcam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picWebcam.Cursor = System.Windows.Forms.Cursors.Default;
            this.picWebcam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picWebcam.GetImageSafe = null;
            this.picWebcam.Location = new System.Drawing.Point(0, 0);
            this.picWebcam.Name = "picWebcam";
            this.picWebcam.Running = false;
            this.picWebcam.Size = new System.Drawing.Size(795, 535);
            this.picWebcam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWebcam.TabIndex = 2;
            this.picWebcam.TabStop = false;
            // 
            // FrmRemoteWebcam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(795, 562);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbResolutions);
            this.Controls.Add(this.cbWebcams);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(480, 320);
            this.Name = "FrmRemoteWebcam";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmRemoteWebcam []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRemoteWebcam_FormClosing);
            this.Load += new System.EventHandler(this.FrmRemoteWebcam_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWebcam)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbWebcams;
        private System.Windows.Forms.ComboBox cbResolutions;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem startMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopMenuItem;
        private System.Windows.Forms.ToolStripTextBox txtSeparator;
        private System.Windows.Forms.ToolStripMenuItem webcamMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resolutionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenCaptureMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.ToolStripTextBox txtwebcam;
        private System.Windows.Forms.Panel panel1;
        private Controls.RapidPictureBox picWebcam;
    }
}