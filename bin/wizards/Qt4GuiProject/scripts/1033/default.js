var QtEngine;

function GetNameFromFile(strFile)
{
    var nPos = strFile.lastIndexOf(".");
    return strFile.substr(0,nPos);
}

function OnFinish(selProj, selObj)
{
    try
    {
        // load right project engine
        var dte = wizard.dte;
        var version = dte.version;
        if (version == "8.0")
            QtEngine = new ActiveXObject("Digia.Qt4ProjectEngine80");
    	else if (version == "9.0")
            QtEngine = new ActiveXObject("Digia.Qt4ProjectEngine90");
    	else if (version == "10.0")
            QtEngine = new ActiveXObject("Digia.Qt4ProjectEngine100");
        else if (version == "11.0")
            QtEngine = new ActiveXObject("Digia.Qt4ProjectEngine110");
        else if (version == "12.0")
            QtEngine = new ActiveXObject("Digia.Qt4ProjectEngine120");
        else {
            wizard.ReportError("Cannot instantiate QtProjectEngine object!");
            return false;
        }

        var strProjectPath = wizard.FindSymbol('PROJECT_PATH');
        var strProjectName = wizard.FindSymbol('PROJECT_NAME');
        var strSolutionName = wizard.FindSymbol('VS_SOLUTION_NAME');
        var strTemplatePath = wizard.FindSymbol('TEMPLATES_PATH') + "\\";
        var bExclusive = wizard.FindSymbol("CLOSE_SOLUTION");

        var vcfileTmp;
        var fileTmp;
        var strClass = wizard.FindSymbol('CLASSNAME_TEXT');
        var strHeader = wizard.FindSymbol('HFILE_TEXT');
        var strSource = wizard.FindSymbol('CPPFILE_TEXT');
        var strForm = wizard.FindSymbol('UIFILE_TEXT');
        var strQrc = wizard.FindSymbol('QRCFILE_TEXT');
        var baseClassID = wizard.FindSymbol('BASECLASS_COMBO');
        var appIcon = wizard.FindSymbol('APP_ICON');
        var bPrecompiled = wizard.FindSymbol('PRECOMPILED_HEADERS');
        var baseClass = "QMainWindow";

        if (baseClassID == 2)
            baseClass = "QWidget";
        else if (baseClassID == 3)
            baseClass = "QDialog";
        else {
            baseClassID = 1;
            baseClass = "QMainWindow";
        }

        var regexp = /\W/g;
        var strDef = strHeader.toUpperCase().replace(regexp,"_");

        var strFormName = GetNameFromFile(strForm);

        QtEngine.CreateApplicationProject(wizard.dte, strProjectName,
            strProjectPath, strSolutionName, bExclusive, bPrecompiled);

        // add the selected modules to the project
        AddModules();

	var strHeaderInclude = strHeader;
        if (bPrecompiled) {
            strHeaderInclude = "stdafx.h\"\n#include \"" + strHeader;
            fileTmp = QtEngine.CopyFileToProjectFolder(strTemplatePath + "stdafx.cpp", "stdafx.cpp");
            QtEngine.AddFileToProject(fileTmp, "QT_SOURCE_FILTER");

            fileTmp = QtEngine.CopyFileToProjectFolder(strTemplatePath + "stdafx.h", "stdafx.h");
            QtEngine.AddFileToProject(fileTmp, "QT_HEADER_FILTER");
        }

        // main.cpp
        fileTmp = QtEngine.CopyFileToProjectFolder(strTemplatePath + "main.cpp", "main.cpp");
        QtEngine.ReplaceTokenInFile(fileTmp, "%INCLUDE%", strHeaderInclude);
        QtEngine.ReplaceTokenInFile(fileTmp, "%CLASS%", strClass);
        QtEngine.AddFileToProject(fileTmp, "QT_SOURCE_FILTER");

        // mywidget.cpp
        fileTmp = QtEngine.CopyFileToProjectFolder(strTemplatePath + "mywidget.cpp", strSource);
        QtEngine.ReplaceTokenInFile(fileTmp, "%INCLUDE%", strHeaderInclude);
        QtEngine.ReplaceTokenInFile(fileTmp, "%CLASS%", strClass);
        QtEngine.ReplaceTokenInFile(fileTmp, "%BASECLASS%", baseClass);
        QtEngine.AddFileToProject(fileTmp, "QT_SOURCE_FILTER");

        // mywidget.h
        fileTmp = QtEngine.CopyFileToProjectFolder(strTemplatePath + "mywidget.h", strHeader);
        QtEngine.ReplaceTokenInFile(fileTmp, "%PRE_DEF%", strDef);
        QtEngine.ReplaceTokenInFile(fileTmp, "%CLASS%", strClass);
        QtEngine.ReplaceTokenInFile(fileTmp, "%UI_HDR%", "ui_" + strFormName + ".h");
        QtEngine.ReplaceTokenInFile(fileTmp, "%BASECLASS%", baseClass);
        vcfileTmp = QtEngine.AddFileToProject(fileTmp, "QT_HEADER_FILTER");

        // widget.ui
        fileTmp = QtEngine.CopyFileToProjectFolder(strTemplatePath + "widget.ui", strForm);
        QtEngine.ReplaceTokenInFile(fileTmp, "%CLASS%", strClass);
        QtEngine.ReplaceTokenInFile(fileTmp, "%BASECLASS%", baseClass);
        QtEngine.ReplaceTokenInFile(fileTmp, "%QRC%", strQrc);
        if (baseClassID == 1) {
            QtEngine.ReplaceTokenInFile(fileTmp, "%CENTRAL_WIDGET%",
                "\r\n  <widget class=\"QMenuBar\" name=\"menuBar\" />" +
                "\r\n  <widget class=\"QToolBar\" name=\"mainToolBar\" />" +
                "\r\n  <widget class=\"QWidget\" name=\"centralWidget\" />" +
                "\r\n  <widget class=\"QStatusBar\" name=\"statusBar\" />");
        } else {
            QtEngine.ReplaceTokenInFile(fileTmp, "%CENTRAL_WIDGET%", "");
        }
        vcfileTmp = QtEngine.AddFileToProject(fileTmp, "QT_FORM_FILTER");
        
        fileTmp = QtEngine.CreateQrcFile(strClass, strQrc);
        vcfileTmp = QtEngine.AddFileToProject(fileTmp, "QT_RESOURCE_FILTER");
        
        if (appIcon == 1) {
            QtEngine.AddApplicationIcon(strTemplatePath + "winapp.ico");
        }

        QtEngine.Finish();
    }
    catch(e)
    {
        if (e.description.length != 0)
            SetErrorInfo(e);
        return e.number
    }
}

