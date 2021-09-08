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
using System.Net;
using System.Threading.Tasks;
using Cloud.Common;
using static Cloud.Transaction.TransactionUtility;

namespace Cloud.Transaction
{
    // TODO: this also needs to account for asynchronous tasks

    /// <summary>
    /// </summary>
    public static class Transaction {
        /// <summary>
        ///
        /// </summary>
        private static HostConfig Config { get; set; }

        /// <summary>
        /// Provides read/write access to the timeout setting.
        /// This is used to determine how much time to wait for requests
        /// before giving up. Its unit is in milliseconds.
        /// </summary>
        public static int Timeout { get; set; } = Settings.DefaultTimeout;

        /// <summary>
        ///
        /// </summary>
        private static bool HasConfig => Config != null;

        /// <summary>
        ///
        /// </summary>
        public static string Host => Config?.Host;

        public static int Port => Config?.Port ?? -1;

        /// <summary>
        /// Attempts to set the HostConfig parameters.
        /// </summary>
        /// <param name="parameters">The parameter that should be used.</param>
        /// <remarks>
        /// This will throw an exception if the parameters do not match the following
        /// criteria.
        ///  + The parameters must not be null.
        ///  + The host must be a valid IP address or domain name.
        ///  + The port can be set to [80], [443], [1024-65535].
        /// </remarks>
        /// <exception cref="TransactionException">TransactionException</exception>
        public static void SetConnectionParameters(HostConfig parameters)
        {
            if (parameters is null)
                throw new TransactionException(Constants.NullArgument, nameof(HostConfig));

            if (!HostConfig.IsValidHostName(parameters.Host))
                throw new TransactionException(Constants.InvalidHost, "Invalid host name");

            if (!HostConfig.IsValidPort(parameters.Port))
                throw new TransactionException(Constants.InvalidPort, "An invalid port was supplied.");

            if (parameters.Timeout < Settings.MinTimeOut)
                parameters.Timeout = Settings.MinTimeOut;
            if (parameters.Timeout > Settings.MaxTimeOut)
                parameters.Timeout = Settings.MaxTimeOut;

            Config  = parameters;
            Timeout = parameters.Timeout;
        }

        /// <summary>
        /// Wraps the SetConnectionParameters method around try / catch.
        /// </summary>
        public static void TrySetConnectionParameters(HostConfig parameters)
        {
            try {
                SetConnectionParameters(parameters);
            } catch (TransactionException ex) {
                LogUtils.Log(LogLevel.Error,
                             nameof(SetConnectionParameters),
                             ex.Message);
            }
        }

        public static Receipt Dispatch(Content content, int method, int contentType)
        {
            // NOTE: this is mainly for testing purposes so, do not
            // spend to much time implementing the details for each method.
            // Under normal operating constraints, the actual protocol method
            // will be restricted to, post only, or possibly get for a few calls.
            // The same thing applies to the content type.
            if (!HasConfig)
                throw new TransactionException();

            return ProcessImpl(content,
                               method,
                               contentType,
                               Constants.HttpSchemeHttp);
        }

        private static string ConstructUriPath(string path, int function, int table)
        {
            // Note: this assumes that function and table define their initial state
            //       to be interpreted as: val < 0 == undefined && val > 0 == functionRoute|tableRoute
            if (function <= 0)
                return path;

            return table > 0 ? $"{path}{function}/{table}" : $"{path}{function}";
        }

