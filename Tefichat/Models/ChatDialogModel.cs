using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;

namespace Tefichat.Models
{
    public class ChatDialogModel : ChatBaseDialogModel
    {
        public Chat Data { get; set; }
        public string Name
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Name");
            }
        }

        //private int participants_count;
        public int ParticipantsCount
        {
            get => Data.participants_count;
            set
            {
                Data.participants_count = value;
                OnPropertyChanged("ParticipantsCount");
            }
        }

        //private DateTime date;
        public DateTime Date
        {
            get => Data.date;
            set
            {
                Data.date = value;
                OnPropertyChanged("Date");
            }
        }

        public int Version => Data.version;

        public ChatDialogModel(Dialog dialog, Chat chat) : base(dialog, chat)
        {
            Data = chat;
        }
    }
}
