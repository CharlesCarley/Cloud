# -----------------------------------------------------------------------------
#
#   Copyright (c) Charles Carley.
#
#   This software is provided 'as-is', without any express or implied
# warranty. In no event will the authors be held liable for any damages
# arising from the use of this software.
#
#   Permission is granted to anyone to use this software for any purpose,
# including commercial applications, and to alter it and redistribute it
# freely, subject to the following restrictions:
#
# 1. The origin of this software must not be misrepresented; you must not
#    claim that you wrote the original software. If you use this software
#    in a product, an acknowledgment in the product documentation would be
#    appreciated but is not required.
# 2. Altered source versions must be plainly marked as such, and must not be
#    misrepresented as being the original software.
# 3. This notice may not be removed or altered from any source distribution.
# ------------------------------------------------------------------------------
import sys, os, subprocess

def trim(line):
    line = line.replace('\t', '')
    line = line.replace('\n', '')
    line = line.replace(' ', '')
    return line
    
def initModules():
    subprocess.call("git submodule init")
    subprocess.call("git submodule update --init --merge")

def updateModules(path):
    print("==> " + path)
    subprocess.call("git checkout master")
    subprocess.call("git pull")

def main():
    initModules()
    currentDir = os.getcwd()

    gitModules = currentDir + os.sep + ".gitmodules"

    if (not os.path.isfile(gitModules)):
        print("No .gitmodule found in", currentDir, "\nNothing to update...")
        return

    file = open(gitModules, mode = 'r')
    lines = file.readlines()

    for line in lines:
        line = trim(line)
        if (line.find("path=") != -1):

            path = line.replace("path=", '')
            try:
                os.chdir(currentDir + os.sep + path)
            except:
               print("Could not change directory to %s"%path)

            updateModules(path)

            os.chdir(currentDir)

if __name__== '__main__':
    main()