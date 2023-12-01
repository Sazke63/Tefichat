using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;

namespace Tefichat.Models
{
    public class ChatBaseDialogModel : DialogModel
    {
        public bool IsActive { get; set; }
        public bool IsChannel { get; set; }
        public bool IsGroup => !IsChannel;
        public string title { get; set; }

        private byte[] _photo;
        public byte[] Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                OnPropertyChanged("Photo");
            }
        }

        public ChatBaseDialogModel(Dialog dialog, ChatBase chat) : base(dialog)
        {
            IsActive = chat.IsActive;
            IsChannel = chat.IsChannel;
            title = chat.Title;           
        }
    }
}
