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

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

namespace Digia.Qt4ProjectLib
{
    #region QMake Process

    class InfoDialog : Form
    {
        private Label label1 = null;
        private IContainer components = null;
        private ProgressBar progressBar1 = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public InfoDialog(string name)
        {
            this.label1 = new Label();
            this.components = new Container();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(370, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = SR.GetString("QMakeProcess_OpenSolutionFromFile") + name;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 28);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(369, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 1;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(394, 67);
            this.MinimumSize = new System.Drawing.Size(402, 94);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = Resources.msgBoxCaption;
            this.StartPosition = FormStartPosition.CenterParent;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void CloseEventHandler()
        {
            this.Close();
        }
    }

    class QMake
    {
        public delegate void ProcessEventHandler();
        public event ProcessEventHandler CloseEvent;

        public delegate void ProcessEventHandlerArg(string data);
        public event ProcessEventHandlerArg PaneMessageDataEvent;

        private string file = null;
        protected int errorValue = 0;
        private EnvDTE.DTE dteObject;
        private bool recursive = false;
        protected static Process qmakeProcess = null;
        protected VersionInformation qtVersionInformation;

        protected static int stdOutputLines = 0;
        protected static int errOutputLines = 0;
        protected static StringBuilder stdOutput = null;
        protected static StringBuilder errOutput = null;
        
        public int ErrorValue
        {
            get { return errorValue; }
        }

        public QMake(EnvDTE.DTE dte, string fileName, bool recursiveRun, VersionInformation vi)
        {
            dteObject = dte;
            file = fileName;
            recursive = recursiveRun;
            qtVersionInformation = vi;
        }

        public void RunQMake()
        {
            FileInfo fi = new FileInfo(file);
            string vcproj = HelperFunctions.RemoveFileNameExtension(fi);
#if (VS2010 || VS2012 || VS2013)
            vcproj += ".vcxproj";
#else
            vcproj += ".vcproj";
#endif

            string qmakeArgs = "-tp vc \"" + fi.Name + "\" ";

            if (recursive)
                qmakeArgs += "-recursive";
            else
                qmakeArgs += "-o \"" + vcproj + "\"";

            qmakeArgs += @" QMAKE_INCDIR_QT=$(QTDIR)\include ";
            qmakeArgs += @"QMAKE_LIBDIR=$(QTDIR)\lib "
                       + @"QMAKE_MOC=$(QTDIR)\bin\moc.exe "
                       + @"QMAKE_QMAKE=$(QTDIR)\bin\qmake.exe";

            CreateQmakeProcess(qmakeArgs, qtVersionInformation.qtDir + "\\bin\\qmake", fi.DirectoryName);

            // We must set the QTDIR environment variable, because we're clearing QMAKE_LIBDIR_QT above.
            // If we do not set this, the Qt libraries will be QtCored.lib instead of QtCore4d.lib even
            // for shared builds.
            qmakeProcess.StartInfo.EnvironmentVariables["QTDIR"] = qtVersionInformation.qtDir;

            // determine which vs version we are currently using and inform qmake about it
            string regPath = dteObject.Application.RegistryRoot + "\\Setup\\VC";
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regPath);
            if (key != null)
            {
                string keyValue = key.GetValue("ProductDir", (object)"").ToString();
                string envVar = qmakeProcess.StartInfo.EnvironmentVariables["path"];
                if (envVar != null)
                {
                    string value = envVar + ";" + keyValue;
                    qmakeProcess.StartInfo.EnvironmentVariables["path"] = value;
                }
                else
                    qmakeProcess.StartInfo.EnvironmentVariables.Add("path", keyValue);
            }

            try
            {
                InvokeExternalTarget(PaneMessageDataEvent, "--- (qmake) : Using: " + qmakeProcess.StartInfo.FileName);
                InvokeExternalTarget(PaneMessageDataEvent, "--- (qmake) : Working Directory: " + qmakeProcess.StartInfo.WorkingDirectory);
                InvokeExternalTarget(PaneMessageDataEvent, "--- (qmake) : Arguments: "
                    + qmakeProcess.StartInfo.Arguments
                    + Environment.NewLine);

                if (qmakeProcess.Start())
                {
                    errOutput = new StringBuilder();
                    errOutputLines = 0;
                    stdOutput = new StringBuilder();
                    stdOutputLines = 0;
                    Thread errorThread = new Thread(new ThreadStart(ReadStandardError));
                    Thread outputThread = new Thread(new ThreadStart(ReadStandardOutput));
                    errorThread.Start();
                    outputThread.Start();

                    qmakeProcess.WaitForExit();

                    errorThread.Join();
                    outputThread.Join();

                    errorValue = qmakeProcess.ExitCode;

                    if (stdOutputLines > 0)
                    {
                        InvokeExternalTarget(PaneMessageDataEvent, stdOutput.ToString());
                        InvokeExternalTarget(PaneMessageDataEvent, "--- (Import): Success: " + stdOutputLines.ToString());
                    }

                    if (errOutputLines > 0)
                    {
                        InvokeExternalTarget(PaneMessageDataEvent, errOutput.ToString());
                        InvokeExternalTarget(PaneMessageDataEvent, "--- (Import): Error(s): " + errOutputLines.ToString());
                    }
                }

                InvokeExternalTarget(PaneMessageDataEvent, "--- (qmake) : Exit Code: " + errorValue + Environment.NewLine);
                qmakeProcess.Close();                
            }
            catch (Exception e)
            {
                qmakeProcess = null;
                InvokeExternalTarget(PaneMessageDataEvent, e.Message);
                errorValue = -1;
            }
            finally
            {
                InvokeExternalTarget(CloseEvent);
                Messages.ActivateMessagePane();
            }
        }

        protected void CreateQmakeProcess(string qmakeArgs, string filename, string workingDir)
        {
            qmakeProcess = new System.Diagnostics.Process();
            qmakeProcess.StartInfo.CreateNoWindow = true;
            qmakeProcess.StartInfo.UseShellExecute = false;
            qmakeProcess.StartInfo.RedirectStandardError = true;
            qmakeProcess.StartInfo.RedirectStandardOutput = true;
            qmakeProcess.StartInfo.Arguments = qmakeArgs;
            qmakeProcess.StartInfo.FileName = filename;
            qmakeProcess.StartInfo.WorkingDirectory = workingDir;
        }

        protected static void InvokeExternalTarget(Delegate dlg, params object[] objList)
        {
            try
            {
                // make sure there are delegates assigned...
                Delegate[] invocationList = dlg.GetInvocationList();
                if (invocationList == null)
                    return;

                // we can only call one delegate at the time...
                foreach (Delegate singleDelegate in invocationList)
                {
                    ISynchronizeInvoke synchronizeTarget = singleDelegate.Target as ISynchronizeInvoke;
                    if (synchronizeTarget == null)
                        singleDelegate.DynamicInvoke(objList);
                    else
                        synchronizeTarget.BeginInvoke(singleDelegate, objList);
                }
            }
            catch { }
        }

        protected void ReadStandardError()
        {
            if (qmakeProcess == null)
                return;

            string error;
            while ((error = qmakeProcess.StandardError.ReadLine()) != null)
            {
                errOutputLines++;
                errOutput.Append("[" + errOutputLines.ToString() + "] - " + error.Trim() + "\n");
            }
        }

        protected void ReadStandardOutput()
        {
            if (qmakeProcess == null)
                return;

            string output;
            while ((output = qmakeProcess.StandardOutput.ReadLine()) != null)
            {
                stdOutputLines++;
                stdOutput.Append("[" + stdOutputLines.ToString() + "] - " + output.Trim() + "\n");
            }
        }
    }

