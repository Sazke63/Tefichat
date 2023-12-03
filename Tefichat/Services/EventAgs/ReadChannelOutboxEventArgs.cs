using System;
using TL;

namespace Tefichat.Services.EventAgs
{
    public class ReadChannelOutboxEventArgs : EventArgs
    {
        public UpdateReadChannelOutbox ReadChannelOutbox { get; }
        public ReadChannelOutboxEventArgs(UpdateReadChannelOutbox readChannelOutbox)
        {
            ReadChannelOutbox = readChannelOutbox;
        }
    }
}
