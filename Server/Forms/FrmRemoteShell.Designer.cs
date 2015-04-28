namespace xServer.Forms
{
    partial class FrmRemoteShell
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRemoteShell));
            this.txtConsoleOutput = new System.Windows.Forms.TextBox();
            this.txtConsoleInput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtConsoleOutput
            // 
            this.txtConsoleOutput.BackColor = System.Drawing.Color.Black;
            this.txtConsoleOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConsoleOutput.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtConsoleOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsoleOutput.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtConsoleOutput.Location = new System.Drawing.Point(0, 0);
            this.txtConsoleOutput.Multiline = true;
            this.txtConsoleOutput.Name = "txtConsoleOutput";
            this.txtConsoleOutput.ReadOnly = true;
            this.txtConsoleOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsoleOutput.Size = new System.Drawing.Size(637, 307);
            this.txtConsoleOutput.TabIndex = 1;
            this.txtConsoleOutput.TextChanged += new System.EventHandler(this.txtConsoleOutput_TextChanged);
            this.txtConsoleOutput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtConsoleOutput_KeyPress);
            // 
            // txtConsoleInput
            // 
            this.txtConsoleInput.BackColor = System.Drawing.Color.Black;
            this.txtConsoleInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConsoleInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtConsoleInput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsoleInput.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.txtConsoleInput.Location = new System.Drawing.Point(0, 307);
            this.txtConsoleInput.MaxLength = 200;
            this.txtConsoleInput.Name = "txtConsoleInput";
            this.txtConsoleInput.Size = new System.Drawing.Size(637, 16);
            this.txtConsoleInput.TabIndex = 0;
            this.txtConsoleInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConsoleInput_KeyDown);
            // 
            // frmRemoteShell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 323);
            this.Controls.Add(this.txtConsoleInput);
            this.Controls.Add(this.txtConsoleOutput);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRemoteShell";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Remote Shell []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRemoteShell_FormClosing);
            this.Load += new System.EventHandler(this.FrmRemoteShell_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConsoleInput;
        public System.Windows.Forms.TextBox txtConsoleOutput;
    }
}