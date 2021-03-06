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
using System.Collections.Generic;
using Cloud.Common;

namespace Cloud.Transaction
{
    public enum ReceiptCode
    {
        Undefined,
        Fail,
        Success,
        True,
        False,
        None,
        List,
        String,
        Json,
        Int
    }

    public class Receipt {
        public ReceiptCode Code { get; internal set; }

        public Bundle Value { get; private set; }

        public bool HasPackagedContent
        {
            get {
                if (Value != null)
                    return !string.IsNullOrEmpty(Value.Package);
                return false;
            }
        }

        private bool CheckReturn(string stream)
        {
            if (string.IsNullOrEmpty(stream)) {
                // ignore here, because the server might
                // not have returned anything.
                Value = null;
                Code   = ReceiptCode.Undefined;
                return true;
            }

            switch (stream) {
            case Constants.ReturnFailure:
                Code = ReceiptCode.Fail;
                return true;
            case Constants.ReturnSuccess:
                Code = ReceiptCode.Success;
                return true;
            case Constants.ReturnTrue:
                Code = ReceiptCode.True;
                return true;
            case Constants.ReturnFalse:
                Code = ReceiptCode.False;
                return true;
            case Constants.ReturnNone:
                Code = ReceiptCode.None;
                return true;
            default:
                return false;
            }
        }

        internal void Process(string stream)
        {
            if (CheckReturn(stream))
                return;

            stream = StringUtils.FromBase64(stream);
            if (string.IsNullOrEmpty(stream)) {
                Code   = ReceiptCode.Fail;
                Value = null;
                LogUtils.Log(LogLevel.Error,
                             nameof(Process),
                             "Failed to extract a base 64 return value from the server's response.");
                return;
            }

            if (stream.StartsWith("{")) {
                // try and unwrap the json bundle.
                Value = (Bundle)JsonParser.Unwrap(stream, typeof(Bundle), false);

                // it's composed of two json files, codes and content.
                if (Value != null && !string.IsNullOrEmpty(Value.Package)) {
                    // the package is the content.
                    Value.Package = StringUtils.FromBase64(Value.Package);
                    Code           = ReceiptCode.Json;
                } else {
                    LogUtils.Log(LogLevel.Error,
                                 nameof(Process),
                                 "Failed to split the package bundle");

                    Code = ReceiptCode.Fail;
                }

            } else if (stream.StartsWith("[")) {
                // The server returned an int list
                Code   = ReceiptCode.List;
                Value = new Bundle {
                    Package = stream
                };
            } else {
                Code = ReceiptCode.Fail;
                LogUtils.Log(LogLevel.Error,
                             nameof(Process),
                             $"this '{stream}' value needs to be processed");
            }
        }

        public List<int> UnpackList()
        {
            var list = new List<int>();
            if (Value?.Package != null && Code == ReceiptCode.List)
                StringUtils.ToIntArrayNoCount(Value.Package, ref list);
            return list;
        }

        public string UnpackString()
        {
            if (Value?.Package != null && Code == ReceiptCode.String)
                return StringUtils.FromBase64(Value.Package);
            return string.Empty;
        }

        public int UnpackInteger()
        {
            if (Value?.Package != null && Code == ReceiptCode.Int)
                return StringUtils.ToInt(Value.Package);
            return -1;
        }

        public JsonObject UnpackJson()
        {
            if (Value?.Package != null && Code == ReceiptCode.Json)
                return JsonObject.TryParse(Value.Package);
            return null;
        }

        public object UnpackJson<T>()
        {
            if (Value?.Package != null && Code == ReceiptCode.Json)
                return JsonParser.Unwrap(Value.Package, typeof(T), false);
            return null;
        }
    }
}
