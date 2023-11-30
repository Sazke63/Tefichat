using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tefichat.Services
{
    public class TelegramService : ITelegramService
    {
        public bool HasLogin { get; set; } = false;

        public TelegramService() { }

        public async Task Authorization(string loginInfo)
        {
            HasLogin = true;
        }
    }
}
