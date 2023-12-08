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
    public class DialogBaseModel : ObservableObject
    {
        public IPeerInfoModel Entity { get; set; }

        private int _topMessage;
        public int TopMessage
        {
            get => _topMessage;
            set
            {
                _topMessage = value;
                OnPropertyChanged(nameof(TopMessage));
            }
        }

        private ObservableCollection<MessageBaseModel> messages = new ObservableCollection<MessageBaseModel>();
        public ObservableCollection<MessageBaseModel> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged("Messages");
            }
        }

        private MessageBaseModel? _lastMessage;
        public MessageBaseModel? LastMessage
        {
            get => _lastMessage; // messages.Count() != 0 ? messages.Last() : null;
            set
            {
                _lastMessage = value;
                OnPropertyChanged("LastMessage");
            }
        }

        public DialogBaseModel(IPeerInfoModel entity, int topMessage)
        {
            Entity = entity;
            _topMessage = topMessage;
        }
    }
}
