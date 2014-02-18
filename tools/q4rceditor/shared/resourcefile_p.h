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

#ifndef RESOURCEFILE_P_H
#define RESOURCEFILE_P_H

#include <QtCore/QAbstractItemModel>
#include <QtCore/QMap>
#include <QtCore/QString>
#include <QtCore/QStringList>
#include <QtGui/QIcon>

QT_BEGIN_NAMESPACE

namespace qdesigner_internal {

struct File;
struct Prefix;

/*!
    \class Node

    Forms the base class for nodes in a \l ResourceFile tree.
*/
class Node
{
protected:
    Node(File *file, Prefix *prefix) : m_file(file), m_prefix(prefix)
    {
        Q_ASSERT(m_prefix);
    }
public:
    File *file() const { return m_file; }
    Prefix *prefix() const { return m_prefix; }
private:
    File *m_file;
    Prefix *m_prefix;
};

/*!
    \class File

    Represents a file node in a \l ResourceFile tree.
*/
struct File : public Node {
    File(Prefix *prefix, const QString &_name = QString(), const QString &_alias = QString())
        : Node(this, prefix), name(_name), alias(_alias) {}
    bool operator < (const File &other) const { return name < other.name; }
    bool operator == (const File &other) const { return name == other.name; }
    bool operator != (const File &other) const { return name != other.name; }
    QString name;
    QString alias;
    QIcon icon;
};

class FileList : public QList<File *>
{
public:
    bool containsFile(File *file);
};

/*!
    \class Prefix

    Represents a prefix node in a \l ResourceFile tree.
*/
struct Prefix : public Node
{
    Prefix(const QString &_name = QString(), const QString &_lang = QString(), const FileList &_file_list = FileList())
        : Node(NULL, this), name(_name), lang(_lang), file_list(_file_list) {}
    ~Prefix()
    {
        qDeleteAll(file_list);
        file_list.clear();
    }
    bool operator == (const Prefix &other) const { return (name == other.name) && (lang == other.lang); }
    QString name;
    QString lang;
    FileList file_list;
};
typedef QList<Prefix *> PrefixList;

/*!
    \class ResourceFile

    Represents the structure of a Qt Resource File (.qrc) file.
*/
class ResourceFile
{
public:
    ResourceFile(const QString &file_name = QString());
    ~ResourceFile();

    void setFileName(const QString &file_name) { m_file_name = file_name; }
    QString fileName() const { return m_file_name; }
    bool load();
    bool save();
    QString errorMessage() const { return m_error_message; }

private:
    QString resolvePath(const QString &path) const;
    QStringList prefixList() const;
    QStringList fileList(int pref_idx) const;

public:
    int prefixCount() const;
    QString prefix(int idx) const;
    QString lang(int idx) const;

    int fileCount(int prefix_idx) const;

    QString file(int prefix_idx, int file_idx) const;
    QString alias(int prefix_idx, int file_idx) const;

    void addFile(int prefix_idx, const QString &file, int file_idx = -1);
    void addPrefix(const QString &prefix, int prefix_idx = -1);

    void removePrefix(int prefix_idx);
    void removeFile(int prefix_idx, int file_idx);

    void replacePrefix(int prefix_idx, const QString &prefix);
    void replaceLang(int prefix_idx, const QString &lang);
    void replaceAlias(int prefix_idx, int file_idx, const QString &alias);

private:
    void replaceFile(int pref_idx, int file_idx, const QString &file);
public:
    int indexOfPrefix(const QString &prefix) const;
    int indexOfFile(int pref_idx, const QString &file) const;

    bool contains(const QString &prefix, const QString &file = QString()) const;
    bool contains(int pref_idx, const QString &file) const;

    QString relativePath(const QString &abs_path) const;
    QString absolutePath(const QString &rel_path) const;

    static QString fixPrefix(const QString &prefix);
    bool split(const QString &path, QString *prefix, QString *file) const;

private:
    bool isEmpty() const;

private:
    PrefixList m_prefix_list;
    QString m_file_name;
    QString m_error_message;

public:
    void * prefixPointer(int prefixIndex) const;
    void * filePointer(int prefixIndex, int fileIndex) const;
    int prefixPointerIndex(const Prefix *prefix) const;

private:
    void clearPrefixList();
};

/*!
    \class ResourceModel

    Wraps a \l ResourceFile as a single-column tree model.
*/
class ResourceModel : public QAbstractItemModel
{
    Q_OBJECT

public:
    ResourceModel(const ResourceFile &resource_file, QObject *parent = 0);

    QModelIndex index(int row, int column,
                        const QModelIndex &parent = QModelIndex()) const;
    QModelIndex parent(const QModelIndex &index) const;
    int rowCount(const QModelIndex &parent) const;
    int columnCount(const QModelIndex &parent) const;
    bool hasChildren(const QModelIndex &parent) const;

protected:
    QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const;

public:
    QString fileName() const { return m_resource_file.fileName(); }
    void setFileName(const QString &file_name) { m_resource_file.setFileName(file_name); }
    void getItem(const QModelIndex &index, QString &prefix, QString &file) const;

    QString lang(const QModelIndex &index) const;
    QString alias(const QModelIndex &index) const;
    QString file(const QModelIndex &index) const;

    virtual QModelIndex addNewPrefix();
    virtual QModelIndex addFiles(const QModelIndex &idx, const QStringList &file_list);
    void addFiles(int prefixIndex, const QStringList &fileNames, int cursorFile, int &firstFile, int &lastFile);
    void insertPrefix(int prefixIndex, const QString &prefix, const QString &lang);
    void insertFile(int prefixIndex, int fileIndex, const QString &fileName, const QString &alias);
    virtual void changePrefix(const QModelIndex &idx, const QString &prefix);
    virtual void changeLang(const QModelIndex &idx, const QString &lang);
    virtual void changeAlias(const QModelIndex &idx, const QString &alias);
    virtual QModelIndex deleteItem(const QModelIndex &idx);
    QModelIndex getIndex(const QString &prefix, const QString &file);
    QModelIndex getIndex(const QString &prefixed_file);
    QModelIndex prefixIndex(const QModelIndex &sel_idx) const;

    QString absolutePath(const QString &path) const
        { return m_resource_file.absolutePath(path); }

private:
    QString relativePath(const QString &path) const
        { return m_resource_file.relativePath(path); }
    QString lastResourceOpenDirectory() const;

public:
    virtual bool reload();
    virtual bool save();
    // QString errorMessage() const { return m_resource_file.errorMessage(); }

    bool dirty() const { return m_dirty; }
    void setDirty(bool b);

private:
    virtual QMimeData *mimeData (const QModelIndexList & indexes) const;

    static bool iconFileExtension(const QString &path);
    static QString resourcePath(const QString &prefix, const QString &file);

signals:
    void dirtyChanged(bool b);

private:
    ResourceFile m_resource_file;
    bool m_dirty;
    QString m_lastResourceDir;
};

} // namespace qdesigner_internal

QT_END_NAMESPACE

#endif // RESOURCEFILE_P_H
