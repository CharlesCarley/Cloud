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

using System.Security.Cryptography;
using System.Text;

namespace Cloud.Common
{
    /// <summary>Utility class to hash strings</summary>
    public static class ShaHashString {
        /// <summary>
        /// Returns a HexEncoded representation of the Sha512 version of the value.
        /// </summary>
        /// <param name="value">The original value.</param>
        public static string Get(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            using (var hash = SHA512.Create())
            {
                var input = Encoding.ASCII.GetBytes(value);
                var result = hash.ComputeHash(input);
                return StringUtils.ToHexString(result);
            }
        }

        /// <summary>
        /// Returns a HexEncoded representation of the Sha1 version of the value.
        /// </summary>
        /// <param name="value">The original value.</param>
        public static string GetSha1(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            using (var hash = SHA1.Create())
            {
                var input = Encoding.ASCII.GetBytes(value);
                var result = hash.ComputeHash(input);
                return StringUtils.ToHexString(result);
            }
        }
    }
}
