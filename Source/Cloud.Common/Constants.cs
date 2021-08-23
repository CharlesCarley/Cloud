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

#pragma warning disable 1591

namespace Cloud.Common
{
    /// <summary>
    /// Shared constants...
    /// </summary>
    public static class Constants {
        /// <summary>
        ///
        /// </summary>
        public const string Version = "1.0.0.0";

        // basic content-types

        /// <summary>
        /// Defines the HTTP Content-Type for application/octet-stream.
        /// </summary>
        public const string HttpContentApplicationString = "application/octet-stream";
        /// <summary>
        /// Defines the HTTP Content-Type for text/html.
        /// </summary>
        public const string HttpContentHtmlString = "text/html";
        /// <summary>
        /// Defines the HTTP Content-Type for application/json.
        /// </summary>
        public const string HttpContentJsonString = "application/json";
        /// <summary>
        /// Defines the HTTP Content-Type for text/plain.
        /// </summary>
        public const string HttpContentPlainString = "text/plain";

        // picked at random ...

        /// <summary>
        /// A constant chosen randomly to represent a method that returns
        /// nothing, but has succeeded.
        /// </summary>
        /// <remarks>
        /// See: https://tinyurl.com/y26xrjqx
        /// </remarks>
        public const string ReturnNone = "9627920";
        /// <summary>
        /// A constant chosen randomly to represent a successful state.
        /// </summary>
        /// <remarks>
        /// See: https://tinyurl.com/y26xrjqx
        /// </remarks>
        public const string ReturnSuccess = "8772795";
        /// <summary>
        /// A constant chosen randomly to represent a failure state.
        /// </summary>
        /// <remarks>
        /// See: https://tinyurl.com/y26xrjqx
        /// </remarks>
        public const string ReturnFailure = "3449541";
        /// <summary>
        /// A constant chosen randomly to represent a true return code.
        /// </summary>
        /// <remarks>
        /// See: https://tinyurl.com/y26xrjqx
        /// </remarks>
        public const string ReturnTrue = "3547747";
        /// <summary>
        /// A constant chosen randomly to represent a false return code.
        /// </summary>
        /// <remarks>
        /// See: https://tinyurl.com/y26xrjqx
        /// </remarks>
        public const string ReturnFalse = "5590349";

        public const string ReturnInvNum               = "6170173";
        public const string ReturnNoID                 = "9580785";
        public const string ReturnInvAlphaNum          = "4674559";
        public const string ReturnInvEmail             = "3807806";
        public const string ReturnInvId                = "1017167";
        public const string ReturnUndefined            = "7874943";
        public const string ReturnNoConnection         = "2904667";
        public const string ReturnNoAccess             = "7233152";
        public const string ReturnNoUser               = "1376761";
        public const string ReturnSQLConnection        = "9335045";
        public const string ReturnSQLCommand           = "3438161";
        public const string ReturnSQLRead              = "7561314";
        public const string ReturnSQLDelete            = "3370629";
        public const string ReturnSQLCatchAll          = "3016130";
        public const string ReturnInvalidKey           = "5982075";
        public const string ReturnInvalidKeySize       = "1702806";
        public const string ReturnInvalidString        = "6703164";
        public const string ReturnInvalidParameterList = "5224114";
        public const string ReturnInvalidRequest       = "6608065";
        public const string ReturnInvalidGUID          = "1968899";
        public const string ReturnInvalidBase64        = "4876368";
        public const string ReturnInvalidAlphabet      = "4501923";
        public const string ReturnInvalidInteger       = "5519944";
        public const string ReturnInvalidPackage       = "7263503";

        public const int SelectArray = 1459248;
        public const int SelectByKey = 2028798;
        public const int SelectById  = 1532435;
        public const int Save        = 4455198;
        public const int Drop        = 1248002;
        public const int Contains    = 8661096;
        public const int Clear       = 3356473;
        public const int Status      = 8755582;

        /// <summary>
        /// Used to define an undefined database Id field.
        /// </summary>
        /// <remarks>
        /// Auto incrementing id fields start at 1, so anything below that will
        /// work as well..
        /// </remarks>
        public const int Undefined = -1;

        // return codes for a transaction
        public const int ReturnNull                     = 0;
        public const int CannotProcessRequest           = 1;
        public const int CannotPackagePayload           = 2;
        public const int CannotProcessResponse          = 3;
        public const int Success                        = 4;
        public const int NameResolutionFailure          = 5;
        public const int ConnectFailure                 = 6;
        public const int ReceiveFailure                 = 7;
        public const int SendFailure                    = 8;
        public const int PipelineFailure                = 9;
        public const int RequestCanceled                = 10;
        public const int ProtocolError                  = 11;
        public const int ConnectionClosed               = 12;
        public const int TrustFailure                   = 13;
        public const int SecureChannelFailure           = 14;
        public const int ServerProtocolViolation        = 15;
        public const int KeepAliveFailure               = 16;
        public const int Pending                        = 17;
        public const int Timeout                        = 18;
        public const int ProxyNameResolutionFailure     = 19;
        public const int UnknownError                   = 20;
        public const int MessageLengthLimitExceeded     = 21;
        public const int CacheEntryNotFound             = 22;
        public const int RequestProhibitedByCachePolicy = 23;
        public const int RequestProhibitedByProxy       = 24;
        public const int StaticAnalysisBugOrMSFault     = 25;
        public const int OutOfMemory                    = 26;
        public const int IORead                         = 27;
        public const int NullArgument                   = 28;
        public const int InvalidPort                    = 29;
        public const int InvalidMethod                  = 30;
        public const int InvalidContentType             = 31;
        public const int InvalidSchemeType              = 32;
        public const int InvalidHost                    = 33;

        /// <summary>
        /// Integer constant for the HTTP method CONNECT
        /// </summary>
        public const int HttpConnect = 0;
        /// <summary>
        /// Integer constant for the HTTP method TRACE
        /// </summary>
        public const int HttpTrace = 1;
        /// <summary>
        /// Integer constant for the HTTP method DELETE
        /// </summary>
        public const int HttpDelete = 2;
        /// <summary>
        /// Integer constant for the HTTP method PUT
        /// </summary>
        public const int HttpPut = 3;
        /// <summary>
        /// Integer constant for the HTTP method POST
        /// </summary>
        public const int HttpPost = 4;
        /// <summary>
        /// Integer constant for the HTTP method HEAD
        /// </summary>
        public const int HttpHead = 5;
        /// <summary>
        /// Integer constant for the HTTP method GET
        /// </summary>
        public const int HttpGet = 6;

        // basic content-types
        public const int HttpContentApplication = 0;
        public const int HttpContentHtml        = 1;
        public const int HttpContentJson        = 2;
        public const int HttpContentPlain       = 3;

        // only using http/https - production should only use https
        public const int HttpSchemeHttp  = 0;
        public const int HttpSchemeHttps = 1;
    }
}
