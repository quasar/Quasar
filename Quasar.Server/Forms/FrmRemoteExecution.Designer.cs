namespace Quasar.Server.Forms
{
    partial class FrmRemoteExecution
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRemoteExecution));
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.groupLocalFile = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupURL = new System.Windows.Forms.GroupBox();
            this.radioLocalFile = new System.Windows.Forms.RadioButton();
            this.radioURL = new System.Windows.Forms.RadioButton();
            this.lstTransfers = new Quasar.Server.Controls.AeroListView();
            this.hClient = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkUpdate = new System.Windows.Forms.CheckBox();
            this.groupLocalFile.SuspendLayout();
            this.groupURL.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(353, 459);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(138, 23);
            this.btnExecute.TabIndex = 6;
            this.btnExecute.Text = "Execute remotely";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(56, 25);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(320, 22);
            this.txtURL.TabIndex = 1;
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(20, 28);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(30, 13);
            this.lblURL.TabIndex = 0;
            this.lblURL.Text = "URL:";
            // 
            // groupLocalFile
            // 
            this.groupLocalFile.Controls.Add(this.btnBrowse);
            this.groupLocalFile.Controls.Add(this.txtPath);
            this.groupLocalFile.Controls.Add(this.label1);
            this.groupLocalFile.Location = new System.Drawing.Point(12, 35);
            this.groupLocalFile.Name = "groupLocalFile";
            this.groupLocalFile.Size = new System.Drawing.Size(479, 75);
            this.groupLocalFile.TabIndex = 1;
            this.groupLocalFile.TabStop = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(382, 23);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(59, 24);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(317, 22);
            this.txtPath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path:";
            // 
            // groupURL
            // 
            this.groupURL.Controls.Add(this.txtURL);
            this.groupURL.Controls.Add(this.lblURL);
            this.groupURL.Enabled = false;
            this.groupURL.Location = new System.Drawing.Point(12, 139);
            this.groupURL.Name = "groupURL";
            this.groupURL.Size = new System.Drawing.Size(479, 75);
            this.groupURL.TabIndex = 3;
            this.groupURL.TabStop = false;
            // 
            // radioLocalFile
            // 
            this.radioLocalFile.AutoSize = true;
            this.radioLocalFile.Checked = true;
            this.radioLocalFile.Location = new System.Drawing.Point(12, 12);
            this.radioLocalFile.Name = "radioLocalFile";
            this.radioLocalFile.Size = new System.Drawing.Size(110, 17);
            this.radioLocalFile.TabIndex = 0;
            this.radioLocalFile.TabStop = true;
            this.radioLocalFile.Text = "Execute local file";
            this.radioLocalFile.UseVisualStyleBackColor = true;
            this.radioLocalFile.CheckedChanged += new System.EventHandler(this.radioLocalFile_CheckedChanged);
            // 
            // radioURL
            // 
            this.radioURL.AutoSize = true;
            this.radioURL.Location = new System.Drawing.Point(12, 116);
            this.radioURL.Name = "radioURL";
            this.radioURL.Size = new System.Drawing.Size(114, 17);
            this.radioURL.TabIndex = 2;
            this.radioURL.Text = "Execute from URL";
            this.radioURL.UseVisualStyleBackColor = true;
            this.radioURL.CheckedChanged += new System.EventHandler(this.radioURL_CheckedChanged);
            // 
            // lstTransfers
            // 
            this.lstTransfers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTransfers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hClient,
            this.hStatus});
            this.lstTransfers.FullRowSelect = true;
            this.lstTransfers.GridLines = true;
            this.lstTransfers.HideSelection = false;
            this.lstTransfers.Location = new System.Drawing.Point(12, 220);
            this.lstTransfers.Name = "lstTransfers";
            this.lstTransfers.Size = new System.Drawing.Size(479, 233);
            this.lstTransfers.TabIndex = 4;
            this.lstTransfers.UseCompatibleStateImageBehavior = false;
            this.lstTransfers.View = System.Windows.Forms.View.Details;
            // 
            // hClient
            // 
            this.hClient.Text = "Client";
            this.hClient.Width = 302;
            // 
            // hStatus
            // 
            this.hStatus.Text = "Status";
            this.hStatus.Width = 173;
            // 
            // chkUpdate
            // 
            this.chkUpdate.AutoSize = true;
            this.chkUpdate.Location = new System.Drawing.Point(180, 463);
            this.chkUpdate.Name = "chkUpdate";
            this.chkUpdate.Size = new System.Drawing.Size(167, 17);
            this.chkUpdate.TabIndex = 5;
            this.chkUpdate.Text = "Update clients with this file";
            this.chkUpdate.UseVisualStyleBackColor = true;
            // 
            // FrmRemoteExecution
            // 
            this.AcceptButton = this.btnExecute;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(503, 494);
            this.Controls.Add(this.chkUpdate);
            this.Controls.Add(this.lstTransfers);
            this.Controls.Add(this.radioURL);
            this.Controls.Add(this.radioLocalFile);
            this.Controls.Add(this.groupURL);
            this.Controls.Add(this.groupLocalFile);
            this.Controls.Add(this.btnExecute);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmRemoteExecution";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRemoteExecution_FormClosing);
            this.Load += new System.EventHandler(this.FrmRemoteExecution_Load);
            this.groupLocalFile.ResumeLayout(false);
            this.groupLocalFile.PerformLayout();
            this.groupURL.ResumeLayout(false);
            this.groupURL.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.GroupBox groupLocalFile;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupURL;
        private System.Windows.Forms.RadioButton radioLocalFile;
        private System.Windows.Forms.RadioButton radioURL;
        private System.Windows.Forms.Button btnBrowse;
        private Controls.AeroListView lstTransfers;
        private System.Windows.Forms.ColumnHeader hClient;
        private System.Windows.Forms.ColumnHeader hStatus;
        private System.Windows.Forms.CheckBox chkUpdate;
    }
}