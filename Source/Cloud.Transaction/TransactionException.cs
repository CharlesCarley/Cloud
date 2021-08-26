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
using Cloud.Transaction.Properties;

namespace Cloud.Transaction
{
    /// <summary>
    /// General exception type for known exceptions when preforming a transaction.
    /// </summary>
    public class TransactionException : Exception
    {
        /// <summary>
        /// Defines the code. This should be one of the constants
        /// declared in the static Constants class.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public TransactionException(int code, string message) :
            base(message)
        {
            Code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public TransactionException(int code)
        {
            Code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public TransactionException(int code, string message, Exception innerException) :
            base(message, innerException)
        {
            Code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        public TransactionException() :
            base(Resources.UnknownTransaction)
        {
            Code = Constants.UnknownError;
        }
    }
}
