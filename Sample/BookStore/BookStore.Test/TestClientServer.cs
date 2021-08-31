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
using BookStore.Client;
using Cloud.Common;
using Cloud.Transaction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookStore.Test
{
    /// This test goes through an in memory stage to communicate
    /// back and forth in the same environment
    [TestClass]
    public class TestClientServer {
        private static LocalTestServer _server;

        private static void Populate()
        {
            var book = new Book {
                Key         = "9780534534653",
                Author      = "Danial Kolak & Raymond  Martin",
                Title       = "Wisdom Without Answers",
                Price       = 16.0f,
                ISBN        = "9780534534653, 0534534651",
                Url         = "https://www.google.com/books/edition/Wisdom_Without_Answers/Ny0MAAAACAAJ?hl=en",
                PublishDate = "2002"
            };
            book.Save();

            var transaction = book.CreateTransaction();
            if (transaction.ExistsRemotely() == ReceiptCode.False)
                transaction.Save();

            book = new Book {
                Key         = "9780495094920",
                Author      = "Joel Feinberg & Russ Shafer-Landau",
                Title       = "Reason & Responsibility",
                Price       = 113.32f,
                ISBN        = "9780495094920, 0495094927",
                Url         = "https://www.google.com/books/edition/Reason_and_Responsibility/QSktgGNd1m4C?hl=en",
                PublishDate = "2008"
            };
            book.Save();

            transaction = book.CreateTransaction();
            if (transaction.ExistsRemotely() == ReceiptCode.False)
                transaction.Save();
        }

        [TestInitialize]
        public void Initialize()
        {
            _server = new LocalTestServer();
            _server.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server?.Stop();
            _server = null;
        }

        [TestMethod]
        public void TestSetup()
        {
            var book = new Book {
                Key         = "Test",
                Author      = "Test",
                Title       = "Test",
                Price       = 99.95f,
                PublishDate = StringUtils.ToString(DateTime.Now.TimeOfDay)
            };

            var result1 = book.Save();
            book.CreateTransaction().Save();
            Assert.IsTrue(result1);

            var result = BookTransaction.SelectArray();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(1, result[1]);

            result1 = book.Save();
            book.CreateTransaction().Save();
            Assert.IsTrue(result1);

            // tests the revision bump
            result = BookTransaction.SelectArray();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
        }

        [TestMethod]
        public void TestGeneral()
        {
            Populate();

            var list = BookTransaction.SelectArray();
            Assert.AreEqual(4, list.Count);

            var book = BookTransaction.SelectById(list[0]);
            book.Save();

            Assert.IsNotNull(book);
            Assert.AreEqual(1, book.Identifier);
            Assert.AreEqual(1, book.ServerId);

            Assert.AreEqual("9780534534653", book.Key);
            Assert.AreEqual("Danial Kolak & Raymond  Martin", book.Author);
            Assert.AreEqual("Wisdom Without Answers", book.Title);
            Assert.AreEqual("9780534534653, 0534534651", book.ISBN);

            book = BookTransaction.SelectById(list[2]);
            Assert.IsNotNull(book);
            Assert.AreEqual("9780495094920", book.Key);
            Assert.AreEqual("Joel Feinberg & Russ Shafer-Landau", book.Author);
            Assert.AreEqual("Reason & Responsibility", book.Title);
            Assert.AreEqual("9780495094920, 0495094927", book.ISBN);

            book = BookTransaction.SelectByKey("9780495094920");
            Assert.IsNotNull(book);
            Assert.AreEqual("9780495094920", book.Key);
            Assert.AreEqual("Joel Feinberg & Russ Shafer-Landau", book.Author);
            Assert.AreEqual("Reason & Responsibility", book.Title);
            Assert.AreEqual("9780495094920, 0495094927", book.ISBN);

            BookTransaction.Drop("9780495094920");
            book = BookTransaction.SelectByKey("9780495094920");
            Assert.IsNull(book);

            BookTransaction.Clear();
            book = BookTransaction.SelectByKey("9780534534653");
            Assert.IsNull(book);

            list = BookTransaction.SelectArray();
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(Transaction.PingDatabase(1000));

            BookTransaction.ContainsKey(null);
            // This key should be valid if Populate was called
            var value = BookTransaction.ContainsKey("9780534534653");
            Assert.IsFalse(value);

            Populate();
            value = BookTransaction.ContainsKey("9780534534653");
            Assert.IsTrue(value);
        }
    }
}
