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
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.VCProjectEngine;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using Digia.Qt4ProjectLib;

namespace Qt4VSAddin
{
    class AddInEventHandler
    {
        private DTE dte;
        private EnvDTE.SolutionEvents solutionEvents;
        private EnvDTE.BuildEvents buildEvents;
        private EnvDTE.DocumentEvents documentEvents;
        private EnvDTE.ProjectItemsEvents projectItemsEvents;
        private EnvDTE.vsBuildAction currentBuildAction = vsBuildAction.vsBuildActionBuild;
        private VCProjectEngineEvents vcProjectEngineEvents = null;
        private CommandEvents debugStartEvents;
        private CommandEvents debugStartWithoutDebuggingEvents;
        private System.Threading.Thread appWrapperThread = null;
        private System.Diagnostics.Process appWrapperProcess = null;
        private bool terminateEditorThread = false;
        private TcpClient client = null;
        private byte[] qtAppWrapperHelloMessage = new byte[] { 0x48, 0x45, 0x4C, 0x4C, 0x4F };
        private SimpleThreadMessenger simpleThreadMessenger = null;
        private int dispId_VCFileConfiguration_ExcludedFromBuild;
        private int dispId_VCCLCompilerTool_UsePrecompiledHeader;
        private int dispId_VCCLCompilerTool_PrecompiledHeaderThrough;
        private int dispId_VCCLCompilerTool_PreprocessorDefinitions;
        private int dispId_VCCLCompilerTool_AdditionalIncludeDirectories;
        
        public AddInEventHandler(DTE _dte)
        {
            simpleThreadMessenger = new SimpleThreadMessenger(this);
            dte = _dte;
            Events2 events = dte.Events as Events2;

            buildEvents = (EnvDTE.BuildEvents)events.BuildEvents;
            buildEvents.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(buildEvents_OnBuildBegin);
            buildEvents.OnBuildProjConfigBegin += new _dispBuildEvents_OnBuildProjConfigBeginEventHandler(this.OnBuildProjConfigBegin);
            buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(this.buildEvents_OnBuildDone);

            documentEvents = (EnvDTE.DocumentEvents)events.get_DocumentEvents(null);
            documentEvents.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(this.DocumentSaved);

            projectItemsEvents = (ProjectItemsEvents)events.ProjectItemsEvents;
            projectItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(this.ProjectItemsEvents_ItemAdded);
            projectItemsEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(this.ProjectItemsEvents_ItemRemoved);
            projectItemsEvents.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(this.ProjectItemsEvents_ItemRenamed);

            solutionEvents = (SolutionEvents)events.SolutionEvents;
            solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler(this.SolutionEvents_ProjectAdded);
            solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(this.SolutionEvents_ProjectRemoved);
            solutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler(SolutionEvents_Opened);
            solutionEvents.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler(SolutionEvents_AfterClosing);

            const string debugCommandsGUID = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            debugStartEvents = events.get_CommandEvents(debugCommandsGUID, 295);
            debugStartEvents.BeforeExecute += new _dispCommandEvents_BeforeExecuteEventHandler(debugStartEvents_BeforeExecute);

            debugStartWithoutDebuggingEvents = events.get_CommandEvents(debugCommandsGUID, 368);
            debugStartWithoutDebuggingEvents.BeforeExecute += new _dispCommandEvents_BeforeExecuteEventHandler(debugStartWithoutDebuggingEvents_BeforeExecute);

            dispId_VCFileConfiguration_ExcludedFromBuild = GetPropertyDispId(typeof(VCFileConfiguration), "ExcludedFromBuild");
            dispId_VCCLCompilerTool_UsePrecompiledHeader = GetPropertyDispId(typeof(VCCLCompilerTool), "UsePrecompiledHeader");
            dispId_VCCLCompilerTool_PrecompiledHeaderThrough = GetPropertyDispId(typeof(VCCLCompilerTool), "PrecompiledHeaderThrough");
            dispId_VCCLCompilerTool_PreprocessorDefinitions = GetPropertyDispId(typeof(VCCLCompilerTool), "PreprocessorDefinitions");
            dispId_VCCLCompilerTool_AdditionalIncludeDirectories = GetPropertyDispId(typeof(VCCLCompilerTool), "AdditionalIncludeDirectories");
            RegisterVCProjectEngineEvents();

            if (Connect.Instance().AppWrapperPath == null)
            {
                Messages.DisplayCriticalErrorMessage("QtAppWrapper can't be found in the installation directory.");
            }
            else
            {
                appWrapperProcess = new System.Diagnostics.Process();
                appWrapperProcess.StartInfo.FileName = Connect.Instance().AppWrapperPath;
            }
            appWrapperThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenForRequests));
            appWrapperThread.Name = "QtAppWrapperListener";
            appWrapperThread.Start();
        }

        void debugStartEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            EnvDTE.Project selectedProject = HelperFunctions.GetSelectedQtProject(dte);
            if (selectedProject != null)
            {
                QtVersionManager.The().SetPlatform(selectedProject.ConfigurationManager.ActiveConfiguration.PlatformName);
                QtProject qtProject = QtProject.Create(selectedProject);
                if (qtProject != null)
                    qtProject.SetQtEnvironment();
            }
        }

        void debugStartWithoutDebuggingEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            EnvDTE.Project selectedProject = HelperFunctions.GetSelectedQtProject(dte);
            if (selectedProject != null)
            {
                QtVersionManager.The().SetPlatform(selectedProject.ConfigurationManager.ActiveConfiguration.PlatformName);
                QtProject qtProject = QtProject.Create(selectedProject);
                if (qtProject != null)
                    qtProject.SetQtEnvironment();
            }
        }

        private void OpenFileExternally(string fileName)
        {
            bool abortOperation;
            CheckoutFileIfNeeded(fileName, out abortOperation);
            if (abortOperation)
                return;

            string lowerCaseFileName = fileName.ToLower();
            if (lowerCaseFileName.EndsWith(".ui"))
            {
                Connect.extLoader.loadDesigner(fileName);

                // Designer can't cope with many files in a short time.
                System.Threading.Thread.Sleep(1000);
            }
            else if (lowerCaseFileName.EndsWith(".ts"))
            {
                ExtLoader.loadLinguist(fileName);
            }
#if false
            // QRC files are directly opened, using the QRC editor.
            else if (lowerCaseFileName.EndsWith(".qrc"))
            {
                Connect.extLoader.loadQrcEditor(fileName);
            }
#endif
        }

