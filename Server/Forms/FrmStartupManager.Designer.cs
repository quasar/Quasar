namespace xServer.Forms
{
    partial class FrmStartupManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmStartupManager));
            this.ctxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxtAddEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtRemoveEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.lstStartupItems = new xServer.Controls.ListViewEx();
            this.hName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxtMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxtMenu
            // 
            this.ctxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtAddEntry,
            this.ctxtRemoveEntry});
            this.ctxtMenu.Name = "ctxtMenu";
            this.ctxtMenu.Size = new System.Drawing.Size(148, 48);
            // 
            // ctxtAddEntry
            // 
            this.ctxtAddEntry.Image = global::xServer.Properties.Resources.application_add;
            this.ctxtAddEntry.Name = "ctxtAddEntry";
            this.ctxtAddEntry.Size = new System.Drawing.Size(147, 22);
            this.ctxtAddEntry.Text = "Add Entry";
            this.ctxtAddEntry.Click += new System.EventHandler(this.ctxtAddEntry_Click);
            // 
            // ctxtRemoveEntry
            // 
            this.ctxtRemoveEntry.Image = global::xServer.Properties.Resources.application_delete;
            this.ctxtRemoveEntry.Name = "ctxtRemoveEntry";
            this.ctxtRemoveEntry.Size = new System.Drawing.Size(147, 22);
            this.ctxtRemoveEntry.Text = "Remove Entry";
            this.ctxtRemoveEntry.Click += new System.EventHandler(this.ctxtRemoveEntry_Click);
            // 
            // lstStartupItems
            // 
            this.lstStartupItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstStartupItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hName,
            this.hPath});
            this.lstStartupItems.ContextMenuStrip = this.ctxtMenu;
            this.lstStartupItems.FullRowSelect = true;
            this.lstStartupItems.GridLines = true;
            this.lstStartupItems.Location = new System.Drawing.Point(12, 12);
            this.lstStartupItems.Name = "lstStartupItems";
            this.lstStartupItems.Size = new System.Drawing.Size(653, 349);
            this.lstStartupItems.TabIndex = 0;
            this.lstStartupItems.UseCompatibleStateImageBehavior = false;
            this.lstStartupItems.View = System.Windows.Forms.View.Details;
            this.lstStartupItems.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstStartupItems_ColumnClick);
            // 
            // hName
            // 
            this.hName.Text = "Name";
            this.hName.Width = 187;
            // 
            // hPath
            // 
            this.hPath.Text = "Path";
            this.hPath.Width = 460;
            // 
            // FrmStartupManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 373);
            this.Controls.Add(this.lstStartupItems);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "FrmStartupManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Startup Manager []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmStartupManager_FormClosing);
            this.Load += new System.EventHandler(this.FrmStartupManager_Load);
            this.ctxtMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public Controls.ListViewEx lstStartupItems;
        private System.Windows.Forms.ColumnHeader hName;
        private System.Windows.Forms.ColumnHeader hPath;
        private System.Windows.Forms.ContextMenuStrip ctxtMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxtAddEntry;
        private System.Windows.Forms.ToolStripMenuItem ctxtRemoveEntry;

    }
}