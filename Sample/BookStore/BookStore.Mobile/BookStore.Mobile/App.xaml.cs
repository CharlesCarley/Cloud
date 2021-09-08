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
using BookStore.Mobile.Views;
using Cloud.Common;
using Cloud.Transaction;
using Xamarin.Forms;

namespace BookStore.Mobile
{
    public partial class App
    {
        private NavigationPage _navigation;
        private MainPage       _mainPage;

        public App()
        {
            try
            {
                RegisterDatabase();
                InitializeComponent();
                LoadContent();
            }
            catch (Exception ex)
            {
                var error = new ErrorPage();
                error.SetException(ex);
                MainPage = error;
            }
        }

        private void LoadContent()
        {
            _mainPage   = new MainPage();
            _navigation = new NavigationPage(_mainPage);

            Resources.TryGetValue("Header", out var header);
            if (header is Color headerColor)
            {
                _navigation.BarBackground = new SolidColorBrush(headerColor);
            }

            Resources.TryGetValue("HeaderText", out var headerText);
            if (headerText is Color headerTextColor)
            {
                _navigation.BarTextColor = headerTextColor;
            }

            MainPage = _navigation;
        }

        /// <summary>
        /// Attempts to register the client database.
        /// </summary>
        private void RegisterDatabase()
        {
            var instanceFileHelper = DependencyService.Get<IPlatformFileHelper>();
            if (instanceFileHelper is null)
            {
                throw new Exception("Failed to instantiate the platform DatabaseFileHelper class.");
            }

            var pathToDataBase = instanceFileHelper.GetFilePath("BookStore.mobile");
            if (!string.IsNullOrEmpty(pathToDataBase))
            {
                Client.Database.Register(pathToDataBase);
            }
            else
            {
                throw new Exception("GetFilePath returned an empty string.");
            }
            // See SettingsViewModel for creation.
            // Settings will not be initialized until they are visited
            // for the first time.
            Client.Settings settings = Client.Settings.SelectByIdentifier(1);

            if (settings == null)
                return;

            try
            {
                Transaction.SetConnectionParameters(new HostConfig {
                    Host    = settings.Host,
                    Port    = settings.Port,
                    Timeout = settings.Timeout,
                });
            }
            catch
            {
                // display in page
            }
        }

        public static void SetException(Exception exception)
        {
            if (Current != null)
            {
                var error = new ErrorPage();
                error.SetException(exception);
                Current.MainPage = error;
            }
            else
            {
                LogUtils.Log(LogLevel.Error,
                             nameof(SetException),
                             exception.Message);
            }
        }
        public static void SetException(ContentPage ctx, Exception exception)
        {
            if (Current != null && ctx != null)
            {
                var error = new ErrorView();
                error.SetException(exception);
                ctx.Content = error;
            }
            else
            {
                LogUtils.Log(LogLevel.Error,
                             nameof(SetException),
                             exception.Message);
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
