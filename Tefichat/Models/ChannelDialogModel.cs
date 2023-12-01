using System;
using System.Collections.Generic;
using TL;

namespace Tefichat.Models
{
    public class ChannelDialogModel : ChatBaseDialogModel
    {
        public long AccessHash { get; set; }
        public string Name
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Name");
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

        public RestrictionReason[] restriction_reason { get; set; }
        public ChatAdminRights admin_rights { get; set; }
        public ChatBannedRights banned_rights { get; set; }
        public ChatBannedRights default_banned_rights { get; set; }
        public Username[] usernames { get; set; }
        public int stories_max_id { get; set; }
        public int color { get; set; }
        public long background_emoji_id { get; set; }
        public IEnumerable<string> ActiveUsernames { get; set; }

        public ChannelDialogModel(Dialog dialog, Channel channel) : base(dialog, channel)
        {
            AccessHash = channel.access_hash;
            ParticipantsCount = channel.participants_count;
            date = channel.date;
            participants_count = channel.participants_count;
            restriction_reason = channel.restriction_reason;
            admin_rights = channel.admin_rights;
            banned_rights = channel.banned_rights;
            usernames = channel.usernames;
            //background_emoji_id = channel.background_emoji_id;
            ActiveUsernames = channel.ActiveUsernames;
        }
    }
}
