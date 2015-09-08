
"""Script to download the references used by the project
"""

import sys, os, time, types, re, traceback, threading, io, datetime, csv, json, urllib, requests, zipfile, tarfile

__dir__ = os.path.dirname(os.path.realpath(__file__))

def main():
    set_working_dir(__dir__ + r'\References\\')
    
    print __doc__
    print 'Last update : ' + file_datetime('references.json', format='%Y-%m-%d %H:%M:%S')
    print ''
    
    Log('Update references ...')
    
    #run tasks in parallel
    with ConfigFile('references.json') as config:
        tasks = Tasks(config)
        exitcode = ParallelWorker(tasks).run(pattern='^update_')
    
    if exitcode:
        print '\nFailed!'
        sys.stderr = ''
        sys.exit(1)
    else:
        print '\nDone'

class Tasks():
    
    def __init__(self, configs):
        self.cfgs = configs
    
    def update_FirefoxDriver(self):
        page = r"https://pypi.python.org/pypi/selenium"
        pattern = 'selenium-([\d\.]+).tar.gz'
        value, version = WebSource(page).findlastversion(pattern, group_value=0, group_version=1)
        url = r'https://pypi.python.org/packages/source/s/selenium/' + value
        cfg = self.cfgs.get('FirefoxDriver')
        if cfg.get('version') != version or not file_exists('firefoxdriver.xpi'):
            with WebGZip(url) as gzip:
                #open the xpi file as a zip
                xpi_bytes = gzip.read(r'.*\webdriver.xpi')
                zip_in = zipfile.ZipFile(xpi_bytes)
                zip_out = zipfile.ZipFile('firefoxdriver.xpi', mode='w', compression=0)
                #copy all the files except the linux ones and remove their references from the manifest
                for fileinfo in zip_in.infolist():
                    filename = fileinfo.filename
                    if filename.endswith(r'chrome.manifest'): #edit manifest
                        manifest_txt = zip_in.read(filename)
                        manifest_txt = re.sub(r'^binary-component platform/Linux.*$\s*', \
                            '', manifest_txt, flags=re.MULTILINE)
                        zip_out.writestr(fileinfo, manifest_txt, compress_type=0)
                    elif not filename.endswith('.so'): #skip linux files
                        bytes = zip_in.read(filename)
                        zip_out.writestr(fileinfo, bytes, compress_type=0)
                zip_out.close()
                zip_in.close()
            cfg.update({'version': version, 'url': url})
            Log("Updated FirefoxDriver to version " + version)
    
    def update_FirefoxPrefs(self):
        url = r"https://raw.githubusercontent.com/SeleniumHQ/selenium/master/javascript/firefox-driver/webdriver.json"
        version = WebSource(url).getEtag()
        cfg = self.cfgs.get('FirefoxPrefs')
        if cfg.get('version') != version or not file_exists('firefox-prefs.js'):
            source = WebSource(url).gettext().decode('utf-8')
            content = json.loads(source)
            with open("firefox-prefs.js", 'w') as file:
                for mainkey in ["frozen", "mutable"]:
                    mainobj = content[mainkey]
                    for key in sorted(mainobj.keys()):
                        value = mainobj[key]
                        if isinstance(value, basestring):
                            txt = 'user_pref("' + key + '", "' + value + '");\n'
                        else:
                            txt = 'user_pref("' + key + '", ' + str(value).lower() + ');\n'
                        file.write(txt)
                    file.write('\n')
            cfg.update({'version': version, 'url': url})
            Log("Updated FirefoxPrefs to version " + version)
    
    def skip_SeleniumLibraries(self):
        page = r"http://selenium-release.storage.googleapis.com"
        pattern = r'<Key>([\d\.]+/selenium-dotnet-([\d\.]+).zip)'
        value, version = WebSource(page).findlastversion(pattern, group_value=1, group_version=2)
        url = page + '/' + value
        cfg = self.cfgs.get('.NetLibraries')
        if cfg.get('version') != version or not file_exists('WebDriver.dll'):
            with WebZip(url) as zip:
                zip.extract(r'^net35/.')
            WebFile('http://selenium.googlecode.com/git/dotnet/CHANGELOG') \
                .save('WebDriver.changelog.txt')
            cfg.update({'version': version, 'url': url})
            Log("Updated Selenium .Net to version " + version)
    
    def update_IE32(self):
        page = r"http://selenium-release.storage.googleapis.com/"
        value, version = WebSource(page).findlastversion( \
            r'<Key>([\d\.]+/IEDriverServer_Win32_([\d\.]+).zip)', group_value=1, group_version=2)
        url = page + value
        cfg = self.cfgs.get('IEDriver')
        if cfg.get('version') != version or not file_exists('iedriver.exe'):
            with WebZip(url) as zip:
                zip.extract(r'IEDriverServer.exe', 'iedriver.exe')
            cfg.update({'version': version, 'url': url})
            Log("Updated IE32 driver to version " + version)
    
    def skip_IE64(self):
        page = r"http://selenium-release.storage.googleapis.com/"
        pattern = r'<Key>([\d\.]+/IEDriverServer_x64_([\d\.]+).zip)'
        value, version = WebSource(page).findlastversion(pattern, group_value=1, group_version=2)
        url = page + value
        cfg = self.cfgs.get('IE64Driver')
        if cfg.get('version') != version or not file_exists('IEDriverServer64.exe'):
            with WebZip(url) as zip:
                zip.extract(r'IEDriverServer.exe', 'iedriver64.exe')
            cfg.update({'version': version, 'url': url})
            Log("Updated IE64 driver to version " + version)
    
    def update_SeleniumIDE(self):
        page1 = r'http://release.seleniumhq.org/selenium-ide/'
        pattern = r'href="((\d[\d\.]+)/)"'
        value, version = WebSource(page1).findlastversion(pattern, group_value=1, group_version=2)
        
        page2 = page1 + value
        pattern = r'selenium-ide-([\d\.]+)\.xpi'
        value, version = WebSource(page2).findlastversion(pattern, group_value=0, group_version=1)
        
        url = page2 + value
        cfg = self.cfgs.get('SeleniumIDE')
        if cfg.get('version') != version or not file_exists('selenium-ide.xpi'):
            with WebZip(url) as zip:
                zip.extract(r'selenium-ide.xpi')
            cfg.update({'version': version, 'url': url})
            Log("Updated Selenium IDE to version " + version)
    
    def update_ChromeDriver(self):
        page = r"http://chromedriver.storage.googleapis.com/"
        version = WebSource(page + r'LATEST_RELEASE').gettext().strip()
        url = page + version + r'/chromedriver_win32.zip'
        cfg = self.cfgs.get('ChromeDriver')
        if cfg.get('version') != version or not file_exists('chromedriver.exe'):
            with WebZip(url) as zip:
                zip.extract(r'chromedriver.exe')
            cfg.update({'version': version, 'url': url})
            Log("Updated Chrome driver to version " + version)
    
    def update_PhantomJS(self):
        page = r'https://bitbucket.org/ariya/phantomjs/downloads/'
        pattern = r'phantomjs-([\d\.]+)-windows.zip'
        value, version = WebSource(page).findlastversion(pattern, group_value=0, group_version=1)
        url = page + value
        cfg = self.cfgs.get('PhantomJSDriver')
        if cfg.get('version') != version or not file_exists('phantomjs.exe'):
            with WebZip(url) as zip:
                zip.extract(r'.*/phantomjs.exe')
            cfg.update({'version': version, 'url': url})
            Log("Updated PhantomJS to version " + version)
    
    def skip_Safari(self):
        page = r"http://selenium-release.storage.googleapis.com/"
        pattern = r'<Key>(([\d\.]+)/SafariDriver.safariextz)'
        value, version = WebSource(page).findlastversion(pattern, group_value=1, group_version=2)
        url = page + value
        cfg = self.cfgs.get('SafariDriver')
        if cfg.get('version') != version or not file_exists('SafariDriver.safariextz'):
            WebFile(url).save('SafariDriver.safariextz')
            cfg.update({'version': version, 'url': url})
            Log("Updated Safari driver to version " + version)
    
    def update_Opera(self):
        page = r'https://api.github.com/repos/operasoftware/operachromiumdriver/releases'
        pattern = r'/v([\d\.]+)/operadriver_win32.zip'
        value, version = WebSource(page).findlastversion(pattern, group_value=0, group_version=1)
        url = r'https://github.com/operasoftware/operachromiumdriver/releases/download' + value
        cfg = self.cfgs.get('OperaDriver')
        if cfg.get('version') != version or not file_exists('operadriver.exe'):
            with WebZip(url) as zip:
                zip.extract(r'operadriver.exe')
            cfg.update({'version': version, 'url': url})
            Log("Updated Opera driver to version " + version)
            
    def update_FirefoxWires(self):
        page = r'https://api.github.com/repos/jgraham/wires/releases'
        pattern = r'/([\d\.]+)/wires-[\d\.]+-windows.zip'
        value, version = WebSource(page).findlastversion(pattern, group_value=0, group_version=1)
        url = r'https://github.com/jgraham/wires/releases/download' + value
        cfg = self.cfgs.get('FirefoxWiresDriver')
        if cfg.get('version') != version or not file_exists('wires.exe'):
            with WebZip(url) as zip:
                zip.extract(r'wires.exe')
            cfg.update({'version': version, 'url': url})
            Log("Updated Firefox Wire driver to version " + version)
    
    def skip_PdfSharp(self):
        page1 = r'http://sourceforge.net/projects/pdfsharp/files/pdfsharp'
        pattern = r'/PDFsharp%20([\d\.]+)'
        value, version = WebSource(page1).findlastversion(pattern, group_value=0, group_version=1)
        page2 = page1 + value
        pattern = r'([^/]+/[^/]*Assemblies[^/]*\.zip)/download'
        value = WebSource(page2).findfirst(pattern)
        url = r'http://sunet.dl.sourceforge.net/project/pdfsharp/pdfsharp/' + value
        cfg = self.cfgs.get('PDFsharp')
        if cfg.get('version') != version or not file_exists( 'PdfSharp.dll'):
            with WebZip(url) as zip:
                zip.extract(r'.*/PdfSharp.dll')
            cfg.update({'version': version, 'url': url})
            Log("Updated PDF Sharp to version " + version)
    
    def skip_DotNetZip(self):
        page = r'https://olex-secure.openlogic.com/packages/dotnetzip'
        pattern = r'https[^"]+dotnetzip-([\d.]+)[^"]+\.zip'
        value, version = WebSource(page).findlastversion(pattern, group_value=0, group_version=1)
        url = urllib.unquote(value)
        cfg = self.cfgs.get('DotNetZip')
        if cfg.get('version') != version or not file_exists('Ionic.Zip.dll'):
            with WebZip(url) as zip:
                zip.extract(r'.*/Release/Ionic.Zip.dll')
            cfg.update({'version': version, 'url': url})
            Log("Updated DotNetZip to version " + version)


