using xServer.Controls;

namespace xServer.Forms
{
    partial class FrmFileManager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFileManager));
            this.cmbDrives = new System.Windows.Forms.ComboBox();
            this.lblDrive = new System.Windows.Forms.Label();
            this.ctxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxtDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtLine2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxtExecute = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtRename = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxtAddToAutostart = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtLine = new System.Windows.Forms.ToolStripSeparator();
            this.ctxtRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtOpenDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.imgListDirectory = new System.Windows.Forms.ImageList(this.components);
            this.botStrip = new System.Windows.Forms.StatusStrip();
            this.btnOpenDLFolder = new System.Windows.Forms.Button();
            this.TabControlFileManager = new System.Windows.Forms.TabControl();
            this.tabFileExplorer = new System.Windows.Forms.TabPage();
            this.lstDirectory = new xServer.Controls.ListViewEx();
            this.hName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabTransfers = new System.Windows.Forms.TabPage();
            this.lstTransfers = new xServer.Controls.ListViewEx();
            this.hID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxtMenu2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxtCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.imgListTransfers = new System.Windows.Forms.ImageList(this.components);
            this.ctxtMenu.SuspendLayout();
            this.TabControlFileManager.SuspendLayout();
            this.tabFileExplorer.SuspendLayout();
            this.tabTransfers.SuspendLayout();
            this.ctxtMenu2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbDrives
            // 
            this.cmbDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDrives.FormattingEnabled = true;
            this.cmbDrives.Location = new System.Drawing.Point(50, 9);
            this.cmbDrives.Name = "cmbDrives";
            this.cmbDrives.Size = new System.Drawing.Size(52, 21);
            this.cmbDrives.TabIndex = 1;
            this.cmbDrives.SelectedIndexChanged += new System.EventHandler(this.cmbDrives_SelectedIndexChanged);
            // 
            // lblDrive
            // 
            this.lblDrive.AutoSize = true;
            this.lblDrive.Location = new System.Drawing.Point(8, 12);
            this.lblDrive.Name = "lblDrive";
            this.lblDrive.Size = new System.Drawing.Size(36, 13);
            this.lblDrive.TabIndex = 0;
            this.lblDrive.Text = "Drive:";
            // 
            // ctxtMenu
            // 
            this.ctxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtDownload,
            this.ctxtLine2,
            this.ctxtExecute,
            this.ctxtRename,
            this.ctxtDelete,
            this.toolStripMenuItem1,
            this.ctxtAddToAutostart,
            this.ctxtLine,
            this.ctxtRefresh,
            this.ctxtOpenDirectory});
            this.ctxtMenu.Name = "ctxtMenu";
            this.ctxtMenu.Size = new System.Drawing.Size(240, 176);
            // 
            // ctxtDownload
            // 
            this.ctxtDownload.Image = global::xServer.Properties.Resources.download;
            this.ctxtDownload.Name = "ctxtDownload";
            this.ctxtDownload.Size = new System.Drawing.Size(239, 22);
            this.ctxtDownload.Text = "Download";
            this.ctxtDownload.Click += new System.EventHandler(this.ctxtDownload_Click);
            // 
            // ctxtLine2
            // 
            this.ctxtLine2.Name = "ctxtLine2";
            this.ctxtLine2.Size = new System.Drawing.Size(236, 6);
            // 
            // ctxtExecute
            // 
            this.ctxtExecute.Image = global::xServer.Properties.Resources.run;
            this.ctxtExecute.Name = "ctxtExecute";
            this.ctxtExecute.Size = new System.Drawing.Size(239, 22);
            this.ctxtExecute.Text = "Execute";
            this.ctxtExecute.Click += new System.EventHandler(this.ctxtExecute_Click);
            // 
            // ctxtRename
            // 
            this.ctxtRename.Image = global::xServer.Properties.Resources.textfield_rename;
            this.ctxtRename.Name = "ctxtRename";
            this.ctxtRename.Size = new System.Drawing.Size(239, 22);
            this.ctxtRename.Text = "Rename";
            this.ctxtRename.Click += new System.EventHandler(this.ctxtRename_Click);
            // 
            // ctxtDelete
            // 
            this.ctxtDelete.Image = global::xServer.Properties.Resources.delete;
            this.ctxtDelete.Name = "ctxtDelete";
            this.ctxtDelete.Size = new System.Drawing.Size(239, 22);
            this.ctxtDelete.Text = "Delete";
            this.ctxtDelete.Click += new System.EventHandler(this.ctxtDelete_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(236, 6);
            // 
            // ctxtAddToAutostart
            // 
            this.ctxtAddToAutostart.Image = global::xServer.Properties.Resources.application_add;
            this.ctxtAddToAutostart.Name = "ctxtAddToAutostart";
            this.ctxtAddToAutostart.Size = new System.Drawing.Size(239, 22);
            this.ctxtAddToAutostart.Text = "Add to Autostart";
            this.ctxtAddToAutostart.Click += new System.EventHandler(this.ctxtAddToAutostart_Click);
            // 
            // ctxtLine
            // 
            this.ctxtLine.Name = "ctxtLine";
            this.ctxtLine.Size = new System.Drawing.Size(236, 6);
            // 
            // ctxtRefresh
            // 
            this.ctxtRefresh.Image = global::xServer.Properties.Resources.refresh;
            this.ctxtRefresh.Name = "ctxtRefresh";
            this.ctxtRefresh.Size = new System.Drawing.Size(239, 22);
            this.ctxtRefresh.Text = "Refresh";
            this.ctxtRefresh.Click += new System.EventHandler(this.ctxtRefresh_Click);
            // 
            // ctxtOpenDirectory
            // 
            this.ctxtOpenDirectory.Name = "ctxtOpenDirectory";
            this.ctxtOpenDirectory.Size = new System.Drawing.Size(239, 22);
            this.ctxtOpenDirectory.Text = "Open Directory in Remote Shell";
            this.ctxtOpenDirectory.Click += new System.EventHandler(this.ctxtOpenDirectory_Click);
            // 
            // imgListDirectory
            // 
            this.imgListDirectory.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListDirectory.ImageStream")));
            this.imgListDirectory.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListDirectory.Images.SetKeyName(0, "back.png");
            this.imgListDirectory.Images.SetKeyName(1, "folder.png");
            this.imgListDirectory.Images.SetKeyName(2, "file.png");
            this.imgListDirectory.Images.SetKeyName(3, "application.png");
            this.imgListDirectory.Images.SetKeyName(4, "text.png");
            this.imgListDirectory.Images.SetKeyName(5, "archive.png");
            this.imgListDirectory.Images.SetKeyName(6, "word.png");
            this.imgListDirectory.Images.SetKeyName(7, "pdf.png");
            this.imgListDirectory.Images.SetKeyName(8, "image.png");
            this.imgListDirectory.Images.SetKeyName(9, "movie.png");
            this.imgListDirectory.Images.SetKeyName(10, "music.png");
            // 
            // botStrip
            // 
            this.botStrip.Location = new System.Drawing.Point(0, 383);
            this.botStrip.Name = "botStrip";
            this.botStrip.Size = new System.Drawing.Size(686, 22);
            this.botStrip.TabIndex = 3;
            this.botStrip.Text = "statusStrip1";
            // 
            // btnOpenDLFolder
            // 
            this.btnOpenDLFolder.Location = new System.Drawing.Point(531, 9);
            this.btnOpenDLFolder.Name = "btnOpenDLFolder";
            this.btnOpenDLFolder.Size = new System.Drawing.Size(139, 21);
            this.btnOpenDLFolder.TabIndex = 4;
            this.btnOpenDLFolder.Text = "&Open Download Folder";
            this.btnOpenDLFolder.UseVisualStyleBackColor = true;
            this.btnOpenDLFolder.Click += new System.EventHandler(this.btnOpenDLFolder_Click);
            // 
            // TabControlFileManager
            // 
            this.TabControlFileManager.Controls.Add(this.tabFileExplorer);
            this.TabControlFileManager.Controls.Add(this.tabTransfers);
            this.TabControlFileManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControlFileManager.Location = new System.Drawing.Point(0, 0);
            this.TabControlFileManager.Name = "TabControlFileManager";
            this.TabControlFileManager.SelectedIndex = 0;
            this.TabControlFileManager.Size = new System.Drawing.Size(686, 383);
            this.TabControlFileManager.TabIndex = 5;
            // 
            // tabFileExplorer
            // 
            this.tabFileExplorer.Controls.Add(this.btnOpenDLFolder);
            this.tabFileExplorer.Controls.Add(this.lstDirectory);
            this.tabFileExplorer.Controls.Add(this.lblDrive);
            this.tabFileExplorer.Controls.Add(this.cmbDrives);
            this.tabFileExplorer.Location = new System.Drawing.Point(4, 22);
            this.tabFileExplorer.Name = "tabFileExplorer";
            this.tabFileExplorer.Padding = new System.Windows.Forms.Padding(3);
            this.tabFileExplorer.Size = new System.Drawing.Size(678, 357);
            this.tabFileExplorer.TabIndex = 0;
            this.tabFileExplorer.Text = "File Explorer";
            this.tabFileExplorer.UseVisualStyleBackColor = true;
            // 
            // lstDirectory
            // 
            this.lstDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDirectory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hName,
            this.hSize,
            this.hType});
            this.lstDirectory.ContextMenuStrip = this.ctxtMenu;
            this.lstDirectory.FullRowSelect = true;
            this.lstDirectory.GridLines = true;
            this.lstDirectory.Location = new System.Drawing.Point(11, 36);
            this.lstDirectory.Name = "lstDirectory";
            this.lstDirectory.Size = new System.Drawing.Size(659, 315);
            this.lstDirectory.SmallImageList = this.imgListDirectory;
            this.lstDirectory.TabIndex = 2;
            this.lstDirectory.UseCompatibleStateImageBehavior = false;
            this.lstDirectory.View = System.Windows.Forms.View.Details;
            this.lstDirectory.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstDirectory_ColumnClick);
            this.lstDirectory.DoubleClick += new System.EventHandler(this.lstDirectory_DoubleClick);
            // 
            // hName
            // 
            this.hName.Text = "Name";
            this.hName.Width = 163;
            // 
            // hSize
            // 
            this.hSize.Text = "Size";
            this.hSize.Width = 117;
            // 
            // hType
            // 
            this.hType.Text = "Type";
            this.hType.Width = 128;
            // 
            // tabTransfers
            // 
            this.tabTransfers.Controls.Add(this.lstTransfers);
            this.tabTransfers.Location = new System.Drawing.Point(4, 22);
            this.tabTransfers.Name = "tabTransfers";
            this.tabTransfers.Padding = new System.Windows.Forms.Padding(3);
            this.tabTransfers.Size = new System.Drawing.Size(678, 357);
            this.tabTransfers.TabIndex = 1;
            this.tabTransfers.Text = "Transfers";
            this.tabTransfers.UseVisualStyleBackColor = true;
            // 
            // lstTransfers
            // 
            this.lstTransfers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTransfers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hID,
            this.hStatus,
            this.hFilename});
            this.lstTransfers.ContextMenuStrip = this.ctxtMenu2;
            this.lstTransfers.FullRowSelect = true;
            this.lstTransfers.GridLines = true;
            this.lstTransfers.Location = new System.Drawing.Point(8, 6);
            this.lstTransfers.Name = "lstTransfers";
            this.lstTransfers.Size = new System.Drawing.Size(662, 345);
            this.lstTransfers.SmallImageList = this.imgListTransfers;
            this.lstTransfers.TabIndex = 0;
            this.lstTransfers.UseCompatibleStateImageBehavior = false;
            this.lstTransfers.View = System.Windows.Forms.View.Details;
            // 
            // hID
            // 
            this.hID.Text = "ID";
            this.hID.Width = 128;
            // 
            // hStatus
            // 
            this.hStatus.Text = "Status";
            this.hStatus.Width = 247;
            // 
            // hFilename
            // 
            this.hFilename.Text = "Filename";
            this.hFilename.Width = 282;
            // 
            // ctxtMenu2
            // 
            this.ctxtMenu2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtCancel});
            this.ctxtMenu2.Name = "ctxtMenu2";
            this.ctxtMenu2.Size = new System.Drawing.Size(111, 26);
            // 
            // ctxtCancel
            // 
            this.ctxtCancel.Image = global::xServer.Properties.Resources.cancel;
            this.ctxtCancel.Name = "ctxtCancel";
            this.ctxtCancel.Size = new System.Drawing.Size(110, 22);
            this.ctxtCancel.Text = "Cancel";
            this.ctxtCancel.Click += new System.EventHandler(this.ctxtCancel_Click);
            // 
            // imgListTransfers
            // 
            this.imgListTransfers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListTransfers.ImageStream")));
            this.imgListTransfers.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListTransfers.Images.SetKeyName(0, "cancel.png");
            this.imgListTransfers.Images.SetKeyName(1, "done.png");
            // 
            // FrmFileManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 405);
            this.Controls.Add(this.TabControlFileManager);
            this.Controls.Add(this.botStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(663, 377);
            this.Name = "FrmFileManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - File Manager []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFileManager_FormClosing);
            this.Load += new System.EventHandler(this.FrmFileManager_Load);
            this.ctxtMenu.ResumeLayout(false);
            this.TabControlFileManager.ResumeLayout(false);
            this.tabFileExplorer.ResumeLayout(false);
            this.tabFileExplorer.PerformLayout();
            this.tabTransfers.ResumeLayout(false);
            this.ctxtMenu2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDrive;
        private System.Windows.Forms.ImageList imgListDirectory;
        private System.Windows.Forms.ColumnHeader hName;
        private System.Windows.Forms.ColumnHeader hSize;
        private System.Windows.Forms.ColumnHeader hType;
        private System.Windows.Forms.ContextMenuStrip ctxtMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxtDownload;
        private System.Windows.Forms.Button btnOpenDLFolder;
        private System.Windows.Forms.TabControl TabControlFileManager;
        private System.Windows.Forms.TabPage tabFileExplorer;
        private System.Windows.Forms.TabPage tabTransfers;
        private System.Windows.Forms.ColumnHeader hStatus;
        private System.Windows.Forms.ColumnHeader hFilename;
        private System.Windows.Forms.ColumnHeader hID;
        private System.Windows.Forms.ImageList imgListTransfers;
        private System.Windows.Forms.ToolStripMenuItem ctxtExecute;
        private System.Windows.Forms.ToolStripMenuItem ctxtRefresh;
        private System.Windows.Forms.ToolStripSeparator ctxtLine;
        private System.Windows.Forms.ToolStripSeparator ctxtLine2;
        private System.Windows.Forms.ToolStripMenuItem ctxtRename;
        private System.Windows.Forms.ToolStripMenuItem ctxtDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ctxtAddToAutostart;
        private System.Windows.Forms.ContextMenuStrip ctxtMenu2;
        private System.Windows.Forms.ToolStripMenuItem ctxtCancel;
        private System.Windows.Forms.ToolStripMenuItem ctxtOpenDirectory;
        private System.Windows.Forms.ComboBox cmbDrives;
        private ListViewEx lstDirectory;
        private ListViewEx lstTransfers;
        private System.Windows.Forms.StatusStrip botStrip;
    }
}