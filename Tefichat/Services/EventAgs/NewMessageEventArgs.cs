using System;
using Tefichat.Models;

namespace Tefichat.Services.EventAgs
{
    public class NewMessageEventArgs : EventArgs
    {
        public MessageModel Message { get; }
        public NewMessageEventArgs(MessageModel message)
        {
            Message = message;
        }
    }
}
