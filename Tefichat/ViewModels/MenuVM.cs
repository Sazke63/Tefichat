using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tefichat.Base;
using Tefichat.Models;
using Tefichat.Services;
using TL;

namespace Tefichat.ViewModels
{
    public class MenuVM : ObservableObject
    {
        private ITelegramService _telegramService;

        private AccountModel account;
        public AccountModel Account
        {
            get => account; 
            set => account = value;
        }

        public MenuVM()
        {
            _telegramService = TelegramService.GetInstance();
        }
    }
}
