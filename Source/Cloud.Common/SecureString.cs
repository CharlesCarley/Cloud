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
using System.Security.Cryptography;
using System.Text;

namespace Cloud.Common
{
    public class SecureString {
        private readonly string _value;

        private SecureString(string input)
        {
            _value = input;
        }

        private string HandleInput(bool encrypt, RSAParameters key)
        {
            if (string.IsNullOrEmpty(_value))
                return null;

            byte[] bytes;
            if (encrypt) {
                var encoding = new ASCIIEncoding();
                bytes        = encoding.GetBytes(_value);
            } else
                bytes = Convert.FromBase64String(_value);

            var rsa = new RSACryptoServiceProvider {
                PersistKeyInCsp = true
            };

            var result = encrypt
                             ? EncryptedImpl(bytes, key)
                             : DecryptedImpl(bytes, key);
            if (!(result is null))
                return encrypt ? Convert.ToBase64String(result) : Encoding.ASCII.GetString(result);
            return null;
        }

        private static byte[] EncryptedImpl(byte[] value, RSAParameters exportParameters)
        {
            if (value is null)
                return null;

            byte[] result;
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(exportParameters);
            try {
                result = rsa.Encrypt(value, false);
            } catch (CryptographicException ex) {
                LogUtils.Log(ex.Message);
                result = null;
            }

            return result;
        }

        private static byte[] DecryptedImpl(byte[] value, RSAParameters exportParameters)
        {
            if (value is null)
                return null;

            byte[] result;
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(exportParameters);

            try {
                result = rsa.Decrypt(value, false);
            } catch (CryptographicException ex) {
                result = null;
                LogUtils.Log(ex.Message);
            }

            return result;
        }

        public static string Encrypt(string value, RSAParameters publicKey)
        {
            return new SecureString(value).HandleInput(true, publicKey);
        }

        public static string Decrypt(string value, RSAParameters privateKey)
        {
            return new SecureString(value).HandleInput(false, privateKey);
        }
    }
}
