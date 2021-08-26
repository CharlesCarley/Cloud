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
using Cloud.Common;
using Cloud.Transaction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BookStore.Test
{
    /// <summary>
    /// This test needs to have an up to date instance of the BookStore.Store application
    /// running externally in order to pass. The host configuration should be placed in a
    /// file named testExternalServer.json
    /// </summary>
    [TestClass]
    public class TestVMServer {
        private HostConfig    _hostConfig;
        private static string Content      => AppContext.BaseDirectory + "..\\..\\..\\VMTest\\";
        private static string HostSettings => $"{Content}..\\testExternalServer.json";

        private void Populate()
        {
            var book = new Client.Book {
                Key         = "9780534534653",
                Author      = "Danial Kolak & Raymond  Martin",
                Title       = "Wisdom Without Answers",
                Price       = 16.0f,
                ISBN        = "9780534534653, 0534534651",
                Url         = "https://www.google.com/books/edition/Wisdom_Without_Answers/Ny0MAAAACAAJ?hl=en",
                PublishDate = "2002"
            };
            book.Save();
            book.CreateTransaction().Save();

            book = new Client.Book {
                Key         = "9780495094920",
                Author      = "Joel Feinberg & Russ Shafer-Landau",
                Title       = "Reason & Responsibility",
                Price       = 113.32f,
                ISBN        = "9780495094920, 0495094927",
                Url         = "https://www.google.com/books/edition/Reason_and_Responsibility/QSktgGNd1m4C?hl=en",
                PublishDate = "2008"
            };
            book.Save();
            book.CreateTransaction().Save();
        }

        [TestInitialize]
        public void Initialize()
        {
            _hostConfig = new HostConfig();
            if (!_hostConfig.LoadFromStorage(HostSettings)) {
                Assert.Fail("Failed to load the host configuration.");
            }
            Transaction.SetConnectionParameters(_hostConfig);

            Assert.IsTrue(
                Transaction.PingDatabase(_hostConfig.Timeout),
                $"Failed to connect to the database located at 'http://{_hostConfig.Host}:{_hostConfig.Port}' ");

            // set up a temp destination directory

            if (!Directory.Exists(Content)) {
                Directory.CreateDirectory(Content);
            } else {
                Directory.Delete(Content, true);
                Directory.CreateDirectory(Content);
            }

            Client.Database.Register($"{Content}\\LocalTestServer.Client.db");
            Client.Database.Open();
            Client.BookTransaction.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Client.Database.Close();

            if (!Directory.Exists(Content))
                Directory.CreateDirectory(Content);
            else {
                Directory.Delete(Content, true);
                Directory.CreateDirectory(Content);
            }
        }

        [TestMethod]
        public void TestSetup()
        {
            var book = new Client.Book {
                Key         = "Test",
                Author      = "Test",
                Title       = "Test",
                Price       = 99.95f,
                PublishDate = StringUtils.ToString(DateTime.Now.TimeOfDay)
            };

            var result1 = book.Save();
            book.CreateTransaction().Save();
            Assert.IsTrue(result1);

            var result = Client.BookTransaction.SelectArray();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(1, result[1]);

            result1 = book.Save();
            book.CreateTransaction().Save();
            Assert.IsTrue(result1);

            // tests the revision bump
            result = Client.BookTransaction.SelectArray();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
        }

        [TestMethod]
        public void TestGeneral()
        {
            Populate();

            //
            var list = Client.BookTransaction.SelectArray();
            Assert.AreEqual(4, list.Count);

            var book = Client.BookTransaction.SelectById(list[0]);
            Assert.IsNotNull(book);
            Assert.AreEqual("9780534534653", book.Key);
            Assert.AreEqual("Danial Kolak & Raymond  Martin", book.Author);
            Assert.AreEqual("Wisdom Without Answers", book.Title);
            Assert.AreEqual("9780534534653, 0534534651", book.ISBN);

            book = Client.BookTransaction.SelectById(list[2]);
            Assert.IsNotNull(book);
            Assert.AreEqual("9780495094920", book.Key);
            Assert.AreEqual("Joel Feinberg & Russ Shafer-Landau", book.Author);
            Assert.AreEqual("Reason & Responsibility", book.Title);
            Assert.AreEqual("9780495094920, 0495094927", book.ISBN);

            book = Client.BookTransaction.SelectByKey("9780495094920");
            Assert.IsNotNull(book);
            Assert.AreEqual("9780495094920", book.Key);
            Assert.AreEqual("Joel Feinberg & Russ Shafer-Landau", book.Author);
            Assert.AreEqual("Reason & Responsibility", book.Title);
            Assert.AreEqual("9780495094920, 0495094927", book.ISBN);

            Client.BookTransaction.Drop("9780495094920");
            book = Client.BookTransaction.SelectByKey("9780495094920");
            Assert.IsNull(book);

            Client.BookTransaction.Clear();
            book = Client.BookTransaction.SelectByKey("9780534534653");
            Assert.IsNull(book);

            list = Client.BookTransaction.SelectArray();
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }
    }
}
