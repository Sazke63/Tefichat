using System;
using System.Collections.Generic;
using TL;

namespace Tefichat.Models
{
    public class ChannelDialogModel : ChatBaseDialogModel
    {
        public Channel Data { get; set; }
        public long AccessHash => Data.access_hash;
        public string Name
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Name");
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

        public ChannelDialogModel(Dialog dialog, Channel channel) : base(dialog, channel)
        {
            Data = channel;
        }
    }
}
