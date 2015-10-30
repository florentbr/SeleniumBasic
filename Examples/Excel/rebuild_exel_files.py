
"""Extract all modules and classes from Excel
"""

import sys, os, shutil, zipfile, re, glob
from win32com import client  #http://sourceforge.net/projects/pywin32/files/pywin32/

__dir__ = os.path.dirname(os.path.realpath(__file__))

vbext_ct_StdModule = 1
vbext_ct_ClassModule = 2
vbext_ct_MSForm = 3
vbext_ct_Document = 100

extensions = {
    vbext_ct_StdModule : 'bas',
    vbext_ct_ClassModule : 'cls',
    vbext_ct_MSForm : 'frm'
}

def main():
    for file in glob.glob(__dir__ + r"/*.xlsm"):
        folder = __dir__ + '\\' + filename_noext(file)
        fname = os.path.basename(file)
        print "Extract %s ..." % fname
        extract(file, folder)
        print "Rebuild %s ..." % fname
        build(folder, file)
    print "\nDone"
    
    
def build(folder, file):
    delete_file(file)
    build_zip(folder, file, r".*", r"^vba\\")
    
    xl = CreateObject("Excel.Application")
    xl.EnableEvents = False
    xl.DisplayAlerts = False
    wb = xl.Workbooks.Open(file)
    try:
        wb.VBProject.References.AddFromGuid("{0277FC34-FD1B-4616-BB19-A9AABCAF2A70}", 2, 0)
        components = wb.VBProject.VBComponents
        for (dirpath, dirnames, filenames) in os.walk(folder + r"\vba"):
            for fname in filenames:
                file_path = os.path.join(dirpath, fname)
                if fname.find(".") == -1:
                    module = components.Item(fname).CodeModule
                    with open(file_path, "r") as f:
                        code = f.read()
                        module.InsertLines(1, code)
                else:
                    components.Import(file_path)
        wb.Save()
        wb.SaveAs(re.sub('.xlsm$', '.xls', file), 56)
        wb.SaveAs(file, 52) #xlOpenXMLWorkbookMacroEnabled
    finally:
        wb.Close(False)
        xl.Quit()


def extract(file, folder):
    clear_dir(folder)
    
    extract_zip(file, folder, r".*", r".*vbaProject\.bin$")
    
    dir_vba = folder + '\\vba'
    clear_dir(dir_vba)
    
    xl = CreateObject("Excel.Application")
    xl.EnableEvents = False
    wb = xl.Workbooks.Open(file)
    try:
        for item in wb.VBProject.VBComponents:
            if item.Type < 4:
                ext = extensions.get(item.Type, None)
                item.Export(r"%s\%s.%s" % (dir_vba, item.Name, ext))
            else:
                module = item.CodeModule
                count = module.CountOfLines
                if count:
                    with open(r"%s\%s" % (dir_vba, item.Name), "w") as f:
                        code = module.Lines(1, count)
                        f.write(code)
                
    finally:
        wb.Close(False)
        xl.Quit()
        
        
def CreateObject(progid):
    return client.Dispatch(progid)

def clear_dir(directory):
    if os.path.isdir(directory):
        shutil.rmtree(directory)
    os.makedirs(directory)
    
def delete_file(file):
    if os.path.isfile(file):
        os.remove(file) 
        
def filename_noext(txt):
    start = txt.rfind("\\") + 1
    end = max(0, txt.rfind(".", start))
    return txt[start: end or None]

def extract_zip(filename, folder, pattern_include, pattern_exclude):
    p_inc = re.compile(pattern_include)
    p_exc = re.compile(pattern_exclude)
    with zipfile.ZipFile(filename, mode="r") as zip:
        for name in zip.namelist():
            if p_inc.match(name) and not p_exc.match(name):
                dest_file = folder + '\\' + name
                dest_dir = os.path.dirname(dest_file)
                if not os.path.isdir(dest_dir):
                    os.makedirs(dest_dir)
                with open(dest_file , 'wb') as file:
                    file.write(zip.read(name))
  
def build_zip(folder, filename, pattern_include, pattern_exclude):
    p_inc = re.compile(pattern_include)
    p_exc = re.compile(pattern_exclude)
    with zipfile.ZipFile(filename, mode="w", compression=8) as zip:
        for (dirpath, dirnames, filenames) in os.walk(folder):
            for file in filenames:
                abs_path = os.path.join(dirpath, file)
                rel_path = os.path.relpath(abs_path, folder)
                if p_inc.match(rel_path) and not p_exc.match(rel_path):
                    zip.write(abs_path, rel_path)


if __name__ == '__main__':
    print __doc__
    main()
