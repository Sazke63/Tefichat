using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tefichat.Base;
using TL;

namespace Tefichat.Models
{
    public class MessageBaseModel : ObservableObject
    {
        public int ID { get; set; }
        public DialogBaseModel From { get; set; }
        public Peer Peer { get; set; }
        public MessageReplyHeaderBase ReplyTo { get; set; }
        public virtual DateTime Date { get; set; }
        public long GroupedId { get; set; }
        public int TtlPeriod { get; set; }

        public MessageBaseModel(MessageBase messageBase, long groupedId)
        {
            ID = messageBase.ID;
            //From = messageBase.From;
            Peer = messageBase.Peer;
            ReplyTo = messageBase.ReplyTo;
            Date = messageBase.Date;
            TtlPeriod = messageBase.TtlPeriod;
            GroupedId = groupedId;
        }
    }
}
