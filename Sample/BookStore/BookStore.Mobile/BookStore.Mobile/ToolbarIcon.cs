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
using Xamarin.Forms;

namespace BookStore.Mobile
{
    public class ToolbarIcon : ToolbarItem
    {
        private IconMapping _icon = IconMapping.AddIcon;

        private FontImageSource UpdateIcon()
        {
            if (!(IconImageSource is FontImageSource))
            {
                IconImageSource = new FontImageSource {
                    FontFamily = "CustomIconFont",
                    Size       = 24,
                    Glyph      = $"{(char)(int)_icon}",
                };
            }

            var val = IconImageSource as FontImageSource;
            if (val != null)
                val.Glyph = $"{(char) (int) _icon}";
            return val;
        }

        public Color Color
        {
            get {
                var fontImageSource = UpdateIcon();
                return fontImageSource?.Color ?? Color.Black;
            }
            set {
                var fontImageSource = UpdateIcon();
                if (fontImageSource != null)
                {
                    fontImageSource.Color = value;
                    OnPropertyChanged(nameof(IconImageSource));
                }
            }
        }

        public IconMapping Mapping
        {
            get => _icon;
            set {
                _icon = value;
                UpdateIcon();
            }
        }

        public static BindableProperty ColorProperty = BindableProperty.Create(
            nameof(Color),
            typeof(Color),
            typeof(ToolbarItem),
            Color.Blue,
            BindingMode.OneWay,
            propertyChanged: (obj, oldValue, newValue) => { OnColorChanged(obj, newValue); });

        private static void OnColorChanged(BindableObject obj, object newValue)
        {
            if (obj is ToolbarIcon icon)
            {
                if (newValue is Color col)
                    icon.Color = col;
            }
        }
    }
}
