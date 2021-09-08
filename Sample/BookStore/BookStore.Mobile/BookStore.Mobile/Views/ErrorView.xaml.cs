using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BookStore.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorView {
        public ErrorView()
        {
            InitializeComponent();
        }

        public void SetException(Exception e)
        {
            if (Brief != null)
                Brief.Text = e.Message;

            if (Stack != null)
                Stack.Text = e.StackTrace;
        }
    }
}
