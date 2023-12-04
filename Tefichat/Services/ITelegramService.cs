using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tefichat.Models;
using Tefichat.Services.EventAgs;
using TL;

namespace Tefichat.Services
{
    public interface ITelegramService
    {
        event EventHandler<EventArgs> Login;
        event EventHandler<NewMessageEventArgs> NewMessage;
        event EventHandler<ReadChannelInboxEventArgs> ReadChannelInbox;
        event EventHandler<ReadChannelOutboxEventArgs> ReadChannelOutbox;
        event EventHandler<ReadHistoryInboxEventArgs> ReadHistoryInbox;
        event EventHandler<ReadHistoryOutboxEventArgs> ReadHistoryOutbox;

        bool HasLogin { get; }
        Task CheckLogin();
        Task Authorization(string loginInfo);
        Task<List<DialogModel>> GetAllDialogs();
        Task<List<MessageBaseModel>> GetLastMessages();
        Task<bool> ReadMessage(DialogModel dialog, int max_id);
        Task<Message> SendMessage(DialogModel dialog, string text);
        Task<List<MessageBaseModel>> GetMessagesHistoryDialog(DialogModel dialog, int offset_id = 0, int add_offset = 0, int count = 20, bool mode = false);
    }
}
