﻿/****************************************************************************
**
** Copyright (C) 2012 Digia Plc and/or its subsidiary(-ies).
** Contact: http://www.qt-project.org/legal
**
** This file is part of the Qt VS Add-in.
**
** $QT_BEGIN_LICENSE:LGPL$
** Commercial License Usage
** Licensees holding valid commercial Qt licenses may use this file in
** accordance with the commercial license agreement provided with the
** Software or, alternatively, in accordance with the terms contained in
** a written agreement between you and Digia.  For licensing terms and
** conditions see http://qt.digia.com/licensing.  For further information
** use the contact form at http://qt.digia.com/contact-us.
**
** GNU Lesser General Public License Usage
** Alternatively, this file may be used under the terms of the GNU Lesser
** General Public License version 2.1 as published by the Free Software
** Foundation and appearing in the file LICENSE.LGPL included in the
** packaging of this file.  Please review the following information to
** ensure the GNU Lesser General Public License version 2.1 requirements
** will be met: http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html.
**
** In addition, as a special exception, Digia gives you certain additional
** rights.  These rights are described in the Digia Qt LGPL Exception
** version 1.1, included in the file LGPL_EXCEPTION.txt in this package.
**
** GNU General Public License Usage
** Alternatively, this file may be used under the terms of the GNU
** General Public License version 3.0 as published by the Free Software
** Foundation and appearing in the file LICENSE.GPL included in the
** packaging of this file.  Please review the following information to
** ensure the GNU General Public License version 3.0 requirements will be
** met: http://www.gnu.org/copyleft/gpl.html.
**
**
** $QT_END_LICENSE$
**
****************************************************************************/

using System;
using System.Windows.Forms;
using Digia.Qt4ProjectLib;

namespace Qt4VSAddin
{
    public partial class FormVSQtSettings : Form
    {
        private QtVersionManager versionManager;
        private VSQtSettings vsQtSettings;

        public FormVSQtSettings()
        {
            InitializeComponent();
            
            foreach (var control in Controls)
            {
                ComboBox comboBox = control as ComboBox;
                if (comboBox != null)
                {
                    comboBox.Resize += (sender, e) =>
                    {
                        if (!comboBox.Focused)
                            comboBox.SelectionLength = 0;
                    };
                }
            }

            versionManager = QtVersionManager.The();

            this.Text = SR.GetString("VSQtOptionsButtonText");
            listView.Columns.Add(SR.GetString("BuildOptionsPage_Name"), 100, HorizontalAlignment.Left);
            listView.Columns.Add(SR.GetString("BuildOptionsPage_Path"), 260, HorizontalAlignment.Left);            
            addButton.Text = SR.GetString(SR.Add);
            deleteButton.Text = SR.GetString(SR.Delete);
            labelX86.Text = SR.GetString("BuildOptionsPage_DefaultQtX86Version");
            labelX64.Text = SR.GetString("BuildOptionsPage_DefaultQtX64Version");
            labelWinCE.Text = SR.GetString("BuildOptionsPage_WinCEQtVersion");
            okButton.Text = SR.GetString("OK");
            cancelButton.Text = SR.GetString("Cancel");
            tabControl1.TabPages[0].Text = SR.GetString("BuildOptionsPage_Title");
            tabControl1.TabPages[1].Text = SR.GetString("QtDefaultSettings");

#if !ENABLE_WINCE
            // Just hide the Windows CE specific combobox and occupy the released screen space.
            labelWinCE.Hide();
            winCECombo.Hide();
            tableLayoutPanel2.RowCount -= 1;
            tableLayoutPanel2.RowStyles.RemoveAt(tableLayoutPanel2.RowCount);
#endif

            SetupVersionComboBox(defaultX86Combo, null, new VIBoolPredicate(isDesktopQt));
            SetupVersionComboBox(defaultX64Combo, null, new VIBoolPredicate(isDesktopQt));
            SetupWinCEVersionComboBox(null);
            UpdateListBox();

            vsQtSettings = new VSQtSettings();
            optionsPropertyGrid.SelectedObject = vsQtSettings;

            this.KeyPress += new KeyPressEventHandler(this.FormQtVersions_KeyPress);
            this.Shown += new EventHandler(FormQtVersions_Shown);
        }

