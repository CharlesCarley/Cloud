# -----------------------------------------------------------------------------
#   $Id: DeleteFiles.py 480 2019-01-08 19:33:21Z Charlie $
#   
#   Copyright (c) Charles Carley.
#   
#   All Rights Reserved.
#
# -----------------------------------------------------------------------------
import glob, string
import os, sys, subprocess, shutil

if len(sys.argv) <= 1:
    print ("Usage: " + sys.argv[0] + " root path" )
    sys.exit(-1)

for root, dirs, files in os.walk(sys.argv[1]):
    file_list = []

    for directory in dirs:
        build_path = root + os.sep + directory
        if (os.path.isdir(build_path)):
            try:
                shutil.rmtree(build_path)
                print ("==> Deleting " + build_path)
            except:
                print ("==> Skipped " + build_path)

    for file in files:
        build_path = root + os.sep + file
        if (os.path.isfile(build_path)):
            try:
                os.remove(build_path)
                print ("==> Deleting " + build_path)
            except:
                print ("==> Skipped " + build_path)
        
curdir = os.getcwd()
os.chdir("Build")
os.chdir(curdir)

build_path = "Build"
if (os.path.isdir(build_path)):
    try:
        shutil.rmtree(build_path)
        os.mkdir(build_path)
        print ("==> Deleting " + build_path)
    except:
        print ("==> Skipped " + build_path)
