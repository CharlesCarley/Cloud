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
            var book = BookTransaction.SelectById(id);
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
                //
                if (!collection.ContainsKey("Key")) {
                    // Something is wrong, an internal form parameter did not map correctly
                    // to the collection....
                    // TODO: display an error message of some sort...
                    return View();
                }

                var key = collection["Key"];
                if (string.IsNullOrEmpty(key)) {
                    // Something else is wrong, it should be a valid ptr....
                    // TODO: display an error message of some sort...
                    BookModel.ErrorMessage = "The key field is required.";
                    return View();
                }

                if (BookTransaction.ContainsKey(key)) // Be aware this transaction could fail too...
                {
                    // error -- duplicate key....
                    // TODO: Add a hidden Error Variable to update the html...
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
            return View();
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try {
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: BookController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try {
                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        public IActionResult OnCreate()
        {
            return RedirectToAction(nameof(Index));
        }

        [BindProperty]
        public static BookModel CurrentItem { get; set; }

        public IActionResult Clear()
        {
            // clear is a part of the create page...
            try {
                BookTransaction.Clear();
                return RedirectToAction(nameof(Index));
            } catch {
                return RedirectToAction(nameof(Create));
            }
        }
    }
}
