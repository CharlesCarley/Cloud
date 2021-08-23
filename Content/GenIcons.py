# -----------------------------------------------------------------------------
#   $Id: GenIcons.py 118 2019-01-09 18:01:19Z Charlie $
#   
#   Copyright (c) Charles Carley.
#   
#   All Rights Reserved.
#
# -----------------------------------------------------------------------------
import glob, string
import os, sys, subprocess

AppIcons = glob.glob("Icons/*.svg");
AndroidResolutions = [
    192,
    144,
    96,
    72,
    48,
]

for appIcon in AppIcons:
    baseName = os.path.basename(appIcon).split('.')[0]
    destName = "Icons/" + baseName + ".png"    
    resolution = 512

    w = " -w%i"%resolution 
    h = " -h%i"%resolution
    fmt = "--export-png="+destName+"  -C"

    fmt += w 
    fmt += h
    print (destName)    
    subprocess.call("inkscape.bat %s %s"%(appIcon, fmt))
