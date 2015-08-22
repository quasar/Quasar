using xServer.Controls;

namespace xServer.Forms
{
    partial class FrmSystemInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSystemInformation));
            this.lstSystem = new xServer.Controls.AeroListView();
            this.hComponent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxtCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstSystem
            // 
            this.lstSystem.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hComponent,
            this.hValue});
            this.lstSystem.ContextMenuStrip = this.ctxtMenu;
            this.lstSystem.FullRowSelect = true;
            this.lstSystem.GridLines = true;
            this.lstSystem.Location = new System.Drawing.Point(12, 12);
            this.lstSystem.Name = "lstSystem";
            this.lstSystem.Size = new System.Drawing.Size(437, 228);
            this.lstSystem.TabIndex = 0;
            this.lstSystem.UseCompatibleStateImageBehavior = false;
            this.lstSystem.View = System.Windows.Forms.View.Details;
            // 
            // hComponent
            // 
            this.hComponent.Text = "Component";
            this.hComponent.Width = 172;
            // 
            // hValue
            // 
            this.hValue.Text = "Value";
            this.hValue.Width = 242;
            // 
            // ctxtMenu
            // 
            this.ctxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtCopy});
            this.ctxtMenu.Name = "ctxtMenu";
            this.ctxtMenu.Size = new System.Drawing.Size(103, 26);
            // 
            // ctxtCopy
            // 
            this.ctxtCopy.Image = global::xServer.Properties.Resources.copy;
            this.ctxtCopy.Name = "ctxtCopy";
            this.ctxtCopy.Size = new System.Drawing.Size(102, 22);
            this.ctxtCopy.Text = "Copy";
            this.ctxtCopy.Click += new System.EventHandler(this.ctxtCopy_Click);
            // 
            // FrmSystemInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 252);
            this.Controls.Add(this.lstSystem);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSystemInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "System Information []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSystemInformation_FormClosing);
            this.Load += new System.EventHandler(this.FrmSystemInformation_Load);
            this.ctxtMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader hComponent;
        private System.Windows.Forms.ColumnHeader hValue;
        private System.Windows.Forms.ContextMenuStrip ctxtMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxtCopy;
        private AeroListView lstSystem;
    }
}