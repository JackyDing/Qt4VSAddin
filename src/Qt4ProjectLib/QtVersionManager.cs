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
using Microsoft.Win32;
using System.Collections.Generic;

namespace Digia.Qt4ProjectLib
{
    /// <summary>
    /// Summary description for QtVersionManager.
    /// </summary>
    public class QtVersionManager
    {
        private static QtVersionManager instance = null;
        private string regVersionPath = null;
        private string strVersionKey = null;
        private string strPlatform = "Win32";
        private Hashtable versionCache = null;

        private class QtVersion
        {
            public string name;
            public VersionInformation vi;
        }

        protected QtVersionManager()
        {
            strVersionKey = Resources.registryVersionKey;
            regVersionPath = Resources.registryVersionPath;
        }

        static public QtVersionManager The(string platform = null)
        {
            if (instance == null)
                instance = new QtVersionManager();
            if (platform != null)
                instance.strPlatform = platform;
            return instance;
        }

        public void SetPlatform(string platform)
        {
            strPlatform = platform;
        }

        public VersionInformation GetVersionInfo(string name)
        {
            if (name == null)
                return null;
            if (name == "$(DefaultQtVersion)")
                name = GetDefaultVersion();
            if (versionCache == null)
                versionCache = new Hashtable();

            VersionInformation vi = versionCache[name] as VersionInformation;
            if (vi != null)
                return vi;

            string qtdir = GetInstallPath(name);
            vi = new VersionInformation(qtdir);
            versionCache[name] = vi;
            return vi;
        }

        public VersionInformation GetVersionInfo(EnvDTE.Project project)
        {
            return GetVersionInfo(GetProjectQtVersion(project));
        }

        public void ClearVersionCache()
        {
            if (versionCache != null)
                versionCache.Clear();
        }

        public string[] GetVersions()
        {
            return GetVersions(Registry.CurrentUser);
        }

        public string GetQtVersionFromInstallDir(string qtDir)
        {
            if (qtDir == null)
                return null;

            qtDir = qtDir.ToLower();
            string[] versions = GetVersions();
            foreach (string version in versions)
            {
                string installPath = GetInstallPath(version);
                if (installPath == null)
                    continue;
                if (installPath.ToLower() == qtDir)
                    return version;
            }

            return null;
        }

        public string GetInstallPath(string version)
        {
            if (version == "$(DefaultQtVersion)")
                version = GetDefaultVersion();
            return GetInstallPath(version, Registry.CurrentUser);
        }

        public string GetInstallPath(EnvDTE.Project project)
        {
            string version = GetProjectQtVersion(project);
            if (version == "$(DefaultQtVersion)")
                version = GetDefaultVersion();
            if (version == null)
                return null;
            return GetInstallPath(version);
        }

