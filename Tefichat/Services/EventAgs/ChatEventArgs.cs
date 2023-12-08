using System;
using TL;

namespace Tefichat.Services.EventAgs
{
    public class ChatEventArgs : EventArgs
    {
        public UpdateChat UpdateChat { get; }

        public ChatEventArgs(UpdateChat updateChat)
        {
            UpdateChat = updateChat;
        }
    }
}
