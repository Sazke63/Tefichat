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
        public Peer Peer { get; }

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

        private ObservableCollection<MessageModel> messages = new ObservableCollection<MessageModel>();
        public ObservableCollection<MessageModel> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged("Messages");
            }
        }

        private MessageModel _lastMessage;
        public MessageModel LastMessage
        {
            get => _lastMessage; // messages.Count() != 0 ? messages.Last() : null;
            set
            {
                _lastMessage = value;
                OnPropertyChanged("LastMessage");
            }
        }

        public DialogBaseModel(Peer peer, int topMessage)
        {
            Peer = peer;
            _topMessage = topMessage;
        }
    }
}
