using System;
using TL;

namespace Tefichat.Models
{
    public class ChatModel : ChatBaseModel
    {
        public Chat Data { get; set; }
        public string Name
        {
            get => Title;
            set
            {
                Title = value;
                OnPropertyChanged("Name");
            }
        }

        private int participantsCount;
        public int ParticipantsCount
        {
            get => participantsCount;
            set
            {
                participantsCount = value;
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

        public int Version => Data.version;

        public ChatModel(Chat chat) : base(chat)
        {
            Data = chat;
            participantsCount = chat.participants_count;
            date = chat.date;
        }
    }
}
