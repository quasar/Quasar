namespace xRAT_2.Forms
{
    partial class frmProfileChooser
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
            this.lblProfileSelector = new System.Windows.Forms.Label();
            this.btnProfileSelect = new System.Windows.Forms.Button();
            this.listViewProfileSelect = new System.Windows.Forms.ListView();
            this.ProfileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProfilePath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblProfileSelector
            // 
            this.lblProfileSelector.AutoSize = true;
            this.lblProfileSelector.Location = new System.Drawing.Point(12, 9);
            this.lblProfileSelector.Name = "lblProfileSelector";
            this.lblProfileSelector.Size = new System.Drawing.Size(81, 13);
            this.lblProfileSelector.TabIndex = 1;
            this.lblProfileSelector.Text = "Select a Profile:";
            // 
            // btnProfileSelect
            // 
            this.btnProfileSelect.Location = new System.Drawing.Point(255, 126);
            this.btnProfileSelect.Name = "btnProfileSelect";
            this.btnProfileSelect.Size = new System.Drawing.Size(90, 23);
            this.btnProfileSelect.TabIndex = 2;
            this.btnProfileSelect.Text = "Select Profile";
            this.btnProfileSelect.UseVisualStyleBackColor = true;
            this.btnProfileSelect.Click += new System.EventHandler(this.btnProfileSelect_Click);
            // 
            // listViewProfileSelect
            // 
            this.listViewProfileSelect.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ProfileName,
            this.ProfilePath});
            this.listViewProfileSelect.Location = new System.Drawing.Point(12, 25);
            this.listViewProfileSelect.Name = "listViewProfileSelect";
            this.listViewProfileSelect.Size = new System.Drawing.Size(333, 95);
            this.listViewProfileSelect.TabIndex = 3;
            this.listViewProfileSelect.UseCompatibleStateImageBehavior = false;
            this.listViewProfileSelect.View = System.Windows.Forms.View.Details;
            // 
            // ProfileName
            // 
            this.ProfileName.Text = "Profile Name";
            this.ProfileName.Width = 96;
            // 
            // ProfilePath
            // 
            this.ProfilePath.Text = "Profile Path";
            this.ProfilePath.Width = 228;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 126);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Create New";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmProfileChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 155);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listViewProfileSelect);
            this.Controls.Add(this.btnProfileSelect);
            this.Controls.Add(this.lblProfileSelector);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmProfileChooser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose a Profile";
            this.Load += new System.EventHandler(this.frmProfileChooser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProfileSelector;
        private System.Windows.Forms.Button btnProfileSelect;
        private System.Windows.Forms.ListView listViewProfileSelect;
        private System.Windows.Forms.ColumnHeader ProfileName;
        private System.Windows.Forms.ColumnHeader ProfilePath;
        private System.Windows.Forms.Button button1;
    }
}