        void FormQtVersions_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        void FormQtVersions_Shown(object sender, System.EventArgs args)
        {
            string error = null;
            if (versionManager.HasInvalidVersions(out error))
                Messages.DisplayErrorMessage(error);
        }

        public void LoadSettings()
        {
            SetupDefaultVersionComboBox();
            SetupWinCEVersionComboBox(versionManager.GetDefaultWinCEVersion());
        }

        public void SaveSettings()
        {
            versionManager.SaveDefaultX86Version(defaultX86Combo.Text);
            versionManager.SaveDefaultX64Version(defaultX64Combo.Text);
            versionManager.SaveDefaultWinCEVersion(winCECombo.Text);
        }

        private void UpdateListBox()
        {
            UpdateListBox(null);
        }

        private void UpdateListBox(string defaultQtVersionDir)
        {
            listView.Items.Clear();
            foreach (string version in versionManager.GetVersions()) 
            {
                string path = null;
                if (defaultQtVersionDir != null && version == "$(DefaultQtVersion)")
                    path = defaultQtVersionDir;
                else
                    path = versionManager.GetInstallPath(version);
                if (path == null && version != "$(QTDIR)")
                    continue;
                ListViewItem itm = new ListViewItem();
                itm.Tag = version;
                itm.Text = version;
                itm.SubItems.Add(path);
                listView.Items.Add(itm);
            }
        }

        private delegate bool VIBoolPredicate(VersionInformation vi);

        private static bool isDesktopQt(VersionInformation vi)
        {
            return !vi.IsWinCEVersion();
        }

        private void SetupDefaultVersionComboBox()
        {
            SetupVersionComboBox(defaultX86Combo, versionManager.GetDefaultX86Version(), new VIBoolPredicate(isDesktopQt));
            SetupVersionComboBox(defaultX64Combo, versionManager.GetDefaultX64Version(), new VIBoolPredicate(isDesktopQt));
        }

        private static bool isQtWinCE(VersionInformation vi)
        {
            return vi.IsWinCEVersion();
        }

        private void SetupWinCEVersionComboBox(string version)
        {
#if ENABLE_WINCE
            SetupVersionComboBox(winCECombo, version, new VIBoolPredicate(isQtWinCE));
#endif
        }

        private void SetupVersionComboBox(ComboBox box, string version, VIBoolPredicate versionInfoCheck)
        {
            string currentItem = box.Text;
            if (version != null)
                currentItem = version;
            box.Items.Clear();

            foreach (string v in versionManager.GetVersions())
            {
                if (v == "$(DefaultQtVersion)")
                    continue;
                try
                {
                    VersionInformation vi = new VersionInformation(versionManager.GetInstallPath(v));
                    if (versionInfoCheck(vi))
                        box.Items.Add(v);
                }
                catch (Exception)
                {
                }
            }

            if (box.Items.Count > 0)
            {
                if (box.Items.Contains(currentItem))
                    box.Text = currentItem;
                else
                    box.Text = (string)box.Items[0];
            }
            else
            {
                box.Text = "";
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            versionManager.ClearVersionCache();
            foreach (ListViewItem item in listView.SelectedItems)
            {
                string name = item.Text;
                versionManager.RemoveVersion(name);
                listView.Items.Remove(item);
                SetupVersionComboBox(defaultX86Combo, null, new VIBoolPredicate(isDesktopQt));
                SetupVersionComboBox(defaultX64Combo, null, new VIBoolPredicate(isDesktopQt));
                SetupWinCEVersionComboBox(null);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddQtVersionDialog dia = new AddQtVersionDialog();
            dia.StartPosition = FormStartPosition.CenterParent;
            MainWinWrapper ww = new MainWinWrapper(Connect._applicationObject);
            if (dia.ShowDialog(ww) == DialogResult.OK)
            {
                UpdateListBox();
                SetupVersionComboBox(defaultX86Combo, null, new VIBoolPredicate(isDesktopQt));
                SetupVersionComboBox(defaultX64Combo, null, new VIBoolPredicate(isDesktopQt));
                SetupWinCEVersionComboBox(null);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            vsQtSettings.SaveSettings();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
