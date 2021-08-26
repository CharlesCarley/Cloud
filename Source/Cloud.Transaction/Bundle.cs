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
using Cloud.Common;

namespace Cloud.Transaction
{
    public enum BundleType
    {
        None,
        /// <summary>
        ///
        /// </summary>
        Json,
        /// <summary>
        ///
        /// </summary>
        String,
        /// <summary>
        ///
        /// </summary>
        Int,
    }

    /// <summary>
    /// Defines a package for transmission
    /// </summary>
    /// <remarks>
    /// A bundle here can have the following forms:
    /// <code>
    /// {
    ///     "ServerId": (id on the server),
    ///     "Revision": (revision number),
    ///     "TableId":  (Class UniqueIdentifier),
    ///     "Key":      (Lookup identifier),
    ///     "Package":  (Content to send),
    /// }
    /// or, as a plain base 64 string representation.
    /// </code>
    /// </remarks>
    public class Bundle {
        /// <summary>
        ///
        /// </summary>
        public int ServerId { get; set; } = Constants.Undefined;

        /// <summary>
        ///
        /// </summary>
        public int Revision { get; set; } = Constants.Undefined;

        /// <summary>
        ///
        /// </summary>
        public int TableId { get; set; } = Constants.Undefined;

        /// <summary>
        ///
        /// </summary>
        public int UserId { get; set; } = Constants.Undefined;

        /// <summary>
        ///
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        ///
        /// </summary>
        public string Package { get; set; } = string.Empty;

        public BundleType Type { get; set; } = BundleType.Json;

        private string PackageAsJson()
        {
            var obj = new JsonObject();

            obj.AddValue("ServerId", ServerId);
            obj.AddValue("Revision", Revision);
            obj.AddValue("TableId", TableId);
            obj.AddValue("UserId", UserId);
            obj.AddValue("Key", Key);
            obj.AddValue("Package", StringUtils.ToBase64(Package));
            return obj.AsBase64();
        }

        public string Pack()
        {
            switch (Type) {
            case BundleType.Json:
                return PackageAsJson();
            case BundleType.String:
            case BundleType.Int:
                return PackageAsBase64();
            case BundleType.None:
                return string.Empty;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        private string PackageAsBase64()
        {
            return StringUtils.ToBase64(Package);
        }
    }
}
