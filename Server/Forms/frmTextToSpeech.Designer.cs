namespace xRAT_2.Forms
{
    partial class frmTextToSpeech
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
            this.btnSend = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtSpeech = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(276, 116);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(195, 116);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtSpeech
            // 
            this.txtSpeech.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSpeech.Location = new System.Drawing.Point(12, 12);
            this.txtSpeech.Name = "txtSpeech";
            this.txtSpeech.Size = new System.Drawing.Size(339, 98);
            this.txtSpeech.TabIndex = 2;
            this.txtSpeech.Text = "";
            // 
            // frmTextToSpeech
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 151);
            this.Controls.Add(this.txtSpeech);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnSend);
            this.Name = "frmTextToSpeech";
            this.Text = "frmTextToSpeech";
            this.Load += new System.EventHandler(this.frmTextToSpeech_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.RichTextBox txtSpeech;
    }
}