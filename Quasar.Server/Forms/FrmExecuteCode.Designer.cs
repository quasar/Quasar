namespace Quasar.Server.Forms {
    partial class FrmExecuteCode {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmExecuteCode));
            this.btnExecuteCode = new System.Windows.Forms.Button();
            this.btnResetCode = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbReferences = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRemoveReference = new System.Windows.Forms.Button();
            this.btnAddReference = new System.Windows.Forms.Button();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cbHidden = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ScriptList = new System.Windows.Forms.ListBox();
            this.script_name = new System.Windows.Forms.TextBox();
            this.tcLanguage = new Quasar.Server.Controls.DotNetBarTabControl();
            this.tabCSharp = new System.Windows.Forms.TabPage();
            this.fctbCSharp = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tabVB = new System.Windows.Forms.TabPage();
            this.fctbVB = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tcLanguage.SuspendLayout();
            this.tabCSharp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fctbCSharp)).BeginInit();
            this.tabVB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fctbVB)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExecuteCode
            // 
            this.btnExecuteCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecuteCode.Image = global::Quasar.Server.Properties.Resources.application_go;
            this.btnExecuteCode.Location = new System.Drawing.Point(1220, 103);
            this.btnExecuteCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnExecuteCode.Name = "btnExecuteCode";
            this.btnExecuteCode.Size = new System.Drawing.Size(52, 46);
            this.btnExecuteCode.TabIndex = 1;
            this.btnExecuteCode.UseVisualStyleBackColor = true;
            this.btnExecuteCode.Click += new System.EventHandler(this.btnExecuteCode_Click);
            // 
            // btnResetCode
            // 
            this.btnResetCode.Image = global::Quasar.Server.Properties.Resources.refresh;
            this.btnResetCode.Location = new System.Drawing.Point(1220, 48);
            this.btnResetCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnResetCode.Name = "btnResetCode";
            this.btnResetCode.Size = new System.Drawing.Size(52, 46);
            this.btnResetCode.TabIndex = 2;
            this.btnResetCode.UseVisualStyleBackColor = true;
            this.btnResetCode.Click += new System.EventHandler(this.btnResetCode_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1281, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Reset Code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1281, 117);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Execute Code";
            // 
            // lbReferences
            // 
            this.lbReferences.FormattingEnabled = true;
            this.lbReferences.ItemHeight = 20;
            this.lbReferences.Items.AddRange(new object[] {
            "System.dll",
            "System.Core.dll"});
            this.lbReferences.Location = new System.Drawing.Point(1220, 240);
            this.lbReferences.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lbReferences.Name = "lbReferences";
            this.lbReferences.Size = new System.Drawing.Size(284, 224);
            this.lbReferences.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1215, 212);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Reference List:";
            // 
            // btnRemoveReference
            // 
            this.btnRemoveReference.Location = new System.Drawing.Point(1220, 515);
            this.btnRemoveReference.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRemoveReference.Name = "btnRemoveReference";
            this.btnRemoveReference.Size = new System.Drawing.Size(141, 35);
            this.btnRemoveReference.TabIndex = 11;
            this.btnRemoveReference.Text = "Remove";
            this.btnRemoveReference.UseVisualStyleBackColor = true;
            this.btnRemoveReference.Click += new System.EventHandler(this.btnRemoveReference_Click);
            // 
            // btnAddReference
            // 
            this.btnAddReference.Location = new System.Drawing.Point(1365, 515);
            this.btnAddReference.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddReference.Name = "btnAddReference";
            this.btnAddReference.Size = new System.Drawing.Size(141, 35);
            this.btnAddReference.TabIndex = 12;
            this.btnAddReference.Text = "Add";
            this.btnAddReference.UseVisualStyleBackColor = true;
            this.btnAddReference.Click += new System.EventHandler(this.btnAddReference_Click);
            // 
            // txtReference
            // 
            this.txtReference.Location = new System.Drawing.Point(1220, 475);
            this.txtReference.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReference.Name = "txtReference";
            this.txtReference.Size = new System.Drawing.Size(284, 26);
            this.txtReference.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1219, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 20);
            this.label4.TabIndex = 14;
            this.label4.Text = "Status:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(1292, 9);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 20);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "Ready.";
            // 
            // cbHidden
            // 
            this.cbHidden.AutoSize = true;
            this.cbHidden.Location = new System.Drawing.Point(1223, 158);
            this.cbHidden.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbHidden.Name = "cbHidden";
            this.cbHidden.Size = new System.Drawing.Size(148, 24);
            this.cbHidden.TabIndex = 16;
            this.cbHidden.Text = "Execute Hidden";
            this.cbHidden.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1220, 842);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 35);
            this.button1.TabIndex = 17;
            this.button1.Text = "Save Script";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1365, 842);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 35);
            this.button2.TabIndex = 18;
            this.button2.Text = "Load Script";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ScriptList
            // 
            this.ScriptList.FormattingEnabled = true;
            this.ScriptList.ItemHeight = 20;
            this.ScriptList.Location = new System.Drawing.Point(1220, 570);
            this.ScriptList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ScriptList.Name = "ScriptList";
            this.ScriptList.Size = new System.Drawing.Size(284, 224);
            this.ScriptList.TabIndex = 19;
            // 
            // script_name
            // 
            this.script_name.Location = new System.Drawing.Point(1220, 806);
            this.script_name.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.script_name.Name = "script_name";
            this.script_name.Size = new System.Drawing.Size(284, 26);
            this.script_name.TabIndex = 20;
            // 
            // tcLanguage
            // 
            this.tcLanguage.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tcLanguage.Controls.Add(this.tabCSharp);
            this.tcLanguage.Controls.Add(this.tabVB);
            this.tcLanguage.Dock = System.Windows.Forms.DockStyle.Left;
            this.tcLanguage.ItemSize = new System.Drawing.Size(44, 136);
            this.tcLanguage.Location = new System.Drawing.Point(0, 0);
            this.tcLanguage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tcLanguage.Multiline = true;
            this.tcLanguage.Name = "tcLanguage";
            this.tcLanguage.SelectedIndex = 0;
            this.tcLanguage.Size = new System.Drawing.Size(1200, 882);
            this.tcLanguage.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tcLanguage.TabIndex = 0;
            // 
            // tabCSharp
            // 
            this.tabCSharp.Controls.Add(this.fctbCSharp);
            this.tabCSharp.Location = new System.Drawing.Point(140, 4);
            this.tabCSharp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabCSharp.Name = "tabCSharp";
            this.tabCSharp.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabCSharp.Size = new System.Drawing.Size(1056, 874);
            this.tabCSharp.TabIndex = 0;
            this.tabCSharp.Text = "C#";
            this.tabCSharp.UseVisualStyleBackColor = true;
            // 
            // fctbCSharp
            // 
            this.fctbCSharp.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fctbCSharp.AutoIndentCharsPatterns = "\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\n^\\s*(case|default)\\s*[^:]*(" +
    "?<range>:)\\s*(?<range>[^;]+);\n";
            this.fctbCSharp.AutoScrollMinSize = new System.Drawing.Size(35, 22);
            this.fctbCSharp.BackBrush = null;
            this.fctbCSharp.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2;
            this.fctbCSharp.CharHeight = 22;
            this.fctbCSharp.CharWidth = 12;
            this.fctbCSharp.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctbCSharp.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctbCSharp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctbCSharp.IsReplaceMode = false;
            this.fctbCSharp.Language = FastColoredTextBoxNS.Language.CSharp;
            this.fctbCSharp.LeftBracket = '(';
            this.fctbCSharp.LeftBracket2 = '{';
            this.fctbCSharp.Location = new System.Drawing.Point(4, 5);
            this.fctbCSharp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fctbCSharp.Name = "fctbCSharp";
            this.fctbCSharp.Paddings = new System.Windows.Forms.Padding(0);
            this.fctbCSharp.RightBracket = ')';
            this.fctbCSharp.RightBracket2 = '}';
            this.fctbCSharp.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctbCSharp.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fctbCSharp.ServiceColors")));
            this.fctbCSharp.Size = new System.Drawing.Size(1048, 864);
            this.fctbCSharp.TabIndex = 0;
            this.fctbCSharp.Zoom = 100;
            // 
            // tabVB
            // 
            this.tabVB.Controls.Add(this.fctbVB);
            this.tabVB.Location = new System.Drawing.Point(140, 4);
            this.tabVB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabVB.Name = "tabVB";
            this.tabVB.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabVB.Size = new System.Drawing.Size(1056, 874);
            this.tabVB.TabIndex = 1;
            this.tabVB.Text = "VB";
            this.tabVB.UseVisualStyleBackColor = true;
            // 
            // fctbVB
            // 
            this.fctbVB.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fctbVB.AutoIndentCharsPatterns = "\n^\\s*[\\w\\.\\(\\)]+\\s*(?<range>=)\\s*(?<range>.+)\n";
            this.fctbVB.AutoScrollMinSize = new System.Drawing.Size(2, 22);
            this.fctbVB.BackBrush = null;
            this.fctbVB.CharHeight = 22;
            this.fctbVB.CharWidth = 12;
            this.fctbVB.CommentPrefix = "\'";
            this.fctbVB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctbVB.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctbVB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctbVB.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.fctbVB.IsReplaceMode = false;
            this.fctbVB.Language = FastColoredTextBoxNS.Language.VB;
            this.fctbVB.LeftBracket = '(';
            this.fctbVB.Location = new System.Drawing.Point(4, 5);
            this.fctbVB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fctbVB.Name = "fctbVB";
            this.fctbVB.Paddings = new System.Windows.Forms.Padding(0);
            this.fctbVB.RightBracket = ')';
            this.fctbVB.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctbVB.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fctbVB.ServiceColors")));
            this.fctbVB.Size = new System.Drawing.Size(1048, 864);
            this.fctbVB.TabIndex = 0;
            this.fctbVB.Zoom = 100;
            // 
            // FrmExecuteCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1514, 882);
            this.Controls.Add(this.script_name);
            this.Controls.Add(this.ScriptList);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbHidden);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtReference);
            this.Controls.Add(this.btnAddReference);
            this.Controls.Add(this.btnRemoveReference);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbReferences);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnResetCode);
            this.Controls.Add(this.btnExecuteCode);
            this.Controls.Add(this.tcLanguage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmExecuteCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Code Executor";
            this.Load += new System.EventHandler(this.FrmExecuteCode_Load);
            this.tcLanguage.ResumeLayout(false);
            this.tabCSharp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fctbCSharp)).EndInit();
            this.tabVB.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fctbVB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.DotNetBarTabControl tcLanguage;
        private System.Windows.Forms.TabPage tabCSharp;
        private System.Windows.Forms.TabPage tabVB;
        private System.Windows.Forms.Button btnExecuteCode;
        private System.Windows.Forms.Button btnResetCode;
        private FastColoredTextBoxNS.FastColoredTextBox fctbCSharp;
        private FastColoredTextBoxNS.FastColoredTextBox fctbVB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbReferences;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRemoveReference;
        private System.Windows.Forms.Button btnAddReference;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox cbHidden;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox ScriptList;
        private System.Windows.Forms.TextBox script_name;
    }
}