def set_working_dir(folder):
    if not os.path.isdir(folder):
        os.makedirs(folder)
    os.chdir(folder)

def file_exists(file_path):
    return os.path.isfile(file_path);

def file_datetime(filepath, format='%c', default='none'):
    if not os.path.isfile(filepath) :
        return default
    return datetime.datetime.fromtimestamp(os.path.getmtime(filepath)).strftime(format)

def format_ex(e_type, e_value, e_trace):
    lines = []
    for filename, lineno, name, line in traceback.extract_tb(e_trace):
        lines.append(' in %s() line %d in %s\n' % (name, lineno, os.path.basename(filename)))
        if line:
            lines.append('  ' + line.strip() + '\n')
    return '\n#%s:\n%s\n\n%s' %  (e_type.__name__, str(e_value), ''.join(lines))


class ConfigFile(dict):

    def __init__(self, filepath):
        self.lock = threading.Lock()
        self.filepath = filepath
        if os.path.isfile(filepath):
            with open(filepath, 'r') as file:
                self.data = json.load(file)
        else:
            self.data = {}

    def __enter__(self):
        return self

    def __exit__(self, type, value, traceback):
        with open(self.filepath, 'w') as file:
            json.dump(self.data, file, sort_keys=False, indent=4, ensure_ascii=False)

    def get(self, key):
        with self.lock:
            return self.data.setdefault(key, {})

