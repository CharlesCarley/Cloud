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
using BookStore.Client;
using System;
using System.IO;
using System.Text;
using Cloud.Common;
using Cloud.Transaction;
using Settings = BookStore.Client.Settings;

namespace BookStore.Cli
{
    public enum StateCodes
    {
        Interactive,
        CommandLine,
    }

    public class Application {
        public StateCodes CurrentState { get; set; }
        public bool       Quit { get; set; }

        private readonly string[] _args;

        private readonly ActionQuery _actionQueryTable;

        private Settings Settings { get; set; }

        public Application(string[] args)
        {
            if (args.Length <= 0) {
                _args        = null;
                CurrentState = StateCodes.Interactive;
            } else {
                CurrentState = StateCodes.CommandLine;
                _args        = args;
            }

            // main screen actions.

            _actionQueryTable = new ActionQuery();
            _actionQueryTable.Add("help", () => Console.WriteLine(Resources.Options));
            _actionQueryTable.Add("ls", () => Console.WriteLine(Resources.Options));
            _actionQueryTable.Add("quit", () => Quit = true);
            _actionQueryTable.Add("q", () => Quit = true);
            _actionQueryTable.Add("cls", Console.Clear);
            _actionQueryTable.Add("clear", Console.Clear);
            _actionQueryTable.Add("ping", PingHost);

            // configuration actions

            var config = new ActionQuery();

            config.Add("help", () => Console.WriteLine(Resources.Configuration));
            config.Add("ls", ConfigList);
            config.Add("list", ConfigList);

            config.Add("host", ConfigSetHost);
            config.Add("port", ConfigSetPort);
            config.Add("timeout", ConfigSetTimeout);

            _actionQueryTable.Add("config", config);
            _actionQueryTable.Add("cfg", config);

            // book table actions.

            var book = new ActionQuery();

            book.Add("help", () => Console.WriteLine(Resources.Book));
            book.Add("ls", BookSelectAll);
            book.Add("list", BookSelectAll);
            book.Add("clear", BookClear);

            book.Add("selectArray", BookSelectArray);
            book.Add("selectById", BookSelectById);

            book.Add("selectByKey", BookSelectByKey);
            book.Add("containsKey", BookContainsKey);
            book.Add("deleteById", BookDropById);
            book.Add("deleteByKey", BookDropByKey);
            book.Add("save", BookSave);

            _actionQueryTable.Add("book", book);
        }
        
        private void ConfigList()
        {
            Console.WriteLine(Settings.ToJson().AsPrettyPrint());
        }

        private void ConfigSetTimeout(int timeout)
        {
            if (timeout >= Cloud.Common.Settings.MinTimeOut && timeout <= Cloud.Common.Settings.MaxTimeOut) {
                Settings.Timeout = timeout;
                Settings.Save();
            } else {
                Console.WriteLine(Resources.TimeoutFail);
            }
        }

        private void ConfigSetPort(int port)
        {
            if (HostConfig.IsValidPort(port)) {
                Settings.Port = port;
                Settings.Save();
            } else {
                Console.WriteLine(Resources.ConfigInvalidPort);
            }
        }

        private void ConfigSetHost(string input)
        {
            if (HostConfig.IsValidHostName(input)) {
                Settings.Host = input;
                Settings.Save();
            } else {
                Console.WriteLine(Resources.ConfigInvalidHost);
            }
        }

        private static void PingHost()
        {
            Console.WriteLine(Transaction.PingDatabase(10000)
                                  ? Resources.ConnectedMessage
                                  : Resources.NotConnectedMessage);
        }

