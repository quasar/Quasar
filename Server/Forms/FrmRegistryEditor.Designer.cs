
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
            this.imageRegistryDirectoryList = new System.Windows.Forms.ImageList(this.components);
            this.lstRegistryKeys = new xServer.Controls.AeroListView();
            this.hName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageRegistryKeyTypeList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.selectedStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tv_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.stringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dWORD32bitValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qWORD64bitValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiStringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandableStringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectedItem_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.modifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyBinaryDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lst_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.keyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.stringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dWORD32bitValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.qWORD64bitValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.multiStringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.expandableStringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tv_ContextMenuStrip.SuspendLayout();
            this.selectedItem_ContextMenuStrip.SuspendLayout();
            this.lst_ContextMenuStrip.SuspendLayout();
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
            this.tvRegistryDirectory.HideSelection = false;
            this.tvRegistryDirectory.ImageIndex = 0;
            this.tvRegistryDirectory.ImageList = this.imageRegistryDirectoryList;
            this.tvRegistryDirectory.Location = new System.Drawing.Point(0, 0);
            this.tvRegistryDirectory.Name = "tvRegistryDirectory";
            this.tvRegistryDirectory.SelectedImageIndex = 0;
            this.tvRegistryDirectory.Size = new System.Drawing.Size(411, 664);
            this.tvRegistryDirectory.TabIndex = 0;
            this.tvRegistryDirectory.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvRegistryDirectory_AfterLabelEdit);
            this.tvRegistryDirectory.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvRegistryDirectory_BeforeExpand);
            this.tvRegistryDirectory.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvRegistryDirectory_BeforeSelect);
            this.tvRegistryDirectory.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvRegistryDirectory_NodeMouseClick);
            // 
            // imageRegistryDirectoryList
            // 
            this.imageRegistryDirectoryList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageRegistryDirectoryList.ImageStream")));
            this.imageRegistryDirectoryList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageRegistryDirectoryList.Images.SetKeyName(0, "folder.png");
            // 
            // lstRegistryKeys
            // 
            this.lstRegistryKeys.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hName,
            this.hType,
            this.hValue});
            this.lstRegistryKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRegistryKeys.FullRowSelect = true;
            this.lstRegistryKeys.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstRegistryKeys.HideSelection = false;
            this.lstRegistryKeys.Location = new System.Drawing.Point(0, 0);
            this.lstRegistryKeys.Name = "lstRegistryKeys";
            this.lstRegistryKeys.Size = new System.Drawing.Size(818, 664);
            this.lstRegistryKeys.SmallImageList = this.imageRegistryKeyTypeList;
            this.lstRegistryKeys.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.lstRegistryKeys.TabIndex = 0;
            this.lstRegistryKeys.UseCompatibleStateImageBehavior = false;
            this.lstRegistryKeys.View = System.Windows.Forms.View.Details;
            this.lstRegistryKeys.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstRegistryKeys_MouseClick);
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
            // imageRegistryKeyTypeList
            // 
            this.imageRegistryKeyTypeList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageRegistryKeyTypeList.ImageStream")));
            this.imageRegistryKeyTypeList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageRegistryKeyTypeList.Images.SetKeyName(0, "reg_string.png");
            this.imageRegistryKeyTypeList.Images.SetKeyName(1, "reg_binary.png");
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
            // tv_ContextMenuStrip
            // 
            this.tv_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.tv_ContextMenuStrip.Name = "contextMenuStrip";
            this.tv_ContextMenuStrip.Size = new System.Drawing.Size(118, 76);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyToolStripMenuItem,
            this.toolStripSeparator2,
            this.stringValueToolStripMenuItem,
            this.binaryValueToolStripMenuItem,
            this.dWORD32bitValueToolStripMenuItem,
            this.qWORD64bitValueToolStripMenuItem,
            this.multiStringValueToolStripMenuItem,
            this.expandableStringValueToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // keyToolStripMenuItem
            // 
            this.keyToolStripMenuItem.Name = "keyToolStripMenuItem";
            this.keyToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.keyToolStripMenuItem.Text = "Key";
            this.keyToolStripMenuItem.Click += new System.EventHandler(this.createNewRegistryKey_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(196, 6);
            // 
            // stringValueToolStripMenuItem
            // 
            this.stringValueToolStripMenuItem.Name = "stringValueToolStripMenuItem";
            this.stringValueToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.stringValueToolStripMenuItem.Text = "String Value";
            this.stringValueToolStripMenuItem.Click += new System.EventHandler(this.createStringRegistryValue_Click);
            // 
            // binaryValueToolStripMenuItem
            // 
            this.binaryValueToolStripMenuItem.Name = "binaryValueToolStripMenuItem";
            this.binaryValueToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.binaryValueToolStripMenuItem.Text = "Binary Value";
            this.binaryValueToolStripMenuItem.Click += new System.EventHandler(this.createBinaryRegistryValue_Click);
            // 
            // dWORD32bitValueToolStripMenuItem
            // 
            this.dWORD32bitValueToolStripMenuItem.Name = "dWORD32bitValueToolStripMenuItem";
            this.dWORD32bitValueToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.dWORD32bitValueToolStripMenuItem.Text = "DWORD (32-bit) Value";
            this.dWORD32bitValueToolStripMenuItem.Click += new System.EventHandler(this.createDwordRegistryValue_Click);
            // 
            // qWORD64bitValueToolStripMenuItem
            // 
            this.qWORD64bitValueToolStripMenuItem.Name = "qWORD64bitValueToolStripMenuItem";
            this.qWORD64bitValueToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.qWORD64bitValueToolStripMenuItem.Text = "QWORD (64-bit) Value";
            this.qWORD64bitValueToolStripMenuItem.Click += new System.EventHandler(this.createQwordRegistryValue_Click);
            // 
            // multiStringValueToolStripMenuItem
            // 
            this.multiStringValueToolStripMenuItem.Name = "multiStringValueToolStripMenuItem";
            this.multiStringValueToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.multiStringValueToolStripMenuItem.Text = "Multi-String Value";
            this.multiStringValueToolStripMenuItem.Click += new System.EventHandler(this.createMultiStringRegistryValue_Click);
            // 
            // expandableStringValueToolStripMenuItem
            // 
            this.expandableStringValueToolStripMenuItem.Name = "expandableStringValueToolStripMenuItem";
            this.expandableStringValueToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.expandableStringValueToolStripMenuItem.Text = "Expandable String Value";
            this.expandableStringValueToolStripMenuItem.Click += new System.EventHandler(this.createExpandStringRegistryValue_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(114, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Enabled = false;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteRegistryKey_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Enabled = false;
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameRegistryKey_Click);
            // 
            // selectedItem_ContextMenuStrip
            // 
            this.selectedItem_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modifyToolStripMenuItem,
            this.modifyBinaryDataToolStripMenuItem,
            this.toolStripSeparator3,
            this.deleteToolStripMenuItem1,
            this.renameToolStripMenuItem1});
            this.selectedItem_ContextMenuStrip.Name = "selectedItem_ContextMenuStrip";
            this.selectedItem_ContextMenuStrip.Size = new System.Drawing.Size(185, 98);
            // 
            // modifyToolStripMenuItem
            // 
            this.modifyToolStripMenuItem.Enabled = false;
            this.modifyToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modifyToolStripMenuItem.Name = "modifyToolStripMenuItem";
            this.modifyToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.modifyToolStripMenuItem.Text = "Modify...";
            // 
            // modifyBinaryDataToolStripMenuItem
            // 
            this.modifyBinaryDataToolStripMenuItem.Enabled = false;
            this.modifyBinaryDataToolStripMenuItem.Name = "modifyBinaryDataToolStripMenuItem";
            this.modifyBinaryDataToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.modifyBinaryDataToolStripMenuItem.Text = "Modify Binary Data...";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(181, 6);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Enabled = false;
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            // 
            // renameToolStripMenuItem1
            // 
            this.renameToolStripMenuItem1.Enabled = false;
            this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
            this.renameToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
            this.renameToolStripMenuItem1.Text = "Rename";
            // 
            // lst_ContextMenuStrip
            // 
            this.lst_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem1});
            this.lst_ContextMenuStrip.Name = "lst_ContextMenuStrip";
            this.lst_ContextMenuStrip.Size = new System.Drawing.Size(99, 26);
            // 
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyToolStripMenuItem1,
            this.toolStripSeparator4,
            this.stringValueToolStripMenuItem1,
            this.binaryValueToolStripMenuItem1,
            this.dWORD32bitValueToolStripMenuItem1,
            this.qWORD64bitValueToolStripMenuItem1,
            this.multiStringValueToolStripMenuItem1,
            this.expandableStringValueToolStripMenuItem1});
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            this.newToolStripMenuItem1.Size = new System.Drawing.Size(98, 22);
            this.newToolStripMenuItem1.Text = "New";
            // 
            // keyToolStripMenuItem1
            // 
            this.keyToolStripMenuItem1.Name = "keyToolStripMenuItem1";
            this.keyToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.keyToolStripMenuItem1.Text = "Key";
            this.keyToolStripMenuItem1.Click += new System.EventHandler(this.createNewRegistryKey_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(196, 6);
            // 
            // stringValueToolStripMenuItem1
            // 
            this.stringValueToolStripMenuItem1.Name = "stringValueToolStripMenuItem1";
            this.stringValueToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.stringValueToolStripMenuItem1.Text = "String Value";
            this.stringValueToolStripMenuItem1.Click += new System.EventHandler(this.createStringRegistryValue_Click);
            // 
            // binaryValueToolStripMenuItem1
            // 
            this.binaryValueToolStripMenuItem1.Name = "binaryValueToolStripMenuItem1";
            this.binaryValueToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.binaryValueToolStripMenuItem1.Text = "Binary Value";
            this.binaryValueToolStripMenuItem1.Click += new System.EventHandler(this.createBinaryRegistryValue_Click);
            // 
            // dWORD32bitValueToolStripMenuItem1
            // 
            this.dWORD32bitValueToolStripMenuItem1.Name = "dWORD32bitValueToolStripMenuItem1";
            this.dWORD32bitValueToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.dWORD32bitValueToolStripMenuItem1.Text = "DWORD (32-bit) Value";
            this.dWORD32bitValueToolStripMenuItem1.Click += new System.EventHandler(this.createDwordRegistryValue_Click);
            // 
            // qWORD64bitValueToolStripMenuItem1
            // 
            this.qWORD64bitValueToolStripMenuItem1.Name = "qWORD64bitValueToolStripMenuItem1";
            this.qWORD64bitValueToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.qWORD64bitValueToolStripMenuItem1.Text = "QWORD (64-bit) Value";
            this.qWORD64bitValueToolStripMenuItem1.Click += new System.EventHandler(this.createQwordRegistryValue_Click);
            // 
            // multiStringValueToolStripMenuItem1
            // 
            this.multiStringValueToolStripMenuItem1.Name = "multiStringValueToolStripMenuItem1";
            this.multiStringValueToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.multiStringValueToolStripMenuItem1.Text = "Multi-String Value";
            this.multiStringValueToolStripMenuItem1.Click += new System.EventHandler(this.createMultiStringRegistryValue_Click);
            // 
            // expandableStringValueToolStripMenuItem1
            // 
            this.expandableStringValueToolStripMenuItem1.Name = "expandableStringValueToolStripMenuItem1";
            this.expandableStringValueToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.expandableStringValueToolStripMenuItem1.Text = "Expandable String Value";
            this.expandableStringValueToolStripMenuItem1.Click += new System.EventHandler(this.createExpandStringRegistryValue_Click);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRegistryEditor_FormClosing);
            this.Load += new System.EventHandler(this.FrmRegistryEditor_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tv_ContextMenuStrip.ResumeLayout(false);
            this.selectedItem_ContextMenuStrip.ResumeLayout(false);
            this.lst_ContextMenuStrip.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip tv_ContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem stringValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem binaryValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dWORD32bitValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qWORD64bitValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiStringValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandableStringValueToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip selectedItem_ContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem modifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modifyBinaryDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip lst_ContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem keyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem stringValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem binaryValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem dWORD32bitValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem qWORD64bitValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem multiStringValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem expandableStringValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}