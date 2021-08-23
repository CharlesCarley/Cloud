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

namespace Cloud.GeneratorApi
{
    /// <summary>
    ///
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        ///
        /// </summary>
        String,
        /// <summary>
        ///
        /// </summary>
        Integer,
        /// <summary>
        ///
        /// </summary>
        Real,
        /// <summary>
        ///
        /// </summary>
        DateAndTime
    }

    /// <summary>
    ///
    /// </summary>
    public class StoreItemProperty {
        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public PropertyType Type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int    MaximumSize { get; set; }
        public int    MinimumSize { get; set; }
        public bool   IsAutoProperty { get; set; }
        public string Default { get; set; }

        public ReflectionApi.ItemPropertyAttribute Property { get; set; }
    }
}
