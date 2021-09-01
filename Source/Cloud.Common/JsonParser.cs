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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Common
{
    /// <summary>
    /// Exception handler for syntax related exceptions.
    /// This class is primarily used for the specific exception itself.
    /// </summary>
    public class JsonSyntaxException : Exception {
        /// <summary>
        /// A basic constructor with a message.
        /// </summary>
        /// <param name="message"></param>
        public JsonSyntaxException(string message) :
            base(message)
        {
        }
    }

    /// <summary>
    /// Represents Json specific terminal symbols.
    /// </summary>
    public enum JsonTokenType
    {
        /// <summary>
        ///  Identifier for the '{' character.
        /// </summary>
        OpenBracket = 0,

        /// <summary>
        ///  Identifier for the '}' character.
        /// </summary>
        CloseBracket,

        /// <summary>
        ///  Identifier for the ':' character.
        /// </summary>
        Colon,

        /// <summary>
        ///  Identifier for the ',' character.
        /// </summary>
        Comma,

        /// <summary>
        ///  Identifier for the '[' character.
        /// </summary>
        OpenBrace,

        /// <summary>
        ///  Identifier for the ']' character.
        /// </summary>
        CloseBrace,

        /// <summary>
        /// Identifier for a complete string.
        /// </summary>
        StringToken,

        /// <summary>
        /// Identifier for a complete number.
        /// </summary>
        Numerical,

        /// <summary>
        /// Identifier for a complete true, or false value.
        /// </summary>
        Boolean,

        /// <summary>
        /// Identifier for a null value.
        /// </summary>
        Null,

        /// <summary>
        /// Code used to return a error token.
        /// </summary>
        SyntaxError,

        /// <summary>
        /// Code used to indicate that the end of the file has been reached.
        /// </summary>
        Eof
    }

    /// <summary>
    /// Implementation of a json parser token.
    /// </summary>
    public class JsonToken {
        /// <summary>
        /// Constructs a token with a specific type and some associated value for that type.
        /// </summary>
        public JsonToken(JsonTokenType type, object value)
        {
            Type  = type;
            Value = value;
        }

        /// <summary>
        /// Constructs a token with no associated value.
        /// </summary>
        public JsonToken(JsonTokenType type)
        {
            Type  = type;
            Value = null;
        }

        /// <summary>
        /// Provides read only access to the specific token type.
        /// </summary>
        public JsonTokenType Type { get; }

        /// <summary>
        /// Provides read only access to value associated with the token.
        /// </summary>
        public object Value { get; }
    }

    /// <summary>
    /// Vanilla parser implementation. Converts the input into tokens, then attempts to
    /// matches rules from the result.
    /// </summary>
    public class JsonParser {
        private readonly JsonObject _root;

        private readonly Stack<JsonObject> _stack;

        private JsonObject _cur;

        /// <summary>
        /// Default constructor. Creates the root object and a stack.
        /// </summary>
        public JsonParser()
        {
            _root  = new JsonObject();
            _cur   = null;
            _stack = new Stack<JsonObject>();
            _stack.Push(_root);
        }

        /// <summary>
        /// Attempts to parse the supplied string.
        /// </summary>
        /// <param name="input">The string to interpret as json.</param>
        /// <returns>
        /// Always returns the root object on success. If a syntax error occurs,
        /// then a JsonSyntaxException will be thrown..
        /// </returns>
        /// <exception cref="Cloud.Common.JsonSyntaxException">JsonSyntaxException</exception>
        public JsonObject Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
                return _root;

            var lex = new JsonLexer(input);

            var c = lex.Lex();
            do {
                if (c is null)
                    break;

                switch (c.Type) {
                case JsonTokenType.OpenBracket:
                case JsonTokenType.Comma:
                    c = HandleNextStatement(lex, c);
                    break;
                case JsonTokenType.StringToken:
                case JsonTokenType.Numerical:
                case JsonTokenType.Boolean:
                case JsonTokenType.Null:
                    c = HandleValue(lex, c);
                    break;
                case JsonTokenType.CloseBracket:
                    c = HandleCloseBracket(lex);
                    break;
                case JsonTokenType.CloseBrace:
                    c = HandleCloseBrace(lex);
                    break;
                case JsonTokenType.Colon:
                case JsonTokenType.OpenBrace:
                case JsonTokenType.SyntaxError:
                case JsonTokenType.Eof:
                    c = null;
                    break;
                default:
                    c = null;
                    break;
                }
            } while (IsValidToken(c));

            return _root;
        }

        private JsonToken HandleNextStatement(JsonLexer lex, JsonToken cur)
        {
            var str = lex.Lex();
            if (str.Type == JsonTokenType.CloseBracket)
                return str;

            if (str.Type != JsonTokenType.StringToken && str.Type != JsonTokenType.Numerical)
                throw new JsonSyntaxException(Resources.Exception1);

            var colon = lex.Lex();
            if (colon.Type != JsonTokenType.Colon)
                throw new JsonSyntaxException(Resources.Exception2);

            var control = lex.Lex();
            if (control == null)
                throw new JsonSyntaxException(Resources.Exception3);

            switch (control.Type) {
            case JsonTokenType.StringToken:
            case JsonTokenType.Numerical:
            case JsonTokenType.Boolean:
            case JsonTokenType.Null:
                cur = ParseValue(lex, str, control);
                break;
            case JsonTokenType.OpenBracket:
                ParseOpenBracket(str);
                break;
            case JsonTokenType.OpenBrace:
                cur = ParseOpenBrace(lex, str);
                break;
            case JsonTokenType.CloseBracket:
            case JsonTokenType.Colon:
            case JsonTokenType.Comma:
            case JsonTokenType.CloseBrace:
            case JsonTokenType.SyntaxError:
            case JsonTokenType.Eof:
                throw new JsonSyntaxException(Resources.Exception4);
            default:
                throw new JsonSyntaxException(Resources.Exception4);
            }

            return cur;
        }

        private JsonToken ParseOpenBrace(JsonLexer lex, JsonToken str)
        {
            var obj = new JsonObject { IsArray = true };
            {
                _cur = _stack.Peek();
                _cur?.AddValue((string)str.Value, obj);
                _stack.Push(obj);

                var cur = lex.Lex();
                if (cur.Type == JsonTokenType.OpenBracket)
                    _stack.Push(new JsonObject());
                return cur;
            }
        }

        private void ParseOpenBracket(JsonToken str)
        {
            var obj = new JsonObject();
            {
                _cur = _stack.Peek();
                _cur?.AddValue((string)str.Value, obj);
                _stack.Push(obj);
            }
        }

        private JsonToken ParseValue(JsonLexer lex, JsonToken str, JsonToken control)
        {
            _cur = _stack.Peek();
            _cur?.AddValue(str.Value.ToString(), control.Value);
            var cur = lex.Lex();
            return cur;
        }

        private JsonToken HandleValue(JsonLexer lex, JsonToken c)
        {
            _cur = _stack.Peek();
            if (_cur != null && _cur.IsArray)
                _cur.AddValue(c.Value);

            c = lex.Lex();
            if (c.Type == JsonTokenType.Comma)
                c = lex.Lex();
            return c;
        }

        private JsonToken HandleCloseBracket(JsonLexer lex)
        {
            var cur = _stack.Count > 0 ? _stack.Peek() : null;
            _stack.Pop();
            if (_stack.Count > 0 && cur != null)
                _stack.Peek().AddObject(cur);
            var c = lex.Lex();
            if (c.Type == JsonTokenType.Comma)
                c = lex.Lex();
            if (c.Type == JsonTokenType.OpenBracket)
                _stack.Push(new JsonObject());
            return c;
        }

        private JsonToken HandleCloseBrace(JsonLexer lex)
        {
            var cur = _stack.Count > 0 ? _stack.Peek() : null;
            _stack.Pop();
            if (_stack.Count > 0 && cur != null)
                _stack.Peek().AddObject(cur);
            var c = lex.Lex();
            return c;
        }

        private bool IsValidToken(JsonToken tok)
        {
            return tok != null &&
                   tok.Type != JsonTokenType.Eof && _stack.Count > 0;
        }

        /// <summary>
        /// Attempts to automatically convert dictionary keys to property
        /// values that are defined in the supplied type.
        /// </summary>
        /// <param name="package">The string to parse as json.</param>
        /// <param name="type">The type to use for conversion.</param>
        /// <param name="inBase64">
        /// If set to true, it is assumed that the supplied string is encoded as base 64 otherwise,
        /// the string is parsed as regular json.
        /// </param>
        /// <returns>Null if an instance of the type cannot be created or if the package can not be parsed.</returns>
        public static object Unwrap(string package, Type type, bool inBase64 = true)
        {
            object result = null;
            try {
                if (string.IsNullOrEmpty(package))
                    return null;

                result = Activator.CreateInstance(type);
                if (result == null)
                    return null;

                var obj = inBase64 ? JsonObject.ParseBase64(package) : JsonObject.Parse(package);
                if (obj is null)
                    return null;

                var properties = type.GetProperties();
                foreach (var property in properties) {
                    if (!property.CanWrite) continue;
                    if (!obj.HasKey(property.Name)) continue;

                    if (property.PropertyType == typeof(int))
                        property.SetValue(result, obj.AsInt(property.Name, 0));
                    else if (property.PropertyType == typeof(string))
                        property.SetValue(result, obj.AsString(property.Name, string.Empty));
                    else if (property.PropertyType == typeof(float))
                        property.SetValue(result, (float)obj.AsDouble(property.Name, 0.0f));
                }
            } catch (Exception exception) {
                LogUtils.Log(exception);
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string Wrap(object instance)
        {
            if (instance is null)
                return "{}";

            try {
                var type       = instance.GetType();
                var properties = type.GetProperties();
                if (properties.Length > 0) {
                    var obj = new JsonObject();
                    foreach (var property in properties) {
                        if (!property.CanWrite) continue;

                        if (property.PropertyType == typeof(int)) {
                            var value = property.GetValue(instance);
                            if (value is int i)
                                obj.AddValue(property.Name, i);
                        } else if (property.PropertyType == typeof(string)) {
                            var value = property.GetValue(instance);
                            if (value is string s)
                                obj.AddValue(property.Name, s);
                        } else if (property.PropertyType == typeof(float)) {
                            var value = property.GetValue(instance);
                            if (value is float f)
                                obj.AddValue(property.Name, f);
                        } else if (property.PropertyType == typeof(double)) {
                            var value = property.GetValue(instance);
                            if (value is double d)
                                obj.AddValue(property.Name, (float)d);
                        }
                    }

                    return obj.AsCompactPrint();
                }
            } catch (Exception exception) {
                LogUtils.Log(exception);
            }

            return "{}";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string WrapAsBase64(object instance)
        {
            var str = Wrap(instance);
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(str));
        }
    }

    /// <summary>
    /// </summary>
    public class JsonLexer {
        private const string           True  = "true";
        private const string           Null  = "null";
        private const string           False = "false";
        private readonly string        _buffer;
        private readonly int           _len;
        private readonly StringBuilder _builder;
        private int                    _current;

        /// <summary></summary>
        public JsonLexer(string cur)
        {
            _buffer  = cur;
            _current = 0;
            _len     = cur?.Length ?? 0;
            _builder = new StringBuilder();
        }

        /// <summary>Tests the buffer position against the end of the buffer.</summary>
        private bool IsEndOfFile => _len == 0 || _current >= _len;

        /// <summary></summary>
        public static bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }

        /// <summary></summary>
        public static bool IsNumeric(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary></summary>
        public static bool IsNumber(string str)
        {
            // 0123456789.+-feE

            // clang-format off
            return !string.IsNullOrEmpty(str) &&
                   str.Where(c => !IsNumeric(c)).All(
                       c => c == '.' &&
                            c == '-' &&
                            c == 'f' &&
                            c == '+' &&
                            c == 'e' &&
                            c == 'E');

            // clang-format on
        }

        /// <summary></summary>
        public JsonToken Lex()
        {
            while (!IsEndOfFile) {
                var ch = _buffer[_current];
                switch (ch) {
                case '\n':
                case '\r':
                    ++_current;
                    break;
                case ' ':
                case '\t':
                    EatWhitespace();
                    break;
                case '{':
                    ++_current;
                    return new JsonToken(JsonTokenType.OpenBracket);
                case '}':
                    ++_current;
                    return new JsonToken(JsonTokenType.CloseBracket);
                case '[':
                    ++_current;
                    return new JsonToken(JsonTokenType.OpenBrace);
                case ']':
                    ++_current;
                    return new JsonToken(JsonTokenType.CloseBrace);
                case ':':
                    ++_current;
                    return new JsonToken(JsonTokenType.Colon);
                case ',':
                    ++_current;
                    return new JsonToken(JsonTokenType.Comma);
                case '"':
                    return ParseString();
                default:
                    if (IsNumeric(ch) || ch == '-' || ch == '.')
                        return ParseNumber(ch);
                    else if (IsAlpha(ch))
                        return ParseBoolean(ch);
                    else
                        return new JsonToken(JsonTokenType.SyntaxError, $"{Resources.UndefinedCharacter} {ch}");
                }
            }

            return new JsonToken(JsonTokenType.Eof);
        }

        private static double GetValueDouble(string text)
        {
            if (double.TryParse(text, out var res))
                return res;
            return 0;
        }

        private JsonToken ParseBoolean(char ch)
        {
            _builder.Clear();

            var i = 0;
            while (IsAlpha(ch) && i++ < 5) {
                _builder.Append(ch);
                ++_current;
                if (IsEndOfFile)
                    return new JsonToken(JsonTokenType.SyntaxError, Resources.PrematureEOF);
                ch = _buffer[_current];
            }

            var val = _builder.ToString();

            switch (val) {
            case True:
                return new JsonToken(JsonTokenType.Boolean, true);
            case False:
                return new JsonToken(JsonTokenType.Boolean, false);
            case Null:
                return new JsonToken(JsonTokenType.Null, null);
            default:
                return new JsonToken(JsonTokenType.SyntaxError, Resources.KeywordError);
            }
        }

        private JsonToken ParseNumber(char ch)
        {
            _builder.Clear();

            while (IsNumeric(ch) || ch == '.' || ch == '-') {
                _builder.Append(ch);
                ++_current;
                if (IsEndOfFile)
                    return new JsonToken(JsonTokenType.SyntaxError, $"Undefined character: '{ch}'");

                ch = _buffer[_current];
            }

            return new JsonToken(JsonTokenType.Numerical, GetValueDouble(_builder.ToString()));
        }

        private bool BuildString()
        {
            _builder.Clear();

            var ch = (char)GetCh();
            while (!IsEndOfFile && ch != '"') {
                _builder.Append(ch);
                if (ch == '\\') {
                    ch = (char)GetCh();
                    if (ch == '"' ||
                        ch == '\\' ||
                        ch == '\n' ||
                        ch == '\r' ||
                        ch == '\b' ||
                        ch == '\r' ||
                        ch == '\f' ||
                        ch == '\t')
                        _builder.Append(ch);
                    else
                        return false;
                }

                ch = (char)GetCh();
            }

            if (ch == '"')
                _current++;
            return true;
        }

        private JsonToken ParseString()
        {
            // some string declaration
            // [.] except ["|\] not including [\n\r\b\r\f\t]

            if (!BuildString())
                return new JsonToken(JsonTokenType.SyntaxError, $"{Resources.UndefinedEscape}");

            var val = _builder.ToString();
            switch (val) {
            case Null:
                // "prop": null
                return new JsonToken(JsonTokenType.Null, null);
            case True:
            case False:
                // "prop": true
                return new JsonToken(JsonTokenType.Boolean, val == True);
            }

            return IsNumber(val)
                       ? new JsonToken(JsonTokenType.Numerical, GetValueDouble(val))
                       : new JsonToken(JsonTokenType.StringToken, val);
        }

        private int GetCh()
        {
            return _current + 1 < _len ? _buffer[++_current] : 0;
        }

        /// <summary></summary>
        private void EatWhitespace()
        {
            while (!IsEndOfFile) {
                if (_buffer[_current] != ' ' && _buffer[_current] != '\t')
                    break;
                ++_current;
            }
        }
    }

    /// <summary>
    /// Base class for json conversion.
    /// </summary>
    public class JsonObject {
        private static int  _depth;
        private static bool _isFormatted;

        private readonly List<object> _array;

        public bool                       IsArray { get; set; }
        public Dictionary<string, object> Dictionary { get; }
        public List<JsonObject>           Nodes { get; }

        /// <summary>
        /// Initializes a new instance of
        /// the <see cref="JsonObject" /> class.
        /// </summary>
        public JsonObject()
        {
            Dictionary   = new Dictionary<string, object>();
            Nodes        = new List<JsonObject>();
            _array       = new List<object>();
            _depth       = 0;
            _isFormatted = false;
            IsArray      = false;
        }

        /// <summary>
        /// Attempts to parse the base64 input string, by first converting it back
        /// to plain text then by parsing the plain text string.
        /// </summary>
        /// <param name="input">An assumed base 64 input string.</param>
        /// <returns>
        /// Null, if the string is null or empty or if an exception occurs, otherwise
        /// returns the result of the parse.
        /// </returns>
        /// <remarks>
        /// If the text is not in base 64 and a FormatException is thrown,
        /// then this method will attempt to parse the input as plain text json.
        /// </remarks>
        public static JsonObject ParseBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            try {
                var bytes = Convert.FromBase64String(input);
                var str   = Encoding.ASCII.GetString(bytes);
                return Parse(str);
            } catch (FormatException) {
                // Attempt to parse it normally..
                return Parse(input);
            } catch (Exception e) {
                LogUtils.Log(e);
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static JsonObject Parse(string val)
        {
            var pse = new JsonParser();
            return pse.Parse(val);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static JsonObject TryParse(string val)
        {
            try {
                var pse = new JsonParser();
                return pse.Parse(val);
            } catch (JsonSyntaxException) {
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static JsonObject TryParseFile(string val)
        {
            try {
                return ParseFile(val);
            } catch (JsonSyntaxException exception) {
                LogUtils.Log(exception.Message);
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(JsonObject obj)
        {
            if (obj != null)
                Nodes.Add(obj);
            if (IsArray)
                _array.Add(obj);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            if (key == null || value == null) return;
            if (Dictionary.ContainsKey(key))
                Dictionary[key] = value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, object value)
        {
            // TODO: The logic behind this and set should be flipped...
            if (key == null || value == null) return;
            if (Dictionary.ContainsKey(key))
                Dictionary[key] = value;
            else
                Dictionary.Add(key, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(object value)
        {
            if (value != null)
                _array.Add(value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
            return key != null && Dictionary.ContainsKey(key);
        }

        public string AsString(string key, string def)
        {
            object val = null;
            if (key != null && Dictionary.ContainsKey(key))
                val = Dictionary[key];
            switch (val) {
            case null:
                return def;
            case string s:
                return s;
            case int i:
                return i.ToString(CultureInfo.InvariantCulture);
            case long l:
                return l.ToString(CultureInfo.InvariantCulture);
            case double d:
                return d.ToString(CultureInfo.InvariantCulture);
            case bool b:
                return b ? "true" : "false";
            default:
                throw new InvalidCastException();
            }
        }

        public double AsDouble(string key, double def)
        {
            object val = null;
            if (key != null && Dictionary.ContainsKey(key))
                val = Dictionary[key];
            switch (val) {
            case null:
                return def;
            case string s when double.TryParse(s, out double d):
                return d;
            case string _:
                return 0;
            case int i:
                return i;
            case double d:
                return d;
            case bool b:
                return b ? 1 : 0;
            default:
                throw new InvalidCastException();
            }
        }

        public long AsLong(string key, long def)
        {
            object val = null;
            if (key != null && Dictionary.ContainsKey(key))
                val = Dictionary[key];
            switch (val) {
            case null:
                return def;
            case string s when long.TryParse(s, out long d):
                return d;
            case string _:
                return def;
            case long l:
                return l;
            case int i:
                return i;
            case double d:
                return (long)d;
            case bool b:
                return b ? 1 : 0;
            default:
                throw new InvalidCastException();
            }
        }

        public int AsInt(string key, int def)
        {
            object val = null;
            if (key != null && Dictionary.ContainsKey(key))
                val = Dictionary[key];

            switch (val) {
            case null:
                return def;
            case string s when int.TryParse(s, out int d):
                return d;
            case string _:
                return def;
            case int i:
                return i;
            case double d:
                return (int)d;
            case bool b:
                return b ? 1 : 0;
            default:
                throw new InvalidCastException();
            }
        }

        public bool AsBool(string key, bool def)
        {
            object val = null;
            if (key != null && Dictionary.ContainsKey(key))
                val = Dictionary[key];

            switch (val) {
            case null:
                return def;
            case string s when bool.TryParse(s, out bool d):
                return d;
            case string _:
                return def;
            case int i:
                return i != 0;
            case double d:
                return (int)d != 0;
            case bool b:
                return b;
            default:
                throw new InvalidCastException();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string AsString(string key)
        {
            return AsString(key, "null");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double AsDouble(string key)
        {
            return AsDouble(key, 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long AsLong(string key)
        {
            return AsLong(key, 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int AsInt(string key)
        {
            return AsInt(key, 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool AsBool(string key)
        {
            return AsBool(key, false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public JsonObject AsObject(string key)
        {
            object val = null;
            if (key != null && Dictionary.ContainsKey(key))
                val = Dictionary[key];
            return val as JsonObject;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            return Encoding.ASCII.GetBytes(AsCompactPrint());
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string ToBase64()
        {
            return Convert.ToBase64String(ToBytes());
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string AsPrettyPrint()
        {
            _isFormatted = true;
            var builder  = new StringBuilder();
            AsPrettyPrint(builder);
            return builder.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string AsCompactPrint()
        {
            _isFormatted = false;
            var builder  = new StringBuilder();
            AsCompactPrint(builder);
            return builder.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string AsBase64()
        {
            _isFormatted = false;
            var builder  = new StringBuilder();
            AsCompactPrint(builder);

            var compactJson = builder.ToString();
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(compactJson));
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _isFormatted ? AsPrettyPrint() : AsCompactPrint();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        private static void WriteSpace(StringBuilder builder)
        {
            for (var i = 0; i < _depth; ++i)
                builder.Append(' ');
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        private void AsPrettyPrint(StringBuilder builder)
        {
            builder.Append(IsArray
                               ? '['
                               : '{');
            builder.Append('\n');
            _depth += 4;
            int count;
            if (IsArray) {
                count = 0;
                foreach (var obj in _array) {
                    WriteSpace(builder);

                    switch (obj) {
                    case string s:
                        builder.Append('"');
                        builder.Append(s);
                        builder.Append('"');
                        break;
                    case bool b:
                        builder.Append(b
                                           ? "true"
                                           : "false");
                        break;
                    default:
                        // Recursive call
                        builder.Append(obj);
                        break;
                    }

                    if (count + 1 < _array.Count) {
                        builder.Append(',');
                        builder.Append('\n');
                    }

                    count++;
                }
            }

            count = 0;
            foreach (var key in Dictionary.Keys) {
                var val = Dictionary[key];
                WriteSpace(builder);
                builder.Append('"');
                builder.Append(key);
                builder.Append('"');
                builder.Append(':');
                builder.Append(' ');
                switch (val) {
                case string s:
                    builder.Append('"');
                    builder.Append(s);
                    builder.Append('"');
                    break;
                case bool b:
                    builder.Append(b
                                       ? "true"
                                       : "false");
                    break;
                default:
                    // Recursive call
                    builder.Append(val);
                    break;
                }

                if (count + 1 < Dictionary.Count) {
                    builder.Append(',');
                    builder.Append('\n');
                }

                count++;
            }

            _depth -= 4;
            builder.Append('\n');
            WriteSpace(builder);
            builder.Append(IsArray
                               ? ']'
                               : '}');
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        private void AsCompactPrint(StringBuilder builder)
        {
            int count;
            builder.Append(IsArray
                               ? '['
                               : '{');
            if (IsArray) {
                count = 0;
                foreach (var obj in _array) {
                    switch (obj) {
                    case string s:
                        builder.Append('"');
                        builder.Append(s);
                        builder.Append('"');
                        break;
                    case bool b:
                        builder.Append(b
                                           ? "true"
                                           : "false");
                        break;
                    default:
                        builder.Append(obj);
                        break;
                    }

                    if (count + 1 < _array.Count)
                        builder.Append(',');
                    count++;
                }
            }

            count = 0;
            foreach (var key in Dictionary.Keys) {
                var val = Dictionary[key];
                builder.Append('"');
                builder.Append(key);
                builder.Append('"');
                builder.Append(':');

                switch (val) {
                case string s:
                    builder.Append('"');
                    builder.Append(s);
                    builder.Append('"');
                    break;
                case bool b:
                    builder.Append(b
                                       ? "true"
                                       : "false");
                    break;
                default:
                    builder.Append(val);
                    break;
                }

                if (count + 1 < Dictionary.Count)
                    builder.Append(',');
                count++;
            }

            builder.Append(IsArray
                               ? ']'
                               : '}');
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JsonObject ParseFile(string path)
        {
            return Parse(StringUtils.ReadFileAsString(path));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<JsonObject> ParseFileAsync(string path)
        {
            return Task.Run(() => {
                var fileString = StringUtils.ReadFileAsString(path);

                var result = ParseFile(fileString);
                return result;
            });
        }
    }
}
