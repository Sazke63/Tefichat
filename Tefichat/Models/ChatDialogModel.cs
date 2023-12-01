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
        public string Name
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Name");
            }
        }

        private int participants_count;
        public int ParticipantsCount
        {
            get => participants_count;
            set
            {
                participants_count = value;
                OnPropertyChanged("ParticipantsCount");
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        public int Version { get; set; }

        public ChatDialogModel(Dialog dialog, Chat chat) : base(dialog, chat)
        {
            ParticipantsCount = chat.participants_count;
            Version = chat.version;
        }
    }
}
