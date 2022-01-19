
"""Utility script to clean the project files and folders
"""

DELETE_FOLDERS_PATTERNS = ['bin', 'obj']
DELETE_FILES_PATTERNS = ['*.suo', '*.bak', '*.pyc']

import sys, os, fnmatch, shutil

__dir__ = os.path.dirname(os.path.realpath(__file__))

def main():
    print(__doc__)
    print("Folders : " + ', '.join(DELETE_FOLDERS_PATTERNS))
    print("Files   : " + ', '.join(DELETE_FILES_PATTERNS))
    print("")
    print("Start cleaning ...")
    
    for root_dir, folders, files in os.walk(__dir__):
        
        for del_dir in DELETE_FOLDERS_PATTERNS:
            for dirname in fnmatch.filter(folders, del_dir):
                dir_path = os.path.join(root_dir, dirname)
                print("rm %s" % dir_path
                shutil.rmtree(dir_path)
        
        for del_file in DELETE_FILES_PATTERNS:
            for filename in fnmatch.filter(files, del_file):
                file_path = os.path.join(root_dir, filename)
                print("rm %s" % file_path
                os.remove(file_path)
    
    print("\nDone")

if __name__ == '__main__':
    main()
