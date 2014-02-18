/**************************************************************************
**
** This file is part of the Qt VS Add-in
**
** Copyright (c) 2011 Nokia Corporation and/or its subsidiary(-ies).
**
** Contact: Nokia Corporation (qt-info@nokia.com)
**
** Commercial Usage
**
** Licensees holding valid Qt Commercial licenses may use this file in
** accordance with the Qt Commercial License Agreement provided with the
** Software or, alternatively, in accordance with the terms contained in
** a written agreement between you and Nokia.
**
** GNU Lesser General Public License Usage
**
** Alternatively, this file may be used under the terms of the GNU Lesser
** General Public License version 2.1 as published by the Free Software
** Foundation and appearing in the file LICENSE.LGPL included in the
** packaging of this file.  Please review the following information to
** ensure the GNU Lesser General Public License version 2.1 requirements
** will be met: http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html.
**
** If you are unsure which license is appropriate for your use, please
** contact the sales department at http://qt.nokia.com/contact.
**
**************************************************************************/

#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QString>
#include <QCloseEvent>

namespace SharedTools {
    class QrcEditor;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    MainWindow();
	void openFile(QString fileName);

protected:
	void closeEvent(QCloseEvent *e);

private slots:
    void slotOpen();
    void slotSave();
    void slotAbout();
    void slotAboutQt();

private:
    int fileChangedDialog();
    void sendFileNameToQtAppWrapper();

private:
    SharedTools::QrcEditor *m_qrcEditor;
    QString                 m_qtAppWrapperPath;
    QString                 m_devenvPIDArg;
};

#endif // MAINWINDOW_H
