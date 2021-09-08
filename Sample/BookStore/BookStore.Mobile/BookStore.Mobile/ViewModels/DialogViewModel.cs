using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Mobile.ViewModels
{
    public class DialogViewModel : ViewModel
    {

        private string _title;
        private string _message;

        public DialogViewModel()
        {
            _title = string.Empty;
            _message = string.Empty;
        }


        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyEvent(nameof(Title));
                }
            }
        }
        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyEvent(nameof(Message));
                }
            }
        }

        public bool Result { get; set; }
    }
}
