from subprocess import Popen, STDOUT, PIPE
import os
import glob
import _winreg

for x in glob.glob('*.log'):
    os.unlink(x)

class VsException(Exception):
    def __init__(self, msg, exit_code = None):
        super(VsException, self).__init__(msg)
        self.exit_code = exit_code

class Vs(object):
    def __init__(self, solution = None, version = None):
        self.solution = solution
        try:
            key = _winreg.OpenKey(_winreg.HKEY_CURRENT_USER, r"Software\Microsoft\VisualStudio\%s_Config" % version)
            self.devenv = _winreg.QueryValueEx(key, "InstallDir")[0] + "devenv.exe"
        except WindowsError, ex:
            self.devenv = "devenv.exe"

    def command(self, *args):
        line = [self.devenv, self.solution] + list(args)
        print(" ".join(line))
        proc = Popen(line, stdout = PIPE, stderr = PIPE)
        out, err = [x.decode("utf-8") for x in proc.communicate()]
        if proc.returncode:
            cmd = (" ".join([self.devenv, self.solution] + list(args)))
            raise VsException("Error running<%d>: %s\n\t%s" % (proc.returncode, cmd, err), proc.returncode)
        return out
        
    def clean(self, config = None):
        if config:
            self.command('/Clean', config)
        else:
            self.command('/Clean')

    def build(self, config = None, log = None):
        if config:
            self.command('/Rebuild', config, '/Out', log)
        else:
            self.command('/Rebuild')

Qt4VSAddin2010 = Vs(os.path.realpath("..\\src\\Qt4VSAddin2010.sln"), 10.0)
Qt4VSAddin2010.clean('Release')
Qt4VSAddin2010.build("Release", "Qt4VSAddin2010.log")

Qt4VSAddin2012 = Vs(os.path.realpath("..\\src\\Qt4VSAddin2012.sln"), 11.0)
Qt4VSAddin2012.clean('Release')
Qt4VSAddin2012.build("Release", "Qt4VSAddin2012.log")

Qt4VSAddin2013 = Vs(os.path.realpath("..\\src\\Qt4VSAddin2013.sln"), 12.0)
Qt4VSAddin2013.clean('Release')
Qt4VSAddin2013.build("Release", "Qt4VSAddin2013.log")

Qt4VSAddin = Vs(os.path.realpath("..\\pkg\\Qt4VSAddin2010.sln"), 10.0)
Qt4VSAddin.build("Release", "Qt4VSAddin.log")

