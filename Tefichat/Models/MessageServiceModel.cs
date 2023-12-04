using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using TL;

namespace Tefichat.Models
{
    public class MessageServiceModel : MessageBaseModel
    {
        private MessageService Data { get; set; }

        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        public MessageServiceModel(MessageService ms) : base(ms, 0)
        {
            Data = ms;
            Message = ms.action.GetType().Name;
        }
    }
}
