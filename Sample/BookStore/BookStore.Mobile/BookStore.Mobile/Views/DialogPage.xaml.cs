using System;
using BookStore.Mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace BookStore.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DialogPage
    {
        public DialogPage(string title = "", string message = "")
        {
            try
            {
                InitializeComponent();

                if (BindingContext is DialogViewModel dialog)
                {
                    dialog.Title   = title;
                    dialog.Message = message;
                }
            }
            catch (Exception exception)
            {
                App.SetException(this, exception);
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }

        private async void OnOkClicked(object sender, EventArgs e)
        {
            if (BindingContext is DialogViewModel dialog)
                dialog.Result = true;

            await Navigation.PopModalAsync(true);
        }
    }
}
