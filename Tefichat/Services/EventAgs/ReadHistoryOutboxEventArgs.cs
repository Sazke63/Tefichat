using System;
using TL;

namespace Tefichat.Services.EventAgs
{
    public class ReadHistoryOutboxEventArgs : EventArgs
    {
        public UpdateReadHistoryOutbox ReadHistoryOutbox { get; }
        public ReadHistoryOutboxEventArgs(UpdateReadHistoryOutbox readHistoryOutbox)
        {
            ReadHistoryOutbox = readHistoryOutbox;
        }
    }
}
