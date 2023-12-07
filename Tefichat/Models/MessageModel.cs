using System;
using Tefichat.Base;
using TL;

namespace Tefichat.Models
{
    public class MessageModel : MessageBaseModel
    {
        public Message Data { get; set; }

        public string FromName
        {
            get
            {
                if (From != null)
                {
                    switch (From)
                    {
                        case UserDialogModel user: return user.Name;
                        case ChatDialogModel chat: return chat.Name;
                        case ChannelDialogModel channel: return channel.Name;
                    }                    
                }
                return From?.Peer.ID.ToString();
            }
        }

        public string Message
        {
            get => Data.message;
            set
            {
                Data.message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public MessageMedia Media => Data.media;
        public ForwardHeaderModel FwdFrom { get; set; }
        public override MessageBaseModel ReplyTo { get; set; }

        private DateTime date;
        public override DateTime Date
        {
            get => date.ToLocalTime();
            set => date = value;
        }

        public bool IsOriginNative { get; set; }

        public MessageModel(Message msg, bool isOriginNative = false) : base(msg, msg.grouped_id)
        {
            Data = msg;
            Date = msg.date;
            IsOriginNative = isOriginNative;
        }
    }
}
