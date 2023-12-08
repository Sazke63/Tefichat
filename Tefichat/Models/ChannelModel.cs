using System;
using TL;

namespace Tefichat.Models
{
    public class ChannelModel : ChatBaseModel
    {
        public Channel Data { get; set; }
        public override long AccessHash { get; set; }
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
            get => Data.date;
            set
            {
                Data.date = value;
                OnPropertyChanged("Date");
            }
        }

        public ChannelModel(Channel channel) : base(channel)
        {
            Data = channel;
            AccessHash = channel.access_hash;
            participantsCount = channel.participants_count;
        }
    }
}
