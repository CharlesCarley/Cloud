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

using Cloud.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace BookStore.Test
{
    // This test is setup to start a local in memory server.
    // The command line application should write elements to the server.
    // The output from the program should be tested against the generated server API.

    [TestClass]
    public class TestCommandLineInterface {
#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif
        private static readonly string OutDir = $"bin/{Configuration}/netcoreapp3.1/";

        // This defines the number of directories needed to back
        // out of the output directory and into the solution directory.
        // <framework>/<configuration>/<bin>/<project directory>/<BookStore>
        private const string DirectoryOffset = "/../../../../";

        private static readonly string CommonDirectory = Path.GetFullPath($"{AppContext.BaseDirectory}/{DirectoryOffset}");
        private static readonly string CliProgram      = Path.GetFullPath($"{CommonDirectory}/BookStore.Cli/{OutDir}/BookStore.Cli.exe");
        private static readonly string CliDirectory    = Path.GetFullPath($"{CommonDirectory}/BookStore.Cli/{OutDir}/");

        private static LocalTestServer _server;

        public static string Spawn(string program, string args, int expectedReturn = 0)
        {
            var proc = Process.Start(new ProcessStartInfo(program) {
                CreateNoWindow         = true,
                RedirectStandardOutput = true,
                UseShellExecute        = false,
                WorkingDirectory       = CliDirectory,
                Arguments              = args
            });

            Assert.IsNotNull(proc);
            var output = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();
            Assert.AreEqual(expectedReturn, proc.ExitCode);
            return output;
        }

        [TestInitialize]
        public void Initialize()
        {
            _server = new LocalTestServer();
            _server.Start();

            if (File.Exists($"{CliDirectory}/BookStore.Cli.db")) {
                File.Delete($"{CliDirectory}/BookStore.Cli.db");
            }

            Spawn(CliProgram, "config host 127.0.0.1");
            Spawn(CliProgram, "config port 5000");
            Spawn(CliProgram, "config timeout 8000");

            Assert.IsTrue(File.Exists($"{CliDirectory}/BookStore.Cli.db"));
            var outString = Spawn(CliProgram, "config list");

            var a = StringUtils.StringToBytes(outString);
            var b = StringUtils.StringToBytes("{\n" +
                                              "    \"Identifier\": 1,\n" +
                                              "    \"Key\": \"MainSettings\",\n" +
                                              "    \"Timeout\": 8000,\n" +
                                              "    \"Host\": \"127.0.0.1\",\n" +
                                              "    \"Port\": 5000\n" +
                                              "}" + Environment.NewLine);

            Assert.AreEqual(a.Length, b.Length);
            for (var i = 0; i < a.Length; i++) {
                var ba = a[i];
                var bb = b[i];
                Assert.AreEqual(ba, bb);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server?.Stop();
            _server = null;
        }

        [TestMethod]
        public void TestPing()
        {
            var ping = Spawn(CliProgram, "ping");
            Assert.AreEqual(
                $"Connected{Environment.NewLine}" ,
                ping);
        }
    }
}