        private static Receipt ProcessImpl(Content content, int method, int contentType, int scheme)
        {
            var methodString = IntHttpMethodToString(method);
            if (string.IsNullOrEmpty(methodString))
                throw new TransactionException(Constants.InvalidMethod);

            // Prep the HTTP content-type
            var contentTypeString = IntHttpContentTypeToString(contentType);
            if (string.IsNullOrEmpty(contentTypeString))
                throw new TransactionException(Constants.InvalidContentType);

            var schemeName = IntHttpSchemeToString(scheme);
            if (string.IsNullOrEmpty(contentTypeString))
                throw new TransactionException(Constants.InvalidSchemeType);

            var uri = new UriBuilder {
                Scheme = schemeName,
                Host   = Config.Host,
                Port   = Config.Port,
                Path   = ConstructUriPath("/", content.Function, content.Table),
            };

            var webRequest         = WebRequest.Create(uri.Uri);
            webRequest.Method      = methodString;
            webRequest.ContentType = contentTypeString;
            webRequest.Timeout     = Timeout;

            var result = new Receipt {
                Code = Constants.ReturnNull,
            };

            if (method == Constants.HttpPost) {
                // only write the bundle with the post method.
                try {
                    var request = webRequest.GetRequestStream();

                    using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        content.WritePayload(writer, content.Payload);
                        writer.FlushAsync();
                    }

                    request.Dispose();

                } catch (IOException ex) {
                    throw new TransactionException(
                        Constants.CannotPackagePayload,
                        ex.Message);
                } catch (NotImplementedException ex) {
                    throw new TransactionException(
                        Constants.CannotProcessRequest,
                        ex.Message);
                }
            }

            // Setup for a response
            try {
                var response = webRequest.GetResponse().GetResponseStream();

                // Guard against the off chance GetResponseStream
                // doesn't throw an exception on error..
                if (response == null) {
                    throw new TransactionException(
                        Constants.StaticAnalysisBugOrMSFault,
                        "null return value from get response stream.");
                }

                using (var responseReader = new StreamReader(response))
                {
                    var responseString = responseReader.ReadToEnd();
                    result.Process(responseString);
                }

                response.Dispose();

            } catch (IOException ex) {
                throw new TransactionException(
                    Constants.IORead,
                    ex.Message);
            } catch (OutOfMemoryException ex) {
                throw new TransactionException(
                    Constants.OutOfMemory,
                    ex.Message);
            } catch (NotSupportedException ex) {
                throw new TransactionException(
                    Constants.CannotProcessResponse,
                    ex.Message);
            } catch (WebException ex) {
                throw new TransactionException(
                    ReplicateExceptionStatus(ex),
                    ex.Message);
            }

            return result;
        }

        private static WebRequest CreateRequest(int    method,
                                                int    contentType,
                                                int    scheme,
                                                int    timeout, // not checked
                                                string path)
        {
            if (Config is null)
                throw new TransactionException(Constants.InvalidHost);

            var methodString = IntHttpMethodToString(method);
            if (string.IsNullOrEmpty(methodString))
                throw new TransactionException(Constants.InvalidMethod);

            // Prep the HTTP content-type
            var contentTypeString = IntHttpContentTypeToString(contentType);
            if (string.IsNullOrEmpty(contentTypeString))
                throw new TransactionException(Constants.InvalidContentType);

            var schemeName = IntHttpSchemeToString(scheme);
            if (string.IsNullOrEmpty(contentTypeString))
                throw new TransactionException(Constants.InvalidSchemeType);

            var uri = new UriBuilder {
                Scheme = schemeName,
                Host   = Config.Host,
                Port   = Config.Port,
                Path   = path,
            };

            var webRequest         = WebRequest.Create(uri.Uri);
            webRequest.Method      = methodString;
            webRequest.ContentType = contentTypeString;
            webRequest.Timeout     = timeout;
            return webRequest;
        }

        public static bool PingDatabaseImpl(int timeout)
        {
            try {
                var webRequest = CreateRequest(
                    Constants.HttpGet,
                    Constants.HttpContentApplication,
                    Constants.HttpSchemeHttp,
                    timeout,
                    Constants.Status.ToString());
                var response = webRequest.GetResponse();
                response.Dispose();
                return true;
            } catch (Exception ex) {
                LogUtils.Log(ex.Message);
                return false;
            }
        }

        public static async Task<bool> PingDatabaseAsync(int timeout)
        {
            try {
                var webRequest = CreateRequest(
                    Constants.HttpGet,
                    Constants.HttpContentApplication,
                    Constants.HttpSchemeHttp,
                    timeout,
                    Constants.Status.ToString());
                var response = await webRequest.GetResponseAsync();
                response.Dispose();
                return true;
            } catch (Exception ex) {
                LogUtils.Log(ex.Message);
                return false;
            }
        }

        public static bool PingDatabase(int timeout)
        {
            try {
                return PingDatabaseImpl(timeout);
            } catch {
                return false;
            }
        }
    }
}