from Queue import Queue

class ParallelWorker(Queue):

    def __init__(self, instance, max_workers=10):
        Queue.__init__(self)
        self.instance = instance
        self.exitcode = 0
        self.max_workers = max_workers

    def __run__(self):
        while not self.empty():
            method = self.get()
            try:
                method()
            except Exception as ex:
                self.exitcode = 1
                e_type, e_value, e_trace = sys.exc_info()
                sys.stderr.write(format_ex(e_type, e_value, e_trace.tb_next))
            finally:
                self.task_done()
                sys.exc_clear()

    def run(self, pattern=''):
        methods = [getattr(self.instance, k) for k, v in self.instance.__class__.__dict__.items() \
            if isinstance(v, types.FunctionType) and re.search(pattern, k)]
        nb_workers = min(self.max_workers, len(methods))
        for method in methods:
            self.put(method)
        for i in range(nb_workers):
            t = threading.Thread(target=self.__run__)
            t.daemon = True
            t.start()
        self.join()
        return self.exitcode

class WebGZip:

    def __init__(self, url):
        response = requests.get(url)
        response.raise_for_status()
        buffer = io.BytesIO(response.content)
        self.tar = tarfile.open(fileobj=buffer, mode='r:gz')

    def __enter__(self):
        return self

    def __exit__(self, type, value, traceback):
        self.tar.close()

    def extract(self, pattern, dest = '.'):
        p = re.compile(pattern)
        destIsdir = os.path.isdir(dest)
        for tarinfo in self.tar.getmembers():
            name = tarinfo.name
            if p.match(name) and not tarinfo.isdir():
                dest_file = dest + '\\' + os.path.basename(name) if destIsdir else dest
                with open(dest_file , 'wb') as file:
                    file.write(self.tar.extractfile(tarinfo).read())
                if not destIsdir:
                    return
    
    def read(self, pattern):
        p = re.compile(pattern)
        for tarinfo in self.tar.getmembers():
            if p.match(tarinfo.name) and not tarinfo.isdir():
                return self.tar.extractfile(tarinfo)

