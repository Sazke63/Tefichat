using Tefichat.Base;
using Tefichat.Services;

namespace Tefichat.ViewModels
{
    public class RootVM : ObservableObject
    {
        private ITelegramService _telegramService;

        private object currentVM;
        public object CurrentVM 
        { 
            get => currentVM;
            set 
            { 
                currentVM = value; 
                OnPropertyChanged(nameof(CurrentVM));
            } 
        }

        public RootVM()
        {
            _telegramService = new TelegramService();
            _telegramService.CheckLogin();
            if (!_telegramService.HasLogin)
            {
                CurrentVM = new LoginPhoneVM(_telegramService);
            }
            else
                CurrentVM = new MainVM();

            _telegramService.Login += _telegramService_Login;
        }

        private void _telegramService_Login(object? sender, System.EventArgs e)
        {
            if (_telegramService.HasLogin)
                CurrentVM = new MainVM();
            else
                CurrentVM = new SendCodeVM(_telegramService);
        }
    }
}