function AddModules()
{
    if (wizard.FindSymbol('CORE_MODULE'))
        QtEngine.AddModule("QtCore");
    if (wizard.FindSymbol('GUI_MODULE'))
        QtEngine.AddModule("QtGui");
    if (wizard.FindSymbol('MULTIMEDIA_MODULE'))
        QtEngine.AddModule("QtMultimedia");
    if (wizard.FindSymbol('XML_MODULE'))
        QtEngine.AddModule("QtXml");
    if (wizard.FindSymbol('SQL_MODULE'))
        QtEngine.AddModule("QtSql");
    if (wizard.FindSymbol('OPENGL_MODULE'))
        QtEngine.AddModule("QtOpenGL");
    if (wizard.FindSymbol('NETWORK_MODULE'))
        QtEngine.AddModule("QtNetwork");
    if (wizard.FindSymbol('SCRIPT_MODULE'))
        QtEngine.AddModule("QtScript");
    if (wizard.FindSymbol('COMPAT_MODULE'))
        QtEngine.AddModule("Qt3Support");
    if (wizard.FindSymbol('AQSERVER_MODULE'))
        QtEngine.AddModule("QAxServer");
    if (wizard.FindSymbol('AQCONTAINER_MODULE'))
        QtEngine.AddModule("QAxContainer");
    if (wizard.FindSymbol('SVG_MODULE'))
        QtEngine.AddModule("QtSvg");
    if (wizard.FindSymbol('HELP_MODULE'))
        QtEngine.AddModule("QtHelp");
    if (wizard.FindSymbol('WEBKIT_MODULE'))
        QtEngine.AddModule("QtWebKit");
    if (wizard.FindSymbol('XMLPATTERNS_MODULE'))
        QtEngine.AddModule("QtXmlPatterns");
    if (wizard.FindSymbol('TEST_MODULE'))
        QtEngine.AddModule("QtTest");
    if (wizard.FindSymbol('DECLARATIVE_MODULE'))
        QtEngine.AddModule("QtDeclarative");
    if (wizard.FindSymbol('PHONON_MODULE'))
        QtEngine.AddModule("phonon");
}
