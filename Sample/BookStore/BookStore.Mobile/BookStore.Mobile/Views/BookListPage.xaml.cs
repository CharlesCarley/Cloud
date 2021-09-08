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
using BookStore.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BookStore.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookListPage
    {
        public BookListPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception exception)
            {
                App.SetException(this, exception);
            }
        }

        private async void OnLinkIconClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ConnectionPage(), true);
            }
            catch (Exception exception)
            {
                App.SetException(this, exception);
            }
        }

        private async void OnAddClicked()
        {
            try
            {
                await Navigation.PushAsync(new BookEditFormPage(), true);
            }
            catch (Exception exception)
            {
                App.SetException(this, exception);
            }
        }

        private async void OnClearClicked()
        {
            try
            {
                var page = new DialogPage("Clear?", "This will clear the book database.");
                page.Disappearing += OnDialogExit;
                await Navigation.PushModalAsync(page);
                

            }
            catch (Exception exception)
            {
                App.SetException(this, exception);
            }
        }

        private async void OnDialogExit(object sender, EventArgs e)
        {
            if (sender is DialogPage page)
            {
                if (page.BindingContext is DialogViewModel dialog)
                {
                    if (dialog.Result)
                    {
                        if (BindingContext is BookCollectionViewModel collection)
                            await collection.ClearCollectionAsync();

                    }
                }
            }
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            try
            {
                if (BindingContext is BookCollectionViewModel collection)
                    collection.PullCollection();
            }
            catch (Exception exception)
            {
                App.SetException(this, exception);
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                BookListView.SelectedItem = null;
            }
            catch (Exception exception)
            {
                App.SetException(this, exception);
            }
        }
    }
}
