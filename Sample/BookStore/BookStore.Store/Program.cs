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
using Cloud.Store;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BookStore.Store
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args);
            builder.UseStartup<DevelopmentStartup>();

            var host = new Cloud.Transaction.HostConfig {
                Host = "127.0.0.1",
                Port = 8080,
            };

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (!arg.Equals("--cfg")) continue;

                if (i + 1 < args.Length)
                {
                    var path = args[++i];
                    Console.WriteLine(
                        host.LoadFromStorage(path) ? Resources.LoadedConfig : Resources.ConfigNotLoaded,
                        path);
                }
                else
                    Console.WriteLine(Resources.MissingConfigArg);
            }

            builder.UseUrls($"http://{host.Host}:{host.Port}");
            builder.Build().Run();
        }
    }
}
