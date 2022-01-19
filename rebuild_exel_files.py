
"""Rebuild Excel files
"""

import sys, os, shutil, zipfile, re, glob, tempfile
from win32com import client  #http://sourceforge.net/projects/pywin32/files/pywin32/

def main(args):
    print(__doc__)
    
    for folder in (r'\Templates', r'\Examples\Excel'):
        rebuild(__dir__ + folder)
    
    print("\nDone")


def rebuild(directory):
    xlbin_dir = make_dir(directory + r'\Xlbin')
    
    for file in glob.glob(directory + r"/*.xl?m"):
        shortname = get_shortname(file)
        extention = get_extention(file)
        
        print("Extract %s ..." % file)
        tmp_folder = extract_excel(file)
        
        print("Rebuild %s ..." % shortname)
        (wb, wb_file) = build_excel(tmp_folder, extention)
        wb.VBProject.References.AddFromGuid("{0277FC34-FD1B-4616-BB19-A9AABCAF2A70}", 2, 0)
        
        if extention == ".xlsm":
            print("Save %s.xlsm ..." % shortname)
            wb.SaveAs(directory + r'\%s.xlsm' % shortname, xlOpenXMLWorkbookMacroEnabled)
            print("Save %s.xls ..." % shortname)
            wb.SaveAs(xlbin_dir + r'\%s.xls' % shortname,  xlWorkbook8)
            
        elif extention == ".xltm":
            print("Save %s.xltm ..." % shortname)
            wb.SaveAs(directory + r'\%s.xltm' % shortname, xlOpenXMLTemplateMacroEnabled)
            print("Save %s.xlt ..." % shortname)
            wb.SaveAs(xlbin_dir + r'\%s.xlt' % shortname,  xlTemplate8)
        
        wb.Application.Quit()
        shutil.rmtree(tmp_folder)
        os.remove(wb_file)
        print("")
    
def build_excel(folder, extension):
    # Create excel file for folder
    file = tempfile.NamedTemporaryFile(suffix=extension).name
    build_zip(folder, file, r".*", r"^vba\\")
    
    # Add VBA code
    xl = CreateObject("Excel.Application")
    xl.EnableEvents = False
    xl.DisplayAlerts = False
    wb = xl.Workbooks.Open(file)
    components = wb.VBProject.VBComponents
    for (dirpath, dirnames, filenames) in os.walk(folder + r"\vba"):
        for filename in filenames:
            file_path = os.path.join(dirpath, filename)
            if filename.find(".") == -1:
                module = components.Item(filename).CodeModule
                with open(file_path, "r") as f:
                    code = f.read()
                    module.InsertLines(1, code)
            else:
                components.Import(file_path)
    return (wb, file)


def extract_excel(file):
    # Create temporary folders
    folder = tempfile.mkdtemp()
    dir_vba = make_dir(folder + r'\vba')
    
    # Extract the file
    extract_zip(file, folder, r".*", r".*vbaProject\.bin$")
    
    # Extract the VBA code
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
        
    return folder
        
        
def CreateObject(progid):
    return client.Dispatch(progid)

def make_dir(directory):
    if not os.path.isdir(directory):
        os.makedirs(directory)
    return directory

def get_shortname(path):
    start = path.rfind("\\") + 1
    end = max(0, path.rfind(".", start))
    return path[start: end or None]
    
def get_extention(path):
    start = path.rfind('.')
    return '' if start == -1 else path[start:]

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


xlTemplate8 = 17
xlWorkbook8 = 56
xlOpenXMLWorkbookMacroEnabled = 52
xlOpenXMLTemplateMacroEnabled = 53

vbext_ct_StdModule = 1
vbext_ct_ClassModule = 2
vbext_ct_MSForm = 3
vbext_ct_Document = 100

extensions = {
    vbext_ct_StdModule : 'bas',
    vbext_ct_ClassModule : 'cls',
    vbext_ct_MSForm : 'frm'
}

__dir__ = os.path.dirname(os.path.realpath(__file__))

if __name__ == '__main__':
    main(sys.argv[1:])
