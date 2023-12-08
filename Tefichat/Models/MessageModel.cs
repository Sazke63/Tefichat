using System;
using TL;

namespace Tefichat.Models
{
    public class MessageModel : MessageBaseModel
    {
        public Message Data { get; set; }

        //public string FromName
        //{
        //    get
        //    {
        //        if (From != null)
        //        {
        //            switch (From)
        //            {
        //                case UserModel user: return user.Name;
        //                case ChatModel chat: return chat.Name;
        //                case ChannelModel channel: return channel.Name;
        //            }                    
        //        }
        //        return From?.ID.ToString();
        //    }
        //}

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

        public MessageModel(Message msg, IPeerInfoModel from = null, bool isOriginNative = false) : base(msg, from, msg.grouped_id)
        {
            Data = msg;
            Date = msg.date;
            IsOriginNative = isOriginNative;
        }
    }
}
