
"""Script to build the xpi add-in for firefox
Usage : python build-vb-format.py "x.x.x.x"
"""

import os, re, sys, shutil, datetime, zipfile, glob

CD = os.path.dirname(os.path.abspath(__file__))

SRC_DIR = CD + r'\vb-format'
OUT_DIR = CD + r'\bin'
RDF_PATH = SRC_DIR + r'\install.rdf'

def main(args):
    arg_version = args and args[0]
    set_working_dir(CD)
    
    last_modified_time = get_file_mtime(RDF_PATH, '%Y-%m-%d %H:%M:%S')
    current_version = find_in_file(RDF_PATH, r'version>([.\d]+)<');
    
    print __doc__
    print 'Last compilation : ' + (last_modified_time or 'none')
    print 'Current Version  : ' + current_version
    
    new_version = arg_version or get_input_version(current_version)
    
    print 'New version : ' + new_version + '\n'
    print 'Update version number ...'
    replace_in_file(RDF_PATH, r'(?<=version>)[.\d]+(?=<)', new_version)
    
    print 'Build formater xpi ...'
    make_dir(OUT_DIR)
    set_working_dir(SRC_DIR)
    with ZipFile(OUT_DIR + r'\vb-formatters.xpi', 'w') as zip:
        zip.add(r'*')
    
    print '\nDone'



def set_working_dir(directory):
    make_dir(directory)
    os.chdir(directory)

def make_dir(directory):
    if not os.path.isdir(directory):
        os.makedirs(directory)

def clear_dir(directory):
    if os.path.isdir(directory):
        shutil.rmtree(directory)
    os.makedirs(directory)

def get_file_mtime(filepath, format=None):
    if(not os.path.isfile(filepath)):
        return None
    dt = datetime.datetime.fromtimestamp(os.path.getmtime(filepath))
    if format:
        return dt.strftime(format)
    return dt

def delete_file(filepath):
    if(os.path.isfile(filepath)):
        os.remove(filepath)

def find_in_file(filepath, pattern):
    with open(filepath, 'r') as f:
        result = re.search(pattern, f.read())
        return result.group(result.re.groups)

def replace_in_file(filepath, pattern, replacement):
    with open(filepath, 'r') as f:
        text = re.sub(pattern, replacement, f.read())
    with open(filepath, 'w') as f:
        f.write(text)

def get_input(message):
    try: return raw_input(message)
    except NameError: return input(message)

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
                self.add(item + r'\*');
            else:
                self.write(item)


if __name__ == '__main__':
    main(sys.argv[1:])
