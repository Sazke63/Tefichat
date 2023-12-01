using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tefichat.Base;
using TL;

namespace Tefichat.Models
{
    public class MessageModel : ObservableObject
    {
        private int id;
        private Peer from_id;
        private Peer peer_id;
        private MessageFwdHeader fwd_from;
        private long via_bot_id;
        private MessageReplyHeaderBase reply_to;
        private DateTime date;
        private string message;
        public MessageMedia media;
        private ReplyMarkup reply_markup;
        private MessageEntity[] entities;
        private int views;
        private int forwards;
        private MessageReplies replies;
        private DateTime edit_date;
        private string post_author;
        private long grouped_id;
        private MessageReactions reactions;
        private RestrictionReason[] restriction_reason;
        private int ttl_period;

        public int ID => id;
        public Peer From => from_id;
        public Peer Peer => peer_id;
        public MessageReplyHeaderBase ReplyTo => reply_to;
        public long Grouped_id => grouped_id;
        public int TtlPeriod => ttl_period;

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public DateTime Date
        {
            get => date.ToLocalTime();
            set
            {
                date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public bool IsOriginNative { get; set; }

        public MessageModel(Message msg, bool isOriginNative = false)
        {
            id = msg.id;
            from_id = msg.from_id;
            peer_id = msg.peer_id;
            fwd_from = msg.fwd_from;
            via_bot_id = msg.via_bot_id;
            reply_to = msg.reply_to;
            grouped_id = msg.grouped_id;
            date = msg.date;
            message = msg.message;
            media = msg.media;
            reply_markup = msg.reply_markup;
            entities = msg.entities;
            IsOriginNative = isOriginNative;
        }

        public MessageModel(MessageService msg)
        {
            id = msg.id;
            from_id = msg.from_id;
            peer_id = msg.peer_id;
            reply_to = msg.reply_to;
            date = msg.date;
            message = msg.action.GetType().Name;
        }
    }
}
