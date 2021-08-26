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
using System.Net;
using Cloud.Common;

namespace Cloud.Transaction {
    /// <summary>
    /// The TransactionUtility class is for internal only utility functions.
    /// </summary>
    internal static class TransactionUtility {
        /// <summary>
        /// Converts the enumerated HTTP method (HttpGet, HttpPost...)
        /// to its string representation for the HTTP header.
        /// </summary>
        /// <param name="method">
        /// one of the following constants:
        /// <see cref="Common.Constants.HttpConnect">HttpConnect</see>
        /// <see cref="Common.Constants.HttpGet">HttpGet</see>
        /// <see cref="Common.Constants.HttpDelete">HttpDelete</see>
        /// <see cref="Common.Constants.HttpHead">HttpHead</see>
        /// <see cref="Common.Constants.HttpConnect">HttpConnect</see>
        /// <see cref="Common.Constants.HttpPost">HttpPost</see>
        /// <see cref="Common.Constants.HttpPut">HttpPut</see>
        /// <see cref="Common.Constants.HttpTrace">HttpTrace</see>
        /// </param>
        /// <returns>
        /// The string representation of the method or null if it is not one of the mentioned values.
        /// </returns>
        /// <remarks>
        /// Only HttpGet and HttpPost are really used in the code.
        /// </remarks>
        public static string IntHttpMethodToString(int method) {
            switch (method)
            {
                case Constants.HttpConnect:
                    return "CONNECT";
                case Constants.HttpGet:
                    return "GET";
                case Constants.HttpDelete:
                    return "DELETE";
                case Constants.HttpHead:
                    return "HEAD";
                case Constants.HttpPost:
                    return "POST";
                case Constants.HttpPut:
                    return "PUT";
                case Constants.HttpTrace:
                    return "TRACE";
            }
            return null;
        }

        public static string IntHttpContentTypeToString(int contentType) {
            string contentTypeString = null;
            switch (contentType) {
            case Constants.HttpContentApplication:
                contentTypeString = "application/octet-stream";
                break;
            case Constants.HttpContentHtml:
                contentTypeString = "text/html";
                break;
            case Constants.HttpContentJson:
                contentTypeString = "application/json";
                break;
            case Constants.HttpContentPlain:
                contentTypeString = "text/plain";
                break;
            }
            return contentTypeString;
        }

        public static string IntHttpSchemeToString(int scheme) {
            string schemeName = null;
            switch (scheme) {
            case Constants.HttpSchemeHttp:
                schemeName = "http";
                break;
            case Constants.HttpSchemeHttps:
                schemeName = "https";
                break;
            }
            return schemeName;
        }

        /// <summary>
        /// Extracts the code from a WebException.
        /// </summary>
        /// <param name="exception">The exception to extract the code from.</param>
        /// <returns>
        /// One of the following constants:
        /// <see cref="Common.Constants.NameResolutionFailure">NameResolutionFailure</see>
        /// <see cref="Common.Constants.ConnectFailure">ConnectFailure</see>
        /// <see cref="Common.Constants.ReceiveFailure">ReceiveFailure</see>
        /// <see cref="Common.Constants.ConnectionClosed">ConnectionClosed</see>
        /// <see cref="Common.Constants.Timeout">Timeout</see>
        /// <see cref="Common.Constants.SecureChannelFailure">SecureChannelFailure</see>
        /// <see cref="Common.Constants.CacheEntryNotFound">CacheEntryNotFound</see>
        /// <see cref="Common.Constants.KeepAliveFailure">KeepAliveFailure</see>
        /// <see cref="Common.Constants.MessageLengthLimitExceeded">MessageLengthLimitExceeded</see>
        /// <see cref="Common.Constants.Pending">Pending</see>
        /// <see cref="Common.Constants.PipelineFailure">PipelineFailure</see>
        /// <see cref="Common.Constants.ProxyNameResolutionFailure">ProxyNameResolutionFailure</see>
        /// <see cref="Common.Constants.RequestCanceled">RequestCanceled</see>
        /// <see cref="Common.Constants.RequestProhibitedByCachePolicy">RequestProhibitedByCachePolicy</see>
        /// <see cref="Common.Constants.RequestProhibitedByProxy">RequestProhibitedByProxy</see>
        /// <see cref="Common.Constants.ServerProtocolViolation">ServerProtocolViolation</see>
        /// <see cref="Common.Constants.Success">Success</see>
        /// <see cref="Common.Constants.TrustFailure">TrustFailure</see>
        /// <see cref="Common.Constants.UnknownError">UnknownError</see>
        /// </returns>
        public static int ReplicateExceptionStatus(WebException exception) {

            //var code = exception.Status switch
            //{
            //    WebExceptionStatus.NameResolutionFailure => Constants.NameResolutionFailure,
            //    WebExceptionStatus.ConnectFailure => Constants.ConnectFailure,
            //    WebExceptionStatus.ReceiveFailure => Constants.ReceiveFailure,
            //    WebExceptionStatus.ConnectionClosed => Constants.ConnectionClosed,
            //    WebExceptionStatus.Timeout => Constants.Timeout,
            //    WebExceptionStatus.ProtocolError => Constants.ProtocolError,
            //    WebExceptionStatus.SecureChannelFailure => Constants.SecureChannelFailure,
            //    WebExceptionStatus.SendFailure => Constants.SendFailure,
            //    WebExceptionStatus.CacheEntryNotFound => Constants.CacheEntryNotFound,
            //    WebExceptionStatus.KeepAliveFailure => Constants.KeepAliveFailure,
            //    WebExceptionStatus.MessageLengthLimitExceeded => Constants.MessageLengthLimitExceeded,
            //    WebExceptionStatus.Pending => Constants.Pending,
            //    WebExceptionStatus.PipelineFailure => Constants.PipelineFailure,
            //    WebExceptionStatus.ProxyNameResolutionFailure => Constants.ProxyNameResolutionFailure,
            //    WebExceptionStatus.RequestCanceled => Constants.RequestCanceled,
            //    WebExceptionStatus.RequestProhibitedByCachePolicy => Constants.RequestProhibitedByCachePolicy,
            //    WebExceptionStatus.RequestProhibitedByProxy => Constants.RequestProhibitedByProxy,
            //    WebExceptionStatus.ServerProtocolViolation => Constants.ServerProtocolViolation,
            //    WebExceptionStatus.Success => Constants.Success,
            //    WebExceptionStatus.TrustFailure => Constants.TrustFailure,
            //    WebExceptionStatus.UnknownError => Constants.UnknownError,
            //    _ => Constants.UnknownError
            //};
            return -1;
        }
    }
}
