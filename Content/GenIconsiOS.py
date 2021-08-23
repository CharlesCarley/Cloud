# -----------------------------------------------------------------------------
#   $Id: GenIconsiOS.py 480 2019-01-08 19:33:21Z Charlie $
#   
#   Copyright (c) Charles Carley.
#   
#   All Rights Reserved.
#
# -----------------------------------------------------------------------------
import glob, string
import os, sys, subprocess

AppIcons = glob.glob("Icons/*.svg");
iOSResolutions = [
    20,
    29,
    40,
    58,
    60,
    76,
    80,
    87,
    120,
    152,
    167,
    180,
    1024
]


for resolution in iOSResolutions:
    for appIcon in AppIcons:
        baseName = os.path.basename(appIcon).split('.')[0]

        if (not os.path.isdir("Icons/" + baseName)):
            os.mkdir("Icons/" + baseName)

        destName = "Icons/" + baseName + "/Icon" + str(resolution) + ".png"    
        print(destName)

        w = " -w%i"%resolution 
        h = " -h%i"%resolution

        fmt = "--export-png="+destName+"  -C"
        fmt += w 
        fmt += h
        subprocess.call("inkscape.bat %s %s"%(appIcon, fmt))
