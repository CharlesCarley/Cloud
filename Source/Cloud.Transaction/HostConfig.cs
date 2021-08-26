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
using System.Security.Cryptography;
using Cloud.Common;

namespace Cloud.Transaction
{
    /// <summary>
    /// HostConfig is used to privately specify the connection's address and port.
    /// </summary>
    public class HostConfig {
        /// <summary>
        /// Either the host domain name alias or the direct IP address.
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        /// Specifies the port on the host.
        /// </summary>
        public int Port { get; set; } = 80;

        /// <summary>
        /// Specifies transaction's timeout rate (in ms).
        /// </summary>
        public int Timeout { get; set; } = 1000;

        /// <summary>
        /// Allows access to the stores key files.
        /// </summary>
        public string CredentialStore { get; private set; }

        /// <summary>
        /// Loads transaction arguments from the file system.
        /// </summary>
        /// <param name="storage">Path to the json file.</param>
        /// <returns>True if the file was loaded </returns>
        public bool LoadFromStorage(string storage)
        {
            try
            {
                if (!File.Exists(storage))
                    return false;

                var obj = JsonObject.ParseFile(storage);
                if (obj == null)
                    return false;

                ExtractParameters(obj);

                // this is tied in via utils because it needs to
                // be available in server code as well.
                Common.CredentialStore.LoadFromStorage(CredentialStore);
                return true;

            } catch (Exception ex) {
                LogUtils.Log(typeof(HostConfig), ex.Message);
                return false;
            }
        }

        public static bool IsValidPort(int port)
        {
            if (port <= 1023) {
                if (port != 80 && port != 443)
                    return false;
            } else if (port > ushort.MaxValue)
                return false;

            return true;
        }

        public static bool IsValidHostName(string host)
        {
            return StringUtils.IsIpAddressOrDomain(host);
        }

        private void ExtractParameters(JsonObject obj)
        {
            var host = obj.AsString("Host", "localhost");
            if (IsValidHostName(host))
                Host = host;

            var port = obj.AsInt("Port", 80);
            if (IsValidPort(port))
                Port = port;

            CredentialStore = GetCredentialStore();

            Timeout = obj.AsInt("Timeout", Settings.DefaultTimeout);
            if (Timeout < Settings.MinTimeOut)
                Timeout = Settings.MinTimeOut;
            if (Timeout > Settings.MaxTimeOut)
                Timeout = Settings.MaxTimeOut;
        }

        public string GetRootUrl()
        {
            return $"http://${Host}:{Port}";
        }

        private static string GetCredentialStore()
        {
            var env = Environment.GetEnvironmentVariables();
            if (!env.Contains("TransactionStore"))
                return string.Empty;

            var store = env["TransactionStore"];
            return store != null ? store.ToString() : string.Empty;
        }

        public RSAParameters GetPublicKey()
        {
            return Common.CredentialStore.GetPublicKey();
        }

        public RSAParameters GetPrivateKey()
        {
            return Common.CredentialStore.GetPrivateKey();
        }
    }
}
