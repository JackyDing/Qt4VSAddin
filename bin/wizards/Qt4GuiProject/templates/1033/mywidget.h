#ifndef %PRE_DEF%
#define %PRE_DEF%

#include <QtGui/%BASECLASS%>
#include "%UI_HDR%"

class %CLASS% : public %BASECLASS%
{
    Q_OBJECT

public:
    %CLASS%(QWidget *parent = 0, Qt::WFlags flags = 0);
    ~%CLASS%();

private:
    Ui::%CLASS%Class ui;
};

#endif // %PRE_DEF%
