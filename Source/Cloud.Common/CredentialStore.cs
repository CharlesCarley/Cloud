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
using System.Xml.Serialization;

namespace Cloud.Common
{
    /// <summary>
    ///
    /// </summary>
    public class CredentialStore {
        /// <summary>
        ///
        /// </summary>
        public static string PublicKeyPath { get; private set; } = string.Empty;
        /// <summary>
        ///
        /// </summary>
        public static string PrivateKeyPath { get; private set; } = string.Empty;

        /// <summary>
        /// Attempts to load the credential file from the supplied path.
        /// </summary>
        /// <param name="path">Full path to the credential storage file.</param>
        /// <returns>false if the file cannot be loaded otherwise true.</returns>
        public static bool LoadFromStorage(string path)
        {
            try {
                var obj = JsonObject.ParseFile(path);
                if (obj == null) {
                    LogUtils.Log(Resources.CredentialLoadError);
                    return false;
                }

                PrivateKeyPath = obj.AsString("PrivateKey", string.Empty);
                PublicKeyPath  = obj.AsString("PublicKey", string.Empty);

                return true;
            } catch (Exception ex) {
                LogUtils.Log(typeof(CredentialStore), ex.Message);
            }
            return true;
        }

        private static RSAParameters GetKey(string path)
        {
            var serializer = new XmlSerializer(typeof(RSAParameters));
            try {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException(nameof(path));

                var fileStream = new FileStream(path, FileMode.Open);
                var result     = (RSAParameters)serializer.Deserialize(fileStream);
                fileStream.Close();
                return result;
            } catch (FileNotFoundException) {
                LogUtils.Log(Resources.KeyLoadError);
            } catch (Exception ex) {
                LogUtils.Log(ex.Message);
                LogUtils.Log(Resources.KeyLoadError);
            }
            return new RSAParameters();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static RSAParameters GetPublicKey()
        {
            return GetKey(PublicKeyPath);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static RSAParameters GetPrivateKey()
        {
            return GetKey(PrivateKeyPath);
        }
    }
}
