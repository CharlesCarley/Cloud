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

namespace Cloud.Common
{
    public enum LogLevel
    {
        None,
        Assert,
        Error,
        Warn,
        Info,
        Debug,
        Verbose,
    }

    public static class LogUtils {
        public static LogLevel Detail { get; set; } = LogLevel.Verbose;

        /// <summary>
        /// Logs a single message string to the system console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            if (message == null) return;
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.WriteLine(message);

            Console.WriteLine(message);
        }

        private static void LogDetail(LogLevel detail, string method, string message)
        {
            // Todo: This needs to be formatted differently.
            // Todo: Enable / disable specific print sections.  


            switch (detail)
            {
                default:
                    break;
                case LogLevel.Assert:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }



            if (method is null) {
                if (detail <= Detail) {
                    Log($"{DateTime.Now:MM/dd/yy@HH:mm:ss}: {detail} : {message}");
                }
            } else {
                if (detail <= Detail) {
                    Log($"{DateTime.Now:MM/dd/yy@HH:mm:ss}: {detail} : [ {method} ] {message}");
                }
            }

            Console.ResetColor();
        }

        /// <summary>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="method"></param>
        /// <param name="message"></param>
        public static void Log(LogLevel level, string method, string message)
        {
            try {
                LogDetail(level, method, message);
            } catch (FormatException) {
                // ignore
            }
        }

        public static void Log(LogLevel level, string message)
        {
            try {
                LogDetail(level, null, message);
            } catch (FormatException) {
                // ignore
            }
        }


        /// <summary>
        /// Logs a message string to the system console.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="argument1">An extra argument to support a formatted string.</param>
        public static void LogF(string message, object argument1)
        {
            try {
                var result = string.Format(message, argument1);
                Log(result);
            } catch (FormatException) {
                // ignore
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        public static void Log(object caller, string message)
        {
            // Used to provide debug information about the
            // location where the message is being delivered
            if (caller == null) return;
            var type = caller.GetType();

            Log($"{type.Name}: {message}");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void Log(Exception ex, string message)
        {
            // Used to provide debug information about the
            // location where the message is being delivered

            LogDetail(LogLevel.Assert, ex.TargetSite?.Name, message);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            LogDetail(LogLevel.Assert, ex.TargetSite?.Name, ex.Message);
        }
    }
}