#if DEBUG
        private void setDirectory(string dir, string value)
        {
            foreach (EnvDTE.Project project in HelperFunctions.ProjectsInSolution(dte))
            {
                VCProject vcProject = project.Object as VCProject;
                if (vcProject == null || vcProject.Files == null)
                    continue;
                QtProject qtProject = QtProject.Create(project);
                if (qtProject == null)
                    continue;

                if (dir == "MocDir")
                {
                    string oldMocDir = QtVSIPSettings.GetMocDirectory(project);
                    QtVSIPSettings.SaveMocDirectory(project, value);
                    qtProject.UpdateMocSteps(oldMocDir);
                }
                else if (dir == "RccDir")
                {
                    string oldRccDir = QtVSIPSettings.GetRccDirectory(project);
                    QtVSIPSettings.SaveRccDirectory(project, value);
                    qtProject.RefreshRccSteps(oldRccDir);
                }
                else if (dir == "UicDir")
                {
                    string oldUicDir = QtVSIPSettings.GetUicDirectory(project);
                    QtVSIPSettings.SaveUicDirectory(project, value);
                    qtProject.UpdateUicSteps(oldUicDir, true);
                }
            }
        }
#endif

        private void OnQRCFileSaved(string fileName)
        {
            foreach (EnvDTE.Project project in HelperFunctions.ProjectsInSolution(dte))
            {
                VCProject vcProject = project.Object as VCProject;
                if (vcProject == null || vcProject.Files == null)
                    continue;

                VCFile vcFile = (VCFile)((IVCCollection)vcProject.Files).Item(fileName);
                if (vcFile == null)
                    continue;

                QtProject qtProject = QtProject.Create(project);
                qtProject.UpdateRccStep(vcFile, null);
            }
        }

        private void CheckoutFileIfNeeded(string fileName, out bool abortOperation)
        {
            abortOperation = false;

            if (QtVSIPSettings.GetDisableCheckoutFiles())
                return;

            SourceControl sourceControl = dte.SourceControl;
            if (sourceControl == null)
                return;

            if (!sourceControl.IsItemUnderSCC(fileName))
                return;

            if (sourceControl.IsItemCheckedOut(fileName))
                return;

            if (QtVSIPSettings.GetAskBeforeCheckoutFile())
            {
                string shortFileName = System.IO.Path.GetFileName(fileName);
                DialogResult dr = MessageBox.Show(
                                    SR.GetString("QuestionSCCCheckoutOnOpen", shortFileName),
                                    Resources.msgBoxCaption, MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Cancel)
                    abortOperation = true;
                if (dr != DialogResult.Yes)
                    return;
            }

            sourceControl.CheckOutItem(fileName);
        }

        /// <summary>
        /// Dumb control to send stuff from one thread to another.
        /// </summary>
        private class SimpleThreadMessenger : Control
        {
            private AddInEventHandler addinEventHandler = null;

            public SimpleThreadMessenger(AddInEventHandler handler)
            {
                addinEventHandler = handler;
                CreateControl();
            }

            public delegate void HandleMessageFromQtAppWrapperDelegate(string message);

            /// <summary>
            /// This function handle file names in the Addin's main thread,
            /// that come from the QtAppWrapper.
            /// </summary>
            public void HandleMessageFromQtAppWrapper(string message)
            {
                if (message.ToLower().EndsWith(".qrc"))
                    addinEventHandler.OnQRCFileSaved(message);
#if DEBUG
                else if (message.StartsWith("Autotests:set"))
                {
                    // Messageformat from Autotests is Autotests:set<dir>:<value>
                    // where dir is MocDir, RccDir or UicDir

                    //remove Autotests:set
                    message = message.Substring(13);

                    string dir = message.Remove(6);
                    string value = message.Substring(7);

                    addinEventHandler.setDirectory(dir, value);
                }
#endif
                else
                    addinEventHandler.OpenFileExternally(message);
            }
        }

        private void ListenForRequests()
        {
            if (appWrapperProcess == null)
                return;

            SimpleThreadMessenger.HandleMessageFromQtAppWrapperDelegate handleMessageFromQtAppWrapperDelegate;
            handleMessageFromQtAppWrapperDelegate = new SimpleThreadMessenger.HandleMessageFromQtAppWrapperDelegate(simpleThreadMessenger.HandleMessageFromQtAppWrapper);

            bool firstIteration = true;
            while (!terminateEditorThread)
            {
                try
                {
                    if (!firstIteration)
                    {
                        if (appWrapperProcess.HasExited)
                            appWrapperProcess.Close();
                    }
                    else
                    {
                        firstIteration = false;
                    }
                }
                catch
                { }

                appWrapperProcess.Start();

                client = new TcpClient();
                int connectionAttempts = 0;
                int appwrapperPort = 12005;
                while (!client.Connected && !terminateEditorThread && connectionAttempts < 10)
                {
                    try
                    {
                        client.Connect(IPAddress.Loopback, appwrapperPort);
                        if (!client.Connected)
                        {
                            ++connectionAttempts;
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    catch
                    {
                        ++connectionAttempts;
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                if (connectionAttempts >= 10)
                {
                    Messages.DisplayErrorMessage(SR.GetString("CouldNotConnectToAppwrapper", appwrapperPort));
                    terminateEditorThread = true;
                }

                if (terminateEditorThread)
                {
                    TerminateClient();
                    return;
                }

                NetworkStream clientStream = client.GetStream();

                // say hello to qtappwrapper
                clientStream.Write(qtAppWrapperHelloMessage, 0, qtAppWrapperHelloMessage.Length);
                clientStream.Flush();

                byte[] message = new byte[4096];
                int bytesRead;

                while (!terminateEditorThread)
                {
                    try
                    {
                        bytesRead = 0;

                        try
                        {
                            //blocks until a client sends a message
                            bytesRead = clientStream.Read(message, 0, 4096);
                        }
                        catch
                        {
                            // A socket error has occured, probably because
                            // the QtAppWrapper has been terminated.
                            // Break and then try to restart the QtAppWrapper.
                            break;
                        }

                        if (bytesRead == 0)
                        {
                            //the client has disconnected from the server
                            break;
                        }

                        //message has successfully been received
                        UnicodeEncoding encoder = new UnicodeEncoding();
                        string fullMessageString = encoder.GetString(message, 0, bytesRead);
                        string[] messages = fullMessageString.Split(new Char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string messageString in messages)
                        {
                            int index = messageString.IndexOf(' ');
                            int requestedPid = Convert.ToInt32(messageString.Substring(0, index));
                            int currentPid = System.Diagnostics.Process.GetCurrentProcess().Id;
                            if (requestedPid == currentPid)
                            {
                                // Actual file opening is done in the main thread.
                                string file = messageString.Substring(index + 1);
                                simpleThreadMessenger.Invoke(handleMessageFromQtAppWrapperDelegate, new object[] { file });
                            }
                        }
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        break;
                    }
                    catch { }
                }
                TerminateClient();
            }
        }

        private void TerminateClient()
        {
            try
            {
                if (client != null && client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    if (stream != null)
                        stream.Close();
                    client.Close();
                    client = null;
                }
            }
            catch
            { /* ignore */ }
        }

        public void Disconnect()
        {
            if (buildEvents != null)
            {
                buildEvents.OnBuildBegin -= new _dispBuildEvents_OnBuildBeginEventHandler(this.buildEvents_OnBuildBegin);
                buildEvents.OnBuildProjConfigBegin -= new _dispBuildEvents_OnBuildProjConfigBeginEventHandler(this.OnBuildProjConfigBegin);
                buildEvents.OnBuildDone -= new _dispBuildEvents_OnBuildDoneEventHandler(this.buildEvents_OnBuildDone);
            }

            if (documentEvents != null)
                documentEvents.DocumentSaved -= new _dispDocumentEvents_DocumentSavedEventHandler(this.DocumentSaved);

            if (projectItemsEvents != null)
            {
                projectItemsEvents.ItemAdded -= new _dispProjectItemsEvents_ItemAddedEventHandler(this.ProjectItemsEvents_ItemAdded);
                projectItemsEvents.ItemRemoved -= new _dispProjectItemsEvents_ItemRemovedEventHandler(this.ProjectItemsEvents_ItemRemoved);
                projectItemsEvents.ItemRenamed -= new _dispProjectItemsEvents_ItemRenamedEventHandler(this.ProjectItemsEvents_ItemRenamed);
            }

            if (solutionEvents != null)
            {
                solutionEvents.ProjectAdded -= new _dispSolutionEvents_ProjectAddedEventHandler(this.SolutionEvents_ProjectAdded);
                solutionEvents.ProjectRemoved -= new _dispSolutionEvents_ProjectRemovedEventHandler(SolutionEvents_ProjectRemoved);
                solutionEvents.Opened -= new _dispSolutionEvents_OpenedEventHandler(SolutionEvents_Opened);
                solutionEvents.AfterClosing -= new _dispSolutionEvents_AfterClosingEventHandler(SolutionEvents_AfterClosing);
            }

            if (debugStartEvents != null)
                debugStartEvents.BeforeExecute -= new _dispCommandEvents_BeforeExecuteEventHandler(debugStartEvents_BeforeExecute);

            if (debugStartWithoutDebuggingEvents != null)
                debugStartWithoutDebuggingEvents.BeforeExecute -= new _dispCommandEvents_BeforeExecuteEventHandler(debugStartWithoutDebuggingEvents_BeforeExecute);

            if (vcProjectEngineEvents != null)
                vcProjectEngineEvents.ItemPropertyChange -= new _dispVCProjectEngineEvents_ItemPropertyChangeEventHandler(OnVCProjectEngineItemPropertyChange);

            if (appWrapperThread != null)
            {
                terminateEditorThread = true;
                if (appWrapperThread.IsAlive)
                {
                    TerminateClient();
                    if (!appWrapperThread.Join(1000))
                        appWrapperThread.Abort();
                }
            }
        }

        public void OnBuildProjConfigBegin(string projectName, string projectConfig, string platform, string solutionConfig)
        {
            if (currentBuildAction != vsBuildAction.vsBuildActionBuild &&
                currentBuildAction != vsBuildAction.vsBuildActionRebuildAll)
            {
                return;     // Don't do anything, if we're not building.
            }

            EnvDTE.Project project = null;
            foreach (EnvDTE.Project p in HelperFunctions.ProjectsInSolution(dte))
            {
                if (p.UniqueName == projectName)
                {
                    project = p;
                    break;
                }
            }

            if (project == null || !HelperFunctions.IsVcProject(project))
                return;

            QtVersionManager versionManager = QtVersionManager.The();
            versionManager.SetPlatform(project.ConfigurationManager.ActiveConfiguration.PlatformName);

            if (HelperFunctions.IsQtProject(project))
            {
                if (HelperFunctions.IsQt4Project(project))
                {
                    QtProject qtpro = QtProject.Create(project);

                    string qtVersion = versionManager.GetProjectQtVersion(project, platform);
                    if (qtVersion == null)
                    {
                        qtVersion = versionManager.GetDefaultVersion();
                        if (qtVersion == null)
                        {
                            Messages.DisplayCriticalErrorMessage(SR.GetString("ProjectQtVersionNotFoundError", platform));
                            dte.ExecuteCommand("Build.Cancel", "");
                            return;
                        }
                    }

                    if (!QtVSIPSettings.GetDisableAutoMocStepsUpdate())
                    {
                        if (qtpro.ConfigurationRowNamesChanged)
                        {
                            qtpro.UpdateMocSteps(QtVSIPSettings.GetMocDirectory(project));
                        }
                    }

                    // Solution config is given to function to get QTDIR property
                    // set correctly also during batch build
                    qtpro.SetQtEnvironment(qtVersion, solutionConfig);
                    if (QtVSIPSettings.GetLUpdateOnBuild(project))
                        Translation.RunlUpdate(project);
                }  
            }
            else
            {
                if (HelperFunctions.IsQMakeProject(project))
                {
                    HelperFunctions.SetQtEnvironment(project, versionManager.GetInstallPath("$(DefaultQtVersion)"), "");
                }
                else
                {
                    string version = versionManager.GetSolutionQtVersion(dte.Solution);
                    if (version != null)
                    {
                        HelperFunctions.SetQtEnvironment(project, versionManager.GetInstallPath(version), "");
                    }
                }
            }
        }

        void buildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            currentBuildAction = Action;
        }

        public void buildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
        }

        public void DocumentSaved(EnvDTE.Document document)
        {
            QtProject qtPro = QtProject.Create(document.ProjectItem.ContainingProject);

            if (!HelperFunctions.IsQt4Project(qtPro.VCProject))
                return;

            VCFile file = (VCFile)((IVCCollection)qtPro.VCProject.Files).Item(document.FullName);

            if (file.Extension == ".ui")
            {
                if (QtVSIPSettings.AutoUpdateUicSteps() && !QtProject.HasUicStep(file))
                    qtPro.AddUic4BuildStep(file);
                return;
            }

            if (!HelperFunctions.HasSourceFileExtension(file.Name) && !HelperFunctions.HasHeaderFileExtension(file.Name))
                return;

            if (HelperFunctions.HasQObjectDeclaration(file))
            {
                if (!qtPro.HasMocStep(file))
                    qtPro.AddMocStep(file);
            }
            else
            {
                qtPro.RemoveMocStep(file);
            }

            if (HelperFunctions.HasSourceFileExtension(file.Name))
            {
                string moccedFileName = "moc_" + file.Name;

                if (qtPro.IsMoccedFileIncluded(file))
                {
                    // exclude moc_foo.cpp from build
#if (VS2012 || VS2013)
                    // Code copied here from 'GetFilesFromProject'
                    // For some reason error CS1771 was generated from function call
                    List<VCFile> tmpList = new System.Collections.Generic.List<VCFile>();
                    moccedFileName = HelperFunctions.NormalizeRelativeFilePath(moccedFileName);

                    FileInfo fi = new FileInfo(moccedFileName);
                    foreach (VCFile f in (IVCCollection)qtPro.VCProject.Files)
                    {
                        if (f.Name.ToLower() == fi.Name.ToLower())
                            tmpList.Add(f);
                    }
                    foreach (VCFile moccedFile in tmpList)
                        QtProject.ExcludeFromAllBuilds(moccedFile);
#else
                    foreach (VCFile moccedFile in qtPro.GetFilesFromProject(moccedFileName))
                        QtProject.ExcludeFromAllBuilds(moccedFile);
#endif
                }
                else
                {
                    // make sure that moc_foo.cpp isn't excluded from build
#if (VS2012 || VS2013)
                    // Code copied here from 'GetFilesFromProject'
                    // For some reason error CS1771 was generated from function call
                    List<VCFile> moccedFiles = new System.Collections.Generic.List<VCFile>();
                    moccedFileName = HelperFunctions.NormalizeRelativeFilePath(moccedFileName);

                    FileInfo fi = new FileInfo(moccedFileName);
                    foreach (VCFile f in (IVCCollection)qtPro.VCProject.Files)
                    {
                        if (f.Name.ToLower() == fi.Name.ToLower())
                            moccedFiles.Add(f);
                    }
#else
                    List<VCFile> moccedFiles = qtPro.GetFilesFromProject(moccedFileName);
#endif
                    if (moccedFiles.Count > 0)
                    {
                        bool hasDifferentMocFilesPerConfig = QtVSIPSettings.HasDifferentMocFilePerConfig(qtPro.Project);
                        bool hasDifferentMocFilesPerPlatform = QtVSIPSettings.HasDifferentMocFilePerPlatform(qtPro.Project);
                        VCFilter generatedFiles = qtPro.FindFilterFromGuid(Filters.GeneratedFiles().UniqueIdentifier);
                        foreach (VCFile fileInFilter in (IVCCollection)generatedFiles.Files)
                        {
                            if (fileInFilter.Name == moccedFileName)
                            {
                                foreach (VCFileConfiguration config in (IVCCollection)fileInFilter.FileConfigurations)
                                {
                                    bool exclude = true;
                                    VCConfiguration vcConfig = config.ProjectConfiguration as VCConfiguration;
                                    if (hasDifferentMocFilesPerConfig && hasDifferentMocFilesPerPlatform)
                                    {
                                        VCPlatform platform = vcConfig.Platform as VCPlatform;
                                        string platformName = platform.Name;
                                        if (fileInFilter.RelativePath.ToLower().Contains(vcConfig.ConfigurationName.ToLower())
                                            && fileInFilter.RelativePath.ToLower().Contains(platform.Name.ToLower()))
                                                exclude = false;
                                    }
                                    else if (hasDifferentMocFilesPerConfig)
                                    {
                                        if (fileInFilter.RelativePath.ToLower().Contains(vcConfig.ConfigurationName.ToLower()))
                                            exclude = false;
                                    }
                                    else if (hasDifferentMocFilesPerPlatform)
                                    {
                                        VCPlatform platform = vcConfig.Platform as VCPlatform;
                                        string platformName = platform.Name;
                                        if (fileInFilter.RelativePath.ToLower().Contains(platformName.ToLower()))
                                            exclude = false;
                                    }
                                    else
                                    {
                                        exclude = false;
                                    }
                                    if (config.ExcludedFromBuild != exclude)
                                        config.ExcludedFromBuild = exclude;
                                }
                            }
                        }
                        foreach (VCFilter filt in (IVCCollection)generatedFiles.Filters)
                        {
                            foreach (VCFile f in (IVCCollection)filt.Files)
                            {
                                if (f.Name == moccedFileName)
                                {
                                    foreach (VCFileConfiguration config in (IVCCollection)f.FileConfigurations)
                                    {
                                        VCConfiguration vcConfig = config.ProjectConfiguration as VCConfiguration;
                                        string filterToLookFor = "";
                                        if (hasDifferentMocFilesPerConfig)
                                            filterToLookFor = vcConfig.ConfigurationName;
                                        if (hasDifferentMocFilesPerPlatform)
                                        {
                                            VCPlatform platform = vcConfig.Platform as VCPlatform;
                                            if (!string.IsNullOrEmpty(filterToLookFor))
                                                filterToLookFor += '_';
                                            filterToLookFor += platform.Name;
                                        }
                                        if (filt.Name == filterToLookFor)
                                        {
                                            if (config.ExcludedFromBuild)
                                                config.ExcludedFromBuild = false;
                                        }
                                        else
                                        {
                                            if (!config.ExcludedFromBuild)
                                                config.ExcludedFromBuild = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ProjectItemsEvents_ItemAdded(ProjectItem projectItem)
        {
            Project project = HelperFunctions.GetSelectedQtProject(Connect._applicationObject);
            QtProject qtPro = QtProject.Create(project);
            if (!HelperFunctions.IsQt4Project(project))
                return;
            VCFilter filter = null;
            VCFile vcFile = GetVCFileFromProject(projectItem.Name, qtPro.VCProject);
            if (vcFile == null)
                return;

            try
            {
                // Try to find the filter, the file is located in
                // If the file is not inside any filter, move it to
                // the according one, used by the Add-in
                filter = (VCFilter)vcFile.Parent;
            }
            catch { }

            try
            {
                FakeFilter ui = Filters.FormFiles();
                FakeFilter qrc = Filters.ResourceFiles();
                FakeFilter ts = Filters.TranslationFiles();
                FakeFilter h = Filters.HeaderFiles();
                FakeFilter src = Filters.SourceFiles();

                VCFilter uiFilter = qtPro.FindFilterFromGuid(ui.UniqueIdentifier);
                VCFilter tsFilter = qtPro.FindFilterFromGuid(ts.UniqueIdentifier);
                VCFilter qrcFilter = qtPro.FindFilterFromGuid(qrc.UniqueIdentifier);
                VCFilter hFilter = qtPro.FindFilterFromGuid(h.UniqueIdentifier);
                VCFilter srcFilter = qtPro.FindFilterFromGuid(src.UniqueIdentifier);

                if (HelperFunctions.HasSourceFileExtension(vcFile.Name))
                {
                    if (vcFile.Name.ToLower().StartsWith("moc_"))
                        return;
                    else if (vcFile.Name.ToLower().StartsWith("qrc_"))
                    {
                        // Do not use precompiled headers with these files
                        QtProject.SetPCHOption(vcFile, pchOption.pchNone);
                        return;
                    }
                    string pcHeaderThrough = qtPro.GetPrecompiledHeaderThrough();
                    if (pcHeaderThrough != null)
                    {
                        string pcHeaderCreator = pcHeaderThrough.Remove(pcHeaderThrough.LastIndexOf('.')) + ".cpp";
                        if (vcFile.Name.ToLower().EndsWith(pcHeaderCreator.ToLower())
                            && HelperFunctions.CxxFileContainsNotCommented(vcFile, "#include \"" + pcHeaderThrough + "\"", false, false))
                        {
                            //File is used to create precompiled headers
                            QtProject.SetPCHOption(vcFile, pchOption.pchCreateUsingSpecific);
                            return;
                        }
                    }
                    if (filter == null && !HelperFunctions.IsInFilter(vcFile, src))
                    {
                        if (null == srcFilter && qtPro.VCProject.CanAddFilter(src.Name))
                        {
                            srcFilter = (VCFilter)qtPro.VCProject.AddFilter(src.Name);
                            srcFilter.Filter = src.Filter;
                            srcFilter.ParseFiles = src.ParseFiles;
                            srcFilter.UniqueIdentifier = src.UniqueIdentifier;
                        }
                        qtPro.RemoveItem(projectItem);
                        qtPro.AddFileToProject(vcFile.FullPath, src);
                    }
                    if (HelperFunctions.HasQObjectDeclaration(vcFile))
                    {
#if (VS2010 || VS2012 || VS2013)
                        HelperFunctions.EnsureCustomBuildToolAvailable(projectItem);
#endif
                        qtPro.AddMocStep(vcFile);
                    }
                }
                else if (HelperFunctions.HasHeaderFileExtension(vcFile.Name))
                {
                    if (vcFile.Name.ToLower().StartsWith("ui_"))
                        return;
                    if (filter == null && !HelperFunctions.IsInFilter(vcFile, h))
                    {
                        if (null == hFilter && qtPro.VCProject.CanAddFilter(h.Name))
                        {
                            hFilter = (VCFilter)qtPro.VCProject.AddFilter(h.Name);
                            hFilter.Filter = h.Filter;
                            hFilter.ParseFiles = h.ParseFiles;
                            hFilter.UniqueIdentifier = h.UniqueIdentifier;
                        }
                        qtPro.RemoveItem(projectItem);
                        qtPro.AddFileToProject(vcFile.FullPath, h);
                    }
                    if (HelperFunctions.HasQObjectDeclaration(vcFile))
                    {
#if (VS2010 || VS2012 || VS2013)
                        HelperFunctions.EnsureCustomBuildToolAvailable(projectItem);
#endif
                        qtPro.AddMocStep(vcFile);
                    }
                }
                else if (vcFile.Name.EndsWith(".ui"))
                {
                    if (filter == null && !HelperFunctions.IsInFilter(vcFile, ui))
                    {
                        if (null == uiFilter && qtPro.VCProject.CanAddFilter(ui.Name))
                        {
                            uiFilter = (VCFilter)qtPro.VCProject.AddFilter(ui.Name);
                            uiFilter.Filter = ui.Filter;
                            uiFilter.ParseFiles = ui.ParseFiles;
                            uiFilter.UniqueIdentifier = ui.UniqueIdentifier;
                        }
                        qtPro.RemoveItem(projectItem);
                        qtPro.AddFileToProject(vcFile.FullPath, ui);
                    }
#if (VS2010 || VS2012 || VS2013)
                    HelperFunctions.EnsureCustomBuildToolAvailable(projectItem);
#endif
                    qtPro.AddUic4BuildStep(vcFile);
                }
                else if (vcFile.Name.EndsWith(".qrc"))
                {
                    if (filter == null && !HelperFunctions.IsInFilter(vcFile, qrc))
                    {
                        if (null == qrcFilter && qtPro.VCProject.CanAddFilter(qrc.Name))
                        {
                            qrcFilter = (VCFilter)qtPro.VCProject.AddFilter(qrc.Name);
                            qrcFilter.Filter = qrc.Filter;
                            qrcFilter.ParseFiles = qrc.ParseFiles;
                            qrcFilter.UniqueIdentifier = qrc.UniqueIdentifier;
                        }
                        qtPro.RemoveItem(projectItem);
                        qtPro.AddFileToProject(vcFile.FullPath, qrc);
                    }
#if (VS2010 || VS2012 || VS2013)
                    HelperFunctions.EnsureCustomBuildToolAvailable(projectItem);
#endif
                    qtPro.UpdateRccStep(vcFile, null);
                }
                else if (HelperFunctions.IsTranslationFile(vcFile))
                {
                    if (filter == null && !HelperFunctions.IsInFilter(vcFile, ts))
                    {
                        if (null == tsFilter && qtPro.VCProject.CanAddFilter(ts.Name))
                        {
                            tsFilter = (VCFilter)qtPro.VCProject.AddFilter(ts.Name);
                            tsFilter.Filter = ts.Filter;
                            tsFilter.ParseFiles = ts.ParseFiles;
                            tsFilter.UniqueIdentifier = ts.UniqueIdentifier;
                        }
                        qtPro.RemoveItem(projectItem);
                        qtPro.AddFileToProject(vcFile.FullPath, ts);
                    }
            }
            }
            catch { }

            return;
        }

        void ProjectItemsEvents_ItemRemoved(ProjectItem ProjectItem)
        {
            Project pro = HelperFunctions.GetSelectedQtProject(Connect._applicationObject);
            if (pro == null)
                return;

            QtProject qtPro = QtProject.Create(pro);
            qtPro.RemoveGeneratedFiles(ProjectItem.Name);
        }

        void ProjectItemsEvents_ItemRenamed(ProjectItem ProjectItem, string OldName)
        {
            if (OldName == null)
                return;
            Project pro = HelperFunctions.GetSelectedQtProject(Connect._applicationObject);
            if (pro == null)
                return;

            QtProject qtPro = QtProject.Create(pro);
            qtPro.RemoveGeneratedFiles(OldName);
            ProjectItemsEvents_ItemAdded(ProjectItem);
        }

        void SolutionEvents_ProjectAdded(Project project)
        {
            if (HelperFunctions.IsQMakeProject(project))
            {
                RegisterVCProjectEngineEvents(project);
                VCProject vcpro = project.Object as VCProject;
                VCFilter filter = null;
                foreach (VCFilter f in vcpro.Filters as IVCCollection)
                {
                    if (f.Name == Filters.HeaderFiles().Name)
                    {
                        filter = f;
                        break;
                    }
                }
                if (filter != null)
                {
                    foreach (VCFile file in filter.Files as IVCCollection)
                    {
                        foreach (VCFileConfiguration config in file.FileConfigurations as IVCCollection)
                        {
                            VCCustomBuildTool tool = HelperFunctions.GetCustomBuildTool(config);
                            if (tool != null && tool.CommandLine != null && tool.CommandLine.Contains("moc.exe"))
                            {
                                Regex reg = new Regex("[^ ^\n]+moc\\.exe");
                                MatchCollection matches = reg.Matches(tool.CommandLine);
                                string qtDir = null;
                                if (matches.Count != 1)
                                {
                                    QtVersionManager vm = QtVersionManager.The();
                                    qtDir = vm.GetInstallPath(vm.GetDefaultVersion());
                                }
                                else
                                {
                                    qtDir = matches[0].ToString();
                                    qtDir = qtDir.Remove(qtDir.LastIndexOf("\\"));
                                    qtDir = qtDir.Remove(qtDir.LastIndexOf("\\"));
                                }
                                qtDir = qtDir.Replace("_(QTDIR)", "$(QTDIR)");
                                HelperFunctions.SetDebuggingEnvironment(project, "PATH=" + qtDir + "\\bin;$(PATH)", true);
                            }
                        }
                    }
                }
            }
        }

        void SolutionEvents_ProjectRemoved(Project project)
        {
        }

        void SolutionEvents_Opened()
        {
            foreach (Project p in HelperFunctions.ProjectsInSolution(Connect._applicationObject))
            {
                if (HelperFunctions.IsQt4Project(p))
                {
                    RegisterVCProjectEngineEvents(p);
                }
            }
        }

        void SolutionEvents_AfterClosing()
        {
            QtProject.ClearInstances();
        }

        /// <summary>
        /// Tries to get a VCProjectEngine from the loaded projects and registers the handlers for VCProjectEngineEvents.
        /// </summary>
        void RegisterVCProjectEngineEvents()
        {
            foreach (EnvDTE.Project project in HelperFunctions.ProjectsInSolution(dte))
                if (project != null && HelperFunctions.IsQt4Project(project))
                    RegisterVCProjectEngineEvents(project);
        }

        /// <summary>
        /// Retrieves the VCProjectEngine from the given project and registers the handlers for VCProjectEngineEvents.
        /// </summary>
        void RegisterVCProjectEngineEvents(Project p)
        {
            if (vcProjectEngineEvents != null)
                return;

            VCProject vcPrj = p.Object as VCProject;
            VCProjectEngine prjEngine = vcPrj.VCProjectEngine as VCProjectEngine;
            if (prjEngine != null)
            {
                vcProjectEngineEvents = prjEngine.Events as VCProjectEngineEvents;
                if (vcProjectEngineEvents != null)
                {
                    try
                    {
                        vcProjectEngineEvents.ItemPropertyChange += new _dispVCProjectEngineEvents_ItemPropertyChangeEventHandler(OnVCProjectEngineItemPropertyChange);
                    }
                    catch
                    {
                        Messages.DisplayErrorMessage("VCProjectEngine events could not be registered.");
                    }
                }
            }
        }

        private void OnVCProjectEngineItemPropertyChange(object item, object tool, int dispid)
        {
            //System.Diagnostics.Debug.WriteLine("OnVCProjectEngineItemPropertyChange " + dispid.ToString());
            VCFileConfiguration vcFileCfg = item as VCFileConfiguration;
            if (vcFileCfg == null)
            {
                // A global or project specific property has changed.

                VCConfiguration vcCfg = item as VCConfiguration;
                if (vcCfg == null)
                    return;
                VCProject vcPrj = vcCfg.project as VCProject;
                if (vcPrj == null)
                    return;
                if (!HelperFunctions.IsQt4Project(vcPrj))
                    return;

                if (dispid == dispId_VCCLCompilerTool_UsePrecompiledHeader
                    || dispid == dispId_VCCLCompilerTool_PrecompiledHeaderThrough
                    || dispid == dispId_VCCLCompilerTool_AdditionalIncludeDirectories
                    || dispid == dispId_VCCLCompilerTool_PreprocessorDefinitions)
                {
                    QtProject qtPrj = QtProject.Create(vcPrj);
                    qtPrj.RefreshMocSteps();
                }
            }
            else
            {
                // A file specific property has changed.

                VCFile vcFile = vcFileCfg.File as VCFile;
                if (vcFile == null)
                    return;
                VCProject vcPrj = vcFile.project as VCProject;
                if (vcPrj == null)
                    return;
                if (!HelperFunctions.IsQt4Project(vcPrj))
                    return;

                if (dispid == dispId_VCFileConfiguration_ExcludedFromBuild)
                {
                    QtProject qtPrj = QtProject.Create(vcPrj);
                    qtPrj.OnExcludedFromBuildChanged(vcFile, vcFileCfg);
                }
                else if (dispid == dispId_VCCLCompilerTool_UsePrecompiledHeader
                    || dispid == dispId_VCCLCompilerTool_PrecompiledHeaderThrough
                    || dispid == dispId_VCCLCompilerTool_AdditionalIncludeDirectories
                    || dispid == dispId_VCCLCompilerTool_PreprocessorDefinitions)
                {
                    QtProject qtPrj = QtProject.Create(vcPrj);
                    qtPrj.RefreshMocStep(vcFile);
                }
            }
        }

        private static VCFile GetVCFileFromProject(string absFileName, VCProject project)
        {
            foreach (VCFile f in (IVCCollection)project.Files)
            {
                if (f.Name.ToLower() == absFileName.ToLower())
                    return f;
            }
            return null;
        }

        /// <summary>
        /// Returns the COM DISPID of the given property.
        /// </summary>
        private static int GetPropertyDispId(Type type, string propertyName)
        {
            System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
            if (pi != null)
            {
                foreach (Attribute attribute in pi.GetCustomAttributes(true))
                {
                    DispIdAttribute dispIdAttribute = attribute as DispIdAttribute;
                    if (dispIdAttribute != null)
                    {
                        return dispIdAttribute.Value;
                    }
                }
            }
            return 0;
        }

    }
}
