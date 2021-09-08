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
    /// <summary>
    /// ASCII order for the icons in pia.ttf
    /// </summary>
    public enum IconMapping
    {
        AddIcon = 32,
        EditIcon,
        DeleteIcon,
        BarrelIcon,
        BrushIcon,
        DropIcon,
        ExitIcon,
        GraphLineIcon,
        GraphAreaIcon,
        GraphBarIcon,
        GraphScatterIcon,
        CameraIcon,
        VisibilityOffIcon,
        VisibilityOnIcon,
        UpdateIcon,
        UndoIcon,
        SyncIcon,
        ShareIcon,
        SettingsIcon,
        SelectIcon,
        PumpIcon,
        UserIcon,
        CircleIcon,
        MenuIcon,
        FunctionIcon,
        LinkIcon,
        CheckIcon,
        BoxIcon,
        PencilIcon,
        BugIcon,
        CloudDownIcon,
        CloudUpIcon,
        RightIcon,
        CommentIcon,
        FileIcon,
        HomeIcon,
        CloseIcon,
        ExpandIcon,
        ContractIcon,
        WestIcon,
        NorthIcon,
        EastIcon,
        SouthIcon,
        BarIcon,
    }

    public class IconButton : ContentView
    {
        public delegate void OnClickedHandler();

        public event OnClickedHandler Clicked;

        private readonly Label _label;

        public IconButton()
        {
            VerticalOptions   = LayoutOptions.CenterAndExpand;
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            BackgroundColor   = Color.Transparent;

            _label = new Label {
                FontFamily    = "CustomIconFont",
                FontSize      = 24,
                WidthRequest  = 24,
                HeightRequest = 24
            };

            Content = _label;
            Icon    = IconMapping.BugIcon;

            var tap = new TapGestureRecognizer();
            tap.Tapped += OnTapped;
            GestureRecognizers.Add(tap);
        }

        private void OnTapped(object sender, System.EventArgs e)
        {
            Clicked?.Invoke();
        }

        public IconMapping Icon
        {
            get => _label.Text.Length > 0? (IconMapping)_label.Text[0] : IconMapping.AddIcon;
            set => _label.Text = $"{(char)(int)value}";
        }

        public Color TextColor
        {
            get => _label.TextColor;
            set => _label.TextColor = value;
        }
    }
}
