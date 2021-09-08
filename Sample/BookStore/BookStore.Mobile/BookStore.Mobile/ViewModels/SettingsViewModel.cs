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
using Cloud.Common;

namespace BookStore.Mobile.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private Client.Settings _settings;

        public SettingsViewModel()
        {
            LoadSettings();
        }

        private async void LoadSettingsImpl()
        {
            try
            {
                if (Client.Settings.ExistsById(1))
                    _settings = Client.Settings.SelectByIdentifier(1);
                else
                {
                    _settings = new Client.Settings {
                        Key     = "AppSettings",
                        Host    = "127.0.0.1",
                        Port    = 5000,
                        Timeout = 9999999,
                    };

                    await _settings.SaveAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private void LoadSettings()
        {
            LoadSettingsImpl();
        }

        public string Host
        {
            get {
                if (_settings != null)
                    return _settings.Host;
                return string.Empty;
            }
            set {
                if (_settings != null)
                {
                    if (!_settings.Host.Equals(value) && !string.IsNullOrEmpty(value))
                    {
                        _settings.Host = value;
                        _settings.SaveAsync();

                        NotifyEvent(nameof(Host));
                    }
                }
            }
        }
        public string Port
        {
            get {
                if (_settings != null)
                    return _settings.Port.ToString();
                return string.Empty;
            }
            set {
                if (_settings != null && !string.IsNullOrEmpty(value))
                {
                    int val = StringUtils.ToInt(value);

                    if (_settings.Port != val)
                    {
                        _settings.Port = val;
                        _settings.SaveAsync();

                        NotifyEvent(nameof(Port));
                    }
                }
            }
        }
    }
}
