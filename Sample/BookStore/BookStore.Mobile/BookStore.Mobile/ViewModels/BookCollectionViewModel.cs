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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Client;

namespace BookStore.Mobile.ViewModels
{
    public class BookCollectionViewModel : ConnectionViewModel
    {
        private readonly ObservableCollection<Book> _bookCollection;

        public ObservableCollection<Book> Items => _bookCollection;

        public BookCollectionViewModel()
        {
            _bookCollection = new ObservableCollection<Book>();
            PullCollection();
        }

        private void NotifyItems()
        {
            NotifyEvent(nameof(Items));
        }

        public async void PullCollection()
        {
            if (await TestConnectionAsync())
            {
                _bookCollection.Clear();
                PullCollectionAsync();
                NotifyItems();
            }
        }

        private async void PullCollectionAsync()
        {
            if (await TestConnectionAsync())
            {
                try
                {
                    var books = new List<Book>();

                    List<int> array = BookTransaction.SelectArray();
                    for (var i = 0; i < array.Count; i += 2)
                    {
                        var bookId = array[i];

                        var book = BookTransaction.SelectById(bookId);
                        if (book != null)
                        {
                            // update it locally
                            if (await book.SaveAsync())
                                books.Add(book);
                        }
                    }

                    _bookCollection.Clear();
                    var bookSort = books.OrderBy(book => book.Key);
                    foreach (var book in bookSort)
                        _bookCollection.Add(book);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        public async void ClearCollection()
        {
            if (await ClearCollectionAsync())
                NotifyEvent(nameof(Items));
        }

        public async Task<bool> ClearCollectionAsync()
        {
            try
            {
                _bookCollection.Clear();

                await Book.ClearAsync();
                BookTransaction.Clear();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return true;
        }
    }
}
