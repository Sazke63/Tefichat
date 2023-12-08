using System;
using TL;
namespace Tefichat.Services.EventAgs
{
    public class ChannelEventArgs : EventArgs
    {
        public UpdateChannel UpdateChannel { get; }

        public ChannelEventArgs(UpdateChannel updateChannel)
        {
            UpdateChannel = updateChannel;
        }
    }
}
