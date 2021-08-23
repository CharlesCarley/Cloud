/*
-------------------------------------------------------------------------------
    Copyright (c) Charles Carley.

  This software is provided 'as-is', without any express or implied
  warranty. In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
-------------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ResourceCompiler {

    public class Application {
        private int   _curArg;
        public string Namespace { get; set; }
        public string Output { get; set; }
        public string ClassName { get; set; }

        private readonly List<string> _sourceFiles;

        public Application()
        {
            _curArg      = 0;
            _sourceFiles = new List<string>();
        }

        public void GenerateFile()
        {
            if (string.IsNullOrEmpty(Namespace))
                Namespace = nameof(ResourceCompiler);
            if (string.IsNullOrEmpty(ClassName))
                ClassName = "Data";

            var builder = new StringWriter();
            var buffer  = new StringBuilder();

            foreach (var file in _sourceFiles) {
                var src      = ReadFileAsString(file);
                var baseName = Path.GetFileNameWithoutExtension(file);

                buffer.Clear();
                for (var i = 0; i < src.Length; i++) {
                    int iChar = src[i];
                    if (i > 0)
                        buffer.Append(',');
                    if (iChar >= 0 && iChar <= 255)
                        buffer.Append($"0x{iChar:X2}");
                }

                var privateTemplate = Resources.PrivateImpl;
                privateTemplate     = privateTemplate.Replace("${BaseName}", baseName);
                privateTemplate     = privateTemplate.Replace("${Bytes}", buffer.ToString());
                builder.WriteLine(privateTemplate);
                builder.Write(Space(8));
                builder.WriteLine($"public static string {baseName} => GetString(_{baseName});");
            }

            var main = Resources.Main;
            main     = main.Replace("${Namespace}", Namespace);
            main     = main.Replace("${ClassName}", ClassName);
            main     = main.Replace("${PrivateImpl}", builder.ToString());

            File.WriteAllText(Output, main);
        }

        public bool ParseArguments(string[] args)
        {
            if (args == null || args.Length < 2)
                throw new Exception(Resources.Usage);

            string opt;

            while ((opt = NextOpt(args)) != null) {
                if (!IsOpt(opt)) {
                    // if it's not an option, it's a file.

                    if (!File.Exists(opt))
                        throw new Exception($"The supplied input file '{opt}' does not exist.");

                    _sourceFiles.Add(opt);
                } else if (opt.Equals("-n")) {
                    // handle the namespace option

                    opt = NextOpt(args);
                    if (opt == null)
                        break;

                    if (IsOpt(opt))
                        throw new Exception("Expected an argument to -n.");

                    Namespace = opt;
                } else if (opt.Equals("-o")) {
                    // handle the output file option

                    opt = NextOpt(args);
                    if (opt == null)
                        break;

                    if (IsOpt(opt))
                        throw new Exception("Expected an argument to -o.");

                    Output = opt;
                } else if (opt.Equals("-c")) {
                    // handle the class name option

                    opt = NextOpt(args);
                    if (opt == null)
                        break;

                    if (IsOpt(opt))
                        throw new Exception("Expected an argument to -c.");

                    ClassName = opt;
                }
            }

            if (string.IsNullOrEmpty(Output))
                throw new Exception("Missing output file.");

            return true;
        }

        private static string Space(int c = 4)
        {
            return WriteSpace(c);
        }

        private static string WriteSpace(int cnt)
        {
            var r = "";
            while (cnt-- > 0)
                r += " ";
            return r;
        }

        public static string ReadFileAsString(string path)
        {
            if (!File.Exists(path))
                return string.Empty;
            try {
                var builder = new StringBuilder();
                var fp      = File.OpenRead(path);

                var buffer = new char[128];
                using (var reader = new StreamReader(fp))
                {
                    int ar;
                    while ((ar = reader.Read(buffer, 0, 128)) > 0)
                        builder.Append(new string(buffer, 0, ar));
                }

                fp.Close();
                return builder.ToString();
            } catch {
                // ignore
                return string.Empty;
            }
        }

        private static bool IsOpt(string inp) { return inp != null && inp[0] == '-'; }

        private string NextOpt(string[] args)
        {
            if (_curArg + 1 > args.Length) return null;
            var ret = args[_curArg++];
            return ret;
        }
    }

    class Program {
        private static void Main(string[] args)
        {
            try {
                var app = new Application();
                if (app.ParseArguments(args))
                    app.GenerateFile();

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(Resources.Usage);
                Environment.Exit(-1);
            }
        }
    }
}
