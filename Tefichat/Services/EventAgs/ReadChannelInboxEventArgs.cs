using System;
using TL;

namespace Tefichat.Services.EventAgs
{
    public  class ReadChannelInboxEventArgs : EventArgs
    {
        public UpdateReadChannelInbox ReadChannelInbox;
        public ReadChannelInboxEventArgs(UpdateReadChannelInbox readChannelInbox)
        {
            ReadChannelInbox = readChannelInbox;
        }
    }
}
