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
using System.Collections.Generic;

namespace BookStore.FrontEnd.Models
{
    public class BookModel {
        public BookModel()
        {
        }

        public BookModel(Client.Book book, int index)
        {
            Key         = book.Key;
            Author      = book.Author?.Trim();
            Title       = book.Title?.Trim();
            Description = book.Description?.Trim();
            Index       = index;
        }

        public static IEnumerable<BookModel> SelectArray()
        {
            var list = new List<BookModel>();
            try {
                var array = Client.BookTransaction.SelectArray();
                for (var i = 0; i < array.Count; i += 2) {
                    var idx  = array[i];
                    var book = Client.BookTransaction.SelectById(idx);
                    if (book != null) {
                        list.Add(new BookModel(book, idx));
                    }
                }
                StateManager.HasElements = list.Count > 0;
                return list;
            } catch {
                StateManager.HasElements = false;
                return list;
            }
        }

        public string Key { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Any extra public facing error message..
        /// </summary>
        public static string ErrorMessage { get; set; }

        public int Index { get; }
    }
}
