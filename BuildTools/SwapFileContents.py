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
import sys, os

def replaceContents(path, withContent):
    file = open(path, 'w')
    file.write(withContent)
    file.close()

def swapFileContent(rootPath, fileName, content):

    for root, dirs, files in os.walk(rootPath):
        root = os.path.abspath(root)

        for file in files:
            if (file == fileName):
                path = root + os.sep + file
                if os.path.isfile(path):
                    replaceContents(path, content)

def main(argc, argv):
    if (argc < 2):
        print("Usage <> <input> .")
        return


    input = argv[1]
    rootPath = argv[2]

    if (not os.path.isfile(input)):
        print("no such file, ", input);
        return
    
    if (not os.path.isdir(rootPath)):
        print("no such directory, ", rootPath);
        return

    file = open(input, "r")
    content = file.read()
    file.close()

    if (len(content) == 0):
        print(input, "has no usable content to swap")
    else:
        fileName = os.path.basename(input)
        swapFileContent(rootPath, fileName, content)


if __name__=='__main__':
    main(len(sys.argv), sys.argv)

