using System;
using Tefichat.Base;
using TL;

namespace Tefichat.Models
{
    public class MessageModel : MessageBaseModel
    {
        private Message Data { get; set; }

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
