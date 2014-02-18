TEMPLATE = app
QT += gui

include(./shared/qrceditor.pri)
SOURCES += main.cpp mainwindow.cpp
HEADERS += mainwindow.h

win32 {
    RC_FILE = q4rceditor.rc
}

CONFIG(debug, debug|release) {
    DESTDIR  = ../../bin/Debug
}

CONFIG(release, debug|release) {
    DESTDIR  = ../../bin/Release
}

TARGET = q4rceditor
