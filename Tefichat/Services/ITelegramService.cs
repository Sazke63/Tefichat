using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tefichat.Models;
using TL;

namespace Tefichat.Services
{
    public interface ITelegramService
    {
        event EventHandler<EventArgs> Login;

        bool HasLogin { get; }
        Task CheckLogin();
        Task Authorization(string loginInfo);
        Task<List<DialogModel>> GetAllDialogs();
        Task<List<MessageModel>> GetLastMessages();
        Task<bool> SendMessage(DialogModel dialog, string text);
        Task<List<MessageModel>> GetMessagesHistoryDialog(DialogModel dialog, int offset_id = 0, int add_offset = 0, int count = 20, bool mode = false);
    }
}
