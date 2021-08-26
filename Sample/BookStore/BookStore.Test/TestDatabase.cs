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
using BookStore.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookStore.Test
{
    [TestClass]
    public class TestDatabase {
        public string TestDatabaseFile => $"{Environment.CurrentDirectory}..\\..\\TestDatabase.db";

        [TestInitialize]
        public void OpenFreshDatabase()
        {
            // The static interface should not be initialized at this point, so
            Assert.IsNull(Database.Connection);    // no connection pointer,
            Assert.IsFalse(Database.IsConnected);  // which effects IsConnected,
            Assert.IsNull(Book.Connection);        // which sub classes extract from the Database class,
            Assert.IsNull(Book.Query);             // and Query should only be valid if there is a connection.
            Assert.IsFalse(Book.IsConnected);      // This is the same as the Database.IsConnected test.
            if (File.Exists(TestDatabaseFile)) {   // Recreate the database for every test.
                try {                              //
                    File.Delete(TestDatabaseFile); //
                } catch {                          //
                                                   // ignored
                    Assert.Fail();                 // Just fail, because the database needs a fresh state..
                }                                  //
            }                                      //
            Database.Register(TestDatabaseFile);
            Database.Open();

            // test the inverse from above they should all be valid now..
            Assert.IsNotNull(Database.Connection);
            Assert.IsTrue(Database.IsConnected);
            Assert.IsNotNull(Book.Connection);
            Assert.IsNotNull(Book.Query);
            Assert.IsTrue(Book.IsConnected);

            Book.Saved += BookSaved;                   // Test the saved trigger
            Book.StateChanged += GuardedExceptionTest; // Tests the behavior of exceptions
                                                       // in trigger callbacks..
        }

        [TestCleanup]
        public void CloseAndAssertDatabase()
        {
            Assert.IsNotNull(Database.Connection);
            Assert.IsTrue(Database.IsConnected);
            Database.Close();

            if (File.Exists(TestDatabaseFile)) {
                try {
                    File.Delete(TestDatabaseFile);
                } catch {
                    // ignored
                    Assert.Fail();
                }
            }

            Assert.IsNull(Database.Connection);
            Assert.IsFalse(Database.IsConnected);
            Assert.IsNull(Book.Connection);
            Assert.IsNull(Book.Query);
            Assert.IsFalse(Book.IsConnected);

            Book.Saved -= BookSaved;
            Book.StateChanged -= GuardedExceptionTest;
        }

        [TestMethod]
        public void TestBook()
        {
            Assert.IsTrue(Database.IsConnected);

            var res = Book.ExistsById(1);
            Assert.IsFalse(res);

            Book.Saved += BookSaved;

            Book.Save(new Book {
                Key         = Database.GenerateKey(),
                Title       = "The wonderful world of tests",
                Author      = "Anon",
                PublishDate = "Friday July 9, 2021",
                Price       = 1
            });

            res = Book.ExistsById(1);
            Assert.IsTrue(res);
        }

        private static void BookSaved(Book item)
        {
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void TestSelectByKey()
        {
            try {
                Book.SelectByKey(null);
                Assert.Fail();
            } catch {
                // ignored
                // pass
            }

            try {
                var hash = Database.GenerateKey();

                var result = Book.SelectByKey(hash);
                Assert.IsNull(result);

                result = new Book {
                    Author      = "TestSelectByKey",
                    Price       = 1,
                    Key         = hash,
                    PublishDate = "1/1/1"
                };
                Assert.IsTrue(result.Save());

                var result2 = Book.SelectByKey(hash);
                Assert.IsNotNull(result2);

                result2.Price = 1000;
                Assert.IsTrue(result2.Save());

                Book.DropByKey(hash);
                Assert.IsNull(Book.SelectByKey(hash));

                // to invoke GuardedExceptionTest
                Book.Clear();
            } catch {
                // ignored
                Assert.Fail();
            }
        }

        private void GuardedExceptionTest()
        {
            // just to test trigger exception handling
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestSelectByIdentifier()
        {
            for (var i = -10; i < 20; ++i)
                Assert.IsNull(Book.SelectByIdentifier(i));

            var result = new Book {
                Author      = "TestSelectByKey",
                Price       = 1,
                Key         = "A",
                PublishDate = "1/1/1"
            };
            result.Save();

            // will be the first element
            Assert.IsNotNull(Book.SelectByIdentifier(1));
        }
    }
}
