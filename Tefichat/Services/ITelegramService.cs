using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tefichat.Services
{
    public interface ITelegramService
    {
        event EventHandler<EventArgs> Login;

        bool HasLogin { get; }
        Task CheckLogin();
        Task Authorization(string loginInfo);
    }
}
