# -----------------------------------------------------------------------------
#   $Id: GenAppIcons.py 137 2019-01-10 02:17:27Z Charlie $
#
#   Copyright (c) Charles Carley.
#
#   All Rights Reserved.
#
# -----------------------------------------------------------------------------
import glob, string
import os, sys, subprocess

AppIcons = glob.glob("AppIcons/*.svg")


# BaseResolutionName  = [18, 24, 36, 48]
BaseResolutionName = [24]

Outputs = {
    "drawable": [18, 24, 36, 48],
    "drawable-mdpi": [18, 24, 36, 48],
    "drawable-hdpi": [27, 36, 54, 72],
    "drawable-xhdpi": [36, 48, 72, 96],
    "drawable-xxhdpi": [54, 72, 108, 144],
    "drawable-xxxhdpi": [72, 96, 144, 192],
    "Resources": [18, 24, 36, 48],
    "Assets": [18, 24, 36, 48],
}

AppIconsMain = glob.glob("Icons/*.svg")


def writeIcon(appIcon, resolution, resName):
    baseName = os.path.basename(appIcon).split("_24px.")[0]
    baseName = baseName.replace(".svg", "")

    destName = "AppIcons/" + DirectoryResolution + "/" + baseName + resName + ".png"
    if os.path.isfile(destName):
        return

    w = " -w%i" % resolution
    h = " -h%i" % resolution
    print("writing ==> " + destName)

    fmt = "--export-png=" + destName + "  -C"
    fmt += w
    fmt += h
    subprocess.call("inkscape.bat %s %s" % (appIcon, fmt))


for DirectoryResolution in Outputs:
    ResolutionList = Outputs.get(DirectoryResolution)
    if not os.path.isdir("AppIcons/" + DirectoryResolution):
        os.mkdir("AppIcons/" + DirectoryResolution)

    resCnt = 1
    resolution = ResolutionList[1]
    resName = "_" + str(BaseResolutionName[0]) + "dp"
    for appIcon in AppIcons:
        writeIcon(appIcon, resolution, resName)

    resName = ""
    for appIcon in AppIconsMain:
        writeIcon(appIcon, ResolutionList[3], resName)
