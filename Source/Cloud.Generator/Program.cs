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
using Cloud.Common;

namespace Cloud.Generator
{
    /// <summary>
    /// Command line entry point for the Cloud.Generator program.
    /// </summary>
    public class Program {
        public static void Main(string[] args)
        {
            var app = new Generator();
            ParseCommandLineOptions(args, app);

            if (string.IsNullOrEmpty(app.Input) || string.IsNullOrEmpty(app.Output)) {
                LogUtils.Log(Resources.MissingParameters);
                Environment.Exit(1);
            } else if (!IsValidType(app.Type)) {
                LogUtils.Log(Resources.InvalidBackend, app.Type);
                LogUtils.Log(Resources.ValidBackends);
                Environment.Exit(1);
            } else
                app.Run();
        }

        private static void ParseCommandLineOptions(IReadOnlyList<string> args, Generator app)
        {
            for (var index = 0; index < args.Count; index++) {
                var arg = args[index];
                switch (arg) {
                case "-i" when index + 1 < args.Count:
                    app.Input = args[index + 1];
                    if (app.Input.StartsWith("-")) {
                        // missing argument
                        LogUtils.Log(Resources.MissingInput);
                        Environment.Exit(1);
                    }

                    ++index;
                    break;
                case "-n" when index + 1 < args.Count:
                    app.Namespace = args[index + 1];
                    if (app.Input.StartsWith("-")) {
                        // missing argument
                        LogUtils.Log(Resources.MissingInput);
                        Environment.Exit(1);
                    }

                    ++index;
                    break;
                case "-i":
                    // missing argument
                    LogUtils.Log(Resources.MissingInput);
                    Environment.Exit(1);
                    break;
                case "-o" when index + 1 < args.Count:
                    app.Output = args[index + 1];
                    if (app.Output.StartsWith("-")) {
                        // missing argument
                        LogUtils.Log(Resources.MissingOutput);
                        Environment.Exit(1);
                    }

                    ++index;
                    break;
                case "-o":
                    // missing argument
                    LogUtils.Log(Resources.MissingOutput);
                    Environment.Exit(1);
                    break;
                case "-t" when index + 1 < args.Count:
                    app.Type = args[index + 1];
                    if (app.Type.StartsWith("-")) {
                        // missing argument
                        LogUtils.Log(Resources.MissingType);
                        Environment.Exit(1);
                    }
                    ++index;
                    break;
                case "-t":
                    // missing argument
                    LogUtils.Log(Resources.MissingType);
                    Environment.Exit(1);
                    break;
                case "-v":
                    app.Verbose = true;
                    break;
                }
            }
        }

        private static bool IsValidType(string appType)
        {
            if (string.IsNullOrEmpty(appType))
                return false;
            if (appType.Equals("ClientSQLite"))
                return true;
            return appType.Equals("ServerSQLite") || appType.Equals("ServerMySQL");
        }
    }
}
