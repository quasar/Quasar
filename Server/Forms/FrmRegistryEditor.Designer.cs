namespace xServer.Forms
{
    partial class FrmRegistryEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRegistryEditor));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tvRegistryDirectory = new System.Windows.Forms.TreeView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.selectedStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageRegistryDirectoryList = new System.Windows.Forms.ImageList(this.components);
            this.imageRegistryKeyTypeList = new System.Windows.Forms.ImageList(this.components);
            this.lstRegistryKeys = new xServer.Controls.AeroListView();
            this.hName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.splitContainer, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.statusStrip, 0, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.148784F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95.85122F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1239, 724);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(3, 32);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tvRegistryDirectory);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.lstRegistryKeys);
            this.splitContainer.Size = new System.Drawing.Size(1233, 664);
            this.splitContainer.SplitterDistance = 411;
            this.splitContainer.TabIndex = 0;
            // 
            // tvRegistryDirectory
            // 
            this.tvRegistryDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvRegistryDirectory.ImageIndex = 0;
            this.tvRegistryDirectory.ImageList = this.imageRegistryDirectoryList;
            this.tvRegistryDirectory.Location = new System.Drawing.Point(0, 0);
            this.tvRegistryDirectory.Name = "tvRegistryDirectory";
            this.tvRegistryDirectory.SelectedImageIndex = 0;
            this.tvRegistryDirectory.Size = new System.Drawing.Size(411, 664);
            this.tvRegistryDirectory.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 702);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1239, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // selectedStripStatusLabel
            // 
            this.selectedStripStatusLabel.Name = "selectedStripStatusLabel";
            this.selectedStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // imageRegistryDirectoryList
            // 
            this.imageRegistryDirectoryList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageRegistryDirectoryList.ImageStream")));
            this.imageRegistryDirectoryList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageRegistryDirectoryList.Images.SetKeyName(0, "folder.png");
            // 
            // imageRegistryKeyTypeList
            // 
            this.imageRegistryKeyTypeList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageRegistryKeyTypeList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageRegistryKeyTypeList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lstRegistryKeys
            // 
            this.lstRegistryKeys.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hName,
            this.hType,
            this.hValue});
            this.lstRegistryKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRegistryKeys.FullRowSelect = true;
            this.lstRegistryKeys.Location = new System.Drawing.Point(0, 0);
            this.lstRegistryKeys.Name = "lstRegistryKeys";
            this.lstRegistryKeys.Size = new System.Drawing.Size(818, 664);
            this.lstRegistryKeys.TabIndex = 0;
            this.lstRegistryKeys.UseCompatibleStateImageBehavior = false;
            this.lstRegistryKeys.View = System.Windows.Forms.View.Details;
            // 
            // hName
            // 
            this.hName.Text = "Name";
            this.hName.Width = 203;
            // 
            // hType
            // 
            this.hType.Text = "Type";
            this.hType.Width = 149;
            // 
            // hValue
            // 
            this.hValue.Text = "Value";
            this.hValue.Width = 384;
            // 
            // FrmRegistryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1239, 724);
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmRegistryEditor";
            this.Text = "Registry Editor";
            this.Load += new System.EventHandler(this.FrmRegistryEditor_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView tvRegistryDirectory;
        private Controls.AeroListView lstRegistryKeys;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel selectedStripStatusLabel;
        private System.Windows.Forms.ImageList imageRegistryDirectoryList;
        private System.Windows.Forms.ColumnHeader hName;
        private System.Windows.Forms.ColumnHeader hType;
        private System.Windows.Forms.ColumnHeader hValue;
        private System.Windows.Forms.ImageList imageRegistryKeyTypeList;
    }
}