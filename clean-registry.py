
"""Utility script to clean the registry """

# To run the script from the context menu with elevation:
# [HKEY_CLASSES_ROOT\Python.File\shell\runas\command]
# @="C:\Program Files (x86)\Python27\python.exe" -i "%1" %* runas

# Additional modules:
# pip install pywin32

STARTSWITH_PROGID = "Selenium."
STARTSWITH_GUID = "{0277FC34-FD1B-4616-BB19"

import sys, os, io, sys, win32api, win32con

__dir__ = os.path.dirname(os.path.realpath(__file__))

HKCR = win32con.HKEY_CLASSES_ROOT
HKLM = win32con.HKEY_LOCAL_MACHINE
HKCU = win32con.HKEY_CURRENT_USER
HKNAMES = {HKCR: 'HKCR', HKLM: 'HKLM', HKCU: 'HKCU'}

def main():
    print __doc__
    print "Start cleaning registry ..."
    print ""
    
    for i in range(0, 2):
        for keyname, res, cls, lwt in win32api.RegEnumKeyEx(HKCR):
            if keyname.startswith(STARTSWITH_PROGID):
                DeleteRegTree(HKCR, keyname)
        
        for folder in [r'TypeLib', r'CLSID', r'Wow6432Node\CLSID', r'Interface', r'Wow6432Node\Interface', r'Record']:
            skey = win32api.RegOpenKeyEx(HKCR, folder, 0, win32con.KEY_READ)
            if skey:
                for keyname, res, cls, lwt in win32api.RegEnumKeyEx(skey):
                    if keyname.startswith(STARTSWITH_GUID):
                        DeleteRegTree(HKCR, folder + '\\' + keyname)
                win32api.RegCloseKey(skey)
    
    print "\nDone"

def DeleteRegTree(root, subkey):
    print r'rm %s\%s' % (HKNAMES[root], subkey)
    try:
        win32api.RegDeleteTree(root, subkey)
    except Exception as ex :
        print ' failed: %s' % ex.strerror
    
def elevate():
    import ctypes, win32com.shell.shell, win32event, win32process
    outpath = r'%s\%s.out' % (os.environ["TEMP"], os.path.basename(__file__))
    if ctypes.windll.shell32.IsUserAnAdmin():
        if os.path.isfile(outpath):
            sys.stderr = sys.stdout = open(outpath, 'w', 0)
        return
    with open(outpath, 'w+', 0) as outfile:
        hProc = win32com.shell.shell.ShellExecuteEx(lpFile=sys.executable, \
            lpVerb='runas', lpParameters=' '.join(sys.argv), fMask=64, nShow=0)['hProcess']
        while True:
            hr = win32event.WaitForSingleObject(hProc, 40)
            while True:
                line = outfile.readline()
                if not line: break
                sys.stdout.write(line)
            if hr != 0x102: break
    os.remove(outpath)
    sys.stderr = ''
    sys.exit(win32process.GetExitCodeProcess(hProc))

if __name__ == '__main__':
    elevate()
    main()