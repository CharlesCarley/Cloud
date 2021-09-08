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
using Xamarin.Forms.Xaml;

namespace BookStore.Mobile.Views
{
    /// <summary>
    /// Defines a basic error page to attempt to display information.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage {
        public ErrorPage()
        {
            InitializeComponent();
        }

        public void SetException(Exception e)
        {
            if (Content is ErrorView ev) {
                if (ev.Brief != null)
                    ev.Brief.Text = e.Message;

                if (ev.Stack != null)
                    ev.Stack.Text = e.StackTrace;
            }
        }
    }
}
