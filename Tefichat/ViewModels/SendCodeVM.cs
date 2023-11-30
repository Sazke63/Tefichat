using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tefichat.Base;
using Tefichat.Services;

namespace Tefichat.ViewModels
{
    public class SendCodeVM : ObservableObject
    {
        private ITelegramService _telegramService;

        private string? codeVerify;
        public string? CodeVerify
        {
            get => codeVerify;
            set
            {
                codeVerify = value;
                OnPropertyChanged("CodeVerify");
            }
        }

        public ICommand SendCodeCommand { get; set; }

        public SendCodeVM(ITelegramService telegramService)
        {
            _telegramService = telegramService;
            SendCodeCommand = new RelayCommand(async (o) => await SendCode(o));
        }

        private async Task SendCode(object o)
        {
            if (!string.IsNullOrEmpty(codeVerify))
            {
                await _telegramService.Authorization(codeVerify);
                //MessageBox.Show(_telegramService.Status);
            }
        }
    }
}
