using Tefichat.Base;
using TL;

namespace Tefichat.Models
{
    public class UserModel : ObservableObject, IPeerInfoModel
    {
        public User Data { get; set; }
        public long ID { get; set; }
        public bool IsActive { get; set; }
        public long AccessHash { get; set; }
        public string Name => Data.first_name;

        private byte[]? _avatar;
        public byte[]? Avatar
        {
            get => _avatar;
            set
            {
                _avatar = value;
                OnPropertyChanged("Avatar");
            }
        }

        public UserModel(User user)
        {
            Data = user;
            ID = user.ID;
            IsActive = user.IsActive;
            AccessHash = user.access_hash;
        }
    }
}
