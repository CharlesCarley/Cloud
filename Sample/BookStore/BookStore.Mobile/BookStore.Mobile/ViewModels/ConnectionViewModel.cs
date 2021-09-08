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
using Cloud.Transaction;
using Xamarin.Forms;

namespace BookStore.Mobile.ViewModels
{
    public class ConnectionViewModel : ViewModel
    {
        private bool _connected;

        public ConnectionViewModel()
        {
            _connected = false;
            TestConnection();
        }

        public Color Connected => _connected ? ColorPalette.BaseGreen : ColorPalette.BaseRed;

        private void OnCheckComplete(bool result)
        {
            _connected = result;
            NotifyEvent(nameof(Connected));
        }

        public async void TestConnection()
        {
            bool result = await Transaction.PingDatabaseAsync(8000);
            OnCheckComplete(result);
        }

        public async Task<bool> TestConnectionAsync()
        {
            bool result = await Transaction.PingDatabaseAsync(8000);
            OnCheckComplete(result);
            return result;
        }

        public string Host => Transaction.Host;
        public string Port => Transaction.Port.ToString();
    }
}
