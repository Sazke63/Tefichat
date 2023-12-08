using System;
using Tefichat.Models;

namespace Tefichat.Services.EventAgs
{
    public class NewMessageEventArgs : EventArgs
    {
        public MessageBaseModel Message { get; }
        public NewMessageEventArgs(MessageBaseModel message)
        {
            Message = message;
        }
    }
}