        private void BookClear()
        {
            Console.WriteLine(Resources.BookClearMessage);

            ActionQueryResult input;
            do {
                input = _actionQueryTable.ReadAction();

                if (input.Value.Equals("yes")) {
                    try {
                        BookTransaction.Clear();
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
            } while (!input.Value.Equals("yes") && !input.Value.Equals("no"));
        }

        private static void BookSelectAll()
        {
            try {
                var pairs = BookTransaction.SelectArray();
                if (pairs == null)
                    return;

                if (pairs.Count <= 0) {
                    Console.WriteLine(Resources.BlankArray);
                } else {
                    for (var i = 0; i < pairs.Count; i += 2)
                        BookSelectById(pairs[i]);
                }

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void BookSelectById(int obj)
        {
            try {
                var book = BookTransaction.SelectById(obj);

                if (book is null)
                    Console.WriteLine(Resources.SelectError, obj);
                else
                    Console.WriteLine(book.ToJson().AsPrettyPrint());

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void BookDropById(int obj)
        {
            try {
                var book = BookTransaction.SelectById(obj);
                if (book != null) {
                    // this needs a return status
                    BookTransaction.Drop(book.Key);
                }

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void BookDropByKey(string key)
        {
            try {
                // this needs a return status
                BookTransaction.Drop(key);

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void BookSave(string text)
        {
            try {
                var book = (Book)JsonParser.Unwrap(text, typeof(Book), false);

                if (book is null)
                    Console.WriteLine(Resources.BookInvalidUnwrap);
                else {
                    if (string.IsNullOrEmpty(book.Key)) {
                        Console.WriteLine(Resources.BookInvalidKey, text);
                    } else {
                        book.CreateTransaction().Save();
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void BookSelectByKey(string key)
        {
            try {
                var book = BookTransaction.SelectByKey(key);

                if (book is null)
                    Console.WriteLine(Resources.SelectKeyError, key);
                else
                    Console.WriteLine(book.ToJson().AsPrettyPrint());

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void BookContainsKey(string key)
        {
            try {
                var book = BookTransaction.ContainsKey(key);
                Console.WriteLine(book);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void BookSelectArray()
        {
            try {
                var pairs = BookTransaction.SelectArray();

                if (pairs is null || pairs.Count <= 0) {
                    Console.WriteLine(Resources.BlankArray);
                } else {
                    Console.WriteLine(StringUtils.IntListToString(pairs));
                }

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void Usage()
        {
            Console.WriteLine(Resources.Options);
        }

        public void InitializeDatabase()
        {
            Database.Register("BookStore.Cli.db");

            Settings.Saved += SettingsOnSaved;
            Settings = Settings.SelectByKey("MainSettings");
            if (Settings is null) {
                Settings = new Settings {
                    Key     = "MainSettings",
                    Host    = "localhost",
                    Port    = 80,
                    Timeout = 1000,
                };
                Settings.Save();
            }

            SettingsOnSaved(Settings);
        }

        private static void SettingsOnSaved(Settings item)
        {
            Transaction.TrySetConnectionParameters(
                new HostConfig {
                    Host    = item.Host,
                    Port    = item.Port,
                    Timeout = item.Timeout,
                });
        }

        public void RunInteractive()
        {
            InitializeDatabase();
            Usage();

            Quit = false;

            while (!Quit) {
                Console.Write('\n');
                Console.Write(Resources.ActionPrompt,
                              Settings.Host,
                              Settings.Port);

                var input = _actionQueryTable.ReadAction();

                if (input.Type == ActionToken.Error) {
                    // record the error
                    Console.WriteLine(Resources.ReadError);
                } else {
                    _actionQueryTable.InvokeAction(input);
                }
            }
        }

        private void RunClArgs()
        {
            InitializeDatabase();

            var builder = new StringBuilder();
            for (var index = 0; index < _args.Length; index++) {
                var arg = _args[index];

                if (index > 0)
                    builder.Append(' ');

                builder.Append(arg);
            }

            var inputStream = new StringReader(builder.ToString());
            Console.SetIn(inputStream);

            ActionQueryResult input;

            do {
                input = _actionQueryTable.ReadAction();
                if (input.Type == ActionToken.Error) {
                    Console.WriteLine(Resources.ReadError);
                } else {
                    _actionQueryTable.InvokeAction(input);
                }
            } while (input.Value.Length > 0 && input.Type != ActionToken.Error);
        }

        public void Run()
        {
            if (CurrentState == StateCodes.Interactive) {
                RunInteractive();
            } else {
                RunClArgs();
            }
        }
    }
}
