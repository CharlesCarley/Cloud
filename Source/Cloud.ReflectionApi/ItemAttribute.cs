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
    /// Represents a type that should be tracked.
    /// </summary>
    public class ItemAttribute : Attribute {
        /// <summary>
        /// Defines the name of the generated type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines a unique code for this type.
        /// Its uniqueness is only relevant in the context of a single API.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// This allows for local only types.
        /// If this is set to false, then only client code will be generated.
        /// If this is set to true then the code to wrap and transmit this type will be generated.
        /// </summary>
        public bool CanSynchronize { get; set; } = false;
    }
}
