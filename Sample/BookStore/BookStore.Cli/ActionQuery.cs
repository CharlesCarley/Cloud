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
using System.Text;
using Cloud.Common;

namespace BookStore.Cli
{
    /// <summary>
    /// Is a basic dictionary container that maps
    /// predefined keywords to function calls.
    /// </summary>
    public class ActionQuery {
        private readonly StringBuilder _builder;
        private ActionQuery            _parent;

        public Dictionary<string, Action>         VoidActions { get; set; }
        public Dictionary<string, Action<int>>    IntActions { get; set; }
        public Dictionary<string, Action<string>> StringActions { get; set; }
        public Dictionary<string, ActionQuery>    SubActions { get; set; }

        public ActionQuery()
        {
            _parent  = null;
            _builder = new StringBuilder();

            VoidActions   = new Dictionary<string, Action>();
            IntActions    = new Dictionary<string, Action<int>>();
            StringActions = new Dictionary<string, Action<string>>();
            SubActions    = new Dictionary<string, ActionQuery>();
        }

        public void Add(string verb, Action method)
        {
            if (!VoidActions.ContainsKey(verb))
                VoidActions.Add(verb, method);
            else
                VoidActions[verb] = method;
        }

        public void Add(string verb, Action<int> method)
        {
            if (!IntActions.ContainsKey(verb))
                IntActions.Add(verb, method);
            else
                IntActions[verb] = method;
        }

        public void Add(string verb, Action<string> method)
        {
            if (!StringActions.ContainsKey(verb))
                StringActions.Add(verb, method);
            else
                StringActions[verb] = method;
        }

        public void Add(string verb, ActionQuery query)
        {
            if (!SubActions.ContainsKey(verb))
                SubActions.Add(verb, query);
            else
                SubActions[verb] = query;

            if (query != null)
                query._parent = this;
        }

        public void InvokeAction(ActionQueryResult input)
        {
            // Attempts to invoke the action mapped to the supplied input.

            if (input.Type == ActionToken.Identifier) {
                if (VoidActions.ContainsKey(input.Value)) {
                    // Handle actions with no parameter

                    VoidActions[input.Value]?.Invoke();

                } else if (IntActions.ContainsKey(input.Value)) {
                    // Handle actions with an integer parameter

                    var action = ReadAction();

                    switch (action.Type) {
                    case ActionToken.Integer:
                        IntActions[input.Value]?.Invoke(StringUtils.ToInt(action.Value));
                        break;
                    case ActionToken.Identifier:
                        Console.WriteLine(Resources.IntReadIdentifer);
                        break;
                    case ActionToken.String:
                        Console.WriteLine(Resources.IntReadString);
                        break;
                    case ActionToken.Empty:
                        Console.WriteLine(Resources.IntReadSpace);
                        break;
                    default:
                        Console.WriteLine(Resources.IntReadError);
                        break;
                    }

                } else if (StringActions.ContainsKey(input.Value)) {
                    // Handle actions with a string parameter.

                    var action = ReadAction();
                    if (action.Type == ActionToken.Identifier ||
                        action.Type == ActionToken.String ||
                        action.Type == ActionToken.Json) {
                        StringActions[input.Value]?.Invoke(action.Value);
                    } else {
                        Console.WriteLine(Resources.ReadInvalidString, action.Value);
                    }

                } else if (SubActions.ContainsKey(input.Value)) {
                    // The input is mapped to another table, so
                    // invoke it's query method.

                    var action = ReadAction();
                    if (action.Type != ActionToken.Identifier)
                        Console.WriteLine(Resources.ReadActionNotFound, input.Value, action.Value);
                    else
                        SubActions[input.Value].InvokeAction(action);
                } else {
                    Console.WriteLine(Resources.NotFound, input.Value);
                }

            } else if (input.Type != ActionToken.Empty) {
                Console.WriteLine(Resources.NotFound, input.Value);
            }
        }

        private static bool IsAlphabet(int ch)
        {
            return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '.';
        }

        /// <summary>
        /// Scans the supplied input for strings, integers or Json.
        /// </summary>
        /// <remarks>
        /// Json input is collected by first entering a grave '`' to signal that it should start collecting,
        /// then it is terminated by entering another grave character.
        /// </remarks>
        public ActionQueryResult ReadAction()
        {
            var result = new ActionQueryResult { Type = ActionToken.Start };
            _builder.Clear();

            var i    = 0;
            var done = false;

            do {
                var ch = Console.Read();
                switch (ch) {
                case -1:
                    result.Type = ActionToken.Eof;
                    done        = true;
                    break;
                case '\r': {
                    if (Console.In.Peek() == '\n')
                        Console.In.Read();

                    if (i > 0) {
                        done = true;
                    }
                    break;
                }
                case '\n':
                    Console.In.Read();
                    done = true;
                    break;
                case ' ': {
                    while (Console.In.Peek() == ' ') {
                        Console.Read();
                    }

                    if (i > 0) {
                        // This is to handle any white space before the text.
                        // IE; If no text is in the builder, keep going.
                        done = true;
                    }
                    break;
                }
                case '`': {
                    // This needs to process all the characters that can compose a json file.
                    // Double quotes should be escaped \".
                    // The entire contents should be placed in between the grave character `{Json Content}`

                    ch = Console.In.Peek();
                    while (ch != '`') {
                        ch = Console.Read();
                        if (ch == '`') continue;

                        if (StringUtils.IsInJsonCharacterSet((char)ch)) {
                            // \\ is only for the command line right now.
                            if (ch != '\\')
                                _builder.Append((char)ch);

                        } else {
                            // Remove any processes thus far and exit.

                            Console.WriteLine(Resources.InvalidChar, ch);

                            _builder.Clear();
                            ch = '`';
                        }
                    }
                    done = true;
                    break;
                }
                default: {
                    if (IsAlphabet(ch)) {
                        _builder.Append((char)ch);

                    } else if (StringUtils.IsDigit(ch)) {
                        _builder.Append((char)ch);
                    } else {
                        // undefined
                        Console.WriteLine(Resources.InvalidChar, ch);
                        done = true;
                    }
                } break;
                }
            } while (!done && ++i < 16);

            result.Value = _builder.ToString();
            if (string.IsNullOrEmpty(result.Value)) {
                result.Type = ActionToken.Empty;
            } else if (result.Value.StartsWith('{') && result.Value.EndsWith('}')) {
                result.Type = ActionToken.Json;
            } else if (result.Value.StartsWith('[') && result.Value.EndsWith(']')) {
                result.Type = ActionToken.Json;
            } else if (StringUtils.IsIpAddressOrDomain(result.Value)) {
                result.Type = ActionToken.Identifier;
            } else if (StringUtils.IsNumber(result.Value)) {
                result.Type = ActionToken.Integer;
            } else if (StringUtils.IsLetter(result.Value)) {
                result.Type = ActionToken.Identifier;
            } else if (result.Type != ActionToken.Eof) {
                result.Type = ActionToken.Error;
            }

            return result;
        }
    }
}
