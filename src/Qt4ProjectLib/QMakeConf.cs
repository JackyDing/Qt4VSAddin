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
using System.Collections;
using System.IO;
using System.Threading;

namespace Digia.Qt4ProjectLib
{
    public class QMakeConf
    {
        protected Hashtable entries = new Hashtable();
        private FileInfo fileInfo = null;
        private string qmakespecFolder = "";

        public QMakeConf(VersionInformation versionInfo)
        {
            Init(versionInfo);
        }

        public enum InitType { InitQtInstallPath, InitQMakeConf }

        /// <param name="str">string for initialization</param>
        /// <param name="itype">determines the use of str</param>
        public QMakeConf(string str, InitType itype)
        {
            switch (itype)
            {
                case InitType.InitQtInstallPath:
                    Init(new VersionInformation(str));
                    break;
                case InitType.InitQMakeConf:
                    Init(str);
                    break;
            }
        }

        protected void Init(VersionInformation versionInfo)
        {
            string filename = versionInfo.qtDir + "\\mkspecs\\default\\qmake.conf";
            fileInfo = new FileInfo(filename);

            // Starting from Qt5 beta2 there is no more "\\mkspecs\\default" folder available
            // To find location of "qmake.conf" there is a need to run "qmake -query" command
            // This is what happens below.
            if (!fileInfo.Exists)
            {
                QMakeQuery qmakeQuery = new QMakeQuery(versionInfo);

                qmakeQuery.ReadyEvent += new QMakeQuery.EventHandler(this.CloseEventHandler);
                System.Threading.Thread qmakeThread = new System.Threading.Thread(new ThreadStart(qmakeQuery.RunQMakeQuery));
                qmakeThread.Start();
                qmakeThread.Join();

                if (qmakeQuery.ErrorValue != 0)
                {
                    throw new Qt4VSException("qmake.conf not found");
                }

                if (qmakespecFolder.Length > 0)
                {
                    filename = versionInfo.qtDir + "\\mkspecs\\" + qmakespecFolder + "\\qmake.conf";
                }
            }

            Init(filename);
        }

        // Qmake thread calls this handler to set qmake.conf folder
        private void CloseEventHandler(string foldername)
        {
            qmakespecFolder = foldername;
        }

        protected void Init(string filename)
        {
            fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                StreamReader streamReader = new StreamReader(filename);
                string line = streamReader.ReadLine();
                while (line != null)
                {
                    ParseLine(line);
                    line = streamReader.ReadLine();
                }
                streamReader.Close();
            }
        }

        public string Get(string key)
        {
            return (string)entries[key];
        }

        private void ParseLine(string line)
        {
            line = line.Trim();
            int commentStartIndex = line.IndexOf('#');
            if (commentStartIndex >= 0)
                line = line.Remove(commentStartIndex);
            int pos = line.IndexOf('=');
            if (pos > 0)
            {
                string op = "=";
                if (line[pos - 1] == '+' || line[pos - 1] == '-')
                    op = line[pos - 1] + "=";

                string lineKey;
                string lineValue;
                lineKey = line.Substring(0, pos - op.Length + 1).Trim();
                lineValue = ExpandVariables(line.Substring(pos + 1).Trim());

                if (op == "+=")
                {
                    entries[lineKey] += " " + lineValue;
                }
                else if (op == "-=")
                {
                    foreach (string remval in lineValue.Split(new char[] { ' ', '\t' }))
                        RemoveValue(lineKey, remval);
                }
                else
                    entries[lineKey] = lineValue;
            }
            else if (line.StartsWith("include"))
            {
                pos = line.IndexOf('(');
                int posEnd = line.LastIndexOf(')');
                if (pos > 0 && pos < posEnd)
                {
                    string filenameToInclude = line.Substring(pos + 1, posEnd - pos - 1);
                    string saveCurrentDir = Environment.CurrentDirectory;
                    Environment.CurrentDirectory = fileInfo.Directory.FullName;
                    FileInfo fileInfoToInclude = new FileInfo(filenameToInclude);
                    if (fileInfoToInclude.Exists)
                    {
                        QMakeConf includeConf = new QMakeConf(fileInfoToInclude.FullName, InitType.InitQMakeConf);
                        foreach (string key in includeConf.entries.Keys)
                        {
                            if (entries.ContainsKey(key))
                                entries[key] += includeConf.entries[key].ToString();
                            else
                                entries[key] = includeConf.entries[key].ToString();
                        }
                    }
                    Environment.CurrentDirectory = saveCurrentDir;
                }
            }
        }

        private string ExpandVariables(string value)
        {
            int pos = value.IndexOf("$$");
            while (pos != -1)
            {
                int startPos = pos + 2;
                int endPos = startPos;

                if (value[startPos] != '[')  // at the moment no handling of qmake internal variables
                {
                    for (; endPos < value.Length; ++endPos)
                    {
                        if ((Char.IsPunctuation(value[endPos]) && value[endPos] != '_')
                            || Char.IsWhiteSpace(value[endPos]))
                        {
                            break;
                        }
                    }
                    if (endPos > startPos)
                    {
                        string varName = value.Substring(startPos, endPos - startPos);
                        object varValueObj = entries[varName];
                        string varValue = "";
                        if (varValueObj != null) varValue = varValueObj.ToString();
                        value = value.Substring(0, pos) + varValue + value.Substring(endPos);
                        endPos = pos + varValue.Length;
                    }
                }

                pos = value.IndexOf("$$", endPos);
            }
            return value;
        }

        private void RemoveValue(string key, string valueToRemove)
        {
            int pos;
            if (entries.Contains(key))
            {
                string value = entries[key].ToString();
                do
                {
                    pos = value.IndexOf(valueToRemove);
                    if (pos >= 0)
                        value = value.Remove(pos, valueToRemove.Length);
                } while (pos >= 0);
                entries[key] = value;
            }
        }
    }
}
