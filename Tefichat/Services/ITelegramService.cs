using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tefichat.Services
{
    public interface ITelegramService
    {
        bool HasLogin { get; }
        Task Authorization(string loginInfo);
    }
}
