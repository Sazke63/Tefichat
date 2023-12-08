using Tefichat.Base;
using TL;

namespace Tefichat.Models
{
    public class ChatBaseModel : ObservableObject, IPeerInfoModel
    {
        public long ID { get; set; }
        public bool IsActive { get; set; }
        public virtual long AccessHash { get; set; }
        public bool IsChannel { get; set; }
        public bool IsGroup => !IsChannel;
        public string Title { get; set; }

        private byte[]? _photo;
        public byte[]? Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                OnPropertyChanged("Photo");
            }
        }

        public ChatBaseModel(ChatBase chat)
        {
            ID = chat.ID;
            IsActive = chat.IsActive;
            IsChannel = chat.IsChannel;
            Title = chat.Title;
        }
    }
}
