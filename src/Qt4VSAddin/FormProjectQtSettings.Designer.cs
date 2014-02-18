namespace Qt4VSAddin
{
    partial class FormProjectQtSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProjectQtSettings));
            this.OptionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.coreLib = new System.Windows.Forms.CheckBox();
            this.guiLib = new System.Windows.Forms.CheckBox();
            this.xmlLib = new System.Windows.Forms.CheckBox();
            this.networkLib = new System.Windows.Forms.CheckBox();
            this.openGLLib = new System.Windows.Forms.CheckBox();
            this.scriptLib = new System.Windows.Forms.CheckBox();
            this.xmlPatternsLib = new System.Windows.Forms.CheckBox();
            this.webKitLib = new System.Windows.Forms.CheckBox();
            this.scriptToolsLib = new System.Windows.Forms.CheckBox();
            this.svgLib = new System.Windows.Forms.CheckBox();
            this.qt3Lib = new System.Windows.Forms.CheckBox();
            this.activeQtSLib = new System.Windows.Forms.CheckBox();
            this.helpLib = new System.Windows.Forms.CheckBox();
            this.phononLib = new System.Windows.Forms.CheckBox();
            this.activeQtCLib = new System.Windows.Forms.CheckBox();
            this.declarativeLib = new System.Windows.Forms.CheckBox();
            this.multimediaLib = new System.Windows.Forms.CheckBox();
            this.sqlLib = new System.Windows.Forms.CheckBox();
            this.testLib = new System.Windows.Forms.CheckBox();
            this.uiToolsLib = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OptionsPropertyGrid
            // 
            this.OptionsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionsPropertyGrid.HelpVisible = false;
            this.OptionsPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.OptionsPropertyGrid.Name = "OptionsPropertyGrid";
            this.OptionsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.OptionsPropertyGrid.Size = new System.Drawing.Size(498, 369);
            this.OptionsPropertyGrid.TabIndex = 8;
            this.OptionsPropertyGrid.ToolbarVisible = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(348, 410);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 24);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Ok";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(429, 410);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 24);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            // 
            // tabControl1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabControl1, 2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(512, 401);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.OptionsPropertyGrid);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(504, 375);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General Settings";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.flowLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(504, 375);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Add/Remove Qt Modules";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.coreLib);
            this.flowLayoutPanel1.Controls.Add(this.guiLib);
            this.flowLayoutPanel1.Controls.Add(this.xmlLib);
            this.flowLayoutPanel1.Controls.Add(this.networkLib);
            this.flowLayoutPanel1.Controls.Add(this.openGLLib);
            this.flowLayoutPanel1.Controls.Add(this.scriptLib);
            this.flowLayoutPanel1.Controls.Add(this.xmlPatternsLib);
            this.flowLayoutPanel1.Controls.Add(this.webKitLib);
            this.flowLayoutPanel1.Controls.Add(this.scriptToolsLib);
            this.flowLayoutPanel1.Controls.Add(this.svgLib);
            this.flowLayoutPanel1.Controls.Add(this.qt3Lib);
            this.flowLayoutPanel1.Controls.Add(this.activeQtSLib);
            this.flowLayoutPanel1.Controls.Add(this.helpLib);
            this.flowLayoutPanel1.Controls.Add(this.phononLib);
            this.flowLayoutPanel1.Controls.Add(this.activeQtCLib);
            this.flowLayoutPanel1.Controls.Add(this.declarativeLib);
            this.flowLayoutPanel1.Controls.Add(this.multimediaLib);
            this.flowLayoutPanel1.Controls.Add(this.sqlLib);
            this.flowLayoutPanel1.Controls.Add(this.testLib);
            this.flowLayoutPanel1.Controls.Add(this.uiToolsLib);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(498, 369);
            this.flowLayoutPanel1.TabIndex = 13;
            // 
            // coreLib
            // 
            this.coreLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.coreLib.Location = new System.Drawing.Point(3, 3);
            this.coreLib.Name = "coreLib";
            this.coreLib.Size = new System.Drawing.Size(160, 22);
            this.coreLib.TabIndex = 0;
            this.coreLib.Text = "Core";
            // 
            // guiLib
            // 
            this.guiLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guiLib.Location = new System.Drawing.Point(169, 3);
            this.guiLib.Name = "guiLib";
            this.guiLib.Size = new System.Drawing.Size(160, 22);
            this.guiLib.TabIndex = 1;
            this.guiLib.Text = "Gui";
            // 
            // xmlLib
            // 
            this.xmlLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlLib.Location = new System.Drawing.Point(335, 3);
            this.xmlLib.Name = "xmlLib";
            this.xmlLib.Size = new System.Drawing.Size(160, 22);
            this.xmlLib.TabIndex = 0;
            this.xmlLib.Text = "XML";
            // 
            // networkLib
            // 
            this.networkLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.networkLib.Location = new System.Drawing.Point(3, 31);
            this.networkLib.Name = "networkLib";
            this.networkLib.Size = new System.Drawing.Size(160, 22);
            this.networkLib.TabIndex = 5;
            this.networkLib.Text = "Network";
            // 
            // openGLLib
            // 
            this.openGLLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openGLLib.Location = new System.Drawing.Point(169, 31);
            this.openGLLib.Name = "openGLLib";
            this.openGLLib.Size = new System.Drawing.Size(160, 22);
            this.openGLLib.TabIndex = 4;
            this.openGLLib.Text = "OpenGL";
            // 
            // scriptLib
            // 
            this.scriptLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scriptLib.Location = new System.Drawing.Point(335, 31);
            this.scriptLib.Name = "scriptLib";
            this.scriptLib.Size = new System.Drawing.Size(160, 22);
            this.scriptLib.TabIndex = 6;
            this.scriptLib.Text = "Script";
            // 
            // xmlPatternsLib
            // 
            this.xmlPatternsLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlPatternsLib.Location = new System.Drawing.Point(3, 59);
            this.xmlPatternsLib.Name = "xmlPatternsLib";
            this.xmlPatternsLib.Size = new System.Drawing.Size(160, 22);
            this.xmlPatternsLib.TabIndex = 7;
            this.xmlPatternsLib.Text = "XML Pattern";
            // 
            // webKitLib
            // 
            this.webKitLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webKitLib.Location = new System.Drawing.Point(169, 59);
            this.webKitLib.Name = "webKitLib";
            this.webKitLib.Size = new System.Drawing.Size(160, 22);
            this.webKitLib.TabIndex = 6;
            this.webKitLib.Text = "WebKit";
            // 
            // scriptToolsLib
            // 
            this.scriptToolsLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scriptToolsLib.Location = new System.Drawing.Point(335, 59);
            this.scriptToolsLib.Name = "scriptToolsLib";
            this.scriptToolsLib.Size = new System.Drawing.Size(160, 22);
            this.scriptToolsLib.TabIndex = 10;
            this.scriptToolsLib.Text = "ScriptTools";
            // 
            // svgLib
            // 
            this.svgLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.svgLib.Location = new System.Drawing.Point(3, 87);
            this.svgLib.Name = "svgLib";
            this.svgLib.Size = new System.Drawing.Size(160, 22);
            this.svgLib.TabIndex = 1;
            this.svgLib.Text = "SVG";
            // 
            // qt3Lib
            // 
            this.qt3Lib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qt3Lib.Location = new System.Drawing.Point(169, 87);
            this.qt3Lib.Name = "qt3Lib";
            this.qt3Lib.Size = new System.Drawing.Size(160, 22);
            this.qt3Lib.TabIndex = 2;
            this.qt3Lib.Text = "Qt3 Support";
            // 
            // activeQtSLib
            // 
            this.activeQtSLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activeQtSLib.Location = new System.Drawing.Point(335, 87);
            this.activeQtSLib.Name = "activeQtSLib";
            this.activeQtSLib.Size = new System.Drawing.Size(160, 22);
            this.activeQtSLib.TabIndex = 3;
            this.activeQtSLib.Text = "ActiveQt Server";
            // 
            // helpLib
            // 
            this.helpLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpLib.Location = new System.Drawing.Point(3, 115);
            this.helpLib.Name = "helpLib";
            this.helpLib.Size = new System.Drawing.Size(160, 22);
            this.helpLib.TabIndex = 5;
            this.helpLib.Text = "Help";
            // 
            // phononLib
            // 
            this.phononLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.phononLib.Location = new System.Drawing.Point(169, 115);
            this.phononLib.Name = "phononLib";
            this.phononLib.Size = new System.Drawing.Size(160, 22);
            this.phononLib.TabIndex = 7;
            this.phononLib.Text = "Phonon";
            // 
            // activeQtCLib
            // 
            this.activeQtCLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activeQtCLib.Location = new System.Drawing.Point(335, 115);
            this.activeQtCLib.Name = "activeQtCLib";
            this.activeQtCLib.Size = new System.Drawing.Size(160, 22);
            this.activeQtCLib.TabIndex = 4;
            this.activeQtCLib.Text = "ActiveQt Container";
            // 
            // declarativeLib
            // 
            this.declarativeLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.declarativeLib.Location = new System.Drawing.Point(3, 143);
            this.declarativeLib.Name = "declarativeLib";
            this.declarativeLib.Size = new System.Drawing.Size(160, 22);
            this.declarativeLib.TabIndex = 8;
            this.declarativeLib.Text = "Declarative";
            // 
            // multimediaLib
            // 
            this.multimediaLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.multimediaLib.Location = new System.Drawing.Point(169, 143);
            this.multimediaLib.Name = "multimediaLib";
            this.multimediaLib.Size = new System.Drawing.Size(160, 22);
            this.multimediaLib.TabIndex = 2;
            this.multimediaLib.Text = "Multimedia";
            // 
            // sqlLib
            // 
            this.sqlLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlLib.Location = new System.Drawing.Point(335, 143);
            this.sqlLib.Name = "sqlLib";
            this.sqlLib.Size = new System.Drawing.Size(160, 22);
            this.sqlLib.TabIndex = 3;
            this.sqlLib.Text = "SQL";
            // 
            // testLib
            // 
            this.testLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testLib.Location = new System.Drawing.Point(3, 171);
            this.testLib.Name = "testLib";
            this.testLib.Size = new System.Drawing.Size(160, 22);
            this.testLib.TabIndex = 8;
            this.testLib.Text = "Test";
            // 
            // uiToolsLib
            // 
            this.uiToolsLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiToolsLib.Location = new System.Drawing.Point(169, 171);
            this.uiToolsLib.Name = "uiToolsLib";
            this.uiToolsLib.Size = new System.Drawing.Size(160, 22);
            this.uiToolsLib.TabIndex = 9;
            this.uiToolsLib.Text = "UiTools";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.27468F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.725322F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(518, 442);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // FormProjectQtSettings
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(518, 442);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormProjectQtSettings";
            this.ShowInTaskbar = false;
            this.Text = "FormAddinSettings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid OptionsPropertyGrid;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox helpLib;
        private System.Windows.Forms.CheckBox webKitLib;
        private System.Windows.Forms.CheckBox xmlLib;
        private System.Windows.Forms.CheckBox activeQtCLib;
        private System.Windows.Forms.CheckBox activeQtSLib;
        private System.Windows.Forms.CheckBox qt3Lib;
        private System.Windows.Forms.CheckBox svgLib;
        private System.Windows.Forms.CheckBox networkLib;
        private System.Windows.Forms.CheckBox coreLib;
        private System.Windows.Forms.CheckBox guiLib;
        private System.Windows.Forms.CheckBox sqlLib;
        private System.Windows.Forms.CheckBox openGLLib;
        private System.Windows.Forms.CheckBox testLib;
        private System.Windows.Forms.CheckBox scriptLib;
        private System.Windows.Forms.CheckBox xmlPatternsLib;
        private System.Windows.Forms.CheckBox phononLib;
        private System.Windows.Forms.CheckBox multimediaLib;
        private System.Windows.Forms.CheckBox declarativeLib;
        private System.Windows.Forms.CheckBox scriptToolsLib;
        private System.Windows.Forms.CheckBox uiToolsLib;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;

    }
}