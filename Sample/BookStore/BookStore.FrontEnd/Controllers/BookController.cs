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
using BookStore.FrontEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.FrontEnd.Controllers
{
    public class BookController : Controller {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // TODO: this needs managed better
            BookModel.SelectArray();
        }

        // GET: BookController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BookController/Details/5
        public ActionResult Details(int id)
        {
            CurrentItem = null;
            var book    = BookTransaction.SelectById(id);
            if (book != null)
                CurrentItem = new BookModel(book, id);
            return View();
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try {
                if (!collection.ContainsKey("Key")) {
                    return View();
                }

                var key = collection["Key"];
                if (string.IsNullOrEmpty(key)) {
                    BookModel.ErrorMessage = "The key field is required.";
                    return View();
                }

                if (BookTransaction.ContainsKey(key)) 
                {
                    BookModel.ErrorMessage = "Duplicate key...";
                    return View();
                }

                var book = new Book {
                    Key = key
                };

                if (collection.ContainsKey("Author"))
                    book.Author = collection["Author"];
                if (collection.ContainsKey("Title"))
                    book.Title = collection["Title"];
                if (collection.ContainsKey("Description"))
                    book.Description = collection["Description"];

                book.CreateTransaction().Save();

                BookModel.ErrorMessage = null;
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: BookController/Edit/5
        public ActionResult Edit(int id)
        {
            CurrentItem = null;
            var book    = BookTransaction.SelectById(id);
            if (book != null)
                CurrentItem = new BookModel(book, id);
            return View();
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try {
                if (!collection.ContainsKey("Key")) {
                    return View();
                }

                var key = collection["Key"];
                if (string.IsNullOrEmpty(key)) {
                    BookModel.ErrorMessage = "The key field is required.";
                    return View();
                }

                if (!BookTransaction.ContainsKey(key)) {
                    BookModel.ErrorMessage = "Unknown item";
                    return View();
                }

                var book = BookTransaction.SelectByKey(key);
                if (book is null) {
                    // Something else is wrong, it should be a valid ptr....
                    BookModel.ErrorMessage = "Failed to extract the book.";
                    return View();
                }

                if (collection.ContainsKey("Author"))
                    book.Author = collection["Author"];
                if (collection.ContainsKey("Title"))
                    book.Title = collection["Title"];
                if (collection.ContainsKey("Description"))
                    book.Description = collection["Description"];

                book.CreateTransaction().Save();

                BookModel.ErrorMessage = null;
                return RedirectToAction(nameof(Details), new {id = CurrentItem.Index});

            } catch {
                return View();
            }
        }
        
        
        public ActionResult Delete(int id)
        {
            CurrentItem = null;
            var book = BookTransaction.SelectById(id);
            if (book != null && BookTransaction.ContainsKey(book.Key))
                BookTransaction.Drop(book.Key);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult OnCreate()
        {
            return RedirectToAction(nameof(Index));
        }

        [BindProperty]
        public static BookModel CurrentItem { get; set; }

        public IActionResult Clear()
        {
            try {
                BookTransaction.Clear();
            } catch {
                // ignore
                // TODO: setup a specific error page.
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