class WebZip:

    def __init__(self, url):
        response = requests.get(url)
        response.raise_for_status()
        buffer = io.BytesIO(response.content)
        self.zip = zipfile.ZipFile(buffer)

    def __enter__(self):
        return self

    def __exit__(self, type, value, traceback):
        self.zip.close()

    def extract(self, pattern, dest='.'):
        p = re.compile(pattern)
        destIsdir = os.path.isdir(dest)
        for name in self.zip.namelist():
            if p.match(name):
                dest_file = dest + '\\' + os.path.basename(name) if destIsdir else dest
                with open(dest_file , 'wb') as file:
                    file.write(self.zip.read(name))
                if not destIsdir:
                    return
                    
class WebFile:

    def __init__(self, url):
        self.url = url

    def save(self, file):
        response = requests.get(self.url, stream=True)
        response.raise_for_status()
        with open(file, 'wb') as handle:
            for block in response.iter_content(1024):
                if not block:
                    break
                handle.write(block)


class WebSource:

    def __init__(self, url):
        self.url = url
        self.text = None

    def gettext(self):
        if self.text is None:
            self.text = requests.get(self.url).text
        return self.text

    def findfirst(self, pattern, group=-1):
        res = re.search(pattern, self.gettext())
        if not res:
            raise PatternNotFound(pattern, self.url)
        return res.group(res.re.groups if group == -1 else group)

    def getEtag(self, default='none'):
        try:
            etag = requests.head(self.url).headers['etag']
            return re.search(r'[\w-]+', etag).group(0)
        except:
            return default

    def findlastversion(self, pattern, group_value=1, group_version=2):
        match = re.finditer(pattern, self.gettext())
        lst_versions = [(m.group(group_value), m.group(group_version), \
            map(int, m.group(group_version).split('.'))) for m in match]
        if not lst_versions:
            raise PatternNotFound(pattern, self.url)
        lst_versions.sort(key=lambda m: m[2], reverse=True)
        return (lst_versions[0][0], lst_versions[0][1])

    def findlastdate(self, pattern, group_value=1, group_datetime=2, datetime_format='%Y-%m-%dT%H:%M:%S'):
        match = re.finditer(pattern, self.gettext())
        lst_dates = [(m.group(group_value), time.strptime(m.group(group_datetime), datetime_format) \
            ) for m in match]
        if not lst_dates:
            raise PatternNotFound(pattern, self.url)
        lst_dates.sort(key=lambda m: m[1], reverse=True)
        return lst_dates[0]

class PatternNotFound(Exception):

    def __init__(self, pattern, source):
        self.__data__ = (pattern, source)

    def __str__(self):
        return 'Pattern "%s" not found in %s' % self.__data__

def Log(message):
    with Logger.lock:
        print message
        Logger.write(message)

class Logger:
    
    @staticmethod
    def write(message, header = ''):
        header = time.strftime('%Y-%m-%d %H:%M:%S') + ' ' + header
        txt = header + message.replace('\r\n', '\n').replace('\n', '\n' + header) + '\n'
        Logger.logfile.write(txt)
    
    def __init__(self):
        Logger.lock = threading.Lock()
        filename = re.sub(r'\.[^.]+$', '.log', __file__)
        Logger.logfile = open(filename, mode='a', buffering = 1)
    
    def __enter__(self):
        return self
    
    def __exit__(self, e_type, e_value, e_trace):
        if e_type:
            Logger.write(''.join(traceback.format_exception(e_type, e_value, e_trace)), 'Err')
        Logger.logfile.close()

if __name__ == '__main__':
    with Logger() as log:
        main()
