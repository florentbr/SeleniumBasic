
"""Script to create the installation package for Windows:
 Builds the Firefox add-ins
 Builds the project with MSBuild.exe
 Builds the 32bit and 64bit Type libraries with TlbExp.exe
 Builds the documentation with Help File Builder [https://shfb.codeplex.com/]
 Creates the setup installer with InnoSetup [http://www.jrsoftware.org/isinfo.php]
"""

import os, re, time, sys, traceback, shutil, datetime, subprocess, zipfile, glob

__dir__ = os.path.dirname(os.path.realpath(__file__))

APP_MSBUILD_PATH = r'c:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe'
APP_TLBEXP_PATH = r'c:\Progra~2\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\TlbExp.exe'
APP_INNOSETUP_PATH = r'c:\Progra~2\Inno Setup 5\ISCC.exe'
APP_PYTHON_PATH = r'c:\Progra~2\Python27\python.exe'
APP_IRONPYTHON_PATH = r'c:\Progra~2\IronPython 2.7\ipy.exe'
APP_SHFBROOT_DIR = r'c:\Progra~2\EWSoftware\Sandcastle Help File Builder'

def main():
    set_working_dir(__dir__)
    check_globals_exists(r'_PATH$|_DIR$')
    
    assemblyinfo_path = __dir__ + r'\Selenium\Properties\AssemblyInfo.cs'
    last_modified_time = get_file_mtime(assemblyinfo_path, '%Y-%m-%d %H:%M:%S')
    current_version = match_in_file(assemblyinfo_path, r'AssemblyFileVersion\("([.\d]+)"\)')
    
    print __doc__
    print 'Last compilation : ' + (last_modified_time or 'none')
    print 'Current Version  : ' + current_version
    print ''
    
    new_version = get_input_version(current_version)

    print 'New version : ' + new_version + '\n'
    print 'Update version number ...'
    replace_in_file(assemblyinfo_path, r'Version\("[.\d]+"\)', r'Version("{}")'.format(new_version))

    print 'Delete previous builds ...'
    clear_dir(r'.\FirefoxAddons\bin')
    clear_dir(r'.\Selenium\bin\Release')
    clear_dir(r'.\Selenium\obj\Release')
    clear_dir(r'.\Selenium\bin\Help')
    clear_dir(r'.\VbsConsole\bin\Release')
    clear_dir(r'.\VbsConsole\obj\Release')
    
    print 'Build vb-format addin ...'
    execute(APP_IRONPYTHON_PATH, __dir__ + r'\FirefoxAddons\build-vb-format.py', current_version)
    
    print 'Build implicit-wait addin ...'
    execute(APP_IRONPYTHON_PATH, __dir__ + r'\FirefoxAddons\build-implicit-wait.py', current_version)
    
    print 'Build extensions package ...'
    with zipfile.ZipFile(__dir__ + r'\FirefoxAddons\bin\extensions.xpi', 'a') as zip:
        zip.write(__dir__ + r'\FirefoxAddons\install.rdf', 'install.rdf')
        zip.write(__dir__ + r'\References\selenium-ide.xpi', 'selenium-ide.xpi')
        zip.write(__dir__ + r'\FirefoxAddons\bin\vb-formatters.xpi', 'vb-formatters.xpi')
        zip.write(__dir__ + r'\FirefoxAddons\bin\implicit-wait.xpi', 'implicit-wait.xpi')
    
    print 'Build .Net library ...'
    execute(APP_MSBUILD_PATH, '/t:build', '/nologo', '/v:quiet',
        r'/p:Configuration=Release;TargetFrameworkVersion=v3.5',
        r'/p:RegisterForComInterop=False',
        r'/p:SignAssembly=true;AssemblyOriginatorKeyFile=key.snk', 
        r'.\Selenium\Selenium.csproj' )
    
    print 'Build Type libraries 32bits ...'
    execute(APP_TLBEXP_PATH, r'.\Selenium\bin\Release\Selenium.dll', 
        r'/win32', r'/out:.\Selenium\bin\Release\Selenium32.tlb' )
    
    print 'Build Type libraries 64bits ...'
    execute(APP_TLBEXP_PATH,
        r'.\Selenium\bin\Release\Selenium.dll', 
        r'/win64', r'/out:.\Selenium\bin\Release\Selenium64.tlb' )
    
    print 'Build console runner ...'
    execute(APP_MSBUILD_PATH, '/v:quiet', '/t:build', '/nologo', 
        r'/p:Configuration=Release;TargetFrameworkVersion=v3.5', 
        r'.\VbsConsole\VbsConsole.csproj' )
    
    print 'Build documentation ...'
    os.environ['SHFBROOT'] = APP_SHFBROOT_DIR
    execute(APP_MSBUILD_PATH, '/p:Configuration=Release', '/nologo', '.\Selenium\Selenium.shfbproj')
    
    print 'Build registration file ...'
    execute(APP_IRONPYTHON_PATH, r'gen-registration.ipy', \
        r'Selenium\bin\Release\Selenium.dll', __dir__ + r'SeleniumBasicSetup.pas')
    
    print 'Rebuild excel files ...'
    execute(APP_PYTHON_PATH, __dir__ + r'\rebuild_exel_files.py')
    
    print 'Build setup package ...'
    execute(APP_INNOSETUP_PATH, '/q', '/O' + __dir__, __dir__ + r'\SeleniumBasicSetup.iss')
    
    print 'Launch install ...'
    execute(__dir__ + '\SeleniumBasic-%s.exe' % new_version)

    print '\nDone'

    

