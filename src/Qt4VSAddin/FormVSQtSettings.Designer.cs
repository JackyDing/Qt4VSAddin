namespace Qt4VSAddin
{
    partial class FormVSQtSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVSQtSettings));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.addButton = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.winCECombo = new System.Windows.Forms.ComboBox();
            this.defaultX64Combo = new System.Windows.Forms.ComboBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.defaultX86Combo = new System.Windows.Forms.ComboBox();
            this.labelWinCE = new System.Windows.Forms.Label();
            this.labelX86 = new System.Windows.Forms.Label();
            this.labelX64 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.optionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(431, 439);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(74, 24);
            this.okButton.TabIndex = 18;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(511, 439);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(74, 24);
            this.cancelButton.TabIndex = 19;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // tabControl1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabControl1, 3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(602, 430);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.tableLayoutPanel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(594, 404);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.Controls.Add(this.addButton, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.listView, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.winCECombo, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.defaultX64Combo, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.deleteButton, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.defaultX86Combo, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelWinCE, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.labelX86, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.labelX64, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(588, 398);
            this.tableLayoutPanel2.TabIndex = 27;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(511, 3);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(74, 22);
            this.addButton.TabIndex = 19;
            this.addButton.Text = "&Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // listView
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.listView, 2);
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(3, 3);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.tableLayoutPanel2.SetRowSpan(this.listView, 2);
            this.listView.Size = new System.Drawing.Size(502, 308);
            this.listView.TabIndex = 18;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // winCECombo
            // 
            this.winCECombo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winCECombo.FormattingEnabled = true;
            this.winCECombo.Location = new System.Drawing.Point(153, 373);
            this.winCECombo.Name = "winCECombo";
            this.winCECombo.Size = new System.Drawing.Size(352, 20);
            this.winCECombo.TabIndex = 26;
            // 
            // defaultX64Combo
            // 
            this.defaultX64Combo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultX64Combo.FormattingEnabled = true;
            this.defaultX64Combo.Location = new System.Drawing.Point(153, 345);
            this.defaultX64Combo.Name = "defaultX64Combo";
            this.defaultX64Combo.Size = new System.Drawing.Size(352, 20);
            this.defaultX64Combo.TabIndex = 24;
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(511, 31);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(74, 22);
            this.deleteButton.TabIndex = 20;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // defaultX86Combo
            // 
            this.defaultX86Combo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultX86Combo.FormattingEnabled = true;
            this.defaultX86Combo.Location = new System.Drawing.Point(153, 317);
            this.defaultX86Combo.Name = "defaultX86Combo";
            this.defaultX86Combo.Size = new System.Drawing.Size(352, 20);
            this.defaultX86Combo.TabIndex = 22;
            // 
            // labelWinCE
            // 
            this.labelWinCE.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelWinCE.AutoSize = true;
            this.labelWinCE.Location = new System.Drawing.Point(3, 378);
            this.labelWinCE.Name = "labelWinCE";
            this.labelWinCE.Size = new System.Drawing.Size(131, 12);
            this.labelWinCE.TabIndex = 25;
            this.labelWinCE.Text = "Default Qt/WinCE version:";
            // 
            // labelX86
            // 
            this.labelX86.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelX86.AutoSize = true;
            this.labelX86.Location = new System.Drawing.Point(3, 322);
            this.labelX86.Name = "labelX86";
            this.labelX86.Size = new System.Drawing.Size(137, 12);
            this.labelX86.TabIndex = 21;
            this.labelX86.Text = "Default Qt/Win x86 version:";
            // 
            // labelX64
            // 
            this.labelX64.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelX64.AutoSize = true;
            this.labelX64.Location = new System.Drawing.Point(3, 350);
            this.labelX64.Name = "labelX64";
            this.labelX64.Size = new System.Drawing.Size(137, 12);
            this.labelX64.TabIndex = 23;
            this.labelX64.Text = "Default Qt/Win x64 version:";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.optionsPropertyGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(594, 404);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            // 
            // optionsPropertyGrid
            // 
            this.optionsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsPropertyGrid.HelpVisible = false;
            this.optionsPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.optionsPropertyGrid.Name = "optionsPropertyGrid";
            this.optionsPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.optionsPropertyGrid.Size = new System.Drawing.Size(588, 398);
            this.optionsPropertyGrid.TabIndex = 0;
            this.optionsPropertyGrid.ToolbarVisible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.90188F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.098121F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(608, 470);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // FormVSQtSettings
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(608, 470);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 420);
            this.Name = "FormVSQtSettings";
            this.ShowInTaskbar = false;
            this.Text = "FormQtVersions";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox winCECombo;
        private System.Windows.Forms.Label labelWinCE;
        private System.Windows.Forms.ComboBox defaultX86Combo;
        private System.Windows.Forms.Label labelX86;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PropertyGrid optionsPropertyGrid;
        private System.Windows.Forms.ComboBox defaultX64Combo;
        private System.Windows.Forms.Label labelX64;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}