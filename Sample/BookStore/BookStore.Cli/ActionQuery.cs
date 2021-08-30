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

        private void TryDisplayLocalHelp()
        {
            if (VoidActions.ContainsKey("help"))
                VoidActions["help"]?.Invoke();
            else
                _parent?.TryDisplayLocalHelp();
        }

        public bool InvokeIf(ActionQueryResult input)
        {
            // Attempts to invoke the action mapped to the supplied input.

            // A result of true means that the command was processed,
            // with or without error.
            var result = false;
            if (input.Type != ActionToken.String) {
                // unknown at this point
                return false;
            }

            if (string.IsNullOrEmpty(input.Value)) {
                //  Just print the help if it's available.

                TryDisplayLocalHelp();

            } else if (VoidActions.ContainsKey(input.Value)) {
                // Handle actions with no parameter

                VoidActions[input.Value]?.Invoke();
                result = true;

            } else if (IntActions.ContainsKey(input.Value)) {
                // Handle actions with an integer parameter

                var action = ReadAction();
                if (action.Type == ActionToken.Integer) {
                    IntActions[input.Value]?.Invoke(StringUtils.ToInt(action.Value));
                } else {
                    Console.WriteLine(Resources.ReadInvalidInt);
                }

                result = true;
            } else if (StringActions.ContainsKey(input.Value)) {
                // Handle actions with a string parameter.

                var action = ReadAction();
                if (action.Type == ActionToken.String) {
                    StringActions[input.Value]?.Invoke(action.Value);
                } else {
                    Console.WriteLine(Resources.ReadInvalidString);
                }

                result = true;

            } else if (SubActions.ContainsKey(input.Value)) {
                // The input is mapped to another table, so
                // invoke it's query method.

                var action = ReadAction();
                result     = true;
                if (action.Type != ActionToken.String)
                    Console.WriteLine(Resources.ReadActionNotFound, input, action);
                else
                    result = SubActions[input.Value].InvokeIf(action);
            } else {
                Console.WriteLine(Resources.NotFound, input.Value);
                result = true;
            }

            return result;
        }

        public void InvalidInput(string input)
        {
            Console.WriteLine(Resources.UnknownOption, input);
        }

        private static bool IsAlphabet(int ch)
        {
            return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '.';
        }

        private static bool IsDigit(int ch)
        {
            return ch >= '0' && ch <= '9';
        }

        public ActionQueryResult ReadAction()
        {
            var result = new ActionQueryResult { Type = ActionToken.Error };
            _builder.Clear();

            var i    = -1;
            var done = false;

            while (!done && ++i < 16) {
                var ch = Console.Read();
                switch (ch) {
                case -1:
                    result.Type = ActionToken.Eof;
                    done        = true;
                    break;
                case '\r': {
                    if (Console.In.Peek() == '\n')
                        Console.In.Read();
                    done = true;
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
                    if (_builder.Length != 0)
                        done = true;
                    break;
                }
                default: {
                    if (IsAlphabet(ch)) {
                        _builder.Append((char)ch);

                    } else if (IsDigit(ch)) {
                        _builder.Append((char)ch);
                    } else {
                        // undefined
                        done = true;
                    }
                } break;
                }
            }

            result.Value = _builder.ToString();
            if (StringUtils.IsIpAddressOrDomain(result.Value)) {
                result.Type = ActionToken.String;
            } else if (StringUtils.IsNumber(result.Value)) {
                result.Type = ActionToken.Integer;
            } else if (StringUtils.IsStringOnlyLetters(result.Value)) {
                result.Type = ActionToken.String;
            } else if (result.Type != ActionToken.Eof) {
                result.Type = ActionToken.Error;
            }

            return result;
        }
    }
}
