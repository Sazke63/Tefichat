using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;

namespace Tefichat.Models
{
    public class UserDialogModel : DialogModel
    {
        public User Data { get; set; }
        public bool IsActive => Data.IsActive;
        public long AccessHash => Data.access_hash;
        public string Name => Data.first_name;

        private byte[] _avatar;
        public byte[] Avatar
        {
            get => _avatar;
            set
            {
                _avatar = value;
                OnPropertyChanged("Avatar");
            }
        }

       public UserDialogModel(Dialog dialog, User user) : base(dialog)
       {
            Data = user;
       }
    }
}
