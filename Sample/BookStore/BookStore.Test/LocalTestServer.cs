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
using System.IO;
using Cloud.Transaction;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BookStore.Test
{
    public class LocalTestServer {
        private static string Content => AppContext.BaseDirectory + "/../../../LocalTestServer/";

        private readonly IWebHost _host;
        private HostConfig        _hostConfig;

        public LocalTestServer(int port = 14587)
        {
            _host = InitializeHost(port);
        }

        private IWebHost InitializeHost(int port)
        {
            _hostConfig = new HostConfig {
                Host = "127.0.0.1",
                Port = port
            };

            // set up a temp destination directory

            if (!Directory.Exists(Content))
                Directory.CreateDirectory(Content);
            else {
                Directory.Delete(Content, true);
                Directory.CreateDirectory(Content);
            }

            Client.Database.Register($"{Content}\\LocalTestServer.Client.db");
            Store.Database.Register($"{Content}\\LocalTestServer.Server.db");

            var host = WebHost.CreateDefaultBuilder();
            host.UseStartup<Store.DevelopmentStartup>();
            host.UseContentRoot(Content);
            host.UseEnvironment("Development");

            string[] url = { _hostConfig.GetRootUrl() };
            host.UseUrls(url);
            host.UseIISIntegration();

            // initialize transaction
            Transaction.SetConnectionParameters(_hostConfig);
            Transaction.Timeout = 999999999;
            return host.Build();
        }

        public void Start()
        {
            _host.StopAsync().Wait();
            _host.StartAsync();
        }

        public void Stop()
        {
            _host.StopAsync().Wait();
            _host.Dispose();

            if (Directory.Exists(Content))
                Directory.Delete(Content, true);
        }
    }
}
