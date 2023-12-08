using System;
using Tefichat.Base;
using TL;

namespace Tefichat.Models
{
    public class MessageBaseModel : ObservableObject
    {
        public int ID { get; set; }
        public IPeerInfoModel? From { get; set; }
        public Peer Peer { get; set; }
        public virtual MessageBaseModel ReplyTo { get; set; }
        public virtual DateTime Date { get; set; }
        public long GroupedId { get; set; }
        public int TtlPeriod { get; set; }

        public MessageBaseModel(MessageBase messageBase, IPeerInfoModel from, long groupedId)
        {
            ID = messageBase.ID;
            From = from;
            Peer = messageBase.Peer;
            //ReplyTo = messageBase.ReplyTo;
            Date = messageBase.Date;
            TtlPeriod = messageBase.TtlPeriod;
            GroupedId = groupedId;
        }
    }
}