        public bool SaveVersion(string versionName, string path)
        {
            string verName = versionName.Trim();
            string dir = "";
            if (verName != "$(QTDIR)")
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (verName.Length < 1 || !di.Exists)
                    return false;
                dir = di.FullName;
            }
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + Resources.registryRootPath, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + Resources.registryRootPath);
                if (key == null)
                    return false;
            }
            RegistryKey versionKey = key.CreateSubKey(strVersionKey + "\\" + verName);
            if (versionKey == null)
                return false;
            versionKey.SetValue("InstallDir", dir);
            return true;
        }

        public void RemoveVersion(string versionName)
        {
            if (versionCache != null)
            {
                string name = versionName;
                if (versionName == "$(DefaultQtVersion)")
                    name = GetDefaultVersion();
                versionCache.Remove(name);
            }

            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + regVersionPath, true);
            if (key == null)
                return;
            key.DeleteSubKey(versionName);
        }

        public static bool HasProjectQtVersion(EnvDTE.Project project)
        {
            if (project == null)
                return false;
            string platform = project.ConfigurationManager.ActiveConfiguration.PlatformName;
            if (project.Globals.get_VariablePersists("Qt4Version " + platform)
                || project.Globals.get_VariablePersists("Qt4Version")
                || project.Globals.get_VariablePersists("QtVersion " + platform)
                || project.Globals.get_VariablePersists("QtVersion"))
                return true;
            else
                return false;
        }

        public string GetDefaultVersion()
        {
            if (strPlatform == "Win32")
                return GetDefaultX86Version(Registry.CurrentUser);
            if (strPlatform == "x64")
                return GetDefaultX64Version(Registry.CurrentUser);
            return GetDefaultVersion(Registry.CurrentUser);
        }

        public string GetDefaultX86Version()
        {
            return GetDefaultX86Version(Registry.CurrentUser);
        }

        public string GetDefaultX64Version()
        {
            return GetDefaultX64Version(Registry.CurrentUser);
        }

        public string GetDefaultWinCEVersion()
        {
            string version = null;
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + regVersionPath, false);
                if (key != null)
                    version = (string)key.GetValue("WinCEQt4Version");
                if (version == null)
                    version = (string)key.GetValue("WinCEQtVersion");
            }
            catch
            {
                Messages.DisplayWarningMessage(SR.GetString("QtVersionManager_CannotLoadQtVersion"));
            }

            if (version == null)
                version = GetDefaultVersion();

            return version;
        }

        public string GetProjectQtVersion(EnvDTE.Project project)
        {
            string platformName = null;
            try
            {
                platformName = project.ConfigurationManager.ActiveConfiguration.PlatformName;
            }
            catch
            {
                // Accessing the ActiveConfiguration property throws an exception
                // if there's an "unconfigured" platform in the Solution platform combo box.
                platformName = "Win32";
            }
            string version = GetProjectQtVersion(project, platformName);

            if (version == null && project.Globals.get_VariablePersists("Qt4Version"))
            {
                version = (string)project.Globals["Qt4Version"];
                ExpandEnvironmentVariablesInQtVersion(ref version);
                return VerifyIfQtVersionExists(version) ? version : null;
            }

            if (version == null && project.Globals.get_VariablePersists("QtVersion"))
            {
                version = (string)project.Globals["QtVersion"];
                ExpandEnvironmentVariablesInQtVersion(ref version);
                return VerifyIfQtVersionExists(version) ? version : null;
            }

            if (version == null)
                version = GetSolutionQtVersion(project.DTE.Solution);

            if (version == null)
                version = "$(DefaultQtVersion)";

            return version;
        }

        public string GetProjectQtVersion(EnvDTE.Project project, string platform)
        {
            string key = "QtVersion4 " + platform;
            if (!project.Globals.get_VariablePersists(key))
                return null;
            string version = (string)project.Globals[key];
            ExpandEnvironmentVariablesInQtVersion(ref version);
            return VerifyIfQtVersionExists(version) ? version : null;
        }

        public string GetSolutionQtVersion(EnvDTE.Solution solution)
        {
            if (solution == null)
                return null;

            if (solution.Globals.get_VariableExists("Qt4Version"))
            {
                string version = (string)solution.Globals["Qt4Version"];
                return VerifyIfQtVersionExists(version) ? version : null;
            }

            if (solution.Globals.get_VariableExists("QtVersion"))
            {
                string version = (string)solution.Globals["QtVersion"];
                return VerifyIfQtVersionExists(version) ? version : null;
            }

            return null;
        }

        public bool SaveDefaultVersion(string version)
        {
            if (version == "$(DefaultQtVersion)")
                return false;
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + regVersionPath);
            if (key == null)
                return false;
            key.SetValue("DefaultQtVersion", version);
            return true;
        }

        public bool SaveDefaultX86Version(string version)
        {
            if (version == "$(DefaultQtVersion)")
                return false;
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + regVersionPath);
            if (key == null)
                return false;
            key.SetValue("DefaultQtX86Version", version);
            return true;
        }

        public bool SaveDefaultX64Version(string version)
        {
            if (version == "$(DefaultQtVersion)")
                return false;
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + regVersionPath);
            if (key == null)
                return false;
            key.SetValue("DefaultQtX64Version", version);
            return true;
        }

        public bool SaveDefaultWinCEVersion(string version)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + regVersionPath);
            if (key == null)
                return false;
            key.SetValue("WinCEQt4Version", version);
            key.SetValue("WinCE4Version", version);
            return true;
        }

        public bool SaveProjectQtVersion(EnvDTE.Project project, string version)
        {
            return SaveProjectQtVersion(project, version, project.ConfigurationManager.ActiveConfiguration.PlatformName);
        }

        public bool SaveProjectQtVersion(EnvDTE.Project project, string version, string platform)
        {
            if (!IsVersionAvailable(version) && version != "$(DefaultQtVersion)")
                return false;
            string key = "Qt4Version " + platform;
            if (!project.Globals.get_VariableExists(key) || project.Globals[key].ToString() != version)
                project.Globals[key] = version;
            if (!project.Globals.get_VariablePersists(key))
                project.Globals.set_VariablePersists(key, true);
            key = "QtVersion " + platform;
            if (!project.Globals.get_VariableExists(key) || project.Globals[key].ToString() != version)
                project.Globals[key] = version;
            if (!project.Globals.get_VariablePersists(key))
                project.Globals.set_VariablePersists(key, true);
            return true;
        }

        public bool SaveSolutionQtVersion(EnvDTE.Solution solution, string version)
        {
            if (!IsVersionAvailable(version) && version != "$(DefaultQtVersion)")
                return false;
            solution.Globals["Qt4Version"] = version;
            if (!solution.Globals.get_VariablePersists("Qt4Version"))
                solution.Globals.set_VariablePersists("Qt4Version", true);
            solution.Globals["QtVersion"] = version;
            if (!solution.Globals.get_VariablePersists("QtVersion"))
                solution.Globals.set_VariablePersists("QtVersion", true);
            return true;
        }

        /// <summary>
        /// Check if all Qt versions are valid and readable.
        /// </summary>
        /// Also sets the default Qt version to the newest version, if needed.
        /// <param name="errorMessage"></param>
        /// <returns>true, if we found an invalid version</returns>
        public bool HasInvalidVersions(out string errorMessage)
        {
            List<QtVersion> validVersions = new List<QtVersion>();
            List<string> invalidVersions = new List<string>();

            foreach (string v in GetVersions())
            {
                if (v == "$(DefaultQtVersion)")
                    continue;
                try
                {
                    VersionInformation vi = new VersionInformation(GetInstallPath(v));
                    QtVersion qtVersion = new QtVersion();
                    qtVersion.name = v;
                    qtVersion.vi = vi;
                    validVersions.Add(qtVersion);
                }
                catch (Exception)
                {
                    invalidVersions.Add(v);
                }
            }

            if (invalidVersions.Count > 0)
            {
                errorMessage = "These Qt version are inaccessible:\n";
                foreach (string invalidVersion in invalidVersions)
                    errorMessage += invalidVersion + " in " + GetInstallPath(invalidVersion) + "\n";
                errorMessage += "Make sure that you have read access to all files in your Qt directories.";

                // Is the default Qt version invalid?
                bool isDefaultQtVersionInvalid = false;
                string defaultQtVersionName = GetDefaultVersion();
                if (String.IsNullOrEmpty(defaultQtVersionName))
                {
                    isDefaultQtVersionInvalid = true;
                }
                else
                {
                    foreach (string name in invalidVersions)
                    {
                        if (name == defaultQtVersionName)
                        {
                            isDefaultQtVersionInvalid = true;
                            break;
                        }
                    }
                }

                // find the newest valid Qt version that can be used as default version
                if (isDefaultQtVersionInvalid && validVersions.Count > 0)
                {
                    QtVersion defaultQtVersion = null;
                    foreach (QtVersion v in validVersions)
                    {
                        if (v.vi.IsWinCEVersion())
                            continue;
                        if (defaultQtVersion == null)
                        {
                            defaultQtVersion = v;
                            continue;
                        }
                        if (defaultQtVersion.vi.qtMajor < v.vi.qtMajor ||
                               (defaultQtVersion.vi.qtMajor == v.vi.qtMajor && (defaultQtVersion.vi.qtMinor < v.vi.qtMinor ||
                                   (defaultQtVersion.vi.qtMinor == v.vi.qtMinor && defaultQtVersion.vi.qtPatch < v.vi.qtPatch))))
                        {
                            defaultQtVersion = v;
                        }
                    }
                    if (defaultQtVersion != null)
                        SaveDefaultVersion(defaultQtVersion.name);
                }

                return true;
            }
            errorMessage = null;
            return false;
        }

        public string[] GetVersions(RegistryKey root)
        {
            RegistryKey key = root.OpenSubKey("SOFTWARE\\" + Resources.registryRootPath, false);
            if (key == null)
                return new string[] {};
            RegistryKey versionKey = key.OpenSubKey(strVersionKey, false);
            if (versionKey == null)
                return new string[] {};
            return versionKey.GetSubKeyNames();
        }

        public string GetInstallPath(string version, RegistryKey root)
        {
            if (version == "$(DefaultQtVersion)")
                version = GetDefaultVersion(root);
            if (version == "$(QTDIR)")
                return System.Environment.GetEnvironmentVariable("QTDIR");

            RegistryKey key = root.OpenSubKey("SOFTWARE\\" + Resources.registryRootPath, false);
            if (key == null)
                return null;
            RegistryKey versionKey = key.OpenSubKey(strVersionKey + "\\" + version, false);
            if (versionKey == null)
                return null;
            return (string)versionKey.GetValue("InstallDir");
        }

        private string GetDefaultVersion(RegistryKey root)
        {
            string defaultVersion = null;
            try
            {
                RegistryKey key = root.OpenSubKey("SOFTWARE\\" + regVersionPath, false);
                if (key != null)
                    defaultVersion = (string)key.GetValue("DefaultQtVersion");
            }
            catch
            {
                Messages.DisplayWarningMessage(SR.GetString("QtVersionManager_CannotLoadQtVersion"));
            }

            if (defaultVersion == null)
            {
                MergeVersions();
                RegistryKey key = root.OpenSubKey("SOFTWARE\\" + regVersionPath, false);
                if (key != null)
                {
                    string[] versions = GetVersions();
                    if (versions != null && versions.Length > 0)
                        defaultVersion = versions[versions.Length - 1];
                    if (defaultVersion != null)
                        SaveDefaultVersion(defaultVersion);
                }
                if (defaultVersion == null)
                {
                    // last fallback... try QTDIR
                    string qtDir = System.Environment.GetEnvironmentVariable("QTDIR");
                    if (qtDir == null)
                        return null;
                    DirectoryInfo d = new DirectoryInfo(qtDir);
                    SaveVersion(d.Name, d.FullName);
                    if (SaveDefaultVersion(d.Name))
                        defaultVersion = d.Name;
                }
            }
            return VerifyIfQtVersionExists(defaultVersion) ? defaultVersion : null;
        }

        private string GetDefaultX86Version(RegistryKey root)
        {
            string defaultVersion = null;
            try
            {
                RegistryKey key = root.OpenSubKey("SOFTWARE\\" + regVersionPath, false);
                if (key != null)
                    defaultVersion = (string)key.GetValue("DefaultQtX86Version");
                if (defaultVersion == null)
                    defaultVersion = (string)key.GetValue("DefaultQtVersion");
            }
            catch
            {
                Messages.DisplayWarningMessage(SR.GetString("QtVersionManager_CannotLoadQtVersion"));
            }

            if (defaultVersion == null)
            {
                MergeVersions();
                RegistryKey key = root.OpenSubKey("SOFTWARE\\" + regVersionPath, false);
                if (key != null)
                {
                    string[] versions = GetVersions();
                    if (versions != null && versions.Length > 0)
                        defaultVersion = versions[versions.Length - 1];
                    if (defaultVersion != null)
                        SaveDefaultX86Version(defaultVersion);
                }
                if (defaultVersion == null)
                {
                    // last fallback... try QTDIR
                    string qtDir = System.Environment.GetEnvironmentVariable("QTDIR");
                    if (qtDir == null)
                        return null;
                    DirectoryInfo d = new DirectoryInfo(qtDir);
                    SaveVersion(d.Name, d.FullName);
                    if (SaveDefaultX86Version(d.Name))
                        defaultVersion = d.Name;
                }
            }
            return VerifyIfQtVersionExists(defaultVersion) ? defaultVersion : null;
        }

        private string GetDefaultX64Version(RegistryKey root)
        {
            string defaultVersion = null;
            try
            {
                RegistryKey key = root.OpenSubKey("SOFTWARE\\" + regVersionPath, false);
                if (key != null)
                    defaultVersion = (string)key.GetValue("DefaultQtX64Version");
                if (defaultVersion == null)
                    defaultVersion = (string)key.GetValue("DefaultQtVersion");
            }
            catch
            {
                Messages.DisplayWarningMessage(SR.GetString("QtVersionManager_CannotLoadQtVersion"));
            }

            if (defaultVersion == null)
            {
                MergeVersions();
                RegistryKey key = root.OpenSubKey("SOFTWARE\\" + regVersionPath, false);
                if (key != null)
                {
                    string[] versions = GetVersions();
                    if (versions != null && versions.Length > 0)
                        defaultVersion = versions[versions.Length - 1];
                    if (defaultVersion != null)
                        SaveDefaultX86Version(defaultVersion);
                }
                if (defaultVersion == null)
                {
                    // last fallback... try QTDIR
                    string qtDir = System.Environment.GetEnvironmentVariable("QTDIR");
                    if (qtDir == null)
                        return null;
                    DirectoryInfo d = new DirectoryInfo(qtDir);
                    SaveVersion(d.Name, d.FullName);
                    if (SaveDefaultX86Version(d.Name))
                        defaultVersion = d.Name;
                }
            }
            return VerifyIfQtVersionExists(defaultVersion) ? defaultVersion : null;
        }

        private void MergeVersions()
        {
            string[] hkcuVersions = GetVersions();
            string[] hklmVersions = GetVersions(Registry.LocalMachine);

            string[] hkcuInstDirs = new string[hkcuVersions.Length];
            for (int i=0; i<hkcuVersions.Length; ++i)
                hkcuInstDirs[i] = GetInstallPath(hkcuVersions[i]);
            string[] hklmInstDirs = new string[hklmVersions.Length];
            for (int i=0; i<hklmVersions.Length; ++i)
                hklmInstDirs[i] = GetInstallPath(hklmVersions[i], Registry.LocalMachine);
            
            for (int i=0; i<hklmVersions.Length; ++i)
            {
                if (hklmInstDirs[i] == null)
                    continue;

                bool found = false;
                for (int j=0; j<hkcuInstDirs.Length; ++j)
                {
                    if (hkcuInstDirs[j] != null
                        && hkcuInstDirs[j].ToLower() == hklmInstDirs[i].ToLower())
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    for (int j=0; j<hkcuVersions.Length; ++j)
                    {
                        if (hkcuVersions[j] != null
                            && hkcuVersions[j] == hklmVersions[i])
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) 
                        SaveVersion(hklmVersions[i], hklmInstDirs[i]);                    
                }
            }
        }

        private bool IsVersionAvailable(string version)
        {
            bool versionAvailable = false;
            string[] versions = GetVersions();
            foreach (string ver in versions)
            {
                if (version == ver)
                {
                    versionAvailable = true;
                    break;
                }
            }
            return versionAvailable;
        }

        private bool VerifyIfQtVersionExists(string version)
        {
            if (version == "$(DefaultQtVersion)")
                version = GetDefaultVersion();
            if (version != null && version.Length > 0) {
                System.Text.RegularExpressions.Regex regExp =
                    new System.Text.RegularExpressions.Regex("\\$\\(.*\\)");
                if (regExp.IsMatch(version))
                    return true;
                return Directory.Exists(GetInstallPath(version));
            }

            return false;
        }

        private static void ExpandEnvironmentVariablesInQtVersion(ref string version)
        {
            if (version != "$(QTDIR)" && version != "$(DefaultQtVersion)")
            {
                // Make it possible to specify the version name
                // via an environment variable
                System.Text.RegularExpressions.Regex regExp =
                    new System.Text.RegularExpressions.Regex("\\$\\((?<VarName>\\S+)\\)");
                System.Text.RegularExpressions.Match match = regExp.Match(version);
                if (match.Success)
                {
                    string env = match.Groups["VarName"].Value;
                    version = System.Environment.GetEnvironmentVariable(env);
                }
            }
        }
    }
}
