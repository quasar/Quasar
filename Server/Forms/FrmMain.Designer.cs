using xServer.Controls;

namespace xServer.Forms
{
    partial class FrmMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.ctxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxtConnection = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtReconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtUninstall = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtSystem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtSystemInformation = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtFileManager = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtStartupManager = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtTaskManager = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtRemoteShell = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtReverseProxy = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtRegistryEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtLine = new System.Windows.Forms.ToolStripSeparator();
            this.ctxtActions = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtShutdown = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtStandby = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtSurveillance = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtRemoteDesktop = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtPasswordRecovery = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtKeylogger = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtMiscellaneous = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtRemoteExecute = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtLocalFile = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtWebFile = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtVisitWebsite = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxtShowMessagebox = new System.Windows.Forms.ToolStripMenuItem();
            this.botStrip = new System.Windows.Forms.StatusStrip();
            this.botListen = new System.Windows.Forms.ToolStripStatusLabel();
            this.imgFlags = new System.Windows.Forms.ImageList(this.components);
            this.nIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.lstClients = new xServer.Controls.AeroListView();
            this.hIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hUserPC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hUserStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hCountry = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hOS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hAccountType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainMenu = new xServer.Controls.MainMenuEx();
            this.menuFile = new System.Windows.Forms.MenuItem();
            this.menuClose = new System.Windows.Forms.MenuItem();
            this.menuSettings = new System.Windows.Forms.MenuItem();
            this.menuBuilder = new System.Windows.Forms.MenuItem();
            this.menuStatistics = new System.Windows.Forms.MenuItem();
            this.menuAbout = new System.Windows.Forms.MenuItem();
            this.ctxtMenu.SuspendLayout();
            this.botStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxtMenu
            // 
            this.ctxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtConnection,
            this.ctxtSystem,
            this.ctxtSurveillance,
            this.ctxtMiscellaneous});
            this.ctxtMenu.Name = "ctxtMenu";
            this.ctxtMenu.Size = new System.Drawing.Size(153, 114);
            // 
            // ctxtConnection
            // 
            this.ctxtConnection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtUpdate,
            this.ctxtReconnect,
            this.ctxtDisconnect,
            this.ctxtUninstall});
            this.ctxtConnection.Image = ((System.Drawing.Image)(resources.GetObject("ctxtConnection.Image")));
            this.ctxtConnection.Name = "ctxtConnection";
            this.ctxtConnection.Size = new System.Drawing.Size(152, 22);
            this.ctxtConnection.Text = "Connection";
            // 
            // ctxtUpdate
            // 
            this.ctxtUpdate.Image = ((System.Drawing.Image)(resources.GetObject("ctxtUpdate.Image")));
            this.ctxtUpdate.Name = "ctxtUpdate";
            this.ctxtUpdate.Size = new System.Drawing.Size(133, 22);
            this.ctxtUpdate.Text = "Update";
            this.ctxtUpdate.Click += new System.EventHandler(this.ctxtUpdate_Click);
            // 
            // ctxtReconnect
            // 
            this.ctxtReconnect.Image = ((System.Drawing.Image)(resources.GetObject("ctxtReconnect.Image")));
            this.ctxtReconnect.Name = "ctxtReconnect";
            this.ctxtReconnect.Size = new System.Drawing.Size(133, 22);
            this.ctxtReconnect.Text = "Reconnect";
            this.ctxtReconnect.Click += new System.EventHandler(this.ctxtReconnect_Click);
            // 
            // ctxtDisconnect
            // 
            this.ctxtDisconnect.Image = ((System.Drawing.Image)(resources.GetObject("ctxtDisconnect.Image")));
            this.ctxtDisconnect.Name = "ctxtDisconnect";
            this.ctxtDisconnect.Size = new System.Drawing.Size(133, 22);
            this.ctxtDisconnect.Text = "Disconnect";
            this.ctxtDisconnect.Click += new System.EventHandler(this.ctxtDisconnect_Click);
            // 
            // ctxtUninstall
            // 
            this.ctxtUninstall.Image = ((System.Drawing.Image)(resources.GetObject("ctxtUninstall.Image")));
            this.ctxtUninstall.Name = "ctxtUninstall";
            this.ctxtUninstall.Size = new System.Drawing.Size(133, 22);
            this.ctxtUninstall.Text = "Uninstall";
            this.ctxtUninstall.Click += new System.EventHandler(this.ctxtUninstall_Click);
            // 
            // ctxtSystem
            // 
            this.ctxtSystem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtSystemInformation,
            this.ctxtFileManager,
            this.ctxtStartupManager,
            this.ctxtTaskManager,
            this.ctxtRemoteShell,
            this.ctxtReverseProxy,
            this.ctxtRegistryEditor,
            this.ctxtLine,
            this.ctxtActions});
            this.ctxtSystem.Image = ((System.Drawing.Image)(resources.GetObject("ctxtSystem.Image")));
            this.ctxtSystem.Name = "ctxtSystem";
            this.ctxtSystem.Size = new System.Drawing.Size(152, 22);
            this.ctxtSystem.Text = "System";
            // 
            // ctxtSystemInformation
            // 
            this.ctxtSystemInformation.Image = ((System.Drawing.Image)(resources.GetObject("ctxtSystemInformation.Image")));
            this.ctxtSystemInformation.Name = "ctxtSystemInformation";
            this.ctxtSystemInformation.Size = new System.Drawing.Size(178, 22);
            this.ctxtSystemInformation.Text = "System Information";
            this.ctxtSystemInformation.Click += new System.EventHandler(this.ctxtSystemInformation_Click);
            // 
            // ctxtFileManager
            // 
            this.ctxtFileManager.Image = ((System.Drawing.Image)(resources.GetObject("ctxtFileManager.Image")));
            this.ctxtFileManager.Name = "ctxtFileManager";
            this.ctxtFileManager.Size = new System.Drawing.Size(178, 22);
            this.ctxtFileManager.Text = "File Manager";
            this.ctxtFileManager.Click += new System.EventHandler(this.ctxtFileManager_Click);
            // 
            // ctxtStartupManager
            // 
            this.ctxtStartupManager.Image = global::xServer.Properties.Resources.startup_programs;
            this.ctxtStartupManager.Name = "ctxtStartupManager";
            this.ctxtStartupManager.Size = new System.Drawing.Size(178, 22);
            this.ctxtStartupManager.Text = "Startup Manager";
            this.ctxtStartupManager.Click += new System.EventHandler(this.ctxtStartupManager_Click);
            // 
            // ctxtTaskManager
            // 
            this.ctxtTaskManager.Image = ((System.Drawing.Image)(resources.GetObject("ctxtTaskManager.Image")));
            this.ctxtTaskManager.Name = "ctxtTaskManager";
            this.ctxtTaskManager.Size = new System.Drawing.Size(178, 22);
            this.ctxtTaskManager.Text = "Task Manager";
            this.ctxtTaskManager.Click += new System.EventHandler(this.ctxtTaskManager_Click);
            // 
            // ctxtRemoteShell
            // 
            this.ctxtRemoteShell.Image = ((System.Drawing.Image)(resources.GetObject("ctxtRemoteShell.Image")));
            this.ctxtRemoteShell.Name = "ctxtRemoteShell";
            this.ctxtRemoteShell.Size = new System.Drawing.Size(178, 22);
            this.ctxtRemoteShell.Text = "Remote Shell";
            this.ctxtRemoteShell.Click += new System.EventHandler(this.ctxtRemoteShell_Click);
            // 
            // ctxtReverseProxy
            // 
            this.ctxtReverseProxy.Image = global::xServer.Properties.Resources.server_link;
            this.ctxtReverseProxy.Name = "ctxtReverseProxy";
            this.ctxtReverseProxy.Size = new System.Drawing.Size(178, 22);
            this.ctxtReverseProxy.Text = "Reverse Proxy";
            this.ctxtReverseProxy.Click += new System.EventHandler(this.ctxtReverseProxy_Click);
            // 
            // ctxtRegistryEditor
            // 
            this.ctxtRegistryEditor.Image = global::xServer.Properties.Resources.registry;
            this.ctxtRegistryEditor.Name = "ctxtRegistryEditor";
            this.ctxtRegistryEditor.Size = new System.Drawing.Size(178, 22);
            this.ctxtRegistryEditor.Text = "Registry Editor";
            this.ctxtRegistryEditor.Click += new System.EventHandler(this.ctxtRegistryEditor_Click);
            // 
            // ctxtLine
            // 
            this.ctxtLine.Name = "ctxtLine";
            this.ctxtLine.Size = new System.Drawing.Size(175, 6);
            // 
            // ctxtActions
            // 
            this.ctxtActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtShutdown,
            this.ctxtRestart,
            this.ctxtStandby});
            this.ctxtActions.Image = global::xServer.Properties.Resources.actions;
            this.ctxtActions.Name = "ctxtActions";
            this.ctxtActions.Size = new System.Drawing.Size(178, 22);
            this.ctxtActions.Text = "Actions";
            // 
            // ctxtShutdown
            // 
            this.ctxtShutdown.Image = global::xServer.Properties.Resources.shutdown;
            this.ctxtShutdown.Name = "ctxtShutdown";
            this.ctxtShutdown.Size = new System.Drawing.Size(128, 22);
            this.ctxtShutdown.Text = "Shutdown";
            this.ctxtShutdown.Click += new System.EventHandler(this.ctxtShutdown_Click);
            // 
            // ctxtRestart
            // 
            this.ctxtRestart.Image = global::xServer.Properties.Resources.restart;
            this.ctxtRestart.Name = "ctxtRestart";
            this.ctxtRestart.Size = new System.Drawing.Size(128, 22);
            this.ctxtRestart.Text = "Restart";
            this.ctxtRestart.Click += new System.EventHandler(this.ctxtRestart_Click);
            // 
            // ctxtStandby
            // 
            this.ctxtStandby.Image = global::xServer.Properties.Resources.standby;
            this.ctxtStandby.Name = "ctxtStandby";
            this.ctxtStandby.Size = new System.Drawing.Size(128, 22);
            this.ctxtStandby.Text = "Standby";
            this.ctxtStandby.Click += new System.EventHandler(this.ctxtStandby_Click);
            // 
            // ctxtSurveillance
            // 
            this.ctxtSurveillance.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtRemoteDesktop,
            this.ctxtPasswordRecovery,
            this.ctxtKeylogger});
            this.ctxtSurveillance.Image = ((System.Drawing.Image)(resources.GetObject("ctxtSurveillance.Image")));
            this.ctxtSurveillance.Name = "ctxtSurveillance";
            this.ctxtSurveillance.Size = new System.Drawing.Size(152, 22);
            this.ctxtSurveillance.Text = "Surveillance";
            // 
            // ctxtRemoteDesktop
            // 
            this.ctxtRemoteDesktop.Image = ((System.Drawing.Image)(resources.GetObject("ctxtRemoteDesktop.Image")));
            this.ctxtRemoteDesktop.Name = "ctxtRemoteDesktop";
            this.ctxtRemoteDesktop.Size = new System.Drawing.Size(175, 22);
            this.ctxtRemoteDesktop.Text = "Remote Desktop";
            this.ctxtRemoteDesktop.Click += new System.EventHandler(this.ctxtRemoteDesktop_Click);
            // 
            // ctxtPasswordRecovery
            // 
            this.ctxtPasswordRecovery.Image = ((System.Drawing.Image)(resources.GetObject("ctxtPasswordRecovery.Image")));
            this.ctxtPasswordRecovery.Name = "ctxtPasswordRecovery";
            this.ctxtPasswordRecovery.Size = new System.Drawing.Size(175, 22);
            this.ctxtPasswordRecovery.Text = "Password Recovery";
            this.ctxtPasswordRecovery.Click += new System.EventHandler(this.ctxtPasswordRecovery_Click);
            // 
            // ctxtKeylogger
            // 
            this.ctxtKeylogger.Image = global::xServer.Properties.Resources.logger;
            this.ctxtKeylogger.Name = "ctxtKeylogger";
            this.ctxtKeylogger.Size = new System.Drawing.Size(175, 22);
            this.ctxtKeylogger.Text = "Keylogger";
            this.ctxtKeylogger.Click += new System.EventHandler(this.ctxtKeylogger_Click);
            // 
            // ctxtMiscellaneous
            // 
            this.ctxtMiscellaneous.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtRemoteExecute,
            this.ctxtVisitWebsite,
            this.ctxtShowMessagebox});
            this.ctxtMiscellaneous.Image = ((System.Drawing.Image)(resources.GetObject("ctxtMiscellaneous.Image")));
            this.ctxtMiscellaneous.Name = "ctxtMiscellaneous";
            this.ctxtMiscellaneous.Size = new System.Drawing.Size(152, 22);
            this.ctxtMiscellaneous.Text = "Miscellaneous";
            // 
            // ctxtRemoteExecute
            // 
            this.ctxtRemoteExecute.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxtLocalFile,
            this.ctxtWebFile});
            this.ctxtRemoteExecute.Image = global::xServer.Properties.Resources.lightning;
            this.ctxtRemoteExecute.Name = "ctxtRemoteExecute";
            this.ctxtRemoteExecute.Size = new System.Drawing.Size(171, 22);
            this.ctxtRemoteExecute.Text = "Remote Execute";
            // 
            // ctxtLocalFile
            // 
            this.ctxtLocalFile.Image = global::xServer.Properties.Resources.drive_go;
            this.ctxtLocalFile.Name = "ctxtLocalFile";
            this.ctxtLocalFile.Size = new System.Drawing.Size(132, 22);
            this.ctxtLocalFile.Text = "Local File...";
            this.ctxtLocalFile.Click += new System.EventHandler(this.ctxtLocalFile_Click);
            // 
            // ctxtWebFile
            // 
            this.ctxtWebFile.Image = global::xServer.Properties.Resources.world_go;
            this.ctxtWebFile.Name = "ctxtWebFile";
            this.ctxtWebFile.Size = new System.Drawing.Size(132, 22);
            this.ctxtWebFile.Text = "Web File...";
            this.ctxtWebFile.Click += new System.EventHandler(this.ctxtWebFile_Click);
            // 
            // ctxtVisitWebsite
            // 
            this.ctxtVisitWebsite.Image = ((System.Drawing.Image)(resources.GetObject("ctxtVisitWebsite.Image")));
            this.ctxtVisitWebsite.Name = "ctxtVisitWebsite";
            this.ctxtVisitWebsite.Size = new System.Drawing.Size(171, 22);
            this.ctxtVisitWebsite.Text = "Visit Website";
            this.ctxtVisitWebsite.Click += new System.EventHandler(this.ctxtVisitWebsite_Click);
            // 
            // ctxtShowMessagebox
            // 
            this.ctxtShowMessagebox.Image = ((System.Drawing.Image)(resources.GetObject("ctxtShowMessagebox.Image")));
            this.ctxtShowMessagebox.Name = "ctxtShowMessagebox";
            this.ctxtShowMessagebox.Size = new System.Drawing.Size(171, 22);
            this.ctxtShowMessagebox.Text = "Show Messagebox";
            this.ctxtShowMessagebox.Click += new System.EventHandler(this.ctxtShowMessagebox_Click);
            // 
            // botStrip
            // 
            this.botStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.botListen});
            this.botStrip.Location = new System.Drawing.Point(0, 382);
            this.botStrip.Name = "botStrip";
            this.botStrip.Size = new System.Drawing.Size(1006, 22);
            this.botStrip.TabIndex = 4;
            this.botStrip.Text = "statusStrip1";
            // 
            // botListen
            // 
            this.botListen.Name = "botListen";
            this.botListen.Size = new System.Drawing.Size(87, 17);
            this.botListen.Text = "Listening: False";
            // 
            // imgFlags
            // 
            this.imgFlags.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgFlags.ImageStream")));
            this.imgFlags.TransparentColor = System.Drawing.Color.Transparent;
            this.imgFlags.Images.SetKeyName(0, "ad.png");
            this.imgFlags.Images.SetKeyName(1, "ae.png");
            this.imgFlags.Images.SetKeyName(2, "af.png");
            this.imgFlags.Images.SetKeyName(3, "ag.png");
            this.imgFlags.Images.SetKeyName(4, "ai.png");
            this.imgFlags.Images.SetKeyName(5, "al.png");
            this.imgFlags.Images.SetKeyName(6, "am.png");
            this.imgFlags.Images.SetKeyName(7, "an.png");
            this.imgFlags.Images.SetKeyName(8, "ao.png");
            this.imgFlags.Images.SetKeyName(9, "ar.png");
            this.imgFlags.Images.SetKeyName(10, "as.png");
            this.imgFlags.Images.SetKeyName(11, "at.png");
            this.imgFlags.Images.SetKeyName(12, "au.png");
            this.imgFlags.Images.SetKeyName(13, "aw.png");
            this.imgFlags.Images.SetKeyName(14, "ax.png");
            this.imgFlags.Images.SetKeyName(15, "az.png");
            this.imgFlags.Images.SetKeyName(16, "ba.png");
            this.imgFlags.Images.SetKeyName(17, "bb.png");
            this.imgFlags.Images.SetKeyName(18, "bd.png");
            this.imgFlags.Images.SetKeyName(19, "be.png");
            this.imgFlags.Images.SetKeyName(20, "bf.png");
            this.imgFlags.Images.SetKeyName(21, "bg.png");
            this.imgFlags.Images.SetKeyName(22, "bh.png");
            this.imgFlags.Images.SetKeyName(23, "bi.png");
            this.imgFlags.Images.SetKeyName(24, "bj.png");
            this.imgFlags.Images.SetKeyName(25, "bm.png");
            this.imgFlags.Images.SetKeyName(26, "bn.png");
            this.imgFlags.Images.SetKeyName(27, "bo.png");
            this.imgFlags.Images.SetKeyName(28, "br.png");
            this.imgFlags.Images.SetKeyName(29, "bs.png");
            this.imgFlags.Images.SetKeyName(30, "bt.png");
            this.imgFlags.Images.SetKeyName(31, "bv.png");
            this.imgFlags.Images.SetKeyName(32, "bw.png");
            this.imgFlags.Images.SetKeyName(33, "by.png");
            this.imgFlags.Images.SetKeyName(34, "bz.png");
            this.imgFlags.Images.SetKeyName(35, "ca.png");
            this.imgFlags.Images.SetKeyName(36, "catalonia.png");
            this.imgFlags.Images.SetKeyName(37, "cc.png");
            this.imgFlags.Images.SetKeyName(38, "cd.png");
            this.imgFlags.Images.SetKeyName(39, "cf.png");
            this.imgFlags.Images.SetKeyName(40, "cg.png");
            this.imgFlags.Images.SetKeyName(41, "ch.png");
            this.imgFlags.Images.SetKeyName(42, "ci.png");
            this.imgFlags.Images.SetKeyName(43, "ck.png");
            this.imgFlags.Images.SetKeyName(44, "cl.png");
            this.imgFlags.Images.SetKeyName(45, "cm.png");
            this.imgFlags.Images.SetKeyName(46, "cn.png");
            this.imgFlags.Images.SetKeyName(47, "co.png");
            this.imgFlags.Images.SetKeyName(48, "cr.png");
            this.imgFlags.Images.SetKeyName(49, "cs.png");
            this.imgFlags.Images.SetKeyName(50, "cu.png");
            this.imgFlags.Images.SetKeyName(51, "cv.png");
            this.imgFlags.Images.SetKeyName(52, "cx.png");
            this.imgFlags.Images.SetKeyName(53, "cy.png");
            this.imgFlags.Images.SetKeyName(54, "cz.png");
            this.imgFlags.Images.SetKeyName(55, "de.png");
            this.imgFlags.Images.SetKeyName(56, "dj.png");
            this.imgFlags.Images.SetKeyName(57, "dk.png");
            this.imgFlags.Images.SetKeyName(58, "dm.png");
            this.imgFlags.Images.SetKeyName(59, "do.png");
            this.imgFlags.Images.SetKeyName(60, "dz.png");
            this.imgFlags.Images.SetKeyName(61, "ec.png");
            this.imgFlags.Images.SetKeyName(62, "ee.png");
            this.imgFlags.Images.SetKeyName(63, "eg.png");
            this.imgFlags.Images.SetKeyName(64, "eh.png");
            this.imgFlags.Images.SetKeyName(65, "england.png");
            this.imgFlags.Images.SetKeyName(66, "er.png");
            this.imgFlags.Images.SetKeyName(67, "es.png");
            this.imgFlags.Images.SetKeyName(68, "et.png");
            this.imgFlags.Images.SetKeyName(69, "europeanunion.png");
            this.imgFlags.Images.SetKeyName(70, "fam.png");
            this.imgFlags.Images.SetKeyName(71, "fi.png");
            this.imgFlags.Images.SetKeyName(72, "fj.png");
            this.imgFlags.Images.SetKeyName(73, "fk.png");
            this.imgFlags.Images.SetKeyName(74, "fm.png");
            this.imgFlags.Images.SetKeyName(75, "fo.png");
            this.imgFlags.Images.SetKeyName(76, "fr.png");
            this.imgFlags.Images.SetKeyName(77, "ga.png");
            this.imgFlags.Images.SetKeyName(78, "gb.png");
            this.imgFlags.Images.SetKeyName(79, "gd.png");
            this.imgFlags.Images.SetKeyName(80, "ge.png");
            this.imgFlags.Images.SetKeyName(81, "gf.png");
            this.imgFlags.Images.SetKeyName(82, "gh.png");
            this.imgFlags.Images.SetKeyName(83, "gi.png");
            this.imgFlags.Images.SetKeyName(84, "gl.png");
            this.imgFlags.Images.SetKeyName(85, "gm.png");
            this.imgFlags.Images.SetKeyName(86, "gn.png");
            this.imgFlags.Images.SetKeyName(87, "gp.png");
            this.imgFlags.Images.SetKeyName(88, "gq.png");
            this.imgFlags.Images.SetKeyName(89, "gr.png");
            this.imgFlags.Images.SetKeyName(90, "gs.png");
            this.imgFlags.Images.SetKeyName(91, "gt.png");
            this.imgFlags.Images.SetKeyName(92, "gu.png");
            this.imgFlags.Images.SetKeyName(93, "gw.png");
            this.imgFlags.Images.SetKeyName(94, "gy.png");
            this.imgFlags.Images.SetKeyName(95, "hk.png");
            this.imgFlags.Images.SetKeyName(96, "hm.png");
            this.imgFlags.Images.SetKeyName(97, "hn.png");
            this.imgFlags.Images.SetKeyName(98, "hr.png");
            this.imgFlags.Images.SetKeyName(99, "ht.png");
            this.imgFlags.Images.SetKeyName(100, "hu.png");
            this.imgFlags.Images.SetKeyName(101, "id.png");
            this.imgFlags.Images.SetKeyName(102, "ie.png");
            this.imgFlags.Images.SetKeyName(103, "il.png");
            this.imgFlags.Images.SetKeyName(104, "in.png");
            this.imgFlags.Images.SetKeyName(105, "io.png");
            this.imgFlags.Images.SetKeyName(106, "iq.png");
            this.imgFlags.Images.SetKeyName(107, "ir.png");
            this.imgFlags.Images.SetKeyName(108, "is.png");
            this.imgFlags.Images.SetKeyName(109, "it.png");
            this.imgFlags.Images.SetKeyName(110, "jm.png");
            this.imgFlags.Images.SetKeyName(111, "jo.png");
            this.imgFlags.Images.SetKeyName(112, "jp.png");
            this.imgFlags.Images.SetKeyName(113, "ke.png");
            this.imgFlags.Images.SetKeyName(114, "kg.png");
            this.imgFlags.Images.SetKeyName(115, "kh.png");
            this.imgFlags.Images.SetKeyName(116, "ki.png");
            this.imgFlags.Images.SetKeyName(117, "km.png");
            this.imgFlags.Images.SetKeyName(118, "kn.png");
            this.imgFlags.Images.SetKeyName(119, "kp.png");
            this.imgFlags.Images.SetKeyName(120, "kr.png");
            this.imgFlags.Images.SetKeyName(121, "kw.png");
            this.imgFlags.Images.SetKeyName(122, "ky.png");
            this.imgFlags.Images.SetKeyName(123, "kz.png");
            this.imgFlags.Images.SetKeyName(124, "la.png");
            this.imgFlags.Images.SetKeyName(125, "lb.png");
            this.imgFlags.Images.SetKeyName(126, "lc.png");
            this.imgFlags.Images.SetKeyName(127, "li.png");
            this.imgFlags.Images.SetKeyName(128, "lk.png");
            this.imgFlags.Images.SetKeyName(129, "lr.png");
            this.imgFlags.Images.SetKeyName(130, "ls.png");
            this.imgFlags.Images.SetKeyName(131, "lt.png");
            this.imgFlags.Images.SetKeyName(132, "lu.png");
            this.imgFlags.Images.SetKeyName(133, "lv.png");
            this.imgFlags.Images.SetKeyName(134, "ly.png");
            this.imgFlags.Images.SetKeyName(135, "ma.png");
            this.imgFlags.Images.SetKeyName(136, "mc.png");
            this.imgFlags.Images.SetKeyName(137, "md.png");
            this.imgFlags.Images.SetKeyName(138, "me.png");
            this.imgFlags.Images.SetKeyName(139, "mg.png");
            this.imgFlags.Images.SetKeyName(140, "mh.png");
            this.imgFlags.Images.SetKeyName(141, "mk.png");
            this.imgFlags.Images.SetKeyName(142, "ml.png");
            this.imgFlags.Images.SetKeyName(143, "mm.png");
            this.imgFlags.Images.SetKeyName(144, "mn.png");
            this.imgFlags.Images.SetKeyName(145, "mo.png");
            this.imgFlags.Images.SetKeyName(146, "mp.png");
            this.imgFlags.Images.SetKeyName(147, "mq.png");
            this.imgFlags.Images.SetKeyName(148, "mr.png");
            this.imgFlags.Images.SetKeyName(149, "ms.png");
            this.imgFlags.Images.SetKeyName(150, "mt.png");
            this.imgFlags.Images.SetKeyName(151, "mu.png");
            this.imgFlags.Images.SetKeyName(152, "mv.png");
            this.imgFlags.Images.SetKeyName(153, "mw.png");
            this.imgFlags.Images.SetKeyName(154, "mx.png");
            this.imgFlags.Images.SetKeyName(155, "my.png");
            this.imgFlags.Images.SetKeyName(156, "mz.png");
            this.imgFlags.Images.SetKeyName(157, "na.png");
            this.imgFlags.Images.SetKeyName(158, "nc.png");
            this.imgFlags.Images.SetKeyName(159, "ne.png");
            this.imgFlags.Images.SetKeyName(160, "nf.png");
            this.imgFlags.Images.SetKeyName(161, "ng.png");
            this.imgFlags.Images.SetKeyName(162, "ni.png");
            this.imgFlags.Images.SetKeyName(163, "nl.png");
            this.imgFlags.Images.SetKeyName(164, "no.png");
            this.imgFlags.Images.SetKeyName(165, "np.png");
            this.imgFlags.Images.SetKeyName(166, "nr.png");
            this.imgFlags.Images.SetKeyName(167, "nu.png");
            this.imgFlags.Images.SetKeyName(168, "nz.png");
            this.imgFlags.Images.SetKeyName(169, "om.png");
            this.imgFlags.Images.SetKeyName(170, "pa.png");
            this.imgFlags.Images.SetKeyName(171, "pe.png");
            this.imgFlags.Images.SetKeyName(172, "pf.png");
            this.imgFlags.Images.SetKeyName(173, "pg.png");
            this.imgFlags.Images.SetKeyName(174, "ph.png");
            this.imgFlags.Images.SetKeyName(175, "pk.png");
            this.imgFlags.Images.SetKeyName(176, "pl.png");
            this.imgFlags.Images.SetKeyName(177, "pm.png");
            this.imgFlags.Images.SetKeyName(178, "pn.png");
            this.imgFlags.Images.SetKeyName(179, "pr.png");
            this.imgFlags.Images.SetKeyName(180, "ps.png");
            this.imgFlags.Images.SetKeyName(181, "pt.png");
            this.imgFlags.Images.SetKeyName(182, "pw.png");
            this.imgFlags.Images.SetKeyName(183, "py.png");
            this.imgFlags.Images.SetKeyName(184, "qa.png");
            this.imgFlags.Images.SetKeyName(185, "re.png");
            this.imgFlags.Images.SetKeyName(186, "ro.png");
            this.imgFlags.Images.SetKeyName(187, "rs.png");
            this.imgFlags.Images.SetKeyName(188, "ru.png");
            this.imgFlags.Images.SetKeyName(189, "rw.png");
            this.imgFlags.Images.SetKeyName(190, "sa.png");
            this.imgFlags.Images.SetKeyName(191, "sb.png");
            this.imgFlags.Images.SetKeyName(192, "sc.png");
            this.imgFlags.Images.SetKeyName(193, "scotland.png");
            this.imgFlags.Images.SetKeyName(194, "sd.png");
            this.imgFlags.Images.SetKeyName(195, "se.png");
            this.imgFlags.Images.SetKeyName(196, "sg.png");
            this.imgFlags.Images.SetKeyName(197, "sh.png");
            this.imgFlags.Images.SetKeyName(198, "si.png");
            this.imgFlags.Images.SetKeyName(199, "sj.png");
            this.imgFlags.Images.SetKeyName(200, "sk.png");
            this.imgFlags.Images.SetKeyName(201, "sl.png");
            this.imgFlags.Images.SetKeyName(202, "sm.png");
            this.imgFlags.Images.SetKeyName(203, "sn.png");
            this.imgFlags.Images.SetKeyName(204, "so.png");
            this.imgFlags.Images.SetKeyName(205, "sr.png");
            this.imgFlags.Images.SetKeyName(206, "st.png");
            this.imgFlags.Images.SetKeyName(207, "sv.png");
            this.imgFlags.Images.SetKeyName(208, "sy.png");
            this.imgFlags.Images.SetKeyName(209, "sz.png");
            this.imgFlags.Images.SetKeyName(210, "tc.png");
            this.imgFlags.Images.SetKeyName(211, "td.png");
            this.imgFlags.Images.SetKeyName(212, "tf.png");
            this.imgFlags.Images.SetKeyName(213, "tg.png");
            this.imgFlags.Images.SetKeyName(214, "th.png");
            this.imgFlags.Images.SetKeyName(215, "tj.png");
            this.imgFlags.Images.SetKeyName(216, "tk.png");
            this.imgFlags.Images.SetKeyName(217, "tl.png");
            this.imgFlags.Images.SetKeyName(218, "tm.png");
            this.imgFlags.Images.SetKeyName(219, "tn.png");
            this.imgFlags.Images.SetKeyName(220, "to.png");
            this.imgFlags.Images.SetKeyName(221, "tr.png");
            this.imgFlags.Images.SetKeyName(222, "tt.png");
            this.imgFlags.Images.SetKeyName(223, "tv.png");
            this.imgFlags.Images.SetKeyName(224, "tw.png");
            this.imgFlags.Images.SetKeyName(225, "tz.png");
            this.imgFlags.Images.SetKeyName(226, "ua.png");
            this.imgFlags.Images.SetKeyName(227, "ug.png");
            this.imgFlags.Images.SetKeyName(228, "um.png");
            this.imgFlags.Images.SetKeyName(229, "us.png");
            this.imgFlags.Images.SetKeyName(230, "uy.png");
            this.imgFlags.Images.SetKeyName(231, "uz.png");
            this.imgFlags.Images.SetKeyName(232, "va.png");
            this.imgFlags.Images.SetKeyName(233, "vc.png");
            this.imgFlags.Images.SetKeyName(234, "ve.png");
            this.imgFlags.Images.SetKeyName(235, "vg.png");
            this.imgFlags.Images.SetKeyName(236, "vi.png");
            this.imgFlags.Images.SetKeyName(237, "vn.png");
            this.imgFlags.Images.SetKeyName(238, "vu.png");
            this.imgFlags.Images.SetKeyName(239, "wales.png");
            this.imgFlags.Images.SetKeyName(240, "wf.png");
            this.imgFlags.Images.SetKeyName(241, "ws.png");
            this.imgFlags.Images.SetKeyName(242, "ye.png");
            this.imgFlags.Images.SetKeyName(243, "yt.png");
            this.imgFlags.Images.SetKeyName(244, "za.png");
            this.imgFlags.Images.SetKeyName(245, "zm.png");
            this.imgFlags.Images.SetKeyName(246, "zw.png");
            this.imgFlags.Images.SetKeyName(247, "xy.png");
            // 
            // nIcon
            // 
            this.nIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("nIcon.Icon")));
            this.nIcon.Text = "Quasar";
            this.nIcon.Visible = true;
            this.nIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.nIcon_MouseDoubleClick);
            // 
            // lstClients
            // 
            this.lstClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hIP,
            this.hTag,
            this.hUserPC,
            this.hVersion,
            this.hStatus,
            this.hUserStatus,
            this.hCountry,
            this.hOS,
            this.hAccountType});
            this.lstClients.ContextMenuStrip = this.ctxtMenu;
            this.lstClients.FullRowSelect = true;
            this.lstClients.Location = new System.Drawing.Point(0, 0);
            this.lstClients.Name = "lstClients";
            this.lstClients.ShowItemToolTips = true;
            this.lstClients.Size = new System.Drawing.Size(1006, 380);
            this.lstClients.SmallImageList = this.imgFlags;
            this.lstClients.TabIndex = 1;
            this.lstClients.UseCompatibleStateImageBehavior = false;
            this.lstClients.View = System.Windows.Forms.View.Details;
            this.lstClients.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstClients_ColumnClick);
            this.lstClients.SelectedIndexChanged += new System.EventHandler(this.lstClients_SelectedIndexChanged);
            // 
            // hIP
            // 
            this.hIP.Text = "IP Address";
            this.hIP.Width = 112;
            // 
            // hTag
            // 
            this.hTag.Text = "Tag";
            // 
            // hUserPC
            // 
            this.hUserPC.Text = "User@PC";
            this.hUserPC.Width = 175;
            // 
            // hVersion
            // 
            this.hVersion.Text = "Version";
            this.hVersion.Width = 66;
            // 
            // hStatus
            // 
            this.hStatus.Text = "Status";
            this.hStatus.Width = 78;
            // 
            // hUserStatus
            // 
            this.hUserStatus.Text = "User Status";
            this.hUserStatus.Width = 72;
            // 
            // hCountry
            // 
            this.hCountry.Text = "Country";
            this.hCountry.Width = 117;
            // 
            // hOS
            // 
            this.hOS.Text = "Operating System";
            this.hOS.Width = 222;
            // 
            // hAccountType
            // 
            this.hAccountType.Text = "Account Type";
            this.hAccountType.Width = 100;
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFile,
            this.menuSettings,
            this.menuBuilder,
            this.menuStatistics,
            this.menuAbout});
            // 
            // menuFile
            // 
            this.menuFile.Index = 0;
            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuClose});
            this.menuFile.Text = "File";
            // 
            // menuClose
            // 
            this.menuClose.Index = 0;
            this.menuClose.Text = "Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // menuSettings
            // 
            this.menuSettings.Index = 1;
            this.menuSettings.Text = "Settings";
            this.menuSettings.Click += new System.EventHandler(this.menuSettings_Click);
            // 
            // menuBuilder
            // 
            this.menuBuilder.Index = 2;
            this.menuBuilder.Text = "Builder";
            this.menuBuilder.Click += new System.EventHandler(this.menuBuilder_Click);
            // 
            // menuStatistics
            // 
            this.menuStatistics.Index = 3;
            this.menuStatistics.Text = "Statistics";
            this.menuStatistics.Click += new System.EventHandler(this.menuStatistics_Click);
            // 
            // menuAbout
            // 
            this.menuAbout.Index = 4;
            this.menuAbout.Text = "About";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 404);
            this.Controls.Add(this.botStrip);
            this.Controls.Add(this.lstClients);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(680, 415);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quasar - Connected: 0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ctxtMenu.ResumeLayout(false);
            this.botStrip.ResumeLayout(false);
            this.botStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader hIP;
        private System.Windows.Forms.ColumnHeader hVersion;
        private System.Windows.Forms.ColumnHeader hCountry;
        private System.Windows.Forms.ColumnHeader hOS;
        private System.Windows.Forms.ContextMenuStrip ctxtMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxtConnection;
        private System.Windows.Forms.ToolStripMenuItem ctxtReconnect;
        private System.Windows.Forms.ToolStripMenuItem ctxtDisconnect;
        private System.Windows.Forms.ColumnHeader hTag;
        private System.Windows.Forms.StatusStrip botStrip;
        private System.Windows.Forms.ToolStripStatusLabel botListen;
        private System.Windows.Forms.ImageList imgFlags;
        private System.Windows.Forms.ToolStripMenuItem ctxtSystem;
        private System.Windows.Forms.ColumnHeader hStatus;
        private System.Windows.Forms.ToolStripMenuItem ctxtUninstall;
        private System.Windows.Forms.ToolStripMenuItem ctxtSurveillance;
        private System.Windows.Forms.ToolStripMenuItem ctxtRemoteDesktop;
        private System.Windows.Forms.ToolStripMenuItem ctxtTaskManager;
        private System.Windows.Forms.ToolStripMenuItem ctxtFileManager;
        private System.Windows.Forms.ColumnHeader hAccountType;
        private System.Windows.Forms.ToolStripMenuItem ctxtSystemInformation;
        private System.Windows.Forms.ColumnHeader hUserStatus;
        private System.Windows.Forms.ToolStripMenuItem ctxtMiscellaneous;
        private System.Windows.Forms.ToolStripMenuItem ctxtVisitWebsite;
        private System.Windows.Forms.ToolStripMenuItem ctxtPasswordRecovery;
        private System.Windows.Forms.ToolStripMenuItem ctxtShowMessagebox;
        private System.Windows.Forms.ToolStripMenuItem ctxtUpdate;
        private Controls.MainMenuEx mainMenu;
        private System.Windows.Forms.MenuItem menuFile;
        private System.Windows.Forms.MenuItem menuClose;
        private System.Windows.Forms.MenuItem menuSettings;
        private System.Windows.Forms.MenuItem menuBuilder;
        private System.Windows.Forms.MenuItem menuStatistics;
        private System.Windows.Forms.MenuItem menuAbout;
        private System.Windows.Forms.ToolStripMenuItem ctxtRemoteShell;
        private System.Windows.Forms.ToolStripSeparator ctxtLine;
        private System.Windows.Forms.ToolStripMenuItem ctxtActions;
        private System.Windows.Forms.ToolStripMenuItem ctxtShutdown;
        private System.Windows.Forms.ToolStripMenuItem ctxtRestart;
        private System.Windows.Forms.ToolStripMenuItem ctxtStandby;
        private System.Windows.Forms.ToolStripMenuItem ctxtStartupManager;
        private System.Windows.Forms.ToolStripMenuItem ctxtRemoteExecute;
        private System.Windows.Forms.ToolStripMenuItem ctxtLocalFile;
        private System.Windows.Forms.ToolStripMenuItem ctxtWebFile;
        private System.Windows.Forms.ToolStripMenuItem ctxtKeylogger;
        private System.Windows.Forms.ToolStripMenuItem ctxtReverseProxy;
        private System.Windows.Forms.ToolStripMenuItem ctxtRegistryEditor;
        private AeroListView lstClients;
        private System.Windows.Forms.NotifyIcon nIcon;
        private System.Windows.Forms.ColumnHeader hUserPC;
    }
}

