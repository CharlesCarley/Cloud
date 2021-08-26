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

using System.Threading.Tasks;
using Cloud.Common;
using Cloud.Transaction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BookStore.FrontEnd
{
    public static class StateManager {
        public static bool IsConnected { get; private set; }

        /// <summary>
        /// Test to determine the server's result
        /// </summary>
        public static bool HasElements { get; set; }

        /// <summary>
        /// Initialize application related backend systems.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="cfg"></param>
        public static void Initialize(IWebHostEnvironment env, IConfiguration cfg)
        {
            var client = (string)cfg.GetValue(typeof(string), "Client");
            if (!string.IsNullOrEmpty(client)) {
                Client.Database.Register(client);
            }

            var host    = cfg["DataBaseHostConfig:Host"];
            var port    = cfg["DataBaseHostConfig:Port"];
            var timeout = cfg["DataBaseHostConfig:Timeout"];

            Transaction.SetConnectionParameters(
                new HostConfig {
                    Host    = host,
                    Port    = StringUtils.ToIntRange(port, 80, ushort.MaxValue),
                    Timeout = StringUtils.ToIntRange(timeout, 1, 10000, 500),
                });

            CheckConnectionAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a get request to test a valid database connection.
        /// </summary>
        /// <param name="timeout">The amount of time in milliseconds to wait before giving up.</param>
        /// <returns>True if the transaction succeeded, otherwise returns false.</returns>
        /// <remarks>
        /// Sets the IsConnected property to the result of the ping.
        /// </remarks>
        public static async Task<bool> CheckConnectionAsync(int timeout = 0)
        {
            return await Task.Run(async () => {
                IsConnected = await Transaction.PingDatabaseAsync(timeout > 0 ? timeout : Transaction.Timeout);
                return IsConnected;
            });
        }
    }
}
