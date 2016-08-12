namespace xServer.Forms
{
    partial class FrmRegValueEditWord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRegValueEditWord));
            this.valueNameTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.baseBox = new System.Windows.Forms.GroupBox();
            this.radioDecimal = new System.Windows.Forms.RadioButton();
            this.radioHexa = new System.Windows.Forms.RadioButton();
            this.valueDataTxtBox = new xServer.Controls.WordTextBox();
            this.baseBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // valueNameTxtBox
            // 
            this.valueNameTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueNameTxtBox.Location = new System.Drawing.Point(12, 27);
            this.valueNameTxtBox.Name = "valueNameTxtBox";
            this.valueNameTxtBox.ReadOnly = true;
            this.valueNameTxtBox.Size = new System.Drawing.Size(334, 20);
            this.valueNameTxtBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Value name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Value data:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(271, 128);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(190, 128);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // baseBox
            // 
            this.baseBox.Controls.Add(this.radioDecimal);
            this.baseBox.Controls.Add(this.radioHexa);
            this.baseBox.Location = new System.Drawing.Point(190, 53);
            this.baseBox.Name = "baseBox";
            this.baseBox.Size = new System.Drawing.Size(156, 63);
            this.baseBox.TabIndex = 6;
            this.baseBox.TabStop = false;
            this.baseBox.Text = "Base";
            // 
            // radioDecimal
            // 
            this.radioDecimal.AutoSize = true;
            this.radioDecimal.Location = new System.Drawing.Point(14, 40);
            this.radioDecimal.Name = "radioDecimal";
            this.radioDecimal.Size = new System.Drawing.Size(63, 17);
            this.radioDecimal.TabIndex = 4;
            this.radioDecimal.Text = "Decimal";
            this.radioDecimal.UseVisualStyleBackColor = true;
            // 
            // radioHexa
            // 
            this.radioHexa.AutoSize = true;
            this.radioHexa.Checked = true;
            this.radioHexa.Location = new System.Drawing.Point(14, 17);
            this.radioHexa.Name = "radioHexa";
            this.radioHexa.Size = new System.Drawing.Size(86, 17);
            this.radioHexa.TabIndex = 3;
            this.radioHexa.TabStop = true;
            this.radioHexa.Text = "Hexadecimal";
            this.radioHexa.UseVisualStyleBackColor = true;
            this.radioHexa.CheckedChanged += new System.EventHandler(this.radioHex_CheckboxChanged);
            // 
            // valueDataTxtBox
            // 
            this.valueDataTxtBox.IsHexNumber = true;
            this.valueDataTxtBox.Location = new System.Drawing.Point(12, 70);
            this.valueDataTxtBox.MaxLength = 8;
            this.valueDataTxtBox.Name = "valueDataTxtBox";
            this.valueDataTxtBox.Size = new System.Drawing.Size(161, 20);
            this.valueDataTxtBox.TabIndex = 0;
            this.valueDataTxtBox.Text = "0";
            this.valueDataTxtBox.Type = xServer.Enums.WordType.DWORD;
            // 
            // FrmRegValueEditWord
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(358, 163);
            this.Controls.Add(this.valueDataTxtBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.baseBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.valueNameTxtBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRegValueEditWord";
            this.ShowIcon = false;
            this.Text = "Edit";
            this.baseBox.ResumeLayout(false);
            this.baseBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox valueNameTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox baseBox;
        private System.Windows.Forms.RadioButton radioDecimal;
        private System.Windows.Forms.RadioButton radioHexa;
        private Controls.WordTextBox valueDataTxtBox;
    }
}