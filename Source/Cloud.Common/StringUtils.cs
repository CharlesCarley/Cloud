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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cloud.Common
{
    /// <summary>
    /// StringUtils is a utility class for miscellaneous string manipulation.
    /// </summary>
    public static class StringUtils {
        /// <summary>
        /// Safely converts a string to an integer.
        /// </summary>
        /// <param name="input">The input string that should be converted.</param>
        /// <param name="output">The output integer.</param>
        /// <param name="defaultValue">A default value if the string can not be converted.</param>
        public static void ToInt32(string input, out int output, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(input))
                output = defaultValue;
            else if (!int.TryParse(input, out output))
                output = defaultValue;
        }

        /// <summary>
        /// Safely converts a string to a long integer.
        /// </summary>
        /// <param name="input">The input string that should be converted.</param>
        /// <param name="output">The output integer.</param>
        /// <param name="defaultValue">A default value if the string can not be converted.</param>
        public static void ToLong(string input, out long output, long defaultValue = 0)
        {
            if (string.IsNullOrEmpty(input))
                output = defaultValue;
            else if (!long.TryParse(input, out output))
                output = defaultValue;
        }

        /// <summary>
        /// Safely converts a string to a double.
        /// </summary>
        /// <param name="input">The input string that should be converted.</param>
        /// <param name="output">The output double.</param>
        /// <param name="defaultValue">A default value if the string can not be converted.</param>
        public static void ToDouble(string input, out double output, double defaultValue = 0)
        {
            if (string.IsNullOrEmpty(input))
                output = defaultValue;
            else if (!double.TryParse(input, out output))
                output = defaultValue;
        }
        /// <summary>
        /// Safely converts a string to an integer.
        /// </summary>
        /// <param name="input">The input string that should be converted.</param>
        /// <param name="defaultValue">A default value if the string can not be converted.</param>
        /// <returns>The converted integer</returns>
        public static int ToInt(string input, int defaultValue = 0)
        {
            ToInt32(input, out var output, defaultValue);
            return output;
        }

        /// <summary>
        /// Safely converts a string to an integer within the min and max ranges.
        /// </summary>
        /// <param name="input">The input string that should be converted.</param>
        /// <param name="defaultValue">A default value if the string can not be converted.</param>
        /// <param name="minRange">The minimum allowed range for the return value.</param>
        /// <param name="maxRange">The maximum allowed range for the return value.</param>
        /// <returns>The converted integer</returns>
        public static int ToIntRange(string input, int minRange, int maxRange, int defaultValue = 0)
        {
            ToInt32(input, out var output, defaultValue);
            return output<minRange ? minRange : output> maxRange ? maxRange : output;
        }

        /// <summary>
        /// Attempts to convert the supplied string to a list of integers.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <param name="destinationList">The output storage for converted integers.</param>
        /// <returns>The number of integers that were converted.</returns>
        /// <remarks>This assumes that the integers are separated by a comma.</remarks>
        public static int ToIntList(string input, ref List<int> destinationList)
        {
            if (destinationList is null)
                return 0;

            if (string.IsNullOrEmpty(input))
                return 0;

            input = input.Replace('[', ' ');
            input = input.Replace(']', ' ');
            input = input.Replace(" ", string.Empty);

            if (string.IsNullOrEmpty(input))
                return 0;

            var validValues = 0;
            var splits      = input.Split(',');
            foreach (var str in splits) {
                if (!int.TryParse(str, out var val))
                    continue;

                destinationList.Add(val);
                ++validValues;
            }
            return validValues;
        }

        /// <summary>
        /// Attempts to convert the supplied string to a list of integers.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <param name="destinationList">The output storage for converted integers.</param>
        /// <remarks>This assumes that the integers are separated by a comma.</remarks>
        public static void ToIntArrayNoCount(string input, ref List<int> destinationList)
        {
            if (destinationList is null)
                return;
            if (string.IsNullOrEmpty(input))
                return;

            input = input.Replace('[', ' ');
            input = input.Replace(']', ' ');
            input = input.Replace(" ", string.Empty);

            if (string.IsNullOrEmpty(input))
                return;

            var splits = input.Split(',');
            foreach (var str in splits) {
                if (int.TryParse(str, out var val))
                    destinationList.Add(val);
            }
        }

        /// <summary>
        /// Attempts to convert the supplied string to plain text from its base 64 representation.
        /// </summary>
        /// <param name="input">The string that should be converted.</param>
        /// <returns>An empty string if the input is null or empty, otherwise the converted string.</returns>
        /// <remarks>If the conversion fails with an exception, this will return an empty string.</remarks>
        public static string FromBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            try {
                return Encoding.ASCII.GetString(Convert.FromBase64String(input));
            } catch (DecoderFallbackException) {
                return string.Empty;
            } catch (FormatException) {
                return string.Empty;
            }
        }

        /// <summary>
        /// Attempts to convert the supplied string to a base 64 representation.
        /// </summary>
        /// <param name="input">The string that should be converted.</param>
        /// <returns>An empty string if the input is null or empty, otherwise the converted string.</returns>
        /// <remarks>If the conversion fails with an exception, this will return an empty string.</remarks>
        public static string ToBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            try {
                return Convert.ToBase64String(Encoding.ASCII.GetBytes(input));
            } catch (DecoderFallbackException) {
                return string.Empty;
            } catch (FormatException) {
                return string.Empty;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dur"></param>
        /// <returns></returns>
        public static string ToString(TimeSpan dur)
        {
            return dur.ToString(dur.Days >= 1 ? "dd\\ hh\\:mm\\:ss" : "hh\\:mm\\:ss");
        }

        /// <summary>
        /// Converts the list of integers into a comma separated string.
        /// </summary>
        /// <param name="input">The integer list to convert.</param>
        /// <param name="asBase64">if true the returned string will be encoded as base 64.</param>
        /// <returns>The comma separated string or an empty string if the list is null or empty.</returns>
        /// <remarks>Formats the list with square brackets identity purposes.</remarks>
        public static string IntListToString(List<int> input, bool asBase64 = false)
        {
            if (input is null || input.Count <= 0)
                return string.Empty;

            var builder = new StringBuilder();

            foreach (var item in input) {
                if (builder.Length == 0) {
                    builder.Append('[');
                    builder.Append(item);
                } else {
                    builder.Append(',');
                    builder.Append(item);
                }
            }
            if (builder.Length > 0)
                builder.Append(']');
            return asBase64 ? ToBase64(builder.ToString()) : builder.ToString();
        }

        /// <summary>
        /// Attempts to load the file from the supplied path into a string.
        /// </summary>
        /// <param name="path">The file system path of the file.</param>
        /// <returns>A string that contains the contents of the file.</returns>
        /// <remarks>If the file does not exist or an exception occurred, then an empty string will be returned.</remarks>
        public static string ReadFileAsString(string path)
        {
            if (!File.Exists(path))
                return string.Empty;
            try {
                var builder = new StringBuilder();
                var fp      = File.OpenRead(path);

                var buffer = new char[128];
                using (var reader = new StreamReader(fp))
                {
                    int ar;
                    while ((ar = reader.Read(buffer, 0, 128)) > 0)
                        builder.Append(new string(buffer, 0, ar));
                }

                fp.Close();
                return builder.ToString();
            } catch {
                // ignore
                return string.Empty;
            }
        }

        /// <summary>
        /// Determines whether the specified value is composed of the character set.
        /// [a-zA-Z]+
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>
        /// True if the specified value is a valid English letter otherwise false.
        /// </returns>
        public static bool IsLetter(string value)
        {
            return value.All(ch => ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z');
        }

        /// <summary>
        /// Determines whether the specified character is in the set [a-zA-Z].
        /// </summary>
        public static bool IsLetter(char ch)
        {
            return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z';
        }

        /// <summary>
        /// Determines whether the specified character is in the set [0-9].
        /// </summary>
        public static bool IsDigit(int ch)
        {
            return ch >= '0' && ch <= '9';
        }

        /// <summary>
        /// Determines whether the specified value is composed of the character set.
        /// [0-9\\-\\.\\f\\e]\\+
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>
        /// True if the specified value is a valid ip-address or domain name otherwise false.
        /// </returns>
        public static bool IsNumber(string value)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.Where(c => c < '0' || c > '9')
                       .All(c => c == '-' || c == '.' || c == 'f' || c == 'e');
        }

        /// <summary>
        /// Determines whether the specified value is composed of the character set.
        /// [a-zA-Z0-9\_\.\/]
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>
        /// True if the specified value is a valid ip-address or domain name otherwise false.
        /// </returns>
        public static bool IsIpAddressOrDomain(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // has to be in the character set
            if (!value.Where(c => c < '0' || c > '9')
                     .Where(c => c < 'A' || c > 'Z')
                     .Where(c => c < 'a' || c > 'z')
                     .All(c => c == '-' || c == '.'))
                return false;

            // cannot start with a hyphen
            if (value.StartsWith("-"))
                return false;

            // cannot end with a hyphen
            if (value.EndsWith("-"))
                return false;

            var ch = value[0];
            if (ch < '0' || ch > '9') {
                var words = value.Split('.');
                foreach (var word in words) {
                    // cannot start with a hyphen
                    if (word.StartsWith("-"))
                        return false;
                    // cannot end with a hyphen
                    if (word.EndsWith("-"))
                        return false;
                }
                return true;
            }

            // NOTE: only testing ipv4
            var splits = value.Split('.');
            return splits.Length == 4;
        }

        /// <summary>
        /// Determines whether the specified value is composed of the character set [a-fA-F0-9]+
        /// </summary>
        /// <param name="value">The value that will be tested.</param>
        /// <returns>
        /// True if all the characters in 'value' are composed of the set [a-fA-F0-9]+ other wise false.
        /// </returns>
        public static bool IsHex(string value)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.Where(c => c < '0' || c > '9')
                       .Where(c => c < 'A' || c > 'F')
                       .All(c => c >= 'a' && c <= 'f');
        }

        /// <summary>
        /// Determines whether the specified value is composed of the character set.
        /// [a-fA-F0-9\-]
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>
        /// True if the specified value is a valid GUID otherwise false.
        /// </returns>
        public static bool IsGuid(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // A GUID must have five components all in hex.
            var result = value.Split('-');
            return result.Length == 5 && result.All(IsHex);
        }

        /// <summary>
        /// Determines whether the specified value is composed of the character set.
        /// [a-fA-F0-9\=\/\\+]
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>
        /// True if the specified value is a valid base64 string otherwise false.
        /// </returns>
        public static bool IsBase64(string value)
        {
            return (from c in value 
                where c != '=' && c != '/' && c != '+' 
                where c < '0' || c > '9'
                where c < 'a' || c > 'z' 
                select c).All(c => c >= 'A' && c <= 'Z');
        }

        /// <summary>
        /// Checks to see if the supplied character is a valid JSON character.
        /// </summary>
        /// <param name="ch">The character to test.</param>
        public static bool IsInJsonCharacterSet(char ch)
        {
            if (ch >= '0' && ch <= '9')
                return true;
            if (ch >= 'a' && ch <= 'z')
                return true;
            if (ch >= 'A' && ch <= 'Z')
                return true;

            switch (ch) {
            case '{':
            case '}':
            case '[':
            case ']':
            case '\r':
            case '\n':
            case ' ':
            case '\t':
            case '-':
            case ',':
            case '\"':
            case ':':
            case '.':
            case '/':
            case '@':
                return true;
            default:
                return false;
            }
        }

        public static bool IsValidDatabaseValue(string value)
        {
            return (from c in value where c != '=' &&
                                          c != '.' &&
                                          c != '-' &&
                                          c != '/' &&
                                          c != '+'
                    where c < '0' || c > '9'
                    where c < 'a' || c > 'z'
                    select c)
                .All(c => c >= 'A' && c <= 'Z');
        }

        /// <summary>
        ///  Returns a Hexadecimal encoded string from the input bytes.
        /// </summary>
        /// <param name="input">The bytes to encode as string</param>
        /// <returns>null if the string is null or empty, otherwise the encoded string.</returns>
        public static string ToHexString(byte[] input)
        {
            if (input is null || input.Length <= 0)
                return null;

            var builder = new StringBuilder();
            foreach (var b in input)
                builder.Append(b.ToString("X2"));
            return builder.ToString();
        }

        /// <summary>
        ///  Returns a Hexadecimal encoded string from the string.
        /// </summary>
        /// <param name="value">The string to encode as hex</param>
        /// <returns>null if the string is null or empty, otherwise the encoded string.</returns>
        public static string ToHexString(string value)
        {
            return string.IsNullOrEmpty(value) ? null : ToHexString(Encoding.ASCII.GetBytes(value));
        }

        private static int Hex2Int(char c)
        {
            if (c >= 'A' && c <= 'Z')
                return 10 + c - 'A';
            if (c >= '0' && c <= '9')
                return c - '0';
            throw new FormatException(
                $"The input character '{c}' falls outside of the predefined range of this function.");
        }

        /// <summary>
        /// Converts the hex encoded input back to plain text.
        /// </summary>
        /// <param name="input">The hex encoded string</param>
        /// <returns>Null if the input is null or empty otherwise returns the plain text string.</returns>
        /// <exception cref="FormatException">
        /// Throws if the input string is not divisible by two or if the input is not in uppercase hexadecimal.
        /// </exception>
        public static string FromHexString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            if (input.Length % 2 != 0) {
                throw new FormatException(
                    "The string is not divisible by 2.");
            }

            int i, len = input.Length;
            var builder = new StringBuilder();

            for (i = 0; i < len; i += 2) {
                var dv = Hex2Int(input[i]);
                var rv = Hex2Int(input[i + 1]);
                var iv = 16 * dv + rv;
                if (iv >= byte.MinValue && iv < byte.MaxValue)
                    builder.Append((char)iv);
            }
            return builder.ToString();
        }

        public static byte[] StringToBytes(string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }
    }
}