def set_working_dir(directory):
    make_dir(directory)
    os.chdir(directory)

def make_dir(directory):
    if not os.path.isdir(directory):
        os.makedirs(directory)
        
def remove_dir(directory):
    if os.path.isdir(directory):
        shutil.rmtree(directory)

def copy_file(src, dest):
    shutil.copyfile(src, dest)

def clear_dir(directory):
    if os.path.isdir(directory):
        shutil.rmtree(directory)
    os.makedirs(directory)

def check_globals_exists(pattern):
    items = globals()
    miss_items = [k for k in items.keys() if re.search(pattern, k) \
        and type(items[k]) == type('') and not os.path.exists(items[k])]
    if miss_items:
        raise Exception('Invalide path(s):\n ' + '\n '.join( \
            ['{}: {}'.format(k, items[k]) for k in miss_items]))

def get_file_mtime(filepath, format=None):
    if(not os.path.isfile(filepath)):
        return None
    dt = datetime.datetime.fromtimestamp(os.path.getmtime(filepath))
    if format:
        return dt.strftime(format)
    return dt

def match_in_file(filepath, pattern):
    with open(filepath, 'r') as f:
        result = re.search(pattern, f.read())
        return result.group(result.re.groups)

def replace_in_file(filepath, pattern, replacement):
    with open(filepath, 'r') as f:
        text = re.sub(pattern, replacement, f.read())
    with open(filepath, 'w') as f:
        f.write(text)

class CommandException(Exception):
    pass

def execute(*arguments):
    cmd = ' '.join(arguments)
    Logger.write('cwd> ', os.getcwd())
    Logger.write('cmd> ', cmd)
    p = subprocess.Popen(arguments, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    stdout, stderr = p.communicate()
    txtout = stdout.decode("utf-8")
    Logger.write('info> ', txtout)
    if p.returncode != 0:
        txterr = stderr.decode("utf-8")
        raise CommandException(cmd + '\n' + txtout + '\n' + txterr)
    Logger.write('', '\n')

def get_input(message):
    try:
        return raw_input(message)
    except NameError:
        return input(message)

def get_input_version(version):
    while True:
        input = get_input('Digit to increment [w.x.y.z] or version [0.0.0.0] or skip [s] ? ').strip()
        if re.match(r's|w|x|y|z', input) :
            idx = {'s': 99, 'w': 0, 'x': 1, 'y': 2, 'z': 3}[input]
            return '.'.join([str((int(v)+(i == idx))*(i <= idx)) for i, v in enumerate(version.split('.'))])
        elif re.match(r'\d+\.\d+\.\d+\.\d+', input):
            return input

class ZipFile(zipfile.ZipFile):
    
    def __init__(cls, file, mode):
        zipfile.ZipFile.__init__(cls, file, mode)
      
    def add(self, path):
        for item in glob.glob(path):
            if os.path.isdir(item):
                self.add(item + r'\*')
            else:
                self.write(item)

class Logger:
    
    @staticmethod
    def write(header, message):
        header = time.strftime('%H:%M:%S') + (' ' + header if header else '')
        txt = header + message.replace('\r\n', '\n') \
            .replace('\r', '').replace('\n', '\n' + header) + '\n'
        Logger.file.write(txt)
    
    def __init__(self):
        filename = re.sub(r'\.[^.]+$', '.log', __file__)
        Logger.file = open(filename, mode='w', buffering = 1)
    
    def __enter__(self):
        return self
    
    def __exit__(self, e_type, e_value, e_trace):
        if e_type:
            Logger.write('error>', ''.join(traceback.format_exception(e_type, e_value, e_trace)))
        Logger.file.close()
        if e_type:
            subprocess.Popen(['cmd', '/C', Logger.file.name]) #open the log file

if __name__ == '__main__':
    with Logger() as log:
        main()
