using System;
using TL;

namespace Tefichat.Services.EventAgs
{
    public class ReadHistoryInboxEventArgs : EventArgs
    {
        public UpdateReadHistoryInbox ReadHistoryInbox { get; }
        public ReadHistoryInboxEventArgs(UpdateReadHistoryInbox readHistoryInbox)
        {
            ReadHistoryInbox = readHistoryInbox;
        }
    }
}
