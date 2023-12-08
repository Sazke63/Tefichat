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

        public MessageServiceModel(MessageService ms) : base(ms, null, 0)
        {
            Data = ms;
            Message = ms.action.GetType().Name;
        }
    }
}
