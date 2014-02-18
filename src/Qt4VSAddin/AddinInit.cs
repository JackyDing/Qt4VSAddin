/****************************************************************************
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

namespace Qt4VSAddin
{
    using EnvDTE;
    using System;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.CommandBars;
    using Digia.Qt4ProjectLib;

    public class AddinInit
    {
        private Commands cmds = null;
        private _CommandBars cmdBars = null;
        private CommandBar cmdProjectContext = null;
        private CommandBar cmdSolutionContext = null;
        private CommandBar cmdItemContext = null;
        private CommandBarPopup qtPopup = null;
        private CommandBarPopup prjPopup = null;
        private CommandBarPopup slnPopup = null;

        private CommandBar FindCommandBar(Guid guidCmdGroup, uint menuID)
        {
            // Retrieve IVsProfferComands via DTE's IOleServiceProvider interface
            IOleServiceProvider sp = (IOleServiceProvider)Connect._applicationObject;
            Guid guidSvc = typeof(IVsProfferCommands).GUID;
            Object objService;
            if (sp.QueryService(ref guidSvc, ref guidSvc, out objService) == 0)
            {
                IVsProfferCommands vsProfferCmds = (IVsProfferCommands)objService;
                return vsProfferCmds.FindCommandBar(IntPtr.Zero, ref guidCmdGroup, menuID) as CommandBar;
            }
            return null;
        }

        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]

        internal interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In]ref Guid guidService, [In]ref Guid riid,
               [MarshalAs(UnmanagedType.Interface)] out System.Object obj);
        }

        // Functions ------------------------------------------------------
        public AddinInit(_DTE a)
        {
            cmdBars = (_CommandBars)a.CommandBars;
            cmds = a.Commands;
            try
            {
                cmdProjectContext = FindCommandBar
                    (new Guid("{D309F791-903F-11D0-9EFC-00A0C911004F}"), 1026);
                cmdSolutionContext = FindCommandBar
                    (new Guid("{D309F791-903F-11D0-9EFC-00A0C911004F}"), 1043);
                cmdItemContext = FindCommandBar
                    (new Guid("{D309F791-903F-11D0-9EFC-00A0C911004F}"), 1072);
            }
            catch
            {
                //MessageBox.Show(e.Message);
            }
        }

        public void removeCommands()
        {
            // Remove all commands we've registered...
            foreach (Command cmdDel in cmds)
            {
                // All our commands starts with "Qt4VSAddin", so we
                // simply remove them. Just looking for the
                // CommandFullName (e.g. NewQtProjectFullCommand)
                // will give use problems if we decide to rename
                // our commands...
                try
                {
                    if (cmdDel.Name.StartsWith(Res.CommandBarName))
                        cmdDel.Delete();
                }
                catch { }
            }
            if (qtPopup != null)
            {
                qtPopup.Visible = false;
                qtPopup.Delete(false);
            }
        }

        private void registerCommandBar()
        {
            try
            {
                CommandBar menuCmdBar = cmdBars["MenuBar"];
                if (qtPopup == null)
                {
                    int targetIndex = 5;
                    try
                    {
                        foreach (CommandBarControl objCommandBarControl in menuCmdBar.Controls)
                        {
                            if (objCommandBarControl.Type == MsoControlType.msoControlPopup)
                            {
                                // See mztools article http://www.mztools.com/articles/2007/MZ2007002.aspx
                                // "HOWTO: Locate commandbars in international versions of Visual Studio"
                                // CommandBar.Name is always the English name. The localized name is stored
                                // in CommandBar.NameLocal.
                                CommandBarPopup objCommandBarPopup = (CommandBarPopup)objCommandBarControl;
                                if (objCommandBarPopup.CommandBar.Name == "View")
                                {
                                    targetIndex = objCommandBarPopup.Index + 1;
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    qtPopup = menuCmdBar.Controls.Add
                        (MsoControlType.msoControlPopup, System.Type.Missing,
                        System.Type.Missing, targetIndex, true)
                        as CommandBarPopup;
                    qtPopup.CommandBar.Name = Res.CommandBarName;
                    qtPopup.Caption = "&Qt4";
                }
                qtPopup.Visible = true;
            }
            catch { }
        }

        private void registerPrjCommandBar()
        {
            try
            {
                if (prjPopup == null)
                {
                    prjPopup = cmdProjectContext.Controls.Add
                        (MsoControlType.msoControlPopup, System.Type.Missing,
                        System.Type.Missing, cmdProjectContext.Controls.Count - 2, true)
                        as CommandBarPopup;

                    prjPopup.CommandBar.Name = Res.CommandBarName;
                    prjPopup.Caption = "&Qt4";
                    prjPopup.BeginGroup = true;
                }
                prjPopup.Visible = true;
            }
            catch { }
        }

        private void registerSlnCommandBar()
        {
            try
            {
                if (slnPopup == null)
                {
                    slnPopup = cmdSolutionContext.Controls.Add
                        (MsoControlType.msoControlPopup, System.Type.Missing,
                        System.Type.Missing, cmdSolutionContext.Controls.Count - 2, true)
                        as CommandBarPopup;

                    slnPopup.CommandBar.Name = Res.CommandBarName;
                    slnPopup.Caption = "&Qt4";
                    slnPopup.BeginGroup = true;
                }
                slnPopup.Visible = true;
            }
            catch { }
        }

        public void registerCommands()
        {
            registerCommandBar();
            registerPrjCommandBar();
            registerSlnCommandBar();
            object[] contextGUIDS = new object[] { };
            try
            {
                CommandBar cmdBar = qtPopup.CommandBar;
                CommandBar cmdPrjBar = prjPopup.CommandBar;
                CommandBar cmdSlnBar = slnPopup.CommandBar;
                if (cmdBar == null)
                    throw new Qt4VSException(SR.GetString("CommandBarNotFound"));

                int disableFlags = (int)vsCommandStatus.vsCommandStatusSupported +
                    (int)vsCommandStatus.vsCommandStatusEnabled;
                Command cmd = null;

                int posBeforeProperties = cmdProjectContext.Controls.Count - 1;

                // Launch Designer command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.LaunchDesigner,
                                SR.GetString("LaunchDesignerButtonText"),
                                SR.GetString("LaunchDesignerToolTip"),
                                false,
                                Res.DesignerBitmapID,
                                ref contextGUIDS,
                                disableFlags);
                CommandBarButton ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("LaunchDesignerButtonText");

                // Launch Linguist command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.LaunchLinguist,
                                SR.GetString("LaunchLinguistButtonText"),
                                SR.GetString("LaunchLinguistToolTip"),
                                false,
                                Res.LinguistBitmapID,
                                ref contextGUIDS,
                                disableFlags);
                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("LaunchLinguistButtonText");

                // Launch Assistant command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.LaunchAssistant,
                                SR.GetString("LaunchAssistantButtonText"),
                                SR.GetString("LaunchAssistantToolTip"),
                                false,
                                Res.AssistantBitmapID,
                                ref contextGUIDS,
                                disableFlags);
                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("LaunchAssistantButtonText");

                // Import pro file command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ImportProFile,
                                SR.GetString("ImportProFileButtonText"),
                                SR.GetString("ImportProFileToolTip"),
                                false,
                                Res.ImportProFileBitmapID,
                                ref contextGUIDS,
                                disableFlags);
                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.BeginGroup = true;
                ctrl.Caption = SR.GetString("ImportProFileButtonText");

                // Import pri file command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ImportPriFile,
                                SR.GetString("ImportPriFileButtonText"),
                                SR.GetString("ImportPriFileToolTip"),
                                false,
                                Res.ImportPriFileBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ImportPriFileButtonText");
                ctrl.BeginGroup = true;

                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ImportPriFileButtonText");

                // Export pri file command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ExportPriFile,
                                SR.GetString("ExportPriFileButtonText"),
                                SR.GetString("ExportPriFileToolTip"),
                                false,
                                Res.ExportPriFileBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ExportPriFileButtonText");

                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ExportPriFileButtonText");

                // Export pro file command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ExportProFile,
                                SR.GetString("ExportProFileButtonText"),
                                SR.GetString("ExportProFileToolTip"),
                                false,
                                Res.ExportProFileBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ExportProFileButtonText");

                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ExportProFileButtonText");

                // Create new translation file command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.CreateNewTranslationFile,
                                SR.GetString("CreateNewTranslationFileButtonText"),
                                SR.GetString("CreateNewTranslationFileToolTip"),
                                false,
                                Res.CreateNewTranslationFileBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("CreateNewTranslationFileButtonText");
                ctrl.BeginGroup = true;

                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("CreateNewTranslationFileButtonText");
                ctrl.BeginGroup = true;

                // lupdateProject command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.lupdateProject,
                                SR.GetString("lupdateProjectButtonText"),
                                null,
                                false,
                                Res.LinguistBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("lupdateProjectButtonText");

                // lreleaseProject command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.lreleaseProject,
                                SR.GetString("lreleaseProjectButtonText"),
                                null,
                                false,
                                Res.LinguistBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("lreleaseProjectButtonText");

                // lupdateSolution command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.lupdateSolution,
                                SR.GetString("lupdateSolutionButtonText"),
                                null,
                                false,
                                Res.LinguistBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdSlnBar, cmdSlnBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("lupdateSolutionButtonText");
                ctrl.BeginGroup = true;

                // lreleaseSolution command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.lreleaseSolution,
                                SR.GetString("lreleaseSolutionButtonText"),
                                null,
                                false,
                                Res.LinguistBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdSlnBar, cmdSlnBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("lreleaseSolutionButtonText");

                // ConvertToQt command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ConvertToQt,
                                SR.GetString("ConvertToQtButtonText"),
                                SR.GetString("ConvertToQtToolTip"),
                                false,
                                Res.ProjectQtSettingsBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ConvertToQtButtonText");
                ctrl.BeginGroup = true;

                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ConvertToQtButtonText");

                // ConvertToQMake command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ConvertToQMake,
                                SR.GetString("ConvertToQMakeButtonText"),
                                SR.GetString("ConvertToQMakeToolTip"),
                                false,
                                Res.ProjectQtSettingsBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ConvertToQMakeButtonText");
                ctrl.BeginGroup = true;

                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ConvertToQMakeButtonText");

                // ProjectQtSettings command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ProjectQtSettings,
                                SR.GetString("ProjectQtSettingsButtonText"),
                                SR.GetString("ProjectQtSettingsToolTip"),
                                false,
                                Res.ProjectQtSettingsBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ProjectQtSettingsButtonText");
                ctrl.BeginGroup = true;

                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ProjectQtSettingsButtonText");

                // ChangeProjectQtVersion command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ChangeProjectQtVersion,
                                SR.GetString("ChangeProjectQtVersionButtonText"),
                                null,
                                false,
                                Res.ChangeProjectQtVersionBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdPrjBar, cmdPrjBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ChangeProjectQtVersionButtonText");
                ctrl.BeginGroup = true;

                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ChangeProjectQtVersionButtonText");

                // VSQtOptions command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.VSQtOptions,
                                SR.GetString("VSQtOptionsButtonText"),
                                SR.GetString("VSQtOptionsToolTip"),
                                false,
                                Res.VSQtOptionsBitmapID,
                                ref contextGUIDS,
                                disableFlags);
                ctrl = (CommandBarButton)cmd.AddControl(cmdBar, cmdBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("VSQtOptionsButtonText");
                ctrl.BeginGroup = true;

                // Change Soltuion Qt version command
                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                                Res.ChangeSolutionQtVersion,
                                SR.GetString("ChangeSolutionQtVersionButtonText"),
                                SR.GetString("ChangeSolutionQtVersionToolTip"),
                                false,
                                Res.QtBitmapID,
                                ref contextGUIDS,
                                disableFlags);

                ctrl = (CommandBarButton)cmd.AddControl(cmdSlnBar, cmdSlnBar.Controls.Count + 1);
                ctrl.Caption = SR.GetString("ChangeSolutionQtVersionButtonText");
                ctrl.BeginGroup = true;

                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                            "lupdate",
                            "lupdate",
                            "lupdate",
                            false,
                            Res.LinguistBitmapID,
                            ref contextGUIDS,
                            disableFlags);

                posBeforeProperties = cmdItemContext.Controls.Count;
                ctrl = (CommandBarButton)cmd.AddControl(cmdItemContext, posBeforeProperties++);
                ctrl.Caption = "lupdate";
                ctrl.BeginGroup = true;

                cmd = cmds.AddNamedCommand(Connect._addInInstance,
                        "lrelease",
                        "lrelease",
                        "lrelease",
                        false,
                        Res.LinguistBitmapID,
                        ref contextGUIDS,
                        disableFlags);

                posBeforeProperties = cmdItemContext.Controls.Count;
                ctrl = (CommandBarButton)cmd.AddControl(cmdItemContext, posBeforeProperties++);
                ctrl.Caption = "lrelease";
            }
            catch (System.Exception e)
            {
                MessageBox.Show(SR.GetString("CommandsNotRegistered") + " : " + e.Message + "\r\n" + e.StackTrace.ToString());
            }
        }

    }
}
