using xServer.Controls;

namespace xServer.Forms
{
    partial class FrmTaskManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTaskManager));
            this.ctxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxtKillProcess = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtStartProcess = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.lstTasks = new ListViewEx();
            this.hProcessname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hPID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxtLine = new System.Windows.Forms.ToolStripSeparator();
            this.ctxtMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxtMenu
            // 
            this.ctxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtKillProcess,
            this.ctxtStartProcess,
            this.ctxtLine,
            this.ctxtRefresh});
            this.ctxtMenu.Name = "ctxtMenu";
            this.ctxtMenu.Size = new System.Drawing.Size(153, 98);
            // 
            // ctxtKillProcess
            // 
            this.ctxtKillProcess.Image = global::xServer.Properties.Resources.cancel;
            this.ctxtKillProcess.Name = "ctxtKillProcess";
            this.ctxtKillProcess.Size = new System.Drawing.Size(152, 22);
            this.ctxtKillProcess.Text = "Kill Process";
            this.ctxtKillProcess.Click += new System.EventHandler(this.ctxtKillProcess_Click);
            // 
            // ctxtStartProcess
            // 
            this.ctxtStartProcess.Image = global::xServer.Properties.Resources.run;
            this.ctxtStartProcess.Name = "ctxtStartProcess";
            this.ctxtStartProcess.Size = new System.Drawing.Size(152, 22);
            this.ctxtStartProcess.Text = "Start Process";
            this.ctxtStartProcess.Click += new System.EventHandler(this.ctxtStartProcess_Click);
            // 
            // ctxtRefresh
            // 
            this.ctxtRefresh.Image = global::xServer.Properties.Resources.refresh;
            this.ctxtRefresh.Name = "ctxtRefresh";
            this.ctxtRefresh.Size = new System.Drawing.Size(152, 22);
            this.ctxtRefresh.Text = "Refresh";
            this.ctxtRefresh.Click += new System.EventHandler(this.ctxtRefresh_Click);
            // 
            // lstTasks
            // 
            this.lstTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hProcessname,
            this.hPID,
            this.hTitle});
            this.lstTasks.ContextMenuStrip = this.ctxtMenu;
            this.lstTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTasks.FullRowSelect = true;
            this.lstTasks.GridLines = true;
            this.lstTasks.Location = new System.Drawing.Point(0, 0);
            this.lstTasks.Name = "lstTasks";
            this.lstTasks.Size = new System.Drawing.Size(335, 411);
            this.lstTasks.TabIndex = 1;
            this.lstTasks.UseCompatibleStateImageBehavior = false;
            this.lstTasks.View = System.Windows.Forms.View.Details;
            this.lstTasks.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstTasks_ColumnClick);
            // 
            // hProcessname
            // 
            this.hProcessname.Text = "Processname";
            this.hProcessname.Width = 143;
            // 
            // hPID
            // 
            this.hPID.Text = "PID";
            // 
            // hTitle
            // 
            this.hTitle.Text = "Title";
            this.hTitle.Width = 115;
            // 
            // ctxtLine
            // 
            this.ctxtLine.Name = "ctxtLine";
            this.ctxtLine.Size = new System.Drawing.Size(149, 6);
            // 
            // frmTaskManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 411);
            this.Controls.Add(this.lstTasks);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(351, 449);
            this.Name = "frmTaskManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "xRAT 2.0 - Task Manager []";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTaskManager_FormClosing);
            this.Load += new System.EventHandler(this.FrmTaskManager_Load);
            this.ctxtMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip ctxtMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxtKillProcess;
        private System.Windows.Forms.ToolStripMenuItem ctxtRefresh;
        private System.Windows.Forms.ToolStripMenuItem ctxtStartProcess;
        public Controls.ListViewEx lstTasks;
        private System.Windows.Forms.ColumnHeader hProcessname;
        private System.Windows.Forms.ColumnHeader hPID;
        private System.Windows.Forms.ColumnHeader hTitle;
        private System.Windows.Forms.ToolStripSeparator ctxtLine;
    }
}