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
using System.Threading.Tasks;
using BookStore.Client;

namespace BookStore.Mobile.ViewModels
{
    public class BookViewModel : ConnectionViewModel
    {
        private readonly Book _book;

        public BookViewModel()
        {
            _book = new Book {
                Author      = string.Empty,
                Description = string.Empty,
                Key         = string.Empty,
                Title       = string.Empty
            };
        }

        public BookViewModel(Book book)
        {
            _book = book ?? throw new ArgumentException(nameof(book));
        }

        public string Key
        {
            get => _book.Key;
            set {
                if (_book.Key != value)
                {
                    _book.Key = value;
                    NotifyEvent(nameof(Key));
                }
            }
        }
        public int Identifier
        {
            get => _book.Identifier;
            set {
                if (_book.Identifier != value)
                {
                    _book.Identifier = value;
                    NotifyEvent(nameof(Identifier));
                }
            }
        }

        public string Author
        {
            get => _book.Author;
            set {
                if (_book.Author != value)
                {
                    _book.Author = value;
                    NotifyEvent(nameof(Author));
                }
            }
        }
        public string Title
        {
            get => _book.Title;
            set {
                if (_book.Title != value)
                {
                    _book.Title = value;
                    NotifyEvent(nameof(Title));
                }
            }
        }
        public string Description
        {
            get => _book.Description;
            set {
                if (_book.Description != value)
                {
                    _book.Description = value;
                    NotifyEvent(nameof(Description));
                }
            }
        }

        public void Save()
        {
            SaveState();
        }

        private async void SaveState()
        {
            try
            {
                if (_book != null)
                {
                    var result = await _book.SaveAsync();
                    if (result)
                        _book.CreateTransaction().Save();
                }
            }
            catch
            {
                //
            }
        }
    }
}