    class QMakeQuery : QMake
    {
        public delegate void EventHandler(string param);
        public event EventHandler ReadyEvent;

        public QMakeQuery(VersionInformation vi)
            : base(null, "", false, vi)
        {
            qtVersionInformation = vi;
        }

        public void RunQMakeQuery()
        {
            string result = "";
            CreateQmakeProcess("-query QMAKE_XSPEC", qtVersionInformation.qtDir + "\\bin\\qmake", qtVersionInformation.qtDir);

            try
            {
                if (qmakeProcess.Start())
                {
                    errOutput = new StringBuilder();
                    errOutputLines = 0;
                    stdOutput = new StringBuilder();
                    stdOutputLines = 0;
                    Thread errorThread = new Thread(new ThreadStart(ReadStandardError));
                    Thread outputThread = new Thread(new ThreadStart(ReadStandardOutput));
                    errorThread.Start();
                    outputThread.Start();

                    qmakeProcess.WaitForExit();

                    errorThread.Join();
                    outputThread.Join();

                    errorValue = qmakeProcess.ExitCode;

                    if (stdOutputLines > 0)
                    {
                        result = stdOutput.ToString();
                        if (result.IndexOf("win32") > 0)
                        {
                            result = result.Substring(result.IndexOf("win32")).Trim();
                        }
                        else if (result.IndexOf("wince") > 0)
                        {
                            result = result.Substring(result.IndexOf("wince")).Trim();
                        }
                    }
                }

                qmakeProcess.Close();
            }
            catch (Exception)
            {
                qmakeProcess = null;
                errorValue = -1;
            }
            finally
            {
                InvokeExternalTarget(ReadyEvent, result);
            }
        }
    }

    #endregion
}