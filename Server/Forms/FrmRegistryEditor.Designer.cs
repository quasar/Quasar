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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRegistryEditor));
            this.tvRegistryDirectory = new System.Windows.Forms.TreeView();
            this.lstRegistryKeys = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // tvRegistryDirectory
            // 
            this.tvRegistryDirectory.Location = new System.Drawing.Point(4, 5);
            this.tvRegistryDirectory.Name = "tvRegistryDirectory";
            this.tvRegistryDirectory.ShowPlusMinus = false;
            this.tvRegistryDirectory.Size = new System.Drawing.Size(155, 402);
            this.tvRegistryDirectory.TabIndex = 0;
            // 
            // lstRegistryKeys
            // 
            this.lstRegistryKeys.Location = new System.Drawing.Point(163, 5);
            this.lstRegistryKeys.Name = "lstRegistryKeys";
            this.lstRegistryKeys.Size = new System.Drawing.Size(526, 402);
            this.lstRegistryKeys.TabIndex = 1;
            this.lstRegistryKeys.UseCompatibleStateImageBehavior = false;
            // 
            // FrmRegistryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 427);
            this.Controls.Add(this.lstRegistryKeys);
            this.Controls.Add(this.tvRegistryDirectory);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(710, 465);
            this.Name = "FrmRegistryEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "xRAT 2.0 - Registry Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvRegistryDirectory;
        private System.Windows.Forms.ListView lstRegistryKeys;
    }
}