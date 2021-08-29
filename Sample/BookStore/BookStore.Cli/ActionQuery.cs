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

        public bool InvokeIf(string input)
        {
            // Attempts to invoke the action mapped to the supplied input.

            
            // A result of true means that the command was processed,
            // with or without error.
            var result = false;

            if (string.IsNullOrEmpty(input)) {
                //  Just print the help if it's available.

                TryDisplayLocalHelp();

            } else if (VoidActions.ContainsKey(input)) {
                // Handle actions with no parameter

                VoidActions[input]?.Invoke();
                result = true;

            } else if (IntActions.ContainsKey(input)) {
                // Handle actions with an integer parameter

                if (ReadIntInput(out var value)) {
                    IntActions[input]?.Invoke(value);
                } else {
                    Console.WriteLine(Resources.ReadInvalidInt);
                }

                result = true;
            } else if (StringActions.ContainsKey(input)) {
                // Handle actions with a string parameter.

                var str = ReadInput().Trim();

                if (!string.IsNullOrEmpty(str)) {
                    // Validation of the argument should happen on a case by case basis.

                    StringActions[input]?.Invoke(str);
                } else {
                    Console.WriteLine(Resources.ReadInvalidString);
                }
                result = true;

            } else if (SubActions.ContainsKey(input)) {
                // The input is mapped to another table, invoke its
                // query method.

                var str = ReadInput().Trim();
                if (!string.IsNullOrEmpty(str)) {
                    result = SubActions[input].InvokeIf(str);
                } else {
                    Console.WriteLine(Resources.MissingArgument, input);
                    result = true;
                }
            }

            return result;
        }

        public void InvalidInput(string input)
        {
            Console.WriteLine(Resources.UnknownOption, input);
        }

        private bool IsStopCharacter(int ch)
        {
            return ch == -1 || ch == ' ' || ch == '\n' || ch == '\r';
        }

        public string ReadInput()
        {
            // Scan a short string sequence until a stop character is recognized,
            // or until the number of characters goes beyond the maximum character limit.

            _builder.Clear();
            var read = 0;
            int ch;

            do {
                ++read;
                ch = Console.Read();
                if (!IsStopCharacter(ch))
                    _builder.Append((char)ch);

            } while (read < 32 && !IsStopCharacter(ch));

            return _builder.ToString();
        }

        public bool ReadIntInput(out int value)
        {
            var str = ReadInput();
            if (StringUtils.IsNumber(str)) {
                if (int.TryParse(str, out value))
                    return true;
            }

            value = 0;
            Console.WriteLine(Resources.IntConversionFail);
            return false;
        }
    }
}
