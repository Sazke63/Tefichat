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
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public long AccessHash { get; set; }

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

       public UserDialogModel(Dialog dialog, User user) : base(dialog)
       {
            IsActive = user.IsActive;
            Name = user.first_name;
            AccessHash = user.access_hash;
       }
    }
}
