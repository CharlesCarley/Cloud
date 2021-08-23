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

using System.IO;
using Cloud.Common;

namespace Cloud.Transaction {
    public class Content {
        public int    Table { get; set; }    = Constants.Undefined;
        public int    Function { get; set; } = Constants.Undefined;
        public Bundle Payload { get; }

        public Content(string data) {
            Payload = new Bundle {
                Package      = data,
                Type = BundleType.String,
                Revision     = Constants.Undefined,
                ServerId     = Constants.Undefined,
                TableId      = Constants.Undefined,
                UserId       = Constants.Undefined
            };
        }
        public Content(int data)
        {
            Payload = new Bundle
            {
                Package = data.ToString(),
                Type = BundleType.Int,
                Revision = Constants.Undefined,
                ServerId = Constants.Undefined,
                TableId = Constants.Undefined,
                UserId = Constants.Undefined
            };
        }

        public Content(Bundle data) {
            Payload = data;
            if (Payload != null)
                Payload.Type = BundleType.Json;
        }

        public Content() {
            Payload = new Bundle {
                Package  = null,
                Type = BundleType.None,
                Revision = Constants.Undefined,
                ServerId = Constants.Undefined,
                TableId  = Constants.Undefined,
                UserId   = Constants.Undefined
            };
        }

        /// <summary>
        /// Writes the supplied bundle to the supplied stream.
        /// </summary>
        /// <param name="writer">The stream to write to.</param>
        /// <param name="requestPayload">The bundle to write.</param>
        public void WritePayload(StreamWriter writer, Bundle requestPayload) {
            if (writer is null || requestPayload is null) {
                // The desired behavior is to skip writing any data
                // and just query a get request.
                return;
            }

            var raw = requestPayload.Pack();
            if (!string.IsNullOrEmpty(raw))
                writer.Write(raw);
        }
    }
}
