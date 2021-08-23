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
using System.Threading.Tasks;
using Cloud.Common;
using Microsoft.AspNetCore.Mvc.Formatters;
using LogUtils = Cloud.Common.LogUtils;

namespace Cloud.Store
{
    public class PostBundleFormatter : InputFormatter
    {
        public static void LogPackage(string text)
        {
            if (text == null)
                return;
            LogUtils.Log("");
            LogUtils.Log("====================[ Package ]====================");
            LogUtils.Log(text);
            LogUtils.Log("");
            LogUtils.Log($"Size: {text.Length}");
            LogUtils.Log("");
        }


        public static void LogError(string text)
        {
            if (text == null)
                return;
            LogUtils.Log("");
            LogUtils.Log("====================[ Error ]====================");
            LogUtils.Log(text);
            LogUtils.Log("");
            LogUtils.Log($"Size: {text.Length}");
            LogUtils.Log("");
        }


        public PostBundleFormatter()
        {
            // Only allow application/octet-stream
            SupportedMediaTypes.Add(Constants.HttpContentApplicationString);
        }

        public override bool CanRead(InputFormatterContext context)
        {
            return context != null &&
                   context.HttpContext.Request.ContentType.Equals(Constants.HttpContentApplicationString);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            try
            {
                if (context is null)
                    return await InputFormatterResult.FailureAsync().ConfigureAwait(false);

                // read the main body..
                using var reader = new StreamReader(context.HttpContext.Request.Body);

                // this is the entire encrypted package bundle...
                var package = await reader.ReadToEndAsync().ConfigureAwait(false);


                if (string.IsNullOrEmpty(package))
                {
                    LogError("Missing Package....");
                    return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
                }

                var body = new PostBundle {
                    Content = package
                };

                // log it and route it down the line.
                LogPackage(package);
                return await InputFormatterResult.SuccessAsync(body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }

            return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
        }
    }
}
