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

namespace Cloud.ReflectionApi
{
    /// <summary>
    /// Defines extra options that can be set on the attributes.
    /// Used as a bit flags.
    /// </summary>
    public enum ItemPropertyOptions
    {
        /// <summary>
        /// No extra options provided.
        /// </summary>
        None = 0,
        /// <summary>
        /// This option should be removed...
        /// </summary>
        IsAutoProperty = 0x001,

        /// <summary>
        /// This is used to store long multi line strings in .json
        /// so that it can be stored as a single string.
        /// IE; It's always encoded as base 64.
        /// </summary>
        MultiLineString = 0x002,
    }

    /// <summary>
    /// Represents a property value that should be tracked.
    /// </summary>
    public class ItemPropertyAttribute : Attribute {
        public ItemPropertyAttribute()
        {
        }


        public ItemPropertyAttribute(string defaultValue, ItemPropertyOptions extraOptions= ItemPropertyOptions.None)
        {
            Default = defaultValue;
            Options = extraOptions;
        }

        public ItemPropertyAttribute(ItemPropertyOptions extraOptions)
        {
            Options = extraOptions;
        }


        /// <summary>
        /// Defines the value's maximum allowed size.
        /// </summary>
        public int MaximumSize { get; set; } = int.MaxValue;

        /// <summary>
        /// Defines the value's minimum allowed size.
        /// </summary>
        public int MinimumSize { get; set; } = int.MinValue;

        /// <summary>
        /// Allows a default value to be specified.
        /// </summary>
        public string Default { get; set; } = string.Empty;

        /// <summary>
        /// ??
        /// </summary>
        public bool IsAutoProperty { get; set; }

        /// <summary>
        /// Extra options that can be set on this attribute.
        /// It is used to modify or extend the usage of the current attribute.
        /// </summary>
        public ItemPropertyOptions Options { get; set; }
    }
